using System.Collections.Generic;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.States {
    public abstract class MotorState : MotorComponent {
        [SerializeField]
        [HideInInspector]
        public List<MotorAttachment> attachments;

        public T AddAttachment<T>() where T : MotorAttachment {
            var value = gameObject.AddComponent<T>();
            attachments.Add(value);
            return value;
        }
        public virtual bool CanBeTransitionedInto(Motor motor) {
            return true;
        }

        public override void Begin(Motor motor, ref Vector2 velocity) {
            foreach (var attachment in attachments) {
                attachment.TryBegin(motor, ref velocity);
            }
        }

        public override void Tick(Motor motor, ref Vector2 velocity) {
            foreach (var attachment in attachments) {
                attachment.TryTick(motor, ref velocity);
            }
        }

        public override void End(Motor motor, ref Vector2 velocity) {
            foreach (var attachment in attachments) {
                attachment.TryEnd(motor, ref velocity);
            }
        }
    }
}