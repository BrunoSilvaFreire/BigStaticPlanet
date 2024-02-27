using Lunari.Tsuki.Editor;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        private void OnGUI() {
            using (var scope = new EditorGUILayout.ScrollViewScope(scroll)) {
                DrawGUI();
                scroll = scope.scrollPosition;
            }
            Repaint();
        }
        private void DrawGUI() {
            EditorGUILayout.LabelField("Settings", Styles.BoldLabel);
            using (new EditorGUILayout.VerticalScope(Styles.box)) {
                using (new EditorGUILayout.HorizontalScope()) {
                    SettingsGUI();
                }
                RecordingGUI();
            }
            using (new EditorGUILayout.VerticalScope(Styles.box)) {
                SceneGUI();
            }
            using (new EditorGUILayout.VerticalScope(Styles.box)) {
                InfoGUI();
            }
        }


        private void SettingsGUI() {

            using (new EditorGUILayout.VerticalScope()) {
                _motor.Current = (Motor) EditorGUILayout.ObjectField("Motor", _motor, typeof(Motor), true);
                MotorSuggestionsGUI();
                samplingMissionControl.Motor = _motor.Current;
                SamplingOptionsGUI();
            }
            // Computed properties

        }
    }
}