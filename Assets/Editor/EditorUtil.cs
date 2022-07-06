#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AssetFactory.Util
{
	[InitializeOnLoad]
	public class EditorUtil
    {
		static EditorUtil()
		{

		}
		
		[MenuItem("Edit/Change play scene")]
		public static void ChangePlayScene()
		{
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
		}
	}
}
#endif