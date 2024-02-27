using Lunari.Tsuki2D.Platforming.States.Classic;
using Lunari.Tsuki2D.Runtime.Input;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
namespace Lunari.Tsuki2D.Samples.Basic.Scripts {
    public interface IDashInput {
        public EntityAction Dash {
            get;
        }
    }
    public class DashEntryAttachment : MotorAttachmentWithInput<IDashInput> {
        public DashingState state;
        public override void Tick(Motor motor, IDashInput input, ref Vector2 velocity) {
            if (input.Dash.Consume()) {
                motor.ActiveState = state;
            }
        }
    }
}