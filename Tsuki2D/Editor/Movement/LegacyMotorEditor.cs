using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Editor.Extenders;
using Lunari.Tsuki.Editor.Utilities;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using Lunari.Tsuki2D.Runtime.Movement.Updaters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
namespace Lunari.Tsuki2D.Editor.Movement {
    //[CustomEditor(typeof(Motor), true)]
    public class LegacyMotorEditor : UnityEditor.Editor {
        private const float BoxSize = 16 + 8;
        private Motor motor;
        private TemplateContainer root;
        private TypeSelectorButton<MotorState> statePicker;
        private Texture2D up, down, left, right, user;
        private UnityEditor.Editor updaterEditor;
        private TypeSelectorButton<SupportStateUpdater> updaterPicker;

        private void OnEnable() {
            motor = (Motor) target;
            up = Resources.Load<Texture2D>("arrow-up-thick");
            down = Resources.Load<Texture2D>("arrow-down-thick");
            left = Resources.Load<Texture2D>("arrow-left-thick");
            right = Resources.Load<Texture2D>("arrow-right-thick");
            user = Resources.Load<Texture2D>("account-circle");
            updaterPicker = new TypeSelectorButton<SupportStateUpdater>(
                new ModularContent<GUIContent>(() =>
                    new GUIContent(motor.updater == null
                        ? "Pick an updater"
                        : motor.updater.GetType().GetLegibleName())),
                OnUpdaterPicked,
                typeFilter: type => type.IsAbstract
            );
            statePicker = new TypeSelectorButton<MotorState>(
                new GUIContent("Add a motor state"),
                OnMotorStatePicked,
                typeFilter: type => type.IsAbstract
            );
        }

        private void OnMotorStatePicked(Type type) {
            var parent = motor.gameObject;
            var child = new GameObject(type.Name);
            child.transform.SetParent(parent.transform);
            child.AddComponent(type);
        }

        private void OnUpdaterPicked(Type type) {
            if (motor.updater != null) {
                DestroyImmediate(motor.updater);
            }

            var updater = (SupportStateUpdater) motor.gameObject.AddComponent(type);
            updater.hideFlags = HideFlags.HideInInspector;
            EditorUtility.SetDirty(updater);
            AssetDatabase.SaveAssets();
            motor.updater = updater;
            updaterEditor = null;
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(motor.maxSpeed)));
            motor.Control = EditorGUILayout.FloatField("Master Control", motor.Control);
            using (new EditorGUI.IndentLevelScope()) {
                motor.LeftControl = EditorGUILayout.FloatField("Left Control", motor.LeftControl);
                motor.RightControl = EditorGUILayout.FloatField("Right Control", motor.RightControl);
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PrefixLabel("Updater");
                updaterPicker.OnInspectorGUI();
            }

            var updater = motor.updater;
            if (updater != null) {
                using (new EditorGUI.IndentLevelScope()) {
                    updaterEditor ??= CreateEditor(updater);
                    updaterEditor.OnInspectorGUI();
                    // HideFlags doesn't seem to persist between loads, NO MATTER, WE BRUTE FORCE IT
                    updater.hideFlags = HideFlags.HideInInspector;
                }
            }

