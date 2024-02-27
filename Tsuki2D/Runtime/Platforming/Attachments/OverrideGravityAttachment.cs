using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
namespace Lunari.Tsuki2D.Platforming.Attachments {
    public class OverrideGravityAttachment : MotorAttachment {
        public float gravity;
        private float oldScale;
        public override void Begin(Motor motor, ref Vector2 velocity) {
            oldScale = motor.rigidbody.gravityScale;
            motor.rigidbody.gravityScale = gravity;
        }
        public override void End(Motor motor, ref Vector2 velocity) {
            motor.rigidbody.gravityScale = oldScale;
        }
    }
    public static class OverrideGravityExtensions {
        public static OverrideGravityAttachment OverrideGravityWhileActive(this MotorState state, float gravityScale) {
            var attachment = state.AddAttachment<OverrideGravityAttachment>();
            attachment.gravity = gravityScale;
            return attachment;
        }
    }
}