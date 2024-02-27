using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor.Utilities;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Drawers {
    [CustomPropertyDrawer(typeof(SupportRequirementNode))]
    public class SupportRequirementNodeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var mode = property.FindPropertyRelative(nameof(SupportRequirementNode.mode));
            var modeEnum = (SupportRequirementNode.EvaluationMode) mode.enumValueIndex;
            float height = 0;
            height += EditorGUI.GetPropertyHeight(mode);
            switch (modeEnum) {
                case SupportRequirementNode.EvaluationMode.Self:
                    height += EditorGUI2.fullSingleLineHeight;
                    var requirement = property.FindPropertyRelative(nameof(SupportRequirementNode.requirement));
                    height += EditorGUI.GetPropertyHeight(requirement);
                    break;
                case SupportRequirementNode.EvaluationMode.All:
                case SupportRequirementNode.EvaluationMode.Any:
                case SupportRequirementNode.EvaluationMode.None:
                    var children = property.FindPropertyRelative(nameof(SupportRequirementNode.children));
                    height += EditorGUI2.GetHeight(2);
                    if (children.isExpanded) {
                        height += EditorGUI2.fullSingleLineHeight;
                        for (var i = 0; i < children.arraySize; i++) {
                            height += GetPropertyHeight(
                                children.GetArrayElementAtIndex(i),
                                GUIContent.none
                            );
                            height += EditorGUIUtility.standardVerticalSpacing + 4;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var mode = property.FindPropertyRelative(nameof(SupportRequirementNode.mode));
            var modeEnum = (SupportRequirementNode.EvaluationMode) mode.enumValueIndex;
            EditorGUI.LabelField(
                position.GetLine(0),
                label
            );


            position = position.AddXMin(16F);
            EditorGUI.PropertyField(
                position.GetLine(1),
                mode
            );
            switch (modeEnum) {
                case SupportRequirementNode.EvaluationMode.Self:
                    var mask = property.FindPropertyRelative(nameof(SupportRequirementNode.requirement));
                    EditorGUI.PropertyField(
                        position.GetLine(2).SetHeight(EditorGUI.GetPropertyHeight(mask)),
                        mask
                    );
                    break;
                case SupportRequirementNode.EvaluationMode.All:
                case SupportRequirementNode.EvaluationMode.Any:
                case SupportRequirementNode.EvaluationMode.None:
                    var children = property.FindPropertyRelative(nameof(SupportRequirementNode.children));
                    EditorGUI.PropertyField(
                        position.AddYMin(EditorGUI2.GetHeight(2)),
                        children
                    );
                    break;
            }

        }
    }
}