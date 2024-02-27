using System;
using Lunari.Tsuki.Editor.Extenders;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        private MotorFramePage analysisRecorder;
        private SamplingMissionControl samplingMissionControl;
        private MotorSampler loop;

        private bool IsSampling => loop != null && loop.Sampling;
        private void InitializeSampling() {
            samplingMissionControl = new SamplingMissionControl();
            OnResized(samplingMissionControl.RingBufferSize);
            samplingMissionControl.OnBufferSizeChanged.AddListener(OnResized);
            samplingMissionControl.OnClockChanged.AddListener(OnSamplerChanged);
            analysisRecorder = new MotorFramePage(SamplingMissionControl.DefaultRingBufferSize);
        }
        private void SamplingOptionsGUI() {
            samplingMissionControl.OnGUI();
        }
        private void OnSamplerChanged() {
            if (IsSampling) {
                StartSampling();
            }
        }
        private void StartSampling() {
            var clock = samplingMissionControl.Clock;
            if (clock == null) {
                return;
            }
            if (_motor.Current == null) {
                return;
            }
            HookOntoScene();
            loop?.Stop();
            loop = new MotorSampler(
                clock,
                _motor,
                analysisRecorder
            );
            loop.Start();
        }
        private void StopSampling() {
            loop?.Stop();
            SceneView.duringSceneGui -= Draw;
        }

    }
}