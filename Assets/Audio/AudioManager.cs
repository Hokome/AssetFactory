using UnityEngine;
using UnityEngine.Audio;

//Originally from AssetFactory
namespace AssetFactory.Audio
{
	public class AudioManager : MonoSingleton<AudioManager>
	{
		//public AudioList list;
		public AudioMixer mixer;

		public AudioSource MusicSource { get; private set; }
		//private Dictionary<string, SoundClip> clipDictionary;

		protected override void Awake()
		{
			//if (list == null)
			//	return;

			base.Awake();
			DontDestroyOnLoad(gameObject);

			//clipDictionary = new Dictionary<string, SoundClip>(list.clips.Count);
			//list.clips.ForEach(c => clipDictionary.Add(c.name, c));
		}

		public static AudioSource PlaySound(Sound sound) => PlaySound(sound, Vector3.zero, PlayOptions.Default);
		public static AudioSource PlaySound(Sound sound, PlayOptions options) => PlaySound(sound, Vector3.zero, options);
		public static AudioSource PlaySound(Sound sound, Vector3 position) => PlaySound(sound, position, PlayOptions.Default);
		public static AudioSource PlaySound(Sound sound, Vector3 position, PlayOptions options)
		{
			AudioSource source = CreateObject(sound, options);
			source.transform.position = position;
			source.Play();
			if (!options.loop)
				Destroy(source.gameObject, source.clip.length);
			return source;
		}
		public static AudioSource PlaySound(Sound sound, GameObject follow, PlayOptions options)
		{
			AudioSource source = sound.CreateSource(follow, options);
			source.Play();
			if (!options.loop)
				Destroy(source, source.clip.length);
			return source;
		}

		//public static void PlayMusic(MusicClip music, PlayOptions options)
		//{
		//	if (Inst.MusicSource == null)
		//	{
		//		Inst.MusicSource = PlaySound(music, options);
		//	}
		//	else
		//	{
		//		StopMusic(1f, music, options);
		//	}
		//}
		//public static void StopMusic(float fadeOut, MusicClip next, PlayOptions options) 
		//	=> Inst.InstStopMusic(fadeOut, next, options);
		//private void InstStopMusic(float fadeOut, MusicClip next, PlayOptions options)
		//{
		//	if (MusicSource == null)
		//		return;
		//	if (musicStopCoroutine != null)
		//	{
		//		musicStopCoroutine = StartCoroutine(FadeRoutine(fadeOut, next, options));
		//	}
		//}

		//private IEnumerator FadeRoutine(float fadeOut, MusicClip next, PlayOptions options)
		//{
		//	float start = Time.unscaledTime;
		//	float increment = MusicSource.volume / fadeOut;
		//	while (Time.unscaledTime - start < fadeOut)
		//	{
		//		MusicSource.volume -= increment * Time.deltaTime;
		//		yield return null;
		//	}
		//	PlayMusic(next, options);
		//}

		private static AudioSource CreateObject(Sound sound, PlayOptions options)
		{
			string type = sound.Type.ToString();
			GameObject go = new($"{type}: {sound.name}");
			go.transform.parent = Inst.transform;
			return sound.CreateSource(go, options);
		}
		public void UpdateMixers()
		{
			mixer.SetFloat("MasterVolume", ToDB(Settings.current.masterVolume));
			mixer.SetFloat("MusicVolume", ToDB(Settings.current.musicVolume));
			mixer.SetFloat("SFXVolume", ToDB(Settings.current.sfxVolume));
			mixer.SetFloat("UIVolume", ToDB(Settings.current.uiVolume));
		}
		private static float ToDB(float initial)
		{
			if (initial <= 0f)
				return -80f;
			return Mathf.Min(0, Mathf.Log10(initial) * 20);
		}
	}
}