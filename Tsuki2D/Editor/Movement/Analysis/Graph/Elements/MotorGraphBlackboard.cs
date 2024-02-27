using System.Collections.Generic;
using Lunari.Tsuki;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements {
    public class MotorGraphBlackboard : Blackboard, IEdgeConnectorListener {
        private readonly BlackboardSection _motorSection;
        public MotorGraphBlackboard(GraphView associatedGraphView = null) : base(associatedGraphView) {
            this.Q<Resizer>().RemoveFromHierarchy();
            hierarchy.Add(new ResizableElement());
            scrollable = true;
            style.width = 400;
            contentContainer.Add(MotorField = new ObjectField("Motor") {
                allowSceneObjects = true,
                objectType = typeof(Motor)
            });
            contentContainer.Add(_motorSection = new BlackboardSection());
            MotorField.RegisterValueChangedCallback(evt => SetMotor((Motor) evt.newValue));
            Motor.OnValueRemoved(ClearAttachments);
            Motor.WhenNotNull(AttachToNewValue);
        }
        
        private void AttachToNewValue(MotorGraph motor) {
            title = motor.Motor.name;
            _motorSection.title = $"{motor.Size} states.";
            foreach (var vertex in motor.Vertices) {
                var field = new ObjectField(vertex.name) {
                    objectType = vertex.GetType(),
                    value = vertex
                };
                field.SetEnabled(false);
                _motorSection.contentContainer.Add(field);
            }
        }
        private void ClearAttachments(MotorGraph old) {
            title = "Motor Graph";
            _motorSection.title = "Load a motor in order to show states.";
            _motorSection.contentContainer.Clear();
        }

        public ObjectField MotorField { get; }

        public Slot<MotorGraph> Motor { get; } = new Slot<MotorGraph>();
        public void SetMotor(Motor motor) {

            SetMotor(motor == null ? null : new MotorGraph(motor));
        }
        public void SetMotor(MotorGraph motor) {
            if (Motor.Value == motor) {
                return;
            }
            var actual = motor?.Motor;
            Motor.Value = motor;
            if (MotorField.value != actual) {
                MotorField.value = actual;
            }
        }
        public void OnDropOutsidePort(Edge edge, Vector2 position) {
            
        }
        public void OnDrop(GraphView graphView, Edge edge) {
        }
    }
}