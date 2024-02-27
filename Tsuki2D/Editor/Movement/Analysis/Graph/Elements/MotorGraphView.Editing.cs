using System;
using System.Collections.Generic;
using Lunari.Tsuki;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements {
    public partial class MotorGraphView {
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            var list = new List<Port>();
            switch (startPort.direction) {
                case Direction.Input:
                    if (!(startPort.userData is MotorState entryPoint)) {
                        return Collections.EmptyList<Port>();
                    }
                    ports.ForEach(port => {
                        if (port.direction == Direction.Input) {
                            return;
                        }
                        if (!(port.userData is MotorComponentTransitionCandidate exitPoint)) {
                            return;
                        }
                        if (!exitPoint.IsSuitableInput(entryPoint)) {
                            return;
                        }
                        list.Add(port);
                    });
                    break;
                case Direction.Output: {
                    if (!(startPort.userData is MotorComponentTransitionCandidate exitPoint)) {
                        return Collections.EmptyList<Port>();
                    }
                    ports.ForEach(port => {
                        if (port.direction == Direction.Output) {
                            return;
                        }
                        if (!(port.userData is MotorState entryPoint)) {
                            return;
                        }
                        if (!exitPoint.IsSuitableInput(entryPoint)) {
                            return;
                        }
                        list.Add(port);
                    });
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return list;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position) {
        }
        public void OnDrop(GraphView graphView, Edge edge) {
            if (!(edge.input.userData is MotorState state)) {
                return;
            }
            if (!(edge.output.userData is MotorComponentTransitionCandidate candidate)) {
                return;
            }
            candidate.Set(state);
        }
    }

}