using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor.Plotting;
using Lunari.Tsuki.Editor.Preference;
using Lunari.Tsuki.Misc;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Editor.Movement.Storage;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorAnalyzer : VisualElement {
        private readonly MotorAnalyzerPlot _velocityPlot;
        private readonly Toggle _playButton;
        private bool _sampling;
        private readonly Toggle _record;
        private readonly SplitPlotter _plotter;
        private Plot _xPlot;
        private Plot _yPlot;
        private DisposableListener _ringBufferListener;
        private MotorFramePage _storage;
        private readonly Slot<Motor> _motor;
        private readonly Slot<MotorSamplingClock> _samplingClock;
        private MinMaxSlider _xMinMaxSlider;
        private MinMaxSlider _yMinMaxSlider;
        private Toolbar _recordingToolbar;
        private VisualElement _motorButtons;
        public MotorAnalyzer(Slot<Motor> motor, Slot<MotorSamplingClock> samplingClock) {
            _motor = motor;
            _samplingClock = samplingClock;
            _xPlot = new Plot();
            _yPlot = new Plot();
            _xPlot.Color = Color.green;
            _yPlot.Color = Color.red;
            _ringBufferListener = MotorAnalyzerSettings.RingBufferSize.Changed.AddDisposableListenerAndInvoke(OnRingBufferSizeChanged);
            _plotter = new SplitPlotter(_xPlot, _yPlot);
            _velocityPlot = new MotorAnalyzerPlot(_plotter);
            _velocityPlot.style.height = 350;
            _storage = new MotorFramePage(MotorAnalyzerSettings.RingBufferSize.Value);
            var box = new Box();
            var header = new Label("Analyzer");
            header.AddToClassList("motor-analyzer-header");
            box.Add(header);
            _recordingToolbar = new Toolbar();
            {
                _motorButtons = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row
                    }
                };
                _playButton = new Toggle();
                _playButton.AddToClassList("play-toggle");
                _motorButtons.Add(_playButton);
                _playButton.RegisterValueChangedCallback(OnPlayChanged);
                _record = new Toggle {
                    text = "Record"
                };
                _record.AddToClassList("recording-toggle");

                _motorButtons.Add(_record);
                _motor
                    .OnChanged(UpdateToolbar)
                    .WhenNull(() => _playButton.value = false);

                _recordingToolbar.Add(_motorButtons);
            }
            var filler = new VisualElement {
                style = {
                    flexGrow = 1
                }
            };
            _recordingToolbar.Add(filler);
            var showXToggle = new ToolbarToggle {
                text = "Show X"
            };
            showXToggle.Bind(MotorAnalyzerSettings.ShowXInPlot);
            var showYToggle = new ToolbarToggle {
                text = "Show Y"
            };
            showYToggle.Bind(MotorAnalyzerSettings.ShowYInPlot);
            var autoScalePlot = new ToolbarToggle {
                text = "Auto Scale Plot"
            };
            autoScalePlot.Bind(MotorAnalyzerSettings.AutoScalePlot);
            autoScalePlot.AddToClassList("analyzer-right-border");
            _recordingToolbar.Add(showXToggle);
            _recordingToolbar.Add(showYToggle);
            _recordingToolbar.Add(autoScalePlot);
            showXToggle.Bind(MotorAnalyzerSettings.AutoScalePlot);
            box.Add(_recordingToolbar);
            _xPlot.Enabled = MotorAnalyzerSettings.ShowXInPlot.Value;
            _yPlot.Enabled = MotorAnalyzerSettings.ShowYInPlot.Value;
            showXToggle.RegisterValueChangedCallback(evt => {
                _xPlot.Enabled = evt.newValue;
            });
            showYToggle.RegisterValueChangedCallback(evt => {
                _yPlot.Enabled = evt.newValue;
            });
            box.AddToClassList("motor-card");
            {
                var limits = new Foldout {
                    text = "Limits",
                    value = false
                };
                limits.SetEnabled(!autoScalePlot.value);
                autoScalePlot.RegisterValueChangedCallback(evt => {
                    limits.SetEnabled(!evt.newValue);
                });
                const float defaultLimits = 100;
                _yMinMaxSlider = new MinMaxSlider("Y Min-Max", minLimit: 0, maxLimit: defaultLimits);
                _xMinMaxSlider = new MinMaxSlider("X Min-Max", minLimit: -defaultLimits, maxLimit: defaultLimits);
                var xLimitsSlider = new Vector2Field("X slider limits") {
                    value = new Vector2(_xMinMaxSlider.lowLimit, _xMinMaxSlider.highLimit)
                };
                var yLimitsSlider = new Vector2Field("Y slider limits") {
                    value = new Vector2(_yMinMaxSlider.lowLimit, _yMinMaxSlider.highLimit)
                };
                xLimitsSlider.RegisterValueChangedCallback(evt => {
                    _xMinMaxSlider.lowLimit = xLimitsSlider.value.x;
                    _xMinMaxSlider.highLimit = xLimitsSlider.value.y;
                });
                yLimitsSlider.RegisterValueChangedCallback(evt => {
                    _yMinMaxSlider.lowLimit = yLimitsSlider.value.x;
                    _yMinMaxSlider.highLimit = yLimitsSlider.value.y;
                });
                _xMinMaxSlider.RegisterValueChangedCallback(evt => {
                    _xPlot.Min = evt.newValue.x;
                    _xPlot.Max = evt.newValue.y;
                });
                _yMinMaxSlider.RegisterValueChangedCallback(evt => {
                    _yPlot.Min = evt.newValue.x;
                    _yPlot.Max = evt.newValue.y;
                });
                limits.Add(xLimitsSlider);
                limits.Add(yLimitsSlider);
                limits.Add(_xMinMaxSlider);
                limits.Add(_yMinMaxSlider);
                box.Add(limits);
            }
            box.Add(_velocityPlot);
            Add(box);
            _motor.When(arg0 => UpdateToolbar());
        }
        private void UpdateToolbar() {
            var canAnalyze = EditorApplication.isPlayingOrWillChangePlaymode && _motor.Value != null && _samplingClock.Value != null;
            _motorButtons.SetEnabled(canAnalyze);
        }
        private void OnPlayChanged(ChangeEvent<bool> evt) {
            var clock = _samplingClock.Value;
            if (clock == null) {
                return;
            }
            if (evt.newValue) {
                clock.Start(Sample);
            } else {
                clock.Stop();
            }
        }
        private void Sample() {
            var motor = _motor.Value;
            if (motor == null) {
                return;
            }
            try {

                var frame = MotorFrame.Sample(motor, motor.rigidbody);
                _storage.Push(frame);
                var i = 0;
                var minX = float.PositiveInfinity;
                var maxX = float.NegativeInfinity;
                var minY = float.PositiveInfinity;
                var maxY = float.NegativeInfinity;
                foreach (var f in _storage.GetFrames()) {
                    var velX = Mathf.Abs(f.velocity.x);
                    var velY = f.velocity.y;
                    _xPlot.Data[i] = velX;
                    _yPlot.Data[i] = velY;
                    i++;
                    minX = Mathf.Min(minX, velX);
                    maxX = Mathf.Max(maxX, velX);
                    minY = Mathf.Min(minY, velY);
                    maxY = Mathf.Max(maxY, velY);
                }
                if (MotorAnalyzerSettings.AutoScalePlot.Value) {
                    _xMinMaxSlider.minValue = minX;
                    _xMinMaxSlider.maxValue = maxX;
                    _yMinMaxSlider.minValue = minY;
                    _yMinMaxSlider.maxValue = maxY;
                }
            }
            catch (Exception e) {
                Debug.LogError(e, motor);
                _playButton.value = false;
                throw;
            }
        }

        private void OnRingBufferSizeChanged() {
            var size = MotorAnalyzerSettings.RingBufferSize.Value;
            if (size <= 0) {
                size = 10;
                MotorAnalyzerSettings.RingBufferSize.Value = 10;
            }
            _xPlot.Resize(size);
            _yPlot.Resize(size);
            _storage = new MotorFramePage(size);
        }
    }
}