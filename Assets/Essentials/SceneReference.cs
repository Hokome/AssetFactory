using UnityEngine;

namespace AssetFactory
{
	[System.Serializable]
	public class SceneReference
	{
		[SerializeField] private Object sceneAsset;

		[SerializeField] private string sceneName = "";

		public string SceneName => sceneName;

		public static implicit operator string(SceneReference scene) => scene.sceneName;
	}
}