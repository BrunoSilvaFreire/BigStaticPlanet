using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Scopes;
using Lunari.Tsuki2D.Editor.Movement.Analysis;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement {
    public static class Tsuki2DLayout {
        public static void SupportStateField(GUIContent label, SupportState state) {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PrefixLabel(label);
                SupportStateField(state);
            }
        }
        [InitializeOnLoadMethod]
        private static void LoadTextures() {
            up = Resources.Load<Texture2D>("arrow-up-thick");
            down = Resources.Load<Texture2D>("arrow-down-thick");
            left = Resources.Load<Texture2D>("arrow-left-thick");
            right = Resources.Load<Texture2D>("arrow-right-thick");
            user = Resources.Load<Texture2D>("account-circle");
        }
        private static Texture2D up, down, left, right, user;
        private const float BoxSize = 16 + 8;
        public static void SupportStateField(SupportState supportState) {
            var rect = GUILayoutUtility.GetRect(
                BoxSize * 3,
                BoxSize * 3,
                GUILayout.ExpandWidth(false)
            ).AddX(BoxSize).AddY(BoxSize);
            SupportStateField(rect, supportState, false);
        }
        public static void SupportStateField(
            Rect rect,
            SupportState supportState,
            bool drawAbsent
        ) {
            SupportStateField(
                up,
                rect,
                Vector2Int.down,
                supportState,
                DirectionFlags.Up,
                drawAbsent
            );
            SupportStateField(
                down,
                rect,
                Vector2Int.up,
                supportState,
                DirectionFlags.Down,
                drawAbsent
            );
            SupportStateField(
                left,
                rect,
                Vector2Int.left,
                supportState,
                DirectionFlags.Left,
                drawAbsent
            );
            SupportStateField(
                right,
                rect,
                Vector2Int.right,
                supportState,
                DirectionFlags.Right,
                drawAbsent
            );
            var perimeter = Mathf.Min(rect.width, rect.height) / 3;
            var centerPos = rect.SetSizeKeepCenter(new Vector2(perimeter, perimeter));
            GUI.DrawTexture(
                centerPos,
                user
            );
        }
        public static SupportState SupportStateMaskField(
            Rect rect,
            bool mustBeSet,
            SupportState supportState
        ) {
            var color = mustBeSet ? MotorDebugging.kCollisionExitColor : MotorDebugging.kCollisionEnterColor;
            GUI.Box(rect, GUIContent.none);

            supportState = SupportStateField(
                up,
                rect,
                Vector2Int.down,
                supportState,
                DirectionFlags.Up,
                color
            );
            supportState = SupportStateField(
                down,
                rect,
                Vector2Int.up,
                supportState,
                DirectionFlags.Down,
                color
            );
            supportState = SupportStateField(
                left,
                rect,
                Vector2Int.left,
                supportState,
                DirectionFlags.Left,
                color
            );
            supportState = SupportStateField(
                right,
                rect,
                Vector2Int.right,
                supportState,
                DirectionFlags.Right,
                color
            );
            var perimeter = Mathf.Min(rect.width, rect.height) / 3;
            var centerPos = rect.SetSizeKeepCenter(new Vector2(perimeter, perimeter));
            GUI.DrawTexture(
                centerPos,
                user
            );
            return supportState;
        }

        private static void SupportStateField(
            Texture2D icon,
            Rect rect,
            Vector2Int direction,
            SupportState supportState,
            DirectionFlags flags,
            bool drawAbsent
        ) {
            if (icon == null) {
                Debug.LogWarning($"No texture found for direction {direction}");
                return;
            }

            if ((supportState.flags & flags) == flags) {
                DrawDirectionIcon(icon, rect, direction, MotorDebugging.kCollisionExitColor);
            } else if (drawAbsent) {
                DrawDirectionIcon(icon, rect, direction, MotorDebugging.kCollisionEnterColor);
            }
        }
        private static SupportState SupportStateField(
            Texture2D icon,
            Rect rect,
            Vector2Int direction,
            SupportState supportState,
            DirectionFlags flags,
            Color color
        ) {
            if (icon == null) {
                Debug.LogWarning($"No texture found for direction {direction}");
                return supportState;
            }
            var cpy = supportState;
            using (new GUIColorScope(color)) {
                var perimeter = Mathf.Min(rect.width, rect.height) / 3;
                Rect iconPos = default;
                iconPos.center = rect.center;

                var size = new Vector2(perimeter, perimeter);
                iconPos = iconPos
                    .SetSizeKeepCenter(size)
                    .AddPosition((Vector2) direction * perimeter);
                switch (Event.current.type) {
                    case EventType.MouseDown:
                        if (iconPos.Contains(Event.current.mousePosition)) {
                            cpy.ToggleFlags(flags);
                        }
                        break;
                    case EventType.Repaint:
                        if ((cpy.flags & flags) == flags) {
                            GUI.DrawTexture(iconPos, icon);
                        }
                        break;
                }
            }
            return cpy;
        }
        private static bool DrawDirectionIcon(Texture icon, Rect rect, Vector2Int direction, Color color) {
            var clicked = false;
            using (new GUIColorScope(color)) {
                var perimeter = Mathf.Min(rect.width, rect.height) / 3;
                Rect iconPos = default;
                iconPos.center = rect.center;

                var size = new Vector2(perimeter, perimeter);
                iconPos = iconPos
                    .SetSizeKeepCenter(size)
                    .AddPosition((Vector2) direction * perimeter);
                switch (Event.current.type) {
                    case EventType.MouseDown:
                        if (iconPos.Contains(Event.current.mousePosition)) {
                            clicked = true;
                        }
                        break;
                    case EventType.Repaint:
                        GUI.DrawTexture(iconPos, icon);
                        break;
                }
            }
            return clicked;
        }

        public static void Velocimeter(float speedPercent) {
            var color = MotorDebugging.kSpeedGradient.Evaluate(speedPercent);
            color.a = 1;
            const float height = 16;
            var rect = GUILayoutUtility.GetRect(
                0,
                float.PositiveInfinity,
                height,
                height
            ).Padding(
                height / 2,
                height / 4
            );
            using (new GUIColorScope(MotorDebugging.kCollisionEqualColor)) {
                GUI.DrawTexture(rect, Textures.WhiteTexture);
            }
            using (new GUIColorScope(color)) {
                var indicator = rect.SetWidth(rect.width * speedPercent);
                GUI.DrawTexture(indicator, Textures.WhiteTexture);
            }
        }
    }
}