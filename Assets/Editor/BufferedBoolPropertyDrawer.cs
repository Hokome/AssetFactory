using UnityEditor;
using UnityEngine;

namespace AssetFactory.Editor
{
	[CustomPropertyDrawer(typeof(BufferedBool))]
	public class BufferedBoolPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var durationProperty = property.FindPropertyRelative("bufferDuration");
			EditorGUI.PropertyField(position, durationProperty, GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
}
