using System.Collections.Generic;
using System.Diagnostics;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Runtime.Movement;
namespace Lunari.Tsuki2D.Editor.Movement.Storage {
    public sealed class TransitionTrackingStorage : IFrameStorage {
        private readonly Motor motor;
        private readonly IFrameStorage child;

        public List<StateTransitionStack> Transitions {
            get;
        } = new List<StateTransitionStack>();

        public TransitionTrackingStorage(Motor motor, IFrameStorage child) {
            this.motor = motor;
            this.child = child;
        }

        private int stateChangeIndex = -1;

        public void Push(MotorFrame frame) {
            frame.transitionIndex = stateChangeIndex;
            if (stateChangeIndex >= 0) {
                stateChangeIndex = -1;
            }
            child.Push(frame);
        }

        public void BeginTracking() {
            motor.onStateChanged.AddListener(OnStateChanged);
        }

        public void StopTracking() {
            motor.onStateChanged.RemoveListener(OnStateChanged);
        }

        private int Push(StateTransitionStack transition) {
            var index = Transitions.Count;
            Transitions.Add(transition);
            return index;
        }

        private void OnStateChanged() {
            stateChangeIndex = Push(
                new StateTransitionStack(
                    motor.NextState,
                    new StackTrace(true)
                )
            );
        }

        public IEnumerable<MotorFrame> GetFrames() {
            return child.GetFrames();
        }
    }
}