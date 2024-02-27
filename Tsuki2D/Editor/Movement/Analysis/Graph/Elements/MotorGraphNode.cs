using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements {


    public sealed class MotorGraphNode : Node {

        private static readonly Color ActiveColor = new Color(0.55f, 1f, 0.61f, 0.96f);
        private static readonly Color InputPortColor = new Color(0.64f, 0.64f, 0.64f);
        private static readonly Color[] PrettyColors = {
            new Color(0.7F, 0.28F, 0.382F, 1F),
            new Color(0.28F, 0.693F, 0.7F, 1F),
            new Color(0.27f, 0.27f, 0.69f),
            new Color(0.27f, 0.68f, 0.27f)
        };

        private readonly Port _inputPort;
        private readonly List<Port> _outputs;

        private readonly VisualElement _activeStateBorder;
        private readonly VisualElement _attachmentsContainer;
        public MotorGraphNode(IEdgeConnectorListener listener, MotorGraph data, int stateIndex) {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/MotorGraphNode"));
            var state = data[stateIndex];
            var exits = data.EdgesFrom(stateIndex).ToList();
            var entries = data.EdgesTo(stateIndex).ToList();
            _outputs = new List<Port>();
            _activeStateBorder = new VisualElement {
                name = "active-state-border",
                pickingMode = PickingMode.Ignore
            };
            extensionContainer.Add(_attachmentsContainer = new VisualElement());
            foreach (var attachment in state.attachments) {
                var txt = attachment.GetType().GetLegibleName();
                var pill = new Box();
                pill.AddToClassList("motor-attachment");
                pill.Add(new Label(txt));

                _attachmentsContainer.Add(pill);
            }
            for (var j = 0; j < exits.Count; j++) {
                var output = exits[j];
                var transition = output.Item1;
                var componentField = transition.TransitionCandidate;
                var instigator = componentField.Owner;
                var instigatorType = instigator.GetType();
                var port = new MotorGraphPort(
                    listener,
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Single,
                    instigatorType
                ) {
                    portColor = GetColor(j, instigator.GetHashCode())
                };
                var fieldTypeStr = transition.TransitionCandidate.Field.FieldType.GetLegibleName();
                var portName = $"{transition.TransitionCandidate.Field.Name} ({fieldTypeStr})";
                if (instigator != state) {
                    portName = $"{instigatorType.GetLegibleName()}: {portName}";
                }
                port.portName = portName;
                port.userData = transition.TransitionCandidate;
                outputContainer.Add(port);
                _outputs.Add(port);
            }
            _inputPort = InstantiatePort(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Multi,
                typeof(MotorComponent)
            );
            _inputPort.userData = state;
            _inputPort.portName = "Entrypoint";
            _inputPort.portColor = InputPortColor;
            inputContainer.Add(_inputPort);
            Add(_activeStateBorder);
        }
        private static Color GetColor(int index, int hashCode) {
            if (index >= PrettyColors.Length) {
                return GetColor(hashCode);
            }
            return PrettyColors[index];
        }
        private static Color GetColor(MotorComponent instigator) {
            var hashCode = instigator.GetHashCode();
            return GetColor(hashCode);
        }
        private static Color GetColor(int hashCode) {

            var bytes = BitConverter.GetBytes(hashCode);
            Color c = new Color32(
                bytes[0],
                bytes[1],
                bytes[2],
                bytes[3]
            );
            return c.SetAlpha(1).SetSaturation(.6F).SetBrightness(0.7F);
        }

        public Port GetInput() {
            return _inputPort;
        }
        public Port GetOutput(int index) {
            return _outputs[index];
        }
        public void SetActive(bool isStateActive) {
            const string clazz = "motor-state-active";
            if (isStateActive) {
                _activeStateBorder.AddToClassList(clazz);
            } else {
                _activeStateBorder.RemoveFromClassList(clazz);
            }
        }
    }
}