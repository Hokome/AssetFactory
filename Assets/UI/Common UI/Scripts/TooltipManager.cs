using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class TooltipManager : MonoBehaviour
	{
		public static TooltipManager Inst { get; private set; }

		[SerializeField] private Tooltip tooltip;
		public static Tooltip Tooltip => Inst.tooltip;

		public Canvas Canvas { get; private set; }

		void Awake()
		{
			if (Inst == null)
			{
				DontDestroyOnLoad(gameObject);
				Inst = this;
				Canvas = GetComponent<Canvas>();
			}
			else
			{
				Debug.LogWarning($"Singleton for {typeof(TooltipManager)} already exists.");
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			Tooltip.GetComponent<CanvasGroup>().alpha = 0f;
		}

		public static void Show(bool value = true)
		{
			Tooltip.Tweener.Animate(value);

		}
		public static void SetText(string content) => Tooltip.SetText(content, "");
		public static void SetText(string content, string header) => Tooltip.SetText(content, header);
		public static void SetText(TooltipData data) => Tooltip.SetText(data.content, data.header);
	}
}
