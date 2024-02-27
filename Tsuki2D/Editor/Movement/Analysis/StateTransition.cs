using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Lunari.Tsuki;
using Lunari.Tsuki2D.Editor.Movement.Serialization;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    [Serializable]
    public class StateTransitionFrame {
        public string file;
        public string method;
        public int line;
        public int column;
        public StateTransitionFrame(string file, string method, int line, int column) {
            this.file = file;
            this.method = method;
            this.line = line;
            this.column = column;
        }
        public StateTransitionFrame(StackFrame frame) {
            file = frame.GetFileName();
            method = frame.GetMethod().Name;
            line = frame.GetFileLineNumber();
            column = frame.GetFileColumnNumber();
        }
        public void Write(BinaryWriter writer) {
            writer.Write(file ?? string.Empty);
            writer.Write(method);
            writer.Write(line);
            writer.Write(column);
        }
        public override string ToString() {
            return $"{method}, at {file}:{line}";
        }
        public static StateTransitionFrame Read(BinaryReader reader) {
            return new StateTransitionFrame(
                reader.ReadString(),
                reader.ReadString(),
                reader.ReadInt32(),
                reader.ReadInt32()
            );
        }
        public bool CanBeOpened() {
            return !file.IsNullOrEmpty();
        }
    }
    [Serializable]
    public class StateTransitionStack {
        public StateTransitionFrame[] frames;
        public string stateName;
        public string className;
        private static PropertyInfo activeStateProperty = typeof(Motor)
            .GetProperty(
                nameof(Motor.ActiveState),
                BindingFlags.Instance | BindingFlags.Public
            );
        private static StackFrame[] FilterFrames(StackTrace trace) {
            var frames = new List<StackFrame>();
            var filter = activeStateProperty.SetMethod;
            int i = 0;
            MethodBase methodBase;
            do {
                var frame = trace.GetFrame(i++);
                methodBase = frame.GetMethod();
            } while (methodBase != filter);

            for (; i < trace.FrameCount; i++) {
                var frame = trace.GetFrame(i);
                frames.Add(frame);
            }
            return frames.ToArray();
        }

        public StateTransitionStack(MotorState newState, StackTrace trace) {
            var f = FilterFrames(trace);
            var numFrames = f.Length;
            frames = new StateTransitionFrame[numFrames];
            for (var i = 0; i < numFrames; i++) {
                frames[i] = new StateTransitionFrame(f[i]);
            }

            var owner = newState.GetType();
            stateName = owner.GetLegibleName();
            className = owner.AssemblyQualifiedName;
        }
        public StateTransitionStack(StateTransitionFrame[] frames) {
            this.frames = frames;
        }

        public void Write(BinaryWriter writer) {
            writer.WriteArray(frames, (value, binaryWriter) => value.Write(binaryWriter));
            writer.Write(stateName);
            writer.Write(className);
        }
        public static StateTransitionStack Read(BinaryReader reader) {
            return new StateTransitionStack(
                reader.ReadArray(() => StateTransitionFrame.Read(reader))
            ) {
                stateName = reader.ReadString(),
                className = reader.ReadString()
            };
        }
    }
}