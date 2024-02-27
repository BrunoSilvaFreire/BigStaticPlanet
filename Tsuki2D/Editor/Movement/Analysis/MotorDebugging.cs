using System;
using System.Globalization;
using System.IO;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Recording;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public static class MotorDebugging {

        public static readonly Color kCollisionEnterColor = new Color(1f, 0.46f, 0f, 0.63f);
        public static readonly Color kCollisionExitColor = new Color(0.34f, 1f, 0f, 0.63f);
        public static readonly Color kCollisionEqualColor = new Color(0f, 0f, 0f, 0.63f);
        public static readonly Color kRecordingSelected = new Color(0.28f, 0f, 1f, 1);
        public static readonly Color kIdleSpeedColor = new Color(0.45f, 0.52f, 0.36f);
        public static readonly Color kLowSpeedColor = kCollisionExitColor;
        public static readonly Color kMediumSpeedColor = kCollisionEnterColor;
        public static readonly Color kHighSpeedColor = new Color(1, 0, 0, 0.63F);
        public static readonly Gradient kSpeedGradient = new Gradient {
            colorKeys = new[] {
                new GradientColorKey(kIdleSpeedColor, 0),
                new GradientColorKey(kLowSpeedColor, 0.5F),
                new GradientColorKey(kMediumSpeedColor, 0.9F),
                new GradientColorKey(kHighSpeedColor, 1)
            }
        };
        public const string FlightExtension = "tsuki2d.flight";

        private const float kExtrusionDistance = 0.125F;

        public static void DrawSide(MotorFrame frame, SupportSide side, Color col) {

            Rect rect = default;
            var bounds = frame.bounds;
            rect.min = bounds.Min;
            rect.max = bounds.Max;
            float limit;
            switch (side.flags) {
                case DirectionFlags.Up:
                    limit = rect.yMax;
                    rect.yMax += kExtrusionDistance;
                    rect.yMin = limit;
                    break;
                case DirectionFlags.Down:
                    limit = rect.yMin;
                    rect.yMax = limit;
                    rect.yMin -= kExtrusionDistance;
                    break;
                case DirectionFlags.Left:
                    limit = rect.xMin;
                    rect.xMax = limit;
                    rect.xMin -= kExtrusionDistance;
                    break;
                case DirectionFlags.Right:
                    limit = rect.xMax;
                    rect.xMax += kExtrusionDistance;
                    rect.xMin = limit;
                    break;
                case DirectionFlags.Horizontal:
                case DirectionFlags.Vertical:
                case DirectionFlags.All:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Handles.DrawSolidRectangleWithOutline(
                rect,
                col,
                Color.clear
            );
        }
        public static string RecordingsFolder => Path.GetFullPath($"{Application.dataPath}/../Temp/Tsuki2D/Recordings");
        public static void ShowFrame(MotorFrame frame) {
            using (new GUIEnabledScope(false)) {
                EditorGUILayout.LabelField("Time", frame.time.ToString(CultureInfo.InvariantCulture));
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.VerticalScope()) {
                        EditorGUILayout.Vector2Field("Position", frame.position);
                        EditorGUILayout.Vector2Field("Velocity", frame.velocity);
                        EditorGUILayout.FloatField("Speed", frame.velocity.magnitude);
                    }
                    using (new EditorGUILayout.VerticalScope()) {
                        Tsuki2DLayout.SupportStateField(frame.support);
                    }
                }
                EditorGUILayout.BoundsField("Bounds", frame.bounds);
                EditorGUILayout.IntField("Transition Index", frame.transitionIndex);
            }
        }
        public static void DrawStateLabel(MotorFlight flight, MotorFrame current, string prefix = null) {
            var transition = flight.GetTransition(current);
            DrawStateLabel(current, transition, prefix);
        }
        public static void DrawStateLabel(MotorFrame current, StateTransitionStack transition, string prefix) {
            string label = null;
            if (transition == null) {
                if (current.transitionIndex > 0) {
                    label = $"{current.transitionIndex} (Transition not found)";
                }
            } else {
                label = transition.stateName;
            }
            if (label != null) {
                if (!prefix.IsNullOrEmpty()) {
                    label = prefix + label;
                }
                Handles.Label(current.position, label);
            }

            Handles2.DrawWireCircle2D(current.position, 0.125F, kRecordingSelected);
        }
        public static void ShowFrame(MotorFrame frame, MotorFlight flight, Motor m) {

            ShowFrame(frame);
            var transition = flight.GetTransition(frame);
            if (transition != null) {
                EditorGUILayout.LabelField("This transition has occured at the following point:");
                foreach (var stackFrame in transition.frames) {
                    using (new GUIEnabledScope(stackFrame.CanBeOpened())) {
                        if (GUILayout.Button(stackFrame.ToString(), Styles.LinkLabel)) {
                            OpenStackFrame(stackFrame);
                        }
                    }
                }
            }
        }
        private static void OpenStackFrame(StateTransitionFrame stackFrame) {

            var path = Paths.RelativeTo(stackFrame.file, Application.dataPath);
            var file = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            AssetDatabase.OpenAsset(file, stackFrame.line, stackFrame.column);
        }
    }

}