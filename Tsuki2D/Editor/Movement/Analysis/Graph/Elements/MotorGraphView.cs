using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements {
    public partial class MotorGraphView : GraphView, IEdgeConnectorListener {
        private MotorState _lastState, _currentState;
        private readonly Dictionary<MotorState, MotorGraphNode> _nodeCache = new Dictionary<MotorState, MotorGraphNode>();
        private readonly MotorGraphBlackboard _blackboard;
        public MotorGraphView() {
            // Manipulators
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/MotorGraph"));
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            // Elements
            _blackboard = new MotorGraphBlackboard(this) {
                title = "Motor Graph",
                capabilities = Capabilities.Resizable | Capabilities.Movable
            };
            Add(_blackboard);

            var grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);

            // Slots
            _blackboard.Motor.WhenNotNull(m => {
                UpdateToMotor();
                _blackboard.Motor.Listen(m.Motor.onStateChanged, OnStateChanged);
            }).OnValueRemoved(arg0 => {
                nodes.ForEach(node => node.RemoveFromHierarchy());
                edges.ForEach(edge => edge.RemoveFromHierarchy());
            });
        }
        private void OnStateChanged() {
            _lastState = _currentState;
            _currentState = _blackboard.Motor.Value.Motor.NextState;
            MotorGraphNode current = null;
            var hasCurrent = _currentState != null && _nodeCache.TryGetValue(_currentState, out current);
            MotorGraphNode last = null;
            var hasLast = _lastState != null && _nodeCache.TryGetValue(_lastState, out last);
            if (hasCurrent) {
                current.SetActive(true);
            }
            if (hasLast) {
                last.SetActive(false);
            }
        }

        public void RepositionNodes(float radius) {
            var i = 0;
            nodes.ForEach(node => {
                var pos = node.GetPosition();
                var angle = (float) i++ / _blackboard.Motor.Value.Size * 360 * Mathf.Deg2Rad;
                pos.x = Mathf.Cos(angle) * radius;
                pos.y = Mathf.Sin(angle) * radius;
                node.SetPosition(pos);
            });
        }
        public void SetMotor(Motor m) {
            _blackboard.SetMotor(m);
            UpdateToMotor();
        }
        private void UpdateToMotor() {

            var data = _blackboard.Motor.Value;
            var index2Node = new Dictionary<int, MotorGraphNode>();
            for (var i = 0; i < data.Size; i++) {
                var state = data[i];
                var node = new MotorGraphNode(this, data, i) {
                    title = state.name
                };
                node.capabilities &= ~Capabilities.Deletable;
                _nodeCache[state] = node;
                index2Node[i] = node;
                AddElement(node);
            }
            for (var i = 0; i < data.Size; i++) {
                var state = data[i];
                var node = index2Node[i];
                var outputs = data.EdgesFrom(i).ToList();
                for (var j = 0; j < outputs.Count; j++) {
                    var output = outputs[j];
                    var other = index2Node[output.Item2];
                    var outgoing = node.GetOutput(j);
                    var edge = outgoing.ConnectTo<Edge>(other.GetInput());
                    edge.capabilities &= ~Capabilities.Deletable;
                    edge.visible = true;
                    AddElement(edge);
                }
            }
            RepositionNodes(GetDefaultRadius(data.Size));
            foreach (var pair in index2Node) {
                var node = pair.Value;
                node.RefreshExpandedState();
                node.RefreshPorts();
            }
        }

        public static float GetDefaultRadius(int numElements) {
            return 112.5F * numElements;
        }

        public override Blackboard GetBlackboard() {
            return _blackboard;
        }

    }
}