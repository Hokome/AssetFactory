using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Audio
{

	[CreateAssetMenu(fileName = "SFX", menuName = "Audio/SFX", order = 215)]
    public class SFXClip : SoundClip
    {
		public override SoundType Type => SoundType.SFX;
		public override AudioSource CreateSource(GameObject obj, PlayOptions options)
		{
			AudioSource source = base.CreateSource(obj, options);
			if (options.maxDistance <= 0f)
				source.spatialBlend = 0f;
			else
			{
				source.maxDistance = options.maxDistance;
				source.spatialBlend = 1f;
			}
			return source;
		}
	}
}
