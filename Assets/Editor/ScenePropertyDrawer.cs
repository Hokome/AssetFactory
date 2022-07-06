
using UnityEditor;
using UnityEngine;

namespace AssetFactory.Editor
{
	[CustomPropertyDrawer(typeof(SceneReference))]
	public class SceneReferencePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			
			SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
			SerializedProperty sceneName = property.FindPropertyRelative("sceneName");

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			if (sceneAsset != null)
			{
				sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if (sceneAsset.objectReferenceValue != null)
				{
					SceneAsset asset = (sceneAsset.objectReferenceValue as SceneAsset);
					sceneName.stringValue = asset.name;
				}
			}
			EditorGUI.EndProperty();
		}
	}

}