using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	[RequireComponent(typeof(BoxCollider2D))]
	[RequireComponent(typeof(ParticleSystem))]
	public class Windzone : MonoBehaviour
	{
		[SerializeField] private Vector2 size;
		[SerializeField] private float power = 2f;
		[SerializeField] private float maxVelocity = 10f;


		private ParticleSystem ps;
		private BoxCollider2D box;

		List<Rigidbody2D> rbs = new List<Rigidbody2D>();

		private void Start() => SetComponents();
		private void OnValidate() => SetComponents();
		private void SetComponents()
		{
			box = GetComponent<BoxCollider2D>();
			ps = GetComponent<ParticleSystem>();

			box.size = size;
			box.offset = Vector2.up * size / 2f;

			ParticleSystem.ShapeModule sh = ps.shape;
			sh.scale = new Vector3(size.x, 1f, 1f);

			float decreasedPower = power * 0.1f;

			ParticleSystem.MainModule m = ps.main;
			m.startSpeed = decreasedPower;
			m.startLifetime = size.y / decreasedPower;
			m.startRotation = transform.eulerAngles.z * -Mathf.Deg2Rad;

			ParticleSystem.EmissionModule e = ps.emission;
			e.rateOverTime = decreasedPower * 0.5f * size.x;
		}

		private void FixedUpdate()
		{
			Vector2 check = transform.up * maxVelocity;
			Vector2 force = transform.up * power * Time.fixedDeltaTime;
			foreach (Rigidbody2D r in rbs)
			{
				if (MathEx.GreaterProjected(check, r.velocity))
					continue;
				r.AddForce(force, ForceMode2D.Impulse);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Rigidbody2D r))
			{
				if (r.bodyType == RigidbodyType2D.Dynamic)
					rbs.Add(r);
			}
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Rigidbody2D r))
			{
				if (r.bodyType == RigidbodyType2D.Dynamic)
				{
					rbs.Remove(r);
					r.AddForce(-transform.up * power);
				}
			}
		}
	}
}
