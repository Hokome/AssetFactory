using AssetFactory.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public abstract class Sound : ScriptableObject
	{
		public abstract AudioClip Clip { get; }
		public abstract SoundType Type { get; }

		public void Play()
		{
			AudioManager.PlaySound(this);
		}
		public virtual AudioSource CreateSource(GameObject obj, PlayOptions options)
		{
			AudioSource source = obj.AddComponent<AudioSource>();
			source.clip = Clip;
			return source;
		}
	}
	public enum SoundType
	{
		SFX,
		Music,
		Voice
	}
}
