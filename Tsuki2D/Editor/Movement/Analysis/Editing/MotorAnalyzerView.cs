using Lunari.Tsuki;
using Lunari.Tsuki.Editor.Plotting;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEngine;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorAnalyzerView : VisualElement {
        private readonly MotorAnalyzerSettingsView _settingsView;
        private readonly MotorAnalyzer _analyzer;
        public MotorAnalyzerView() {
            var scroll = new ScrollView();

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/MotorAnalyzer"));

            SamplingSlot = new Slot<MotorSamplingClock>()
                .WhenNull(now => {
                    MotorAnalyzerSettings.LastSelectedSamplingClock.Value = string.Empty;
                }).WhenNotNull(now => {
                    MotorAnalyzerSettings.LastSelectedSamplingClock.Value = now.GetType().FullName;
                });
            MotorSlot = new Slot<Motor>()
                .WhenNotNull(m => {
                    SamplingSlot.Value = new InjectedFixedTimeClock(m);
                }).WhenNull(() => {
                    SamplingSlot.Value = null;
                });

            scroll.Add(_settingsView = new MotorAnalyzerSettingsView(MotorSlot, SamplingSlot));
            scroll.Add(_analyzer = new MotorAnalyzer(MotorSlot, SamplingSlot));
            Add(scroll);

        }

        public Slot<Motor> MotorSlot { get; }

        public Slot<MotorSamplingClock> SamplingSlot { get; }
    }
}