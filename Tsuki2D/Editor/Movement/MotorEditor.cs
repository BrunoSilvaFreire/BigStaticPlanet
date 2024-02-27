using Lunari.Tsuki.Entities.Editor;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Editor.Movement.Analysis.Graph;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement {
    [CustomEditor(typeof(Motor))]
    public class MotorEditor : TraitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            using (new EditorGUILayout.HorizontalScope()) {
                var motor = (Motor) target;
                if (GUILayout.Button("Open in recorder")) {
                    MotorAnalyzerWindow.Open().SetMotor(motor);
                }
                var first = MotorAnalyzerWindow.First();
                if (first != null && GUILayout.Button("Open in existing recorder")) {
                    first.SetMotor(motor);
                    first.Focus();
                }
                if (GUILayout.Button("Open in Graph Editor")) {
                    
                    var w = EditorWindow.GetWindow<MotorGraphWindow>();
                    w.SetMotor(motor);
                    w.Show();
                }
            }
        }
    }
}