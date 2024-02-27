using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
using UnityEngine.Events;
namespace Lunari.Tsuki2D.Platforming.States {
    public class DoubleJumpAttachment : MotorAttachmentWithInput<IPlatformerInput> {
        public float jumpHeight = 20.0F;
        public float horizontalForce = 4;
        public UnityEvent onDoubleJump;
        private bool ellegible;
        public override void Begin(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            ellegible = true;
        }
        public override void Tick(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            if (motor.supportState.down) {
                ellegible = true;
            }

            if (!ellegible) {
                return;
            }


            if (!motor.supportState.down && input.Jump.Consume()) {
                DoubleJump(input,ref velocity);
                ellegible = false;
            }
        }
        private void DoubleJump(IPlatformerInput input,ref Vector2 velocity) {
            velocity.x += input.Horizontal * horizontalForce;
            if (velocity.y > 0) {
                velocity.y += jumpHeight;
            } else {
                velocity.y = jumpHeight;
            }

            onDoubleJump.Invoke();
        }
    }
}