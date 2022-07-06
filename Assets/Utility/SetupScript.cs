using AssetFactory.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

//Originally from AssetFactory
namespace AssetFactory
{
	public class SetupScript : MonoBehaviour
    {
		public static bool HasInitialized { get; private set; }

		private void Start()
		{
			Settings.current = new Settings();
			Settings.current.SetScreen();
			SceneManager.LoadScene(1);
			AudioManager.Inst.UpdateMixers();

			HasInitialized = true;
		}
	}
}
