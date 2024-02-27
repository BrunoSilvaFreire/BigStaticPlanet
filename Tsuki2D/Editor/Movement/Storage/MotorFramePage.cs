using System.Collections.Generic;
using Lunari.Tsuki.Algorithm;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
namespace Lunari.Tsuki2D.Editor.Movement.Storage {
    /**
     * A storage of fixed size for frames.
     */
    public class MotorFramePage : IFrameStorage {
        public MotorFramePage(int size) {
            Buffer = new RingBuffer<MotorFrame>(size);
        }

        public RingBuffer<MotorFrame> Buffer {
            get;
        }

        public void Push(MotorFrame frame) {
            Buffer.LoopPush(frame);
        }
        public IEnumerable<MotorFrame> GetFrames() {
            return Buffer;
        }
    }
}