            var inGame = EditorApplication.isPlaying;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(motor.input)));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("activeState"),
                new GUIContent(inGame ? "Active State" : "Initial State")
            );
            if (inGame) {
                using (new GUIEnabledScope(false)) {
                    using (new EditorGUI.IndentLevelScope()) {
                        EditorGUILayout.ObjectField("Next State", motor.NextState, typeof(MotorState), true);
                    }
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(Styles.box, GUILayout.Width(BoxSize * 4))) {
                using (new GUIEnabledScope(false)) {
                    EditorGUILayout.EnumFlagsField(motor.supportState.flags);
                }

                DrawSupportPreview();
            }

            DrawComponentList(
                "Global Attachments",
                motor.globalAttachments,
                false,
                attachment => {
                    motor.globalAttachments.Add(attachment);
                    EditorUtility.SetDirty(motor);
                },
                index => motor.globalAttachments.RemoveAt(index)
            );
            var entity = motor.GetComponentInParent<Entity>();
            if (entity == null) {
                return;
            }

            var states = entity.GetComponentsInChildren<MotorState>();
            if (states.IsNullOrEmpty()) {
                return;
            }

            DrawComponentList("States", states, true);
        }

        private void DrawComponentList<T>(
            string header,
            IEnumerable<T> components,
            bool drawPicker,
            UnityAction<T> onAdded = null,
            UnityAction<int> onRemove = null
        ) where T : MotorComponent {
            var isPlaying = EditorApplication.isPlaying;

            using (new EditorGUILayout.VerticalScope(Tsuki2DStyles.GroupStyle)) {
                EditorGUILayout.LabelField(header, Styles.BoldLabel);
                if (onAdded != null) {
                    var component = (T) EditorGUILayout.ObjectField("Drop here to add", null, typeof(T), true);
                    if (component != null) {
                        onAdded(component);
                    }
                }

                if (drawPicker) {
                    statePicker.OnInspectorGUI();
                }


                var arr = components.ToArray();
                for (var i = 0; i < arr.Length; i++) {
                    Tsuki2DStyles.DrawSeparator();
                    var component = arr[i];

                    var shouldHighlight = isPlaying && motor.ActiveState != null && component == motor.ActiveState;
                    using (new GUIColorScope(shouldHighlight, new Color32(0, 230, 118, 255),
                        GUIColorScope.ColorTarget.Content)) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            if (component == null) {
                                if (onRemove != null) {
                                    EditorGUILayout.PrefixLabel("Null component (Please remove)");
                                    if (Tsuki2DStyles.DestroyButton()) {
                                        onRemove(i);
                                    }
                                } else {
                                    EditorGUILayout.PrefixLabel("Null component");
                                }

                                continue;
                            }

                            var componentName = component.GetType().Name;

                            EditorGUILayout.PrefixLabel(componentName);
                            if (GUILayout.Button("Select")) {
                                Selection.activeObject = component;
                            }

                            if (component is MotorState actual) {
                                var isInitial = component == motor.ActiveState;
                                if (!isInitial && GUILayout.Button("Set as initial")) {
                                    motor.SetMotorStateUnsafe(actual);
                                }
                            }

                            if (Tsuki2DStyles.DestroyButton()) {
                                var go = component.gameObject;
                                var existing = go.GetComponents<Component>();
                                var isOnly = existing.Length == 2;
                                DestroyImmediate(component);
                                if (isOnly) {
                                    DestroyImmediate(go);
                                }
                            }

                            if (component != null && component.gameObject == motor.gameObject) {
                                if (GUILayout.Button("Move to own game object")) {
                                    var child = new GameObject(componentName);
                                    child.transform.SetParent(motor.transform);
                                    var newObj = child.AddComponent(component.GetType());
                                    EditorUtility.CopySerialized(component, newObj);
                                    DestroyImmediate(component);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawSupportPreview() {
            var rect = GUILayoutUtility.GetRect(
                BoxSize * 3,
                BoxSize * 3,
                GUILayout.ExpandWidth(false)
            ).AddX(BoxSize).AddY(BoxSize);
            DrawSupportPreview(
                up,
                rect,
                Vector2Int.down,
                motor.supportState,
                DirectionFlags.Up
            );
            DrawSupportPreview(
                down,
                rect,
                Vector2Int.up,
                motor.supportState,
                DirectionFlags.Down
            );
            DrawSupportPreview(
                left,
                rect,
                Vector2Int.left,
                motor.supportState,
                DirectionFlags.Left
            );
            DrawSupportPreview(
                right,
                rect,
                Vector2Int.right,
                motor.supportState,
                DirectionFlags.Right
            );
            GUI.DrawTexture(rect.SetSize(BoxSize, BoxSize), user);
        }

        private void DrawSupportPreview(
            Texture2D texture2D,
            Rect rect,
            Vector2Int direction,
            SupportState supportState,
            DirectionFlags flags
        ) {
            if (texture2D == null) {
                Debug.LogWarning($"No texture found for direction {direction}");
                return;
            }

            if ((supportState.flags & flags) == flags) {
                using (new GUIColorScope(new Color(0.32f, 1f, 0.41f))) {
                    var iconRect = rect.SetSize(BoxSize, BoxSize).AddX(direction.x * BoxSize)
                        .AddY(direction.y * BoxSize);
                    GUI.DrawTexture(iconRect, texture2D);
                }
            }
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}