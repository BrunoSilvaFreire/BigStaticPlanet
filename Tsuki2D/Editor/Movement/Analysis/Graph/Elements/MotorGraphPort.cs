using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph.Elements {
    public class MotorGraphPort : Port {
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener() {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position) {
            }

            public void OnDrop(GraphView graphView, Edge edge) {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single) {
                    foreach (var connection in edge.input.connections) {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (edge.output.capacity == Capacity.Single) {
                    foreach (var connection in edge.output.connections) {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (m_EdgesToDelete.Count > 0) {
                    graphView.DeleteElements(m_EdgesToDelete);
                }
                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null) {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }
                foreach (var edge1 in edgesToCreate) {
                    graphView.AddElement(edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }
            }
        }
        private class CompositeListener : IEdgeConnectorListener {
            private List<IEdgeConnectorListener> listeners;
            public CompositeListener(List<IEdgeConnectorListener> listeners) {
                this.listeners = listeners;
            }
            public void OnDropOutsidePort(Edge edge, Vector2 position) {
                foreach (var l in listeners) {
                    l.OnDropOutsidePort(edge, position);
                }
            }
            public void OnDrop(GraphView graphView, Edge edge) {
                foreach (var l in listeners) {
                    l.OnDrop(graphView, edge);
                }
            }
        }
        public MotorGraphPort(
            IEdgeConnectorListener listener,
            Orientation portOrientation,
            Direction portDirection,
            Capacity portCapacity,
            Type type
        ) : base(portOrientation, portDirection, portCapacity, type) {
            m_EdgeConnector = new EdgeConnector<Edge>(new CompositeListener(new List<IEdgeConnectorListener> {
                new DefaultEdgeConnectorListener(),
                listener
            }));
            this.AddManipulator(m_EdgeConnector);
        }
    }
}