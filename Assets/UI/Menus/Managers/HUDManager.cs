using UnityEngine;
using UnityEngine.UI;
using AssetFactory.UI;
using TMPro;

//Originally from AssetFactory
namespace AssetFactory
{
	[RequireComponent(typeof(CanvasGroup))]
	public class HUDManager : MonoSingleton<HUDManager>
    {
		private CanvasGroup main;

		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
			main = GetComponent<CanvasGroup>();
			enabled = false;
		}

		private void OnEnable()
		{
			main.Display(true, false);
		}
		private void OnDisable()
		{
			main.Display(false, false);
		}
	}
}
