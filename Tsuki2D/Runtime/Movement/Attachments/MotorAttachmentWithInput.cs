using Lunari.Tsuki.Entities;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Attachments {
    public class MotorAttachmentWithInput<I> : MotorAttachment where I : class {
        public sealed override void Begin(Motor motor, ref Vector2 velocity) {
            if (motor.TryGetInput(out I input)) {
                Begin(motor, input, ref velocity);
            }
        }

        public sealed override void Tick(Motor motor, ref Vector2 velocity) {
            if (motor.TryGetInput(out I input)) {
                Tick(motor, input, ref velocity);
            }
        }

        public sealed override void End(Motor motor, ref Vector2 velocity) {
            if (motor.TryGetInput(out I input)) {
                End(motor, input, ref velocity);
            }
        }

        public virtual void Begin(Motor motor, I input, ref Vector2 velocity) {
        }

        public virtual void Tick(Motor motor, I input, ref Vector2 velocity) {
        }

        public virtual void End(Motor motor, I input, ref Vector2 velocity) {
        }
    }
}