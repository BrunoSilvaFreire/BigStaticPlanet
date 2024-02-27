using Lunari.Tsuki.Editor.Utilities;
using Lunari.Tsuki2D.Runtime.Input;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement {
    [CustomPropertyDrawer(typeof(EntityAction))]
    public class PlatformerActionDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var platformerAction = property.FindInstanceWithin<EntityAction>();
            platformerAction.Current = EditorGUI.Toggle(position, label, platformerAction.Current);
        }
    }
}