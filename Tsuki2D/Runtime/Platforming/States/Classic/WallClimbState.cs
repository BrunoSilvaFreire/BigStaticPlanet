using System;
using System.Collections;
using Lunari.Tsuki.Stacking;
using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
namespace Lunari.Tsuki2D.Platforming.States.Classic {
    public class WallClimbState : MotorStateWithInput<IPlatformerInput> {
        public MotorState whenSupportLost;
        public MotorState whenGrounded;
        public FloatStackable jumpHeight = 12;
        public FloatStackable wallPushForce = 5;
        public AnimationCurve slideDownCurve = AnimationCurve.EaseInOut(0, 0, 3, 1);
        public float reEnterCooldown = .25F;
        public float controlReductionDuration = 3;
        public AnimationCurve controlAfterJump = AnimationCurve.Linear(0, 0, 1, 1);
        private float timeInState;
        private float cooldown;
        private IEnumerator ApplyControlReduction(Motor motor, int direction) {
            float currentTime = 0;
            while (currentTime < controlReductionDuration) {
                yield return new WaitForFixedUpdate();
                currentTime += Time.fixedDeltaTime;
                var control = controlAfterJump.Evaluate(currentTime / controlReductionDuration);
                motor.SetDirectionControl(direction, control);

            }
            motor.SetDirectionControl(direction, 1);
        }

        private void Start() {
            cooldown = float.NegativeInfinity;
        }

        public override bool CanBeTransitionedInto(Motor motor) {
            if (motor.TryGetInput(out IPlatformerInput input)) {
                return cooldown < 0 && IsHoldingWall(motor.supportState, input);
            }
            return false;
        }

        private void FixedUpdate() {
            if (cooldown > 0) {
                cooldown -= Time.fixedDeltaTime;
            }
        }

        public override void End(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            cooldown = reEnterCooldown;
        }

        public override void Begin(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            timeInState = 0;
        }

        public override void Tick(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            var state = motor.supportState;

            if (state.down) {
                motor.ActiveState = whenGrounded;
            } else if (!IsHoldingWall(state, input)) {
                motor.ActiveState = whenSupportLost;
            } else {
                if (input.Jump.Consume()) {
                    JumpOffWall(motor, out velocity);
                    motor.ActiveState = whenSupportLost;
                } else {
                    SlideDown(ref velocity);
                }
            }
            timeInState += Time.fixedDeltaTime;
        }
        private bool IsHoldingWall(SupportState state, IPlatformerInput input) {
            var direction = state.HorizontalDirection;
            return direction != 0 && Math.Sign(input.Horizontal) == direction;
        }

        private void JumpOffWall(Motor motor, out Vector2 velocity) {
            var normal = motor.supportState.HorizontalNormal;
            velocity = new Vector2(
                normal * wallPushForce,
                jumpHeight.Value
            );
            StartCoroutine(ApplyControlReduction(motor, -normal));
        }

        private void SlideDown(ref Vector2 velocity) {
            velocity.y = slideDownCurve.Evaluate(timeInState);
        }
    }
}