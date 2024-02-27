using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph {
    public static class MotorGraphExtensions {
        private static readonly FieldInfo onConnectDelegateField = typeof(Port).GetField(
            "OnConnect",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        public static void SetOnConnectedCallbacks(
            this Port port,
            Action<Port> callbacks
        ) {
            if (onConnectDelegateField != null) {
                onConnectDelegateField.SetValue(port, callbacks);
            }
        }
    }
}