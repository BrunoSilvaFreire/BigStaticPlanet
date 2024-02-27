using System;
using System.IO;
using Lunari.Tsuki.Misc;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    [Serializable]
    public struct MotorFrame {
        public float time;
        public Vector2 position;
        public Vector2 velocity;
        public int transitionIndex;
        public SupportState support;
        public Bounds2D bounds;

        public static MotorFrame Sample(Motor motor, Rigidbody2D body) {
            return new MotorFrame {
                time = Time.time,
                position = motor.transform.position,
                velocity = body.velocity,
                transitionIndex = -1,
                support = motor.supportState,
                bounds = motor.collider.bounds
            };
        }
        private static void WriteBinary(BinaryWriter writer, Vector2 value) {

            writer.Write(value.x);
            writer.Write(value.y);
        }
        public void Write(BinaryWriter writer) {
            writer.Write(time);
            WriteBinary(writer, position);
            WriteBinary(writer, velocity);
            writer.Write(transitionIndex);
            writer.Write((byte) support.flags);
            WriteBinary(writer, bounds.Min);
            WriteBinary(writer, bounds.Max);
        }
        public static MotorFrame Read(BinaryReader reader) {
            return new MotorFrame {
                time = reader.ReadSingle(),
                position = ReadVector2(reader),
                velocity = ReadVector2(reader),
                transitionIndex = reader.ReadInt32(),
                support = new SupportState {
                    flags = (DirectionFlags) reader.ReadByte()
                },
                bounds = new Bounds2D {
                    Min = ReadVector2(reader),
                    Max = ReadVector2(reader)
                }
            };
        }

        private static Vector2 ReadVector2(BinaryReader reader) {
            return new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }
    }
}