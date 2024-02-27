using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lunari.Tsuki.Graphs;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Graph {
    /**
     * This is a field found in a MotorComponent of type MotorState that is Unity serializable.
     * Which means that it very possibly is a state that a motor can transition to.
     */
    public class MotorComponentTransitionCandidate {
        public MotorComponentTransitionCandidate(MotorComponent owner, FieldInfo field) {
            Owner = owner;
            Field = field;
        }

        public MotorComponent Owner { get; }

        public FieldInfo Field { get; }

        public bool IsSuitableInput(MotorState other) {
            return Field.FieldType.IsInstanceOfType(other);
        }
        public void Set(MotorState other) {
            EditorUtility.SetDirty(Owner);
            Field.SetValue(Owner, other);
        }
    }
    public class MotorGraph : AdjacencyList<MotorState, MotorGraph.StateTransition> {
        private Dictionary<Type, List<MotorState>> statesByType;
        public class StateTransition {
            /**
             * The component that initiates this transition.
             * Maybe be a motor state or one of it's attachments
             */
            public MotorComponentTransitionCandidate TransitionCandidate;
        }
        public MotorGraph(Motor m) {
            var currentState = m.ActiveState;
            var state2Index = new Dictionary<int, int>();
            Motor = m;
            statesByType = new Dictionary<Type, List<MotorState>>();
            ProcessState(currentState, state2Index);
        }

        public Motor Motor { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="graph"></param>
        /// <param name="state2Index"></param>
        /// <returns>The index of the state vertex in the graph</returns>
        private int ProcessState(MotorState state, Dictionary<int, int> state2Index) {
            if (!statesByType.TryGetValue(state.GetType(), out var list)) {
                list = new List<MotorState>();
                statesByType[state.GetType()] = list;
            }
            list.Add(state);
            var stateIndex = AddVertex(state);
            state2Index[state.GetInstanceID()] = stateIndex;
            ProcessComponent(state2Index, state, stateIndex);
            foreach (var attachment in state.attachments) {
                ProcessComponent(state2Index, attachment, stateIndex);
            }
            return stateIndex;
        }
        private void ProcessComponent(Dictionary<int, int> state2Index, MotorComponent component, int stateIndex) {
            foreach (var info in GetAllMotorStateFields(component.GetType())) {
                var neighbor = info.GetValue(component) as MotorState;
                if (neighbor == null) {
                    continue;
                }
                if (!state2Index.TryGetValue(neighbor.GetInstanceID(), out var neighborIndex)) {
                    neighborIndex = ProcessState(neighbor, state2Index);
                    if (neighborIndex == -1) {
                        continue;
                    }
                }
                Connect(
                    stateIndex, neighborIndex,
                    new StateTransition {
                        TransitionCandidate = new MotorComponentTransitionCandidate(component, info)
                    }
                );
            }
        }
        private static IEnumerable<FieldInfo> GetAllMotorStateFields(Type type) {
            var allFields = new List<FieldInfo>();
            allFields.AddRange(
                type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            );
            allFields.AddRange(
                type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(info => info.GetCustomAttribute<SerializeField>() != null)
            );
            ;
            return allFields.Where(info => typeof(MotorState).IsAssignableFrom(info.FieldType) && info.GetCustomAttribute<ExcludeFromMotorGraphAttribute>() == null);
        }
    }
}