using System.Collections.Generic;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
namespace Lunari.Tsuki2D.Editor.Movement.Storage {
    /// <summary>
    /// Growable storage for frames consisting of many MotorFramePages
    /// </summary>
    public class MotorFrameBook : IFrameStorage {
        private readonly Queue<MotorFramePage> pages = new Queue<MotorFramePage>();
        private readonly int numSamplesPerPage;
        public MotorFrameBook(int numSamplesPerPage) {
            this.numSamplesPerPage = numSamplesPerPage;
            NumFrames = 0;
            AddNewPage();
        }

        public int NumFrames {
            get;
            private set;
        }

        protected MotorFramePage AddNewPage() {
            var page = new MotorFramePage(numSamplesPerPage);
            pages.Enqueue(page);
            return page;

        }
        public void Push(MotorFrame frame) {
            var buf = pages.Peek().Buffer;
            if (buf.IsFull) {
                buf = AddNewPage().Buffer;
            }

            buf.Push(frame);

            NumFrames++;
        }
        public IEnumerable<MotorFrame> GetFrames() {
            foreach (var page in pages) {
                foreach (var frame in page.Buffer) {
                    yield return frame;
                }
            }
        }
    }
}