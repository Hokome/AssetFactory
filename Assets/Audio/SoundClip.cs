using UnityEngine;
using UnityEngine.Audio;

//Originally from AssetFactory
namespace AssetFactory.Audio
{
	public abstract class SoundClip : Sound
	{
		[SerializeField] protected AudioClip clip;
		[Space]
		[Range(0f,5f)] public float volume = 1f;
		[Range(0.1f, 3f)] public float pitch = 1f;
		[Space]
		public AudioMixerGroup channel;

		public override AudioClip Clip => clip;
		public override AudioSource CreateSource(GameObject obj, PlayOptions options)
		{
			AudioSource source = base.CreateSource(obj, options);
			source.volume = volume * options.volume;
			source.pitch = pitch * options.pitch;
			source.loop = options.loop;
			source.outputAudioMixerGroup = channel;
			return source;
		}
	}
}