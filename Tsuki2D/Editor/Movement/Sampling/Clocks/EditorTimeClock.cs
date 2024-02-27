using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks {
    public class TimeSamplerConfiguration {
        public float sampleTime = SamplingMissionControl.DefaultTimeSpan;
        public void DrawGUI() {
            sampleTime = EditorGUILayout.FloatField("Sample time", sampleTime);
            var samplesPerSecond = 1 / sampleTime;
            samplesPerSecond = EditorGUILayout.FloatField("Sample Rate (Samples per second)", samplesPerSecond);
            if (samplesPerSecond > 0) {
                sampleTime = 1 / samplesPerSecond;
            }
        }
    }
    public class EditorTimeClock : MotorSamplingClock {
        private float sampleCooldown;
        private readonly TimeSamplerConfiguration configuration = new TimeSamplerConfiguration();
        protected override void OnStart() {
            EditorApplication.update += Step;
        }

        public TimeSamplerConfiguration Configuration => configuration;

        private void Step() {
            var deltaTime = Time.deltaTime;
            sampleCooldown -= deltaTime;
            if (sampleCooldown < 0) {
                sampleCooldown += configuration.sampleTime;
                Sample();
            }
        }
        protected override void OnStop() {
            EditorApplication.update -= Step;
        }
    }
}