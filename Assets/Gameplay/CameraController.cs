using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Originally from AssetFactory
namespace AssetFactory
{
	[RequireComponent(typeof(PlayerInput))]
	public class CameraController : MonoBehaviour
    {
		[SerializeField] private float speed;
		[SerializeField] private float fastSpeed;
		[SerializeField] private Vector2 sensitivity;

		Vector2 movement;
		float fly;
		Vector2 look;
		bool running;

		private void Update()
		{
			float spd = running ? fastSpeed : speed;

			Vector3 h = new Vector3(movement.x, 0, movement.y);
			transform.Translate(spd * Time.deltaTime * h, Space.Self);
			transform.Translate(0, fly * spd * Time.deltaTime, 0, Space.World);

			transform.Rotate(Vector3.up, look.x * sensitivity.x * Time.deltaTime, Space.World);
			transform.Rotate(Vector3.right, -look.y * sensitivity.y * Time.deltaTime);
		}

		private void OnMove(InputValue iv)
		{
			movement = iv.Get<Vector2>();
		}
		private void OnLook(InputValue iv)
		{
			look = iv.Get<Vector2>();
		}
		private void OnFly(InputValue iv)
		{
			fly = iv.Get<float>();
		}
		private void OnRun()
		{
			running = !running;
		}
	}
}
