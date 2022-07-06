using UnityEngine;

//Originally from AssetFactory
namespace ShieldRider
{
	public class AnimationManager : MonoBehaviour
	{
		[SerializeField] private Animator animator;

		private string currentAnimation = "";
		public string CurrentAnimation
		{
			get => currentAnimation;
			set
			{
				if (value == currentAnimation)
					return;
				currentAnimation = value;

				animator.Play(value, 0);
			}
		}
		public AnimationClip GetClip(string name)
		{
			foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
			{
				if (clip.name == name)
				{
					return clip;
				}
			}
			Debug.LogError($"Animation clip with name '{name}' not found");
			return null;
		}
		private void Start()
		{
			if (animator == null)
				animator = GetComponent<Animator>();
		}
	}
}