using Lunari.Tsuki.Editor.Preference;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public static class MotorAnalyzerSettings {
        public static readonly IntEditorPreference RingBufferSize = new IntEditorPreference("tsuki2d.motor_analyzer.ring_buffer_size", 300);
        public static readonly StringEditorPreference LastSelectedSamplingClock = new StringEditorPreference("tsuki2d.motor_analyzer.ring_buffer_size", string.Empty);
        public static readonly BoolEditorPreference AutoScalePlot = new BoolEditorPreference("tsuki2d.motor_analyzer.auto_scale_plot", true);
        public static readonly BoolEditorPreference ShowXInPlot = new BoolEditorPreference("tsuki2d.motor_analyzer.show_x_in_plot", true);
        public static readonly BoolEditorPreference ShowYInPlot = new BoolEditorPreference("tsuki2d.motor_analyzer.show_y_in_plot", true);
    }
}