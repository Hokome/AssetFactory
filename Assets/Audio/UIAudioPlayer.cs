using AssetFactory.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class UIAudioPlayer : MonoBehaviour,
		IPointerEnterHandler, IPointerClickHandler, ISelectHandler
	{
		[SerializeField] private SFXClip hoverSound;
		[SerializeField] private SFXClip selectSound;
		[SerializeField] private SFXClip clickSound;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (clickSound != null)
				AudioManager.PlaySound(clickSound);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (hoverSound != null)
				AudioManager.PlaySound(hoverSound);
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (selectSound != null)
				AudioManager.PlaySound(selectSound);
		}
	}
}
