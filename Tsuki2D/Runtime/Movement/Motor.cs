using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lunari.Tsuki;
using Lunari.Tsuki.Entities.Common;
#if TSUKI_ENTITIES
using Lunari.Tsuki.Entities;
#endif
using Lunari.Tsuki.Stacking;
using Lunari.Tsuki2D.Runtime.Input;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
namespace Lunari.Tsuki2D.Runtime.Movement {
    [Flags]
    public enum DirectionFlags : byte {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Horizontal = Left | Right,
        Vertical = Up | Down,
        All = Horizontal | Vertical
    }
#if TSUKI_ENTITIES
    [TraitLocation("Movement")]
    public partial class Motor : Trait {
#else
    public partial class Motor : MonoBehaviour {
#endif
        public FloatStackable maxSpeed;

        public SupportState supportState;

        [SerializeField]
        private MotorState activeState;

        public List<MotorAttachment> globalAttachments;
#if TSUKI_ENTITIES
        public new Rigidbody2D rigidbody { get; private set; }
        public new Collider2D collider { get; private set; }

        public EntityInput input {
            get;
            internal set;
        }

#else
#if ODIN_INSPECTOR
        [Required]
#endif
        public new Rigidbody2D rigidbody;
#if ODIN_INSPECTOR
        [Required]
#endif
        public new EntityInput input;
#if ODIN_INSPECTOR
        [Required]
#endif
        public new Collider2D collider;
#endif
        public MotorState ActiveState {
            get => activeState;
            set {
                NextState = value;
                onStateChanged.Invoke();
            }
        }

        public UnityEvent onStateChanged;
        public MotorState NextState { get; private set; }
        private void Start() {
            var velocity = rigidbody.velocity;
            if (activeState != null) {
                activeState.TryBegin(this, ref velocity);
            }

            foreach (var permanentState in globalAttachments) {
                permanentState.TryBegin(this, ref velocity);
            }

            rigidbody.velocity = velocity;
        }

        private void FixedUpdate() {
            ConsumeAndUpdateSupportState();
            var velocity = rigidbody.velocity;
            if (activeState != null) {
                activeState.TryTick(this, ref velocity);
            }

            foreach (var permanentState in globalAttachments) {
                permanentState.TryTick(this, ref velocity);
            }

            FilterVelocity(ref velocity);
            if (NextState != null && activeState != NextState) {
                if (!NextState.CanBeTransitionedInto(this)) {
                    NextState = null;
                    rigidbody.velocity = velocity;
                    return;
                }

                if (activeState != null) {
                    activeState.TryEnd(this, ref velocity);
                }

                if (NextState != null) {
                    NextState.TryBegin(this, ref velocity);
                }

                activeState = NextState;
            }

            rigidbody.velocity = velocity;
#if UNITY_EDITOR
            _editorOnlyOnFixedUpdate.Invoke();
#endif
        }

        private void FilterVelocity(ref Vector2 velocity) {
            if (velocity.x < 0) {
                if (supportState.left) {
                    velocity.x = 0;
                }
            }
            if (velocity.x > 0) {
                if (supportState.right) {
                    velocity.x = 0;
                }
            }
            if (velocity.y < 0) {
                if (supportState.down) {
                    velocity.y = 0;
                }
            }
            if (velocity.y > 0) {
                if (supportState.up) {
                    velocity.y = 0;
                }
            }
        }
#if TSUKI_ENTITIES
        public override void Describe(TraitDescriptor descriptor) {
            rigidbody = descriptor.RequiresComponent<Rigidbody2D>(string.Empty);
            collider = descriptor.RequiresComponent<Collider2D>(string.Empty);
            input = descriptor.DependsOn<EntityInput>(true);
            if (rigidbody != null) {
                descriptor.EnsureEqual(
                    rigidbody.bodyType,
                    RigidbodyType2D.Dynamic,
                    arg0 => rigidbody.bodyType = arg0,
                    "Rigidbody Type"
                );
                descriptor.EnsureIsAnyOf(
                    rigidbody.interpolation,
                    arg0 => rigidbody.interpolation = arg0,
                    "Rigidbody Interpolation",
                    RigidbodyInterpolation2D.Interpolate, RigidbodyInterpolation2D.Extrapolate
                );
                descriptor.EnsureEqual(
                    rigidbody.collisionDetectionMode,
                    CollisionDetectionMode2D.Continuous,
                    arg0 => rigidbody.collisionDetectionMode = arg0,
                    "Rigidbody Collision Detection Mode"
                );
                descriptor.EnsureEqual(rigidbody.simulated, true, arg0 => rigidbody.simulated = arg0, "Rigidbody Simulated");
            }
            if (activeState == null) {
                var builder = descriptor.AddProblem(
                    "No initial state is set"
                );
                foreach (var child in GetComponentsInChildren<MotorState>()) {
                    builder.WithSolution(
                        $"Set {child.name} as initial state",
                        () => {
                            activeState = child;
                        }
                    );
                }
            }
            if (activeState != null) {
                activeState.Describe(new MotorComponentDescriptor(this, activeState, descriptor));
                if (globalAttachments != null) {
                    foreach (var attachment in globalAttachments) {
                        attachment.Describe(new MotorComponentDescriptor(this, attachment, descriptor));
                    }
                }
            }
        }
#endif

        public void SetActiveState(MotorState state) {
            ActiveState = state;
        }

        // Do **NOT** use this unless you know exactly what you're doing and why you're doing it.
        // This maybe be simple and stupid but it has a very good reason to named this way
        public void SetMotorStateUnsafe(MotorState state) {
            activeState = state;
        }


        public bool WithState<T>(out T result) where T : MotorState {
            return (result = activeState as T) != null;
        }

        public bool TryGetInput<T>(out T result) where T : class {
            return (result = input as T) != null;
        }
#if UNITY_EDITOR
        public GameObject _GetProbableColliderGameObject() {
            Behaviour reference;
#if TSUKI_ENTITIES
            reference = GetComponentInParent<Entity>();
#else
            reference = this;
#endif
            return reference.gameObject;
        }
        [HideInInspector, SerializeField]
        public UnityEvent _editorOnlyOnFixedUpdate;
#endif


        public void AddGlobalAttachment<T>() where T : MotorAttachment {
            var component = gameObject.AddComponent<T>();
            globalAttachments ??= new List<MotorAttachment>();
            globalAttachments.Add(component);
        }
        public T GetGlobalAttachment<T>() where T : MotorAttachment {
            return globalAttachments?.FirstOf<T>();
        }
    }
}