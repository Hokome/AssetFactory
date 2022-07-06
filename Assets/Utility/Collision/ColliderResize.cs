using UnityEditor;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	public class ColliderResize : MonoBehaviour
	{
		[SerializeField] private Vector2 size = new Vector2(2f, 2f);
		[SerializeField] float borderOffset = 0f;

		void Start()
		{
			Set();
		}
#if UNITY_EDITOR
		private void OnValidate()
		{
			EditorApplication.delayCall += () => Set();
		}
#endif
		void Set()
		{
			try
			{
				if (!TryGetComponent(out SpriteRenderer sr))
					return;
				if (!TryGetComponent(out Collider2D col))
					return;

				if (sr.drawMode == SpriteDrawMode.Simple)
				{
					Vector3 scale = transform.localScale;
					sr.drawMode = SpriteDrawMode.Sliced;
					transform.localScale = scale;
				}

				if (col is BoxCollider2D box)
				{
					box.size = size + Vector2.one * borderOffset;
					sr.size = size;
					Vector2 pivot = sr.sprite.pivot / sr.sprite.rect.size;
					box.offset = size * -pivot + size * 0.5f;
				}
				else if (col is CircleCollider2D c)
				{
					sr.size = Vector2.one * size.x;
					c.radius = (size.x + borderOffset) * 0.5f;
				}
			}
			catch (MissingReferenceException)
			{
				return;
			}
		}
	}
}