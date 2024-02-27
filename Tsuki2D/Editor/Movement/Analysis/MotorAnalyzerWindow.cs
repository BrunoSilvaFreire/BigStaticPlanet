using System;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Misc;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
using Styles = Lunari.Tsuki.Editor.Styles;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow : EditorWindow {

        private UnityHistoric<Motor> _motor;
        private static MotorAnalyzerWindow _first;
        private Vector2 scroll;
        public void SetMotor(Motor m) {
            _motor.Current = m;
            HookOntoScene();
            if (m != null) {
                samplingMissionControl.Clock = new InjectedFixedTimeClock(m);
                StartSampling();
            }
        }
        private void OnDisable() {
            if (_first == this) {
                _first = null;
            }
        }
        private void OnEnable() {
            if (_first == null) {
                _first = this;
            }
            Initialize();
        }
        private void Initialize() {
            _motor = new UnityHistoric<Motor>();
            titleContent = new GUIContent("Motor Analyzer");
            HookOntoScene();
            InitializeSampling();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        private void OnPlayModeChanged(PlayModeStateChange obj) {
            if (obj == PlayModeStateChange.ExitingPlayMode) {
                if (IsSampling) {
                    StopSampling();
                }
                if (recording) {
                    recorder.Finish();
                }
                SceneView.duringSceneGui -= Draw;
            }
            if (obj == PlayModeStateChange.EnteredPlayMode) {
                HookOntoScene();
            }
        }


        private void MotorSuggestionsGUI() {
            var motors = FindObjectsOfType<Motor>();
            using (new EditorGUILayout.VerticalScope(Styles.box)) {
                EditorGUILayout.LabelField("Motor Selection");
                TsukiGUILayout.Table(
                    3,
                    motors,
                    motor => {
                        if (GUILayout.Button($"Analyze {motor}")) {
                            SetMotor(motor);
                        }
                        return true;
                    }
                );
            }
        }
        private void InfoGUI() {
            EditorGUILayout.LabelField("Info", Styles.BoldLabel);
            using (new GUIEnabledScope(false)) {
                TryShowMotorInfo();
            }
            var current = _motor.Current;
            if (current == null) {
                EditorGUILayout.HelpBox(
                    "No motor currently assigned",
                    MessageType.Warning
                );
            } else {
                if (current.rigidbody == null) {
                    EditorGUILayout.HelpBox(
                        "Currently assigned motor doesn't have a rigidbody",
                        MessageType.Warning
                    );

                }
            }
            if (current != null) {
                Plot(current);
            }
        }
        private void OnResized(int newSize) {
            analysisRecorder = new MotorFramePage(newSize);
            if (IsSampling) {
                StartSampling();
            }
            ResizePlotters(newSize);
        }
        private void TryShowMotorInfo() {
            var current = _motor.Current;
            if (current != null) {
                var buf = analysisRecorder.Buffer;
                if (buf.Count > 0) {
                    // MotorDebugging.ShowFrame(buf.PeekHead(1), TODO, current);
                    TryShowVelocity(current);
                }
            }
        }
        private static void TryShowVelocity(Motor current) {
            var body = current.rigidbody;
            if (body != null) {
                EditorGUILayout.Vector2Field("Current Velocity", body.velocity);
            }
        }

        public static MotorAnalyzerWindow First() {
            return _first;
        }
    }
}