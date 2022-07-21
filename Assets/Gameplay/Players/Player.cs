using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace AssetFactory
{
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public class Player : InputBehaviour
	{
		[SerializeField] private float acceleration = 1f;
		[SerializeField] private float maxSpeed = 1f;
		[SerializeField] private float maxHealth = 5f;

		public Vector2 MoveInput { get; private set; }
		public Rigidbody2D Rb { get; private set; }

		private float health;
		public float Health
		{
			get => health;
			set
			{
				health = Mathf.Min(value, maxHealth);
			}
		}

		private void Start()
		{
			Rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			float force = acceleration * Time.fixedDeltaTime;

			if (MathEx.MaxSpeed(Rb.velocity, MoveInput * maxSpeed))
				Rb.velocity += MoveInput * force;
		}

		protected override IEnumerable<KeyValuePair<InputAction, InputCallback>> GetHandlers()
		{
			yield return GetValue<Vector2>("Move", v => MoveInput = v);
		}

		public void Hit(float damage)
		{
			Health -= damage;
			if (Health <= 0f)
				Die();
		}
		private void Die()
		{
			Destroy(gameObject);
		}
	}
}