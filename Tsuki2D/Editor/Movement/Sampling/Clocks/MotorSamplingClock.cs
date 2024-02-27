using UnityEngine.Events;
namespace Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks {
    /**
     * Defines
     */
    public abstract class MotorSamplingClock {
        private UnityAction sampleCallback;
        protected void Sample() {
            sampleCallback.Invoke();
        }
        public void Start(UnityAction callback) {
            sampleCallback = callback;
            OnStart();
            
        }
        public void Stop() {
            sampleCallback = null;
            OnStop();
        }
        protected abstract void OnStart();
        protected abstract void OnStop();
    }
}