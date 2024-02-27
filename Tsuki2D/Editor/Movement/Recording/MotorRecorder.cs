using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    public class MotorRecorder {
        private readonly MotorSampler sampler;
        private TransitionTrackingStorage storage;
        private Motor motor;

        public UnityEvent<MotorFlight> OnFinished {
            get;
        } = new UnityEvent<MotorFlight>();

        public MotorRecorder(
            Motor motor,
            MotorSamplingClock clock,
            int numSamplesPerPage
        ) {
            this.motor = motor;
            storage = new TransitionTrackingStorage(
                motor,
                new MotorFrameBook(numSamplesPerPage)
            );
            sampler = new MotorSampler(
                clock,
                motor,
                storage
            );
            Begin();
        }
        private void Begin() {
            storage.BeginTracking();
            sampler.Start();
        }

        public MotorFlight Finish() {
            storage.StopTracking();
            var frames = storage.GetFrames().ToList();
            var flight = new MotorFlight(frames, motor.GetInstanceID(), storage.Transitions);
            OnFinished.Invoke(flight);
            return flight;
        }

    }
}