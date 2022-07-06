using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	public class WallCheck : CollisionCheck
	{
		[SerializeField] private float extraSpace = 0.1f;
		[SerializeField] private float bufferDuration = 0.3f;
		[SerializeField] private float lockDuration = 0.3f;

		[SerializeField] private BoxCollider2D box;

		public bool WallLeft { get; private set; }
		public bool WallRight { get; private set; }
		public BufferedBool WallLeftBuffer { get; private set; }
		public BufferedBool WallRightBuffer { get; private set; }

		public bool IsLocked => lockTime < lockDuration;

		float lockTime = 0f;

		private void Start()
		{
			WallLeftBuffer = new BufferedBool(bufferDuration);
			WallRightBuffer = new BufferedBool(bufferDuration);
			if (box == null)
			{
				box = GetComponentInChildren<BoxCollider2D>();
			}
		}

		private void FixedUpdate()
		{
			lockTime += Time.fixedDeltaTime;
			if (IsLocked) return;

			RaycastHit2D lRch = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.left, extraSpace, mask);
			RaycastHit2D rRch = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.right, extraSpace, mask);
			if (rRch.collider != null)
			{
				WallRight = true;
				WallRightBuffer.Buffer(true);
			}
			else
			{
				WallRight = false;
			}
			if (lRch.collider != null)
			{
				WallLeft = true;
				WallLeftBuffer.Buffer(true);
			}
			else
			{
				WallLeft = false;
			}
		}
		public void Lock()
		{
			lockTime = 0f;
			WallLeftBuffer.Reset();
			WallRightBuffer.Reset();
			WallLeft = false;
			WallRight = false;
		}

		private void OnDrawGizmos()
		{
			if (box == null)
				box = GetComponent<BoxCollider2D>();

			Rect l = new Rect(box.bounds.min.x - extraSpace, box.bounds.min.y, box.size.x * 0.5f + extraSpace, box.size.y);
			Rect r = new Rect(box.bounds.center.x, box.bounds.min.y, box.size.x * 0.5f + extraSpace, box.size.y);
			Draw(l, Vector2.right, WallLeft, WallLeftBuffer, IsLocked);
			Draw(r, Vector2.left, WallRight, WallRightBuffer, IsLocked);
		}
	}
}
