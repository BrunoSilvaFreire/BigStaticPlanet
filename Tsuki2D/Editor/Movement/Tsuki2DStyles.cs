using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Scopes;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement {
    internal static class Tsuki2DStyles {
        private static GUIStyle groupStyle;

        public static GUIStyle GroupStyle {
            get {
                if (groupStyle == null) {
                    groupStyle = new GUIStyle(Styles.GroupBox);
                    groupStyle.padding.left += 8;
                }

                return groupStyle;
            }
        }

        public static void DrawSeparator() {
            const float separatorSize = 16F;
            var rect = GUILayoutUtility.GetRect(0, separatorSize, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint) {
                using (new GUIColorScope(new Color(0.1f, 0.1f, 0.1f))) {
                    GUI.DrawTexture(rect.SetHeight(1F).SubXMin(17F).AddXMax(9F).AddY(separatorSize / 2),
                        Texture2D.whiteTexture);
                }
            }
        }

        public static bool DestroyButton() {
            return GUILayout.Button(Icons.treeeditor_trash, GUILayout.Height(22), GUILayout.Width(22));
        }
    }
}