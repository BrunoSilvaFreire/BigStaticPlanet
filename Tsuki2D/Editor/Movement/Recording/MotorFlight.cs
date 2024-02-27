using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Editor.Movement.Serialization;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    [Serializable]
    public class MotorFlight {
        private int motorId;
        [SerializeField]
        private List<MotorFrame> frames;
        private List<StateTransitionStack> transitionStacks;
        public MotorFlight(List<MotorFrame> frames, int motorId, List<StateTransitionStack> transitionStacks) {
            this.frames = frames;
            this.motorId = motorId;
            this.transitionStacks = transitionStacks;
        }

        public IList<MotorFrame> Frames => frames;

        public StateTransitionStack GetTransition(MotorFrame frame) {
            if (frame.transitionIndex < 0 || frame.transitionIndex >= transitionStacks.Count) {
                return null;
            }
            return transitionStacks[frame.transitionIndex];
        }
        public void SaveToFile(string saveOutputTo) {
            using (var stream = File.Open(saveOutputTo, FileMode.Create)) {
                using (var writer = new BinaryWriter(stream)) {
                    Debug.Log($"Writing flight to {saveOutputTo}");
                    writer.Write(motorId);
                    writer.WriteArray(
                        frames,
                        (value, binaryWriter) => {
                            value.Write(binaryWriter);
                        }
                    );
                    writer.WriteArray(
                        transitionStacks,
                        (value, binaryWriter) => {
                            value.Write(binaryWriter);
                        }
                    );
                }
            }
        }

        public static MotorFlight LoadFromFile(string file) {
            using (var stream = File.Open(file, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    var motorId = reader.ReadInt32();
                    var frames = reader.ReadArray(() => MotorFrame.Read(reader));
                    var transitions = reader.ReadArray(() => StateTransitionStack.Read(reader));

                    return new MotorFlight(frames.ToList(), motorId, transitions.ToList());
                }
            }
        }
    }
}