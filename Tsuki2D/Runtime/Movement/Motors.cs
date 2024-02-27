using System;
using System.Linq;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki.Entities.Problems;
using Lunari.Tsuki2D.Runtime.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
#endif

namespace Lunari.Tsuki2D.Runtime.Movement {
    [Serializable]
    public class MotorEvent : UnityEvent<Motor> { }

    public static class Motors {
        public static void TryTick(this MotorComponent component, Motor motor, ref Vector2 velocity) {
#if UNITY_EDITOR
            if (CheckComponentNull(component, motor)) {
                return;
            }
#endif
            if (component.enabled) {
                component.Tick(motor, ref velocity);
            }
        }

        private static bool CheckComponentNull(MotorComponent component, Motor motor) {
            if (component) {
                return false;
            }

            Debug.LogWarning(
                $"Found null motor component in {motor} (In Component: '{component}'), this will cause a NullReferenceException in release.",
                motor
            );
            return true;
        }

        public static void TryBegin(this MotorComponent component, Motor motor, ref Vector2 velocity) {
#if UNITY_EDITOR
            if (CheckComponentNull(component, motor)) {
                return;
            }
#endif
            if (component.enabled) {
                component.Begin(motor, ref velocity);
            }
        }

        public static void TryEnd(this MotorComponent component, Motor motor, ref Vector2 velocity) {
#if UNITY_EDITOR
            if (CheckComponentNull(component, motor)) {
                return;
            }
#endif
            if (component.enabled) {
                component.End(motor, ref velocity);
            }
        }

        public class MotorInputOfTypeMissing<T> : Problem where T : EntityInput {
            public MotorInputOfTypeMissing(ITrait requisitor, Motor motor, Entity entity) : base(requisitor,
                entity, $"Motor {motor} doesn't have input of type {typeof(T).Name} required by {requisitor}") {
                var possibilities = entity.gameObject.GetComponentsInChildren<EntityInput>().OfType<T>();
                foreach (var possibility in possibilities) {
                    WithSolution($"Assign {possibility} as input", () => {
                        motor.input = possibility;
#if UNITY_EDITOR
                        EditorUtility.SetDirty(motor);
#endif
                    });
                }
            }
        }

#if TSUKI_ENTITIES
        public static bool RequiresMotorInputOfType<T>(this TraitDescriptor descriptor, out Motor motor, out T input)
            where T : EntityInput {
            
            if (descriptor.Access(out motor)) {
                if (motor.TryGetInput(out input)) {
                    return true;
                }

                descriptor.Problems.Add(new MotorInputOfTypeMissing<T>(descriptor.Of, motor, descriptor.Entity));
            } else {
                descriptor.DependsOn(out motor); // Add missing motor problem
            }

            input = null;
            return false;
        }
#endif
    }
}