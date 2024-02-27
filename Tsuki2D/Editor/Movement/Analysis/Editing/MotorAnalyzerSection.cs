using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorAnalyzerSection : VisualElement {
        public Foldout Foldout { get; }
        public MotorAnalyzerSection(string title) {
            AddToClassList("motor-analyzer-section");
            Foldout = new Foldout {
                text = title
            };
            Foldout.AddToClassList("motor-analyzer-section-title");
            hierarchy.Add(Foldout);
        }

        public override VisualElement contentContainer => Foldout;
    }
}