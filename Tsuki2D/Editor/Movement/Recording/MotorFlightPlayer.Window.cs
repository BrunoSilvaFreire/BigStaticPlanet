using UnityEditor;
namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    public partial class MotorFlightPlayer {
        [MenuItem("Tools/Tsuki2D/MotorFlightPlayer")]
        public static MotorFlightPlayer Open() {
            var motorAnalyzer = CreateWindow<MotorFlightPlayer>();
            motorAnalyzer.ShowTab();
            return motorAnalyzer;
        }
    }
}