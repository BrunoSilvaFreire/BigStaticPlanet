using Lunari.Tsuki.Editor;
using Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph {
    public class MotorGraphWindow : GraphViewBlackboardWindow {
        // Persisted
        [SerializeField]
        private Motor motor;
        // asdsa
        private MotorGraphView _view;
        private FloatField _radius;
        protected new void OnEnable() {
            base.OnEnable();
            titleContent = new GUIContent("Motor Graph", Resources.Load<Texture>("motor_graph_icon"));
            _view = new MotorGraphView();
            _view.StretchToParentSize();
            rootVisualElement.Add(_view);
            m_SelectedGraphView = _view;

            if (motor != null) {
                SetMotor(motor);
            }
            var blackboard = (MotorGraphBlackboard) _view.GetBlackboard();
            blackboard.Motor.OnChanged(arg0 => motor = arg0?.Motor);
        }
        private void RepositionNodes() {
            _view.RepositionNodes(_radius.value);
        }
        private void OnDisable() {
            rootVisualElement.Remove(_view);
        }

        public void SetMotor(Motor newMotor) {
            motor = newMotor;
            if (newMotor == null) {
                return;
            }

            _view.SetMotor(newMotor);
        }
    }
}