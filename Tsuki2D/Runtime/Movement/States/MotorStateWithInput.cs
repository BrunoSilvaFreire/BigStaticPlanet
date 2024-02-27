using Lunari.Tsuki.Entities;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.States {

    public abstract class MotorStateWithInput<I> : MotorState where I : class {
        public sealed override void Begin(Motor motor, ref Vector2 velocity) {
            base.Begin(motor, ref velocity);
            if (motor.TryGetInput(out I input)) {
                Begin(motor, input, ref velocity);
            }
        }

        public sealed override void Tick(Motor motor, ref Vector2 velocity) {
            base.Tick(motor, ref velocity);
            if (motor.TryGetInput(out I input)) {
                Tick(motor, input, ref velocity);
            }
        }

        public sealed override void End(Motor motor, ref Vector2 velocity) {
            base.End(motor, ref velocity);
            if (motor.TryGetInput(out I input)) {
                End(motor, input, ref velocity);
            }
        }

        public virtual void Begin(Motor motor, I input, ref Vector2 velocity) { }

        public virtual void Tick(Motor motor, I input, ref Vector2 velocity) { }

        public virtual void End(Motor motor, I input, ref Vector2 velocity) { }
    }
}