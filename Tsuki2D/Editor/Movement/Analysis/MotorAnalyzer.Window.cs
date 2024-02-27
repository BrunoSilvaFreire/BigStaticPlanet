using UnityEditor;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        [MenuItem("Tools/Tsuki2D/MotorAnalyzer")]
        public static MotorAnalyzerWindow Open() {
            var motorAnalyzer = CreateWindow<MotorAnalyzerWindow>();
            motorAnalyzer.ShowTab();
            return motorAnalyzer;
        }
    }
}