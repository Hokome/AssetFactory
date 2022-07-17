using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AssetFactory.UI
{
	public class Prompt : MonoBehaviour
	{
		[SerializeField] protected TextMeshProUGUI text;
		[SerializeField] protected CanvasGroup group;

		public void Show() => group.Display(true);
		public virtual void Hide() => group.Display(false);
	}
}
