using UnityEngine;

namespace AssetFactory.Audio
{

	[CreateAssetMenu(fileName = "Selection", menuName = "Audio/Selection", order = 215)]
	public class AudioSelection : Sound
	{
		[SerializeField] private Sound[] sounds;

		public override AudioClip Clip
		{
			get
			{
				int select = Random.Range(0, sounds.Length);
				return sounds[select].Clip;
			}
		}
		public override SoundType Type => sounds[0].Type;
	}
}
