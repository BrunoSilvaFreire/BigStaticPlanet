using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor.Utilities;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Drawers {
    [CustomPropertyDrawer(typeof(SupportRequirement))]
    public class SupportRequirementDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI2.GetHeight(3);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var flags = property
                .FindPropertyRelative(nameof(SupportRequirement.state))
                .FindPropertyRelative(nameof(SupportState.flags));

            var fields = position.SubXMax(position.height);
            var mask = flags.intValue;
            var valueProperty = property.FindPropertyRelative(nameof(SupportRequirement.value));
            DrawFields(property, fields, flags, valueProperty);

            var statePos = position.AlignRight(position.height);
            flags.intValue = (int) Tsuki2DLayout.SupportStateMaskField(
                statePos,
                valueProperty.boolValue,
                new SupportState {
                    flags = (DirectionFlags) mask
                }
            ).flags;
        }
        private static string Describe(SupportState state, bool target) {
            string value = string.Empty;
            if (!target) {
                value += "Not ";
            }
            bool hasAny = false;
            foreach (var side in SupportSide.AllSides) {
                if (state.HasSupport(side)) {
                    if (hasAny) {
                        value += ", ";
                    }
                    value += Enum.GetName(typeof(DirectionFlags), side.flags);
                    if (!hasAny) {
                        hasAny = true;
                    }
                }
            }
            return value;
        }
        private static void DrawFields(SerializedProperty property, Rect fields, SerializedProperty flags, SerializedProperty valueProperty) {
            var tSize = EditorGUI2.fullSingleLineHeight;

            var line = fields.GetLine(0);
            EditorGUI.PropertyField(
                line.SubXMax(tSize),
                flags
            );
            EditorGUI.PropertyField(
                line.AlignRight(tSize),
                valueProperty,
                GUIContent.none
            );
        }
    }
}