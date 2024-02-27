using Lunari.Tsuki2D.Runtime.Movement.States;
namespace Lunari.Tsuki2D.Runtime.Movement.Attachments {
    public abstract class MotorAttachment : MotorComponent {
        public virtual bool CompatibleWith(MotorState state) {
            return true;
        }
    }

    public abstract class MotorAttachmentOfState<T> : MotorAttachment {
        public T State { get; private set; }

        private void Awake() {
            State = GetComponent<T>();
        }

        public override bool CompatibleWith(MotorState state) {
            return state is T;
        }
    }
}