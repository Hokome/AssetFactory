using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	public class GroundCheck : CollisionCheck
	{
		[Header("Check")]
		[SerializeField] private float checkHeight = 0.2f;
		[SerializeField] private BufferedBool coyoteTime = 0.3f;
		[SerializeField] private float lockDuration = 0.3f;
		[SerializeField] private float horizontalOffset = 0.3f;
		[Space]
		[SerializeField] private BoxCollider2D box;
#if UNITY_EDITOR
		[Header("Editor")]
		[SerializeField] private bool gizmos;
#endif

		private Rigidbody2D rb;
		public BoxCollider2D Box
		{
			get
			{
				if (box == null)
					box = GetComponent<BoxCollider2D>();
				return box;
			}
			private set => box = value;
		}

		public bool IsGrounded { get; private set; }
		public bool IsGroundedBuffer => coyoteTime;
		public bool IsLocked => lockTime < lockDuration;

		private float lockTime = 0f;

		private float VerticalOffset
		{
			get
			{
				Vector2 hSize = HalfSize;
				float ret = hSize.y - hSize.x;
				ret += Mathf.Sqrt((hSize.x * hSize.x) - (horizontalOffset * horizontalOffset));
				return ret;
			}
		}
		private Vector2 Center => (Vector2)transform.position + Box.offset;
		private Vector2 HalfSize => Box.size * 0.5f;

		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			lockTime += Time.fixedDeltaTime;
			if (IsLocked) return;

			CheckGround();
		}
		private void CheckGround()
		{
			RaycastHit2D rch = CastGround(mask);
			if (rch.collider != null)
			{
				IsGrounded = true;
				coyoteTime.Buffer(true);
			}
			else
			{
				IsGrounded = false;
			}
		}
		public RaycastHit2D CastGround(LayerMask mask)
		{
			Vector2 pos = Center - ((VerticalOffset + checkHeight * 0.5f) * (Vector2)transform.up);

			Vector2 size = new Vector2(horizontalOffset * 2f, checkHeight);
			return Physics2D.BoxCast(pos, size, rb.rotation, -transform.up, 0f, mask);
		}
		public void Lock()
		{
			lockTime = 0f;
			coyoteTime.Reset();
			IsGrounded = false;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Color color;

			Vector2 pos = Center;

			if (gizmos)
			{
				if (Application.isPlaying)
					color = IsGrounded ? Color.green : coyoteTime ? Color.cyan : IsLocked ? Color.gray : Color.red;
				else
					color = Color.cyan;
				float verticalOffset = VerticalOffset;

				Vector2 down = -transform.up;
				Vector2 bottom = down * verticalOffset;
				Vector2 bottomCheck = down * (verticalOffset + checkHeight);
				Vector2 right = transform.right * horizontalOffset;

				Vector2 tl = pos - right + bottom;
				Vector2 tr = pos + right + bottom;

				Vector2 bl = pos - right + bottomCheck;
				Vector2 br = pos + right + bottomCheck;

				Debug.DrawLine(tl, bl, color);
				Debug.DrawLine(tr, br, color);
			}
		}
#endif
	}
}