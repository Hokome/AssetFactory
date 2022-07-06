#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Util
{
	[CreateAssetMenu(menuName = "Build Settings", order = 1000)]
	public class AutoBuildSettings : ScriptableObject
	{
		public List<SceneAsset> scenes;

		public string[] GetScenesPath()
		{
			string[] sc = new string[scenes.Count];
			for (int i = 0; i < sc.Length; i++)
				sc[i] = $"Assets/Scenes/{scenes[i].name}.unity";
			return sc;
		}
	}
}
#endif