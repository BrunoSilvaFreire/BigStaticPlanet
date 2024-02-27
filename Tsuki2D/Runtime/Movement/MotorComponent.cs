using System;
using Lunari.Tsuki;
#if TSUKI_ENTITIES
using Lunari.Tsuki.Entities;
using Lunari.Tsuki.Entities.Problems;
using UnityEditor;
#endif
using Lunari.Tsuki2D.Runtime.Movement.Animation;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;

namespace Lunari.Tsuki2D.Runtime.Movement
{
    public class MotorComponentDescriptor
    {
        private readonly Motor _motor;
        private readonly MotorComponent _component;
        private readonly AnimatorSynchronizer _synchronizer;
#if TSUKI_ENTITIES
        private readonly TraitDescriptor _descriptor;
        private bool _hasSynchronizerProblem;

        public MotorComponentDescriptor(Motor motor, MotorComponent component, TraitDescriptor descriptor) : this(motor, component) {
            _descriptor = descriptor;
        }
#endif

        public MotorComponentDescriptor(Motor motor, MotorComponent component)
        {
            _motor = motor;
            _component = component;
            _synchronizer = motor.globalAttachments.FirstOf<AnimatorSynchronizer>();
        }


        private void AddSyncronizer(AnimatorSync sync)
        {
            if (_synchronizer)
            {
                _synchronizer.Add(sync);
#if TSUKI_ENTITIES
                _descriptor.RequiresAnimatorParameter(sync.Property, sync.ParameterType);
#endif
            }
            else
            {
#if TSUKI_ENTITIES
                if (!_hasSynchronizerProblem) {
                    _hasSynchronizerProblem = true;
                    _descriptor
                        .AddProblem($"Motor doesn't have an AnimatorSyncronizer, but it's required by component {_component}")
                        .WithAddGlobalAttachmentSolution<AnimatorSynchronizer>(_motor, "Add Animator Synchronizer");
                }

#else
                // TODO: Non tsuki fix
#endif
            }
        }

        public void SyncAnimator(string parameter, AnimatorSyncGetter<bool> value)
        {
            AddSyncronizer(new BoolAnimatorSync(parameter, value));
        }

        public void SyncAnimator(string parameter, AnimatorSyncGetter<float> value)
        {
            AddSyncronizer(new FloatAnimatorSync(parameter, value));
        }

        public void SyncAnimator(string parameter, AnimatorSyncGetter<int> value)
        {
            AddSyncronizer(new IntAnimatorSync(parameter, value));
        }

        public void RequiresComponent<T>(out T animator) where T : Component
        {
#if TSUKI_ENTITIES
            _descriptor.RequiresComponent(out animator);
#else

            if (!_motor.TryGetComponentInChildren(out animator))
            {
                throw new Exception($"Unable to find component of type {nameof(T)} in {_motor}");
            }
#endif
        }

        public bool RequiresGlobalAttachment<T>(out T attachment, bool shouldAddProblem = true)
            where T : MotorAttachment
        {
            attachment = _motor.GetGlobalAttachment<T>();
#if TSUKI_ENTITIES
            if (attachment == null && shouldAddProblem) {
                _descriptor.AddProblem(
                    $"MotorComponent {_component} of motor {_motor} requires an global attachment of type {typeof(T).GetLegibleName()}"
                ).WithAddGlobalAttachmentSolution<T>(_motor);
            }
            if (!_descriptor.Initialize) {
                return false;
            }
#endif
            return attachment != null;
        }
    }

    public abstract class MotorComponent : MonoBehaviour
    {
        public virtual void Describe(MotorComponentDescriptor descriptor)
        {
        }

        /**
         * Called when this state becomes the active state in the motor.
         */
        public virtual void Begin(Motor motor, ref Vector2 velocity)
        {
        }

        /**
         * Called every fixed frame while this state is the active state in the motor.
         */
        public virtual void Tick(Motor motor, ref Vector2 velocity)
        {
        }

        /**
         * Called when this state no longer is the active state in the motor.
         */
        public virtual void End(Motor motor, ref Vector2 velocity)
        {
        }
    }

    public abstract class MotorComponent<I> : MotorComponent
    {
        public sealed override void Begin(Motor motor, ref Vector2 velocity)
        {
        }

        public sealed override void Tick(Motor motor, ref Vector2 velocity)
        {
        }

        public sealed override void End(Motor motor, ref Vector2 velocity)
        {
        }
    }
}