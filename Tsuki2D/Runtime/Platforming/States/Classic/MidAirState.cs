using Lunari.Tsuki.Stacking;
using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
namespace Lunari.Tsuki2D.Platforming.States.Classic {
    // TODO: Rename to AirborneState
    public class MidAirState : ClassicHorizontalState {
        public MotorState whenGrounded;
        public override void Tick(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            input.Jump.Consume();
            Horizontal(motor, input, ref velocity);
            if (motor.supportState.down) {
                motor.ActiveState = whenGrounded;
            }
        }
    }
}