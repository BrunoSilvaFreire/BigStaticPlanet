using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor.Extenders;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
namespace Lunari.Tsuki2D.Editor.Movement.Sampling {
    public class SamplingMissionControl {
        public const float DefaultTimeSpan = 1F / 60F;
        public const float DefaultTotalDuration = 5; // Seconds
        public static readonly int DefaultRingBufferSize = Mathf.RoundToInt(DefaultTotalDuration / DefaultTimeSpan);

        private readonly DropdownButton samplerSelector;
        private int ringBufferSize = DefaultRingBufferSize;
        private MotorSamplingClock clock;
        public SamplingMissionControl() {
            samplerSelector = MotorSamplingRegistry.CreateClockSelectorButton(OnSamplerSelected);
        }

        public Motor Motor {
            get;
            set;
        }

        private void OnSamplerSelected(int index, Type item) {
            Clock = MotorSamplingRegistry.CreateClockByType(Motor, item);
        }
        public void OnGUI() {
            using (new GUIEnabledScope(Motor != null)) {
                samplerSelector.OnGUI();
                var oldNumSamples = ringBufferSize;
                var candidate = EditorGUILayout.IntField("Ring Buffer Size", oldNumSamples);
                if (candidate != 0) {
                    RingBufferSize = candidate;
                }
                if (Clock is EditorTimeClock timeSampler) {
                    timeSampler.Configuration.DrawGUI();
                }
            }
        }

        public int RingBufferSize {
            get => ringBufferSize;
            set {
                if (value == ringBufferSize) {
                    return;
                }
                ringBufferSize = value;
                OnBufferSizeChanged.Invoke(value);
            }
        }

        public UnityEvent<int> OnBufferSizeChanged {
            get;
        } = new UnityEvent<int>();

        public UnityEvent OnClockChanged {
            get;
        } = new UnityEvent();

        public MotorSamplingClock Clock {
            get => clock;
            set {
                if (Equals(clock, value)) {
                    return;
                }
                clock = value;
                if (value != null) {
                    samplerSelector.Label = new GUIContent(value.GetType().GetLegibleName());
                } else {
                    samplerSelector.Label = MotorSamplingRegistry.GetDefaultSamplerLabel();
                }
                OnClockChanged.Invoke();
            }
        }
    }
}