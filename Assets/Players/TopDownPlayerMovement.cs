using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace AssetFactory
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownPlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _maxSpeed = 1f;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _deceleration = 1f;
        [Tooltip("At what speed does the velocity get set to zero")]
        [SerializeField] private float _velocitySnapThreshold = 0.01f;

        public Vector2 MoveInput { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetMoveInput(InputContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }
    }
}
