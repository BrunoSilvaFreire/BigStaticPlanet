using System.Collections.Generic;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
namespace Lunari.Tsuki2D.Editor.Movement.Storage {
    /// <summary>
    /// Provides space for storing MotorFrames
    /// </summary>
    public interface IFrameStorage {
        void Push(MotorFrame frame);
        IEnumerable<MotorFrame> GetFrames();

    }

    public static class FrameStorageExtensions {

        public delegate void SequenceProcessor(MotorFrame previous, MotorFrame current);

        public static void ForEach(this IFrameStorage storage, SequenceProcessor processor) {
            using var enumerator = storage.GetFrames().GetEnumerator();
            var previous = enumerator.Current;
            while (enumerator.MoveNext()) {
                var current = enumerator.Current;
                processor(previous, current);
                previous = current;
            }
        }
    }
}