using Lunari.Tsuki.Editor.Plotting;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorAnalyzerPlot : IMGUIContainer {
        private readonly SplitPlotter _plotter;
        public MotorAnalyzerPlot(SplitPlotter plotter) {
            _plotter = plotter;
            onGUIHandler += Draw;
        }
        private void Draw() {
            _plotter.Plot(style.height.value.value);
            MarkDirtyRepaint();
        }
    }
}