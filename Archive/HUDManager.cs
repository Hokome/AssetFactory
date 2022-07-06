using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
	public class HUDManager : MonoBehaviour
	{
		public static HUDManager Inst { get; private set; }

		void Awake()
		{
			if (Inst == null && Inst != this)
			{
				DontDestroyOnLoad(gameObject);
				Inst = this;
			}
			else
				Destroy(this);
		}
	}
}