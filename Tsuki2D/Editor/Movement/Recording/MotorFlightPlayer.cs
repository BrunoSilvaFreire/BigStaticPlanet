using System;
using System.IO;
using System.Linq;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Editor.Preference;
using Lunari.Tsuki.Entities.Editor;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    public partial class MotorFlightPlayer : EditorWindow {
        private Motor motor;
        private Collider2D collider;
        private MotorFlight flight;
        private int currentFrame;
        private bool isPlaying;
        private FlightPlayback playback;
        private float playbackSpeed = 1;
        private BoolEditorPreference prefixMotorStates = new BoolEditorPreference("tsuki2d.flight_player.prefix_motor_states", true);
        private BoolEditorPreference coloredPath = new BoolEditorPreference("tsuki2d.flight_player.coloredPath", true);
        private Vector2 scrollPos;
        private void Awake() {
            flight = null;
            playback = null;
        }
        private void Update() {
            if (isPlaying) {
                var dt = Time.fixedDeltaTime * playbackSpeed;
                var candidate = playback.Step(dt);
                if (candidate != currentFrame) {
                    Seek(candidate);
                }
            }
        }
        private void SetMotor(Motor motorToUse) {
            motor = motorToUse;
            collider = motor.collider;
            if (collider == null) {
                var reference = motorToUse._GetProbableColliderGameObject();
                collider = reference.GetComponentInChildren<Collider2D>();
            }
        }

        public void SetFlight(MotorFlight motorFlight) {
            flight = motorFlight;
            playback = new FlightPlayback(motorFlight);
            EnsureSceneHook();
        }

        private void EnsureSceneHook() {
            SceneView.duringSceneGui -= DrawScene;
            SceneView.duringSceneGui += DrawScene;
        }

        private void OnEnable() {
            titleContent = new GUIContent("MotorFlightPlayer");
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            isPlaying = false;
            if (flight != null && playback == null) {
                playback = new FlightPlayback(flight);
                EnsureSceneHook();
            }
        }
        private void OnPlayModeChanged(PlayModeStateChange obj) {
            EnsureSceneHook();
        }
        private void OnDisable() {
            SceneView.duringSceneGui -= DrawScene;
        }
        private void OnGUI() {
            using (var scope = new EditorGUILayout.ScrollViewScope(scrollPos)) {
               
                var candidate = (Motor) EditorGUILayout.ObjectField("Motor", motor, typeof(Motor), true);
                if (candidate != null) {
                    if (candidate != motor) {
                        SetMotor(candidate);
                    }
                } else {
                    motor = null;
                }
                
                if (flight == null) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        SelectFlightGUI();
                        if (GUILayout.Button("Close")) {
                            flight = null;
                        }
                    }
                } else {
                    FlightGUI();
                }
                prefixMotorStates.DrawField("Prefix Motor States");
                coloredPath.DrawField("Colored Path");
                scrollPos = scope.scrollPosition;
            }
            if (isPlaying) {
                Repaint();
            }
        }
        private void FlightGUI() {
            
            int selected;
            playbackSpeed = EditorGUILayout.FloatField("Playback Speed", playbackSpeed);
            using (new EditorGUILayout.HorizontalScope()) {
                var content = isPlaying ? new GUIContent(Icons.pausebutton) : new GUIContent(Icons.playbutton);
                if (GUILayout.Button(new GUIContent(Icons.animation_prevkey), GUILayout.Width(32))) {
                    Seek(currentFrame - 1);
                }
                if (GUILayout.Button(content, GUILayout.Width(32))) {
                    isPlaying = !isPlaying;
                }
                if (GUILayout.Button(new GUIContent(Icons.animation_nextkey), GUILayout.Width(32))) {
                    Seek(currentFrame + 1);
                }
                selected = EditorGUILayout.IntSlider("Current Frame", currentFrame, 0, flight.Frames.Count - 1);
            }

            if (selected != currentFrame) {
                Seek(selected);
            }

            var frames = flight?.Frames;
            if (frames != null && !frames.IsEmpty()) {
                var frame = frames[currentFrame];
                if (motor != null) {
                    Tsuki2DLayout.Velocimeter(frame.velocity.magnitude / motor.maxSpeed);
                }
                MotorDebugging.ShowFrame(frame, flight, motor);
            }


        }

        private void SelectFlightGUI() {

            if (!HasAnyFlight()) {
                EditorGUILayout.LabelField("No flights exist. You can record one using the MotorAnalyzer (Tsuki2D/MotorAnalyzer).", Styles.ErrorLabel);
            } else {
                if (GUILayout.Button("Open Flight")) {
                    var file = EditorUtility.OpenFilePanel(
                        "Select flight",
                        MotorDebugging.RecordingsFolder,
                        MotorDebugging.FlightExtension
                    );
                    if (file.IsNullOrEmpty()) {
                        return;
                    }
                    SetFlight(MotorFlight.LoadFromFile(file));
                }
            }

        }
        private bool HasAnyFlight() {
            var folder = MotorDebugging.RecordingsFolder;
            if (!Directory.Exists(folder)) {
                return false;
            }
            if (Directory.EnumerateFiles(folder, $"*.{MotorDebugging.FlightExtension}").IsEmpty()) {
                return false;
            }
            return true;
        }
        private void Seek(int selected) {
            selected = Mathf.Max(selected, 0);
            if (flight?.Frames != null) {
                var lastIndex = flight.Frames.Count - 1;
                if (selected > lastIndex) {
                    selected = Math.Min(selected, lastIndex);
                }
                if (selected == lastIndex) {
                    isPlaying = false;
                }
            }
            currentFrame = selected;
            playback.Frame = currentFrame;
        }

        private void DrawScene(SceneView sceneView) {
            if (flight == null || motor == null) {
                return;
            }
            if (flight.Frames == null) {
                return;
            }
            var frame = flight.Frames[currentFrame];
            MotorDebugging.ShowFrame(frame);
            foreach (var side in SupportSide.AllSides) {
                var hasSupport = frame.support.HasSupport(side);
                var color = hasSupport ? MotorDebugging.kCollisionEnterColor : MotorDebugging.kCollisionExitColor;
                MotorDebugging.DrawSide(frame, side, color);
            }


            var frames = flight.Frames;
            var firstIndex = 0;
            var lastIndex = frames.Count - 1;
            var firstFrame = frames[firstIndex];
            var lastFrame = frames[lastIndex];
            var lastState = firstFrame.transitionIndex;
            MotorDebugging.DrawStateLabel(flight, firstFrame, GetFramePrefix(firstIndex));
            MotorDebugging.DrawStateLabel(flight, lastFrame, GetFramePrefix(lastIndex));
            var colored = coloredPath.Value;
            if (!colored) {
                Handles.color = MotorDebugging.kCollisionEqualColor;
            }
            for (var i = 1; i < frames.Count; i++) {
                var previous = frames[i - 1];

                var current = frames[i];
                if (colored) {
                    Handles.color = MotorDebugging.kSpeedGradient.Evaluate(
                        current.velocity.magnitude / motor.maxSpeed
                    );
                }

                var currentState = current.transitionIndex;
                if (currentState != lastState) {
                    MotorDebugging.DrawStateLabel(flight, current, GetFramePrefix(i));
                    lastState = currentState;
                }
                Handles.DrawLine(previous.position, current.position, 2);
            }

            sceneView.Repaint();
        }
        private string GetFramePrefix(int frameIndex) {
            return prefixMotorStates.Value ? $"#{frameIndex}: " : null;
        }
    }

}