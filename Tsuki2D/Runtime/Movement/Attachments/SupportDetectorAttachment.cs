using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Lunari.Tsuki2D.Runtime.Movement.Attachments {
    [Serializable]
    public class SupportListener {
        public UnityEvent onActive;
        public SupportRequirementNode requirement;

        public bool Allowed(SupportState state) {
            return requirement.IsMet(state);
        }
    }

    [Serializable]
    public struct SupportRequirementNode {
        public enum EvaluationMode {
            Self,
            All,
            Any,
            None
        }
#if ODIN_INSPECTOR
        [HideIf(nameof(IsSelf))]
#endif
        public List<SupportRequirementNode> children;
#if ODIN_INSPECTOR
        [HideLabel]
        [ShowIf(nameof(IsSelf))]
#endif
        public SupportRequirement requirement;

        public EvaluationMode mode;

        private bool IsSelf() {
            return mode == EvaluationMode.Self;
        }

        public bool IsMet(SupportState supportState) {
            switch (mode) {
                case EvaluationMode.All:
                    return children.All(node => node.IsMet(supportState));
                case EvaluationMode.Any:
                    return children.Any(node => node.IsMet(supportState));
                case EvaluationMode.None:
                    return !children.Any(node => node.IsMet(supportState));
                case EvaluationMode.Self:
                    return requirement.IsMet(supportState);
                default:
                    return false;
            }
        }
    }

    [Serializable]
    public struct SupportRequirement {
#if ODIN_INSPECTOR
        [HideLabel]
        [HorizontalGroup]
#endif
        public SupportState state;
#if ODIN_INSPECTOR
        [HideLabel]
        [HorizontalGroup]
#endif
        public bool value;


        public bool IsMet(SupportState supportState) {
            return (state.flags & supportState.flags) == state.flags == value;
        }
    }

    public class SupportDetectorAttachment : MotorAttachment {
        [Tooltip("Events are fired when the motor's support state match the listener's requirement.")]
        public List<SupportListener> listeners;

        public override void Tick(Motor motor, ref Vector2 velocity) {
            EvaluateListeners(motor);
        }

        private void EvaluateListeners(Motor m) {
            foreach (var supportListener in listeners) {
                if (!supportListener.Allowed(m.supportState)) {
                    continue;
                }

                supportListener.onActive.Invoke();
            }
        }
    }
}