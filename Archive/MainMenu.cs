using AssetFactory;
using AssetFactory.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetFactory.UI
{
	public class MainMenu : MenuManager
	{
		public static MainMenu Inst { get; private set; }

		bool paused = false;
		public bool IsPaused
		{
			get => paused;
			set
			{
				paused = value;
				Time.timeScale = value ? 0f : 1f;
				if (value)
					SoleSelect("pause");
			}
		}

		public bool pausingEnabled;
		public int gameSceneIndex;

		void Awake()
		{
			if (Inst == null && Inst != this)
				Inst = this;
			else
				Destroy(this);
		}

		void Start()
		{
			Init();
		}

		public void Play()
		{
			SceneTransitioner.Inst.LoadScene(gameSceneIndex, delegate
			{
				Display(false);
				IsPaused = false;
				pausingEnabled = true;
			});
		}
		public void Pause()
		{
			if (pausingEnabled)
			{
				IsPaused = !IsPaused;
				Display(IsPaused);
			}
			else
			{
				IsPaused = false;
			}
		}
		public void ExitGame()
		{
			SceneTransitioner.Inst.LoadScene(0, delegate 
			{
				ToMain();
				IsPaused = false;
				pausingEnabled = false;
			});
		}
		public void Quit() => Application.Quit();
	}
}