using System;
using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
namespace Lunari.Tsuki2D.Platforming.States.Classic {
    public class ClassicHorizontalState : MotorStateWithInput<IPlatformerInput> {
        public string animatorHorizontalParameter = "Horizontal";
        public string animatorVerticalParameter = "Vertical";
        public string animatorGroundedParameter = "Grounded";
        public AnimationCurve acceleration = AnimationCurve.Linear(0, 5, 1, 10);
        public AnimationCurve deceleration = AnimationCurve.Constant(0, 1, 10);
        public override void Describe(MotorComponentDescriptor descriptor) {
            descriptor.SyncAnimator(animatorHorizontalParameter, motor => Mathf.Abs(motor.rigidbody.velocity.x) / motor.maxSpeed);
            descriptor.SyncAnimator(animatorVerticalParameter, motor => motor.rigidbody.velocity.y);
            descriptor.SyncAnimator(animatorGroundedParameter, motor => motor.supportState.down);
        }

        protected void Horizontal(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            var horizontal = input.Horizontal;
            var inputDir = Math.Sign(horizontal);
            var control = motor.GetDirectionControl(inputDir);
            var max = motor.maxSpeed * control * Mathf.Abs(horizontal);

            var curveTime = Mathf.InverseLerp(0, motor.maxSpeed, Mathf.Abs(velocity.x));
            var currentDeceleration = deceleration.Evaluate(curveTime) * Time.fixedDeltaTime;
            if (Mathf.Approximately(horizontal, 0)) {
                // Slowly stop
                velocity.x = Mathf.Lerp(velocity.x, 0, currentDeceleration);
            } else {
                var untilMaxSpeed = Mathf.Max(0, max - Mathf.Abs(velocity.x));
                var wantsToAdd = acceleration.Evaluate(curveTime) * control * Time.fixedDeltaTime;
                var velDir = Mathf.Sign(velocity.x);
                if (Mathf.Approximately(velDir, inputDir)) {
                    // Accelerate
                    velocity.x += inputDir * Mathf.Min(untilMaxSpeed, wantsToAdd);
                } else {
                    // Decelerate
                    velocity.x += inputDir * Mathf.Min(max, wantsToAdd);
                    velocity.x = Mathf.Lerp(velocity.x, 0, currentDeceleration);
                }

                if (motor.supportState.down) {
                    velocity.x = Mathf.Clamp(velocity.x, -max, max);
                }
            }
        }
    }
}