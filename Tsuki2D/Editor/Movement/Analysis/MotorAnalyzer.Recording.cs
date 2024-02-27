using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using UnityEditor;
using UnityEngine;
using MotorFlightPlayer = Lunari.Tsuki2D.Editor.Movement.Recording.MotorFlightPlayer;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        private bool recording;
        private MotorRecorder recorder;
        private void RecordingGUI() {
            using (new EditorGUILayout.HorizontalScope()) {

                var sampling = IsSampling;
                var analyzeIcon = sampling ? Icons.record_on : Icons.record_off;
                var analyzeMsg = sampling ? "Stop Sampling " : "Start Sampling";
                var hasMotorAndClock = samplingMissionControl.Motor != null && samplingMissionControl.Clock != null;
                var canStartAnalyzing = EditorApplication.isPlaying && hasMotorAndClock;
                using (new GUIEnabledScope(canStartAnalyzing)) {
                    if (GUILayout.Button(new GUIContent(analyzeMsg, analyzeIcon))) {
                        if (sampling) {
                            StopSampling();
                        } else {
                            StartSampling();
                        }
                    }
                }
                var icon = recording ? Icons.record_on : Icons.record_off;
                var msg = recording ? "Recording... " : "Record motor";
                using (new GUIEnabledScope(EditorApplication.isPlaying && hasMotorAndClock)) {
                    if (GUILayout.Button(new GUIContent(msg, icon))) {
                        if (recording) {
                            StopRecording();
                        } else {
                            StartRecording();
                        }
                    }
                }
            }
        }
        private void StopRecording() {
            if (!recording) {
                return;
            }
            recorder.Finish();
        }

        public void StartRecording() {
            if (recording) {
                return;
            }
            recording = true;
            var motor = _motor.Current;
            if (motor == null) {
                return;
            }
            var sampler = samplingMissionControl.Clock;
            recorder = new MotorRecorder(motor, sampler, 64);
            recorder.OnFinished.AddSingleFireListener(delegate(MotorFlight flight) {
                recording = false;
                var player = CreateWindow<MotorFlightPlayer>();
                var motorToUse = _motor.Current;
                var s = DateTime.Now.ToString("MM_dd_yyyy_HH_m_s");
                var output = $"{MotorDebugging.RecordingsFolder}/{s}_{motor}.{MotorDebugging.FlightExtension}";
                Files.EnsureParentFolderExists(output);
                flight.SaveToFile(output);
                player.SetFlight(flight);
                player.ShowAuxWindow();
            });
        }
    }

}