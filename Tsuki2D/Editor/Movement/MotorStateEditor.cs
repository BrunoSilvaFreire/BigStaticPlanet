using System;
using System.Collections.Generic;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Editor.Extenders;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lunari.Tsuki2D.Editor.Movement {
    [CustomEditor(typeof(MotorState), true)]
    public class MotorStateEditor : UnityEditor.Editor {
        private TypeSelectorButton<MotorAttachment> attachment;
        private bool drawAttachments;
        private Dictionary<MotorAttachment, UnityEditor.Editor> editors;
        private MotorState state;

        private static Color BackgroundColor => EditorGUIUtility.isProSkin
            ? new Color32(194, 194, 194, 127)
            : new Color32(56, 56, 56, 127);

        private void OnEnable() {
            editors = new Dictionary<MotorAttachment, UnityEditor.Editor>();
            state = (MotorState) target;
            attachment = new TypeSelectorButton<MotorAttachment>(
                new GUIContent("Add an attachment"),
                OnSelected,
                null,
                type => {
                    if (type.IsAbstract) {
                        return true;
                    }

                    if (IsSubclassRawGeneric(typeof(MotorAttachmentOfState<>), type.BaseType)) {
                        var required = type.BaseType.GenericTypeArguments[0];
                        if (!required.IsInstanceOfType(state)) {
                            return true;
                        }
                    }

                    return false;
                });
        }

        private static bool IsSubclassRawGeneric(Type generic, Type toCheck) {
            while (toCheck != null && toCheck != typeof(object)) {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        private void OnSelected(Type type) {
            var component = (MotorAttachment) state.gameObject.AddComponent(type);
            component.hideFlags = HideFlags.HideInInspector;
            state.attachments.Add(component);
        }

        public override void OnInspectorGUI() {
            using (new EditorGUILayout.VerticalScope(Tsuki2DStyles.GroupStyle)) {
                base.OnInspectorGUI();
            }

            DrawAttachments();
        }

        private void DrawAttachments() {
            using (new EditorGUILayout.VerticalScope(Tsuki2DStyles.GroupStyle)) {
                var attachments = state.attachments;
                if (attachments == null) {
                    state.attachments = attachments = new List<MotorAttachment>();
                }

                using (new EditorGUILayout.HorizontalScope()) {
                    drawAttachments = EditorGUILayout.Foldout(
                        drawAttachments,
                        "Attachments (" + attachments.Count + "):",
                        Styles.FoldoutHeader
                    );
                    attachment.OnInspectorGUI();
                }

                var component = (MotorAttachment) EditorGUILayout.ObjectField(
                    "Drop here to add",
                    null,
                    typeof(MotorAttachment),
                    true
                );
                if (component != null) {
                    attachments.Add(component);
                    EditorUtility.SetDirty(state);
                }

                if (!drawAttachments) {
                    return;
                }

                var toDelete = new List<MotorAttachment>();
                using (new EditorGUI.ChangeCheckScope()) {

                }
                var previous = state.attachments.Count;
                state.attachments.RemoveAll(attached => attached == null);
                foreach (var attached in state.attachments) {
                    attached.hideFlags = HideFlags.HideInInspector;
                }
                var attachmentsCount = state.attachments.Count;
                if (previous != attachmentsCount) {
                    EditorUtility.SetDirty(target);
                }
                EditorGUILayout.Space();
                for (var i = 0; i < attachmentsCount; i++) {
                    using (new EditorGUILayout.VerticalScope()) {
                        var stateAttachment = state.attachments[i];
                        if (!editors.TryGetValue(stateAttachment, out var child)) {
                            child = CreateEditorWithContext(new Object[] {
                                    stateAttachment
                                },
                                state
                            );
                            editors[stateAttachment] = child;
                        }

                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            if (Tsuki2DStyles.DestroyButton()) {
                                toDelete.Add(stateAttachment);
                                continue;
                            }
                        }

                        child.OnInspectorGUI();
                        if (i != attachmentsCount - 1) {
                            Tsuki2DStyles.DrawSeparator();
                        }
                    }
                }

                foreach (var motorAttachment in toDelete) {
                    RemoveAttachment(motorAttachment);
                }
            }
        }


        private void RemoveAttachment(MotorAttachment motorAttachment) {
            state.attachments.Remove(motorAttachment);
            if (editors.TryGetValue(motorAttachment, out var editor)) {
                DestroyImmediate(editor);
                editors.Remove(motorAttachment);
            }

            DestroyImmediate(motorAttachment);
        }
    }
}