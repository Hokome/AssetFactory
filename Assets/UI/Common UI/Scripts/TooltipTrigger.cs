using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public TooltipData data;
		[SerializeField] private float delay = 0.7f;

		private LTDescr descr;

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (string.IsNullOrEmpty(data.content))
				return;
			TooltipManager.SetText(data);
			descr = LeanTween.delayedCall(delay, () =>
			{
				TooltipManager.Show(true);
			});
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (descr != null)
				LeanTween.cancel(descr.uniqueId);
			TooltipManager.Show(false);
		}
	}
}
