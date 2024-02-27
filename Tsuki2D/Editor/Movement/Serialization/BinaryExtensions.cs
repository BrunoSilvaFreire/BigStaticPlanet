using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Lunari.Tsuki2D.Editor.Movement.Serialization {
    public static class BinaryExtensions {
        public delegate T Reader<T>();

        public delegate void Writer<T>(T value, BinaryWriter writer);

        public static void WriteArray<T>(this BinaryWriter writer, IEnumerable<T> arr, Writer<T> writeEach) {
            writer.WriteArray(arr.ToArray(), writeEach);
        }
        public static void WriteArray<T>(this BinaryWriter writer, T[] arr, Writer<T> writeEach) {
            writer.Write(arr.Length);
            foreach (var value in arr) {
                writeEach(value, writer);
            }
        }
        public static T[] ReadArray<T>(this BinaryReader binReader, Reader<T> reader) {
            var length = binReader.ReadInt32();
            var arr = new T[length];
            for (var i = 0; i < length; i++) {
                arr[i] = reader();
            }
            return arr;
        }
    }
}