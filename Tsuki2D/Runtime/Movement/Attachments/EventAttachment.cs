using UnityEngine;
using UnityEngine.Events;
namespace Lunari.Tsuki2D.Runtime.Movement.Attachments {
    public class EventAttachment : MotorAttachment {
        public UnityEvent onBegin, onEnd;

        public override void Begin(Motor motor, ref Vector2 velocity) {
            onBegin.Invoke();
        }

        public override void End(Motor motor, ref Vector2 velocity) {
            onEnd.Invoke();
        }
    }
}