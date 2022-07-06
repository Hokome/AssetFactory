using UnityEditor;
using UnityEditor.ShortcutManagement;
namespace AssetFactory
{

	[InitializeOnLoad]
	public static class PlayModeBindings
	{
		static PlayModeBindings()
		{
			EditorApplication.playModeStateChanged += ModeChanged;
			EditorApplication.quitting += Quitting;

			//foreach (var item in ShortcutManager.instance.GetAvailableProfileIds())
			//{
			//	if (item == "play")
			//		return;
			//}

			//ShortcutManager.instance.CreateProfile("Play");
		}



		private static void ModeChanged(PlayModeStateChange playModeState)
		{
			foreach (var item in ShortcutManager.instance.GetAvailableProfileIds())
			{
				if (item == "Play")
					goto Found;
			}
			return;

			Found:
			if (playModeState == PlayModeStateChange.EnteredPlayMode)
				ShortcutManager.instance.activeProfileId = "Play";
			else if (playModeState == PlayModeStateChange.EnteredEditMode)
				ShortcutManager.instance.activeProfileId = "Default";
		}
		private static void Quitting()
		{
			ShortcutManager.instance.activeProfileId = "Default";
		}
	}
}
