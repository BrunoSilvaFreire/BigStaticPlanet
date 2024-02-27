using Lunari.Tsuki2D.Runtime.Input.TopDown;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.States {
    public class TopDownState : MotorStateWithInput<TopDownInput> {
        public float speed = 5;
        public float deceleration = .5F;

        public override void Tick(Motor motor, TopDownInput input, ref Vector2 velocity) {
            var dir = new Vector2(input.horizontal, input.vertical);

            if (Mathf.Approximately(dir.magnitude, 0)) {
                velocity = Vector2.Lerp(velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            } else {
                dir.Normalize();
                dir *= speed * motor.Control * Time.fixedDeltaTime;
                velocity += dir;
                velocity = Vector2.ClampMagnitude(velocity, motor.maxSpeed * motor.Control);
            }
        }
    }
}