using System;
using System.Diagnostics;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using Debug = UnityEngine.Debug;

namespace Lunari.Tsuki2D.Editor.Movement.Sampling {
    public class MotorSampler {
        private readonly MotorSamplingClock clock;
        private readonly Motor motor;
        private readonly IFrameStorage storage;
        public MotorSampler(MotorSamplingClock clock, Motor motor, IFrameStorage storage) {
            if (motor == null) {
                throw new Exception("Motor cannot be null");
            }
            this.motor = motor;
            this.clock = clock ?? throw new Exception("Clock cannot be null");
            this.storage = storage ?? throw new Exception("Storage cannot be null");
        }

        public bool Sampling {
            get;
            private set;
        } = false;

        public void Start() {
            if (Sampling) {
                return;
            }
            Sampling = true;
            clock.Start(OnSample);
        }
        public void Stop() {
            if (!Sampling) {
                return;
            }
            Sampling = false;
            clock.Stop();
        }
        private void OnSample() {
            try {
                var frame = MotorFrame.Sample(motor, motor.rigidbody);
                storage.Push(frame);
            }
            catch (Exception e) {
                Debug.LogError(e, motor);
                Stop();
                throw;
            }
        }
    }
}