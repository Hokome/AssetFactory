using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
    public class SpriteFlash : MonoBehaviour
    {
		[SerializeField] private float duration = 2f;
		[SerializeField] private float flickerFrequency = 0.1f;
		[SerializeField] private SpriteRenderer sprite;
		public SpriteRenderer SpriteRenderer
		{
			get
			{
				if (sprite == null)
					sprite = GetComponent<SpriteRenderer>();
				return sprite;
			}
			private set => sprite = value;
		}

		private Coroutine routine;
		public void StartFlash() => StartFlash(duration);
		public void StartFlash(float duration)
		{
			if (routine != null)
				StopCoroutine(routine);
			routine = StartCoroutine(Flash(duration));
		}
		private IEnumerator Flash(float duration)
		{
			float flickerStart = Time.time;
			while (Time.time - flickerStart < duration)
			{
				SpriteRenderer.enabled = !SpriteRenderer.enabled;
				yield return new WaitForSeconds(flickerFrequency);
			}
			sprite.enabled = true;
		}
	}
}
