using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
	[SerializeField] GameObject pauseMenu;
	public bool stopTime;
	public UnityEvent<bool> onPause;

	bool p = false;
	public bool Pause
	{
		get => p; set
		{
			p = value;
			if (stopTime)
				Time.timeScale = p ? 0f : 1f;
			onPause.Invoke(p);
		}
	}
}
