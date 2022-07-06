using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	public class Settings
	{
		public static Settings current;

		public float masterVolume = .5f;
		public float musicVolume = .5f;
		public float sfxVolume = .5f;
		public float uiVolume = .5f;

		public Resolution resolution;
		public bool fullscreen = false;

		private FullScreenMode FullScreenMode => fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

		const string MASTER_VOLUME = "Master_volume";
		const string SFX_VOLUME = "SFX_volume";
		const string UI_VOLUME = "UI_volume";
		const string MUSIC_VOLUME = "Music_volume";

		const string RESOLUTION_X = "Resolution_x";
		const string RESOLUTION_Y = "Resolution_y";

		const string FULLSCREEN = "Fullscreen";

		public Settings()
		{
			if (PlayerPrefs.HasKey(MASTER_VOLUME))
				masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
			if (PlayerPrefs.HasKey(SFX_VOLUME))
				sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
			if (PlayerPrefs.HasKey(UI_VOLUME))
				uiVolume = PlayerPrefs.GetFloat(UI_VOLUME);
			if (PlayerPrefs.HasKey(MUSIC_VOLUME))
				musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);

			if (PlayerPrefs.HasKey(RESOLUTION_X) && PlayerPrefs.HasKey(RESOLUTION_Y))
			{
				resolution = new Resolution()
				{
					width = PlayerPrefs.GetInt(RESOLUTION_X),
					height = PlayerPrefs.GetInt(RESOLUTION_Y)
				};
			}
			else
			{
				resolution = Screen.currentResolution;
			}

			if (PlayerPrefs.HasKey(FULLSCREEN))
				fullscreen = PlayerPrefs.GetInt(FULLSCREEN) != 0;
			else
			{
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				fullscreen = true;
			}
		}

		public void Save()
		{
			PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolume);
			PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
			PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
			PlayerPrefs.SetFloat(UI_VOLUME, uiVolume);

			PlayerPrefs.SetInt(RESOLUTION_X, resolution.width);
			PlayerPrefs.SetInt(RESOLUTION_Y, resolution.height);

			PlayerPrefs.SetInt(FULLSCREEN, fullscreen ? 1 : 0);
		}

		public void SetScreen()
		{
			Screen.SetResolution(resolution.width, resolution.height, FullScreenMode);
		}
	}
}