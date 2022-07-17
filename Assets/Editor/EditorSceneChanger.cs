using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AssetFactory.Util
{
	[InitializeOnLoad]
	public static class EditorSceneChanger
    {
		//static EditorSceneChanger()
		//{
		//	string path = @"Scenes/SetupScene.unity";
		//	string fullPath = @$"{Application.dataPath}/{path}";
		//	if (!File.Exists(fullPath)) return;
		//	SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
		//	EditorSceneManager.playModeStartScene = scene;
		//}

		[MenuItem("Edit/Change play scene")]
		public static void ChangePlayScene()
		{
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
		}
	}
}
