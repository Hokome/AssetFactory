using AssetFactory.Legacy.UI.InputRebinding;
using UnityEditor;
using UnityEngine;

namespace AssetFactory.Legacy.Editor
{
    [CustomPropertyDrawer(typeof(InputIconIdentifier))]
    public class IconDictionaryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect bindingPosition = position;
            bindingPosition.width /= 2;
            Rect spritePosition = bindingPosition;
            spritePosition.x += bindingPosition.width + 2;
            spritePosition.width -= 2;

            var bindingProperty = property.FindPropertyRelative("key");
            var spriteProperty = property.FindPropertyRelative("sprite");
            EditorGUI.PropertyField(bindingPosition, bindingProperty, GUIContent.none);
            EditorGUI.PropertyField(spritePosition, spriteProperty, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
