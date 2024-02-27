using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Editor.Preference;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine.UI;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorAnalyzerSettingsView : VisualElement {
        private VisualElement container;
        private ObjectField _motorField;
        private readonly Slot<Motor> _motor;
        private readonly Slot<MotorSamplingClock> _samplingClock;
        private readonly ToolbarMenu _samplingMethodDropdown;
        private IntegerField _ringBufferSize;
        private FloatField _sceneLineWidth;
        private readonly HelpBox _noMotorHelpBox;
        private readonly VisualElement _helpBoxContainer;
        public MotorAnalyzerSettingsView(Slot<Motor> motor, Slot<MotorSamplingClock> samplingClock) {
            _motor = motor;
            _samplingClock = samplingClock;
            var background = new Box {
                name = "settings-content",
            };
            container = new Foldout {
                text = "Settings",
                value = false
            };
            var header = container.Q<Label>();
            header.name = "settings-title";
            header.AddToClassList("motor-analyzer-header");
            background.AddToClassList("motor-card");
            background.Add(container);
            container.AddToClassList("unity-title");
            container.hierarchy.Insert(1, _motorField = new ObjectField("Motor") {
                objectType = typeof(Motor)
            });

            {
                var sContainer = new MotorAnalyzerSection("Sampling");
                _samplingMethodDropdown = new ToolbarMenu();
                _samplingMethodDropdown.menu.AppendAction(
                    "None",
                    action => {
                        _samplingClock.Value = null;
                    }
                );
                foreach (var pair in MotorSamplingRegistry.Registry) {
                    var clockType = pair.Key;
                    var factory = pair.Value;
                    _samplingMethodDropdown.menu.AppendAction(
                        clockType.GetLegibleName(),
                        action => _samplingClock.Value = factory(_motor.Value)
                    );
                }

                sContainer.Add(_samplingMethodDropdown);
                _ringBufferSize = new IntegerField("Ring Buffer Size");
                _ringBufferSize.Bind(MotorAnalyzerSettings.RingBufferSize);
                sContainer.Add(_ringBufferSize);
                container.Add(sContainer);
            }
            {
                var sceneContainer = new MotorAnalyzerSection("Scene");
                _sceneLineWidth = new FloatField("Scene Line Width") {
                    value = 1.0F
                };
                sceneContainer.Add(_sceneLineWidth);
                container.Add(sceneContainer);
            }
            {
                var infoContainer = new MotorAnalyzerSection("Info");
                _noMotorHelpBox = new HelpBox("No motor currently assigned. Info is not available", HelpBoxMessageType.Warning);
                _helpBoxContainer = new VisualElement();
                _helpBoxContainer.Add(_noMotorHelpBox);
                infoContainer.Add(_helpBoxContainer);
                container.Add(infoContainer);
            }
            Add(background);

            _motor
                .SyncWith(_motorField)
                .WhenNull(OnMotorNull)
                .WhenNotNull(OnMotorAvailable);

            _samplingClock
                .WhenNull(OnSamplerNull)
                .WhenNotNull(OnSamplerAvailable);
        }
        private void OnSamplerAvailable(MotorSamplingClock clock) {
            SetSamplingNameToCurrentClock();
        }
        private void OnSamplerNull() {
            RefreshSamplerText();
        }
        private void RefreshSamplerText() {
            if (_samplingClock.Value != null) {
                SetSamplingNameToCurrentClock();
            } else {
                _samplingMethodDropdown.text = _motor.Value != null ? "Select a sampling method" : "Please select a motor to analyze before picking sampler";
            }
        }
        private void SetSamplingNameToCurrentClock() {

            _samplingMethodDropdown.text = _samplingClock.Value.GetType().GetLegibleName();
        }
        private void OnMotorAvailable(Motor arg0) {
            var last = MotorSamplingRegistry.GetLastSelectedSamplerFactory();
            if (last != null) {
                var motor = _motor.Value;
                if (motor != null) {
                    _samplingClock.Value = last(motor);
                }
            }
            AddToClassList("has-motor");
            RefreshSamplerText();
            _samplingMethodDropdown.SetEnabled(true);
            _helpBoxContainer.Remove(_noMotorHelpBox);
            _noMotorHelpBox.style.visibility = Visibility.Hidden;
        }
        private void OnMotorNull() {
            RemoveFromClassList("has-motor");
            RefreshSamplerText();
            _samplingMethodDropdown.SetEnabled(false);
            _helpBoxContainer.Add(_noMotorHelpBox);
            _noMotorHelpBox.style.visibility = Visibility.Visible;
        }
    }
}