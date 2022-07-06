using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Audio
{
	[System.Serializable]
    public struct PlayOptions 
	{
		public static PlayOptions Default => new PlayOptions(1f, 1f, 0f, false);

		public float volume;
		public float pitch;
		public float maxDistance;
		public bool loop;

		public PlayOptions(float volume, float pitch, float maxDistance, bool loop)
		{
			this.volume = volume;
			this.pitch = pitch;
			this.maxDistance = maxDistance;
			this.loop = loop;
		}

		public void RandomizeVolume(float difference) => volume += GetRandom(difference);
		public void RandomizePitch(float difference) => pitch += GetRandom(difference);

		private float GetRandom(float dif) => Random.Range(-dif, dif);
	}
}
