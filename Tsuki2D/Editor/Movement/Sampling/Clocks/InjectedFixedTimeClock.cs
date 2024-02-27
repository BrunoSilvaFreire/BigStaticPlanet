using Lunari.Tsuki2D.Runtime.Movement;
namespace Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks {
    public class InjectedFixedTimeClock : MotorSamplingClock {
        private readonly Motor motor;
        public InjectedFixedTimeClock(Motor motor) {
            this.motor = motor;
        }
        protected override void OnStart() {
            motor._editorOnlyOnFixedUpdate.AddListener(Sample);
        }
        protected override void OnStop() {
            motor._editorOnlyOnFixedUpdate.RemoveListener(Sample);
        }
    }
}