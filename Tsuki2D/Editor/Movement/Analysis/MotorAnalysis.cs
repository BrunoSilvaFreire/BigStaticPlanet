/*using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEngine;

namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public class MotorAnalysis {

        public void Resize(int size) {
            Page = new MotorRecordingPage(size);
        }

        public MotorRecordingPage Page {
            get;
            private set;
        }

        public void Pool(Motor motor, Rigidbody2D body) {
            Page.Buffer.LoopPush(MotorFrame.Sample(motor, body));
        }


        public delegate void SequenceProcessor(MotorFrame previous, MotorFrame current);

        public void ForEach(SequenceProcessor processor) {
            using var enumerator = Page.Buffer.GetEnumerator();
            var previous = enumerator.Current;
            enumerator.MoveNext();
            while (enumerator.MoveNext()) {
                var current = enumerator.Current;
                processor(previous, current);
                previous = current;
            }
        }
    }
}*/