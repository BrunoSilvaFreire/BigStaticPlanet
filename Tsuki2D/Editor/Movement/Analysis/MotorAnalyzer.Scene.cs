using System.Linq;
using Lunari.Tsuki.Editor.Preference;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        private readonly FloatEditorPreference lineWidth = new FloatEditorPreference("motor_analyzer_gui_line_width", 1F);
        private void SceneGUI() {
            lineWidth.DrawField("Scene Line Width");
        }
        private void HookOntoScene() {
            SceneView.duringSceneGui -= Draw;
            SceneView.duringSceneGui += Draw;
        }
        private void DrawCollision(MotorFrame frame, SupportState previous, SupportState current) {
            foreach (var side in SupportSide.AllSides) {
                Color col;
                var previously = previous.HasSupport(side);
                var currently = current.HasSupport(side);
                if (currently == previously) {
                    col = MotorDebugging.kCollisionEqualColor;
                } else if (currently) {
                    col = MotorDebugging.kCollisionEnterColor;
                } else {
                    col = MotorDebugging.kCollisionExitColor;
                }
                MotorDebugging.DrawSide(frame, side, col);
            }
        }

        private void Draw(SceneView obj) {
            if (analysisRecorder == null) {
                return;
            }
            var motor = _motor.Current;

            var frames = analysisRecorder.Buffer.ToArray();
            var pos = frames.Select(frame => (Vector3) frame.position).ToArray();
            for (var i = 1; i < pos.Length; i++) {
                Handles.DrawLine(
                    pos[i - 1],
                    pos[i],
                    lineWidth.Value
                );
            }
            analysisRecorder.ForEach((frame, current) => {
                var previousState = frame.transitionIndex;
                var currentState = current.transitionIndex;
                if (previousState != currentState) {
                    // MotorDebugging.DrawStateLabel(
                    //     ,
                    //     current
                    // );
                }
            });
            if (motor != null) {
                analysisRecorder.ForEach(
                    (previous, current) => {
                        var previousState = previous.support;
                        var currentState = current.support;
                        if (previousState != currentState) {
                            DrawCollision(current, previousState, currentState);
                        }
                    }
                );
            }
        }
    }
}