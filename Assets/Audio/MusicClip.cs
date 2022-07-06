using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Audio
{

	[CreateAssetMenu(fileName = "Music Track", menuName = "Audio/Music", order = 215)]
    public class MusicClip : SoundClip
    {
		public override SoundType Type => SoundType.Music;
	}
}
