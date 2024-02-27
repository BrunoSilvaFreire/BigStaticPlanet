using Lunari.Tsuki.Misc;
using Lunari.Tsuki.Stacking;
using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
using UnityEngine.Events;
namespace Lunari.Tsuki2D.Platforming.States.Classic {
    public class GroundedState : ClassicHorizontalState {
        public FloatStackable jumpHeight;
        public float extraGravity = 10;
        public UnityEvent onJumped;
        public MotorState whenMidAir;
        private bool _blockExtraGravityUntilGrounded;
        public BooleanHistoric JumpedThisFrame { get; } = new BooleanHistoric();

        public bool SuccessfullyJumpedThisFrame { get; private set; }

        public void BlockExtraGravityUntilGrounded() {
            _blockExtraGravityUntilGrounded = true;
        }

        public override void End(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            base.End(motor, input, ref velocity);
            _blockExtraGravityUntilGrounded = false;
        }
        public override void Tick(
            Motor motor,
            IPlatformerInput input,
            ref Vector2 velocity
        ) {
            if (_blockExtraGravityUntilGrounded) {
                _blockExtraGravityUntilGrounded = !motor.supportState.down;
            }
            Horizontal(motor, input, ref velocity);
            Jump(motor, input, ref velocity);
        }

        private void Jump(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            var jumped = input.Jump.Consume();
            var grounded = motor.supportState.down;
            SuccessfullyJumpedThisFrame = jumped && grounded;
            if (SuccessfullyJumpedThisFrame) {
                velocity.y = jumpHeight;
            }

            if (!_blockExtraGravityUntilGrounded && !motor.supportState.down && !input.Jump.Current && velocity.y > 0) {
                velocity.y -= extraGravity;
            }

            JumpedThisFrame.Current = jumped;
            if (JumpedThisFrame.JustActivated && grounded) {
                onJumped.Invoke();
            }
            if (!motor.supportState.down) {
                motor.ActiveState = whenMidAir;
            }
        }

    }
}