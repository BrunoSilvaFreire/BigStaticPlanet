using System;
using Lunari.Tsuki2D.Platforming.Attachments;
using Lunari.Tsuki2D.Platforming.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using Lunari.Tsuki2D.Samples.Basic.Scripts;
using UnityEngine;
namespace Lunari.Tsuki2D.Platforming.States.Classic {
    public class DashingState : MotorStateWithInput<IPlatformerInput> {
        public float distance;
        public float speed;
        public MotorState onExitMidAir;
        public MotorState onExitGrounded;
        public MotorState onExitCollidingWithWall;
        private Vector2 _direction;
        private float _distanceTravelled;
        private bool _wasGroundedOnBegin;
        private void Awake() {
            this.OverrideGravityWhileActive(0);
        }
        public override void Describe(MotorComponentDescriptor descriptor) {
            if (descriptor.RequiresGlobalAttachment<DashEntryAttachment>(out var entry)) {
                entry.state = this;
            }
            
        }
        public override void Begin(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            _direction = new Vector2(input.Horizontal, 0);
            _distanceTravelled = 0;
            _wasGroundedOnBegin = motor.supportState.down;
        }

        public override void Tick(Motor motor, IPlatformerInput input, ref Vector2 velocity) {
            input.Jump.Consume();
            velocity = _direction * speed;
            _distanceTravelled += velocity.magnitude * Time.fixedDeltaTime;
            if (_distanceTravelled > distance) {
                Exit(motor, ref velocity);
            }
            var supportState = motor.supportState;
            if (_wasGroundedOnBegin) {
                if (supportState.HorizontalDirection != 0 || supportState.up) {
                    Exit(motor, ref velocity);
                }
            } else {
                if (supportState.Any) {
                    Exit(motor, ref velocity);
                }
            }
        }

        private void Exit(Motor motor, ref Vector2 velocity) {
            var state = motor.supportState;
            if (state.None) {
                motor.ActiveState = onExitMidAir;
            } else if (state.down) {
                motor.ActiveState = onExitGrounded;
            } else if (state.HasHorizontalCollision()) {
                motor.ActiveState = onExitCollidingWithWall;
            } else {
                motor.ActiveState = onExitMidAir;
            }
            velocity.Normalize();
            velocity *= motor.maxSpeed;
        }
    }
}