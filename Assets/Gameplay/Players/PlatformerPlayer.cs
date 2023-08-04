using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace AssetFactory.Legacy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(GroundCheck))]
    public class PlatformerPlayer : InputBehaviour, IKillable, ISingleton<PlatformerPlayer>
    {
        public static PlatformerPlayer Inst
        {
            get => ISingleton<PlatformerPlayer>.Inst;
            set => ISingleton<PlatformerPlayer>.Inst = value;
        }
        public static bool Exists => ISingleton<PlatformerPlayer>.Exists;

        [Header("Movement")]
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float airControl = 0.5f;
        //This is here to avoid the player sliding too much when the input is zero
        [SerializeField] private float neutralInputDeceleration = 5f;
        [SerializeField] private float decelerationThreshold = 0.1f;
        [Header("Jump")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private AnimationCurve jumpGravityCurve;
        [SerializeField] private Vector2 jumpCurveGraphScale = Vector2.one;
        [SerializeField] private BufferedBool jumpBuffer = 0.2f;
        [SerializeField] private int maxAirJumps = 0;
        //Used when jumping mid-air to fight gravity (sort of)
        [SerializeField] private float jumpVelocityAttenuation = 0.5f;
        [Header("Misc")]
        [SerializeField] private bool flyMode;

        private int jumpCount;

        private Rigidbody2D rb;
        public Rigidbody2D Rb
        {
            get
            {
                if (rb == null)
                    rb = GetComponent<Rigidbody2D>();
                return rb;
            }
            private set => rb = value;
        }
        private GroundCheck gc;
        public GroundCheck Gc
        {
            get
            {
                if (gc == null)
                    gc = GetComponent<GroundCheck>();
                return gc;
            }
            private set => gc = value;
        }

        //public Checkpoint Checkpoint { get; set; }

        private bool jumping;
        private bool Jumping
        {
            get => jumping;
            set
            {
                jumping = value;
                jumpTime = Time.time;
            }
        }

        private float jumpTime = float.NegativeInfinity;

        public bool FlyMode
        {
            get => flyMode;
            set
            {
                flyMode = value;
                if (Application.isPlaying)
                {
                    if (value)
                    {
                        Rb.constraints = RigidbodyConstraints2D.FreezeAll;
                        Rb.gravityScale = 0f;
                    }
                    else
                    {
                        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                        Rb.gravityScale = 1f;
                    }
                }
            }
        }

        //public bool Frozen
        //{
        //	get => frozen;
        //	set
        //	{
        //		frozen = value;
        //	}
        //}

        protected override void Awake()
        {
            base.Awake();
            Inst = this;
        }
        private void Start()
        {
            FlyMode = FlyMode;
        }

        private void FixedUpdate()
        {
            //DebugEx.SetElement(0, Rb.velocity);
            if (PauseMenu.IsPaused) return;
            //if (Frozen)
            //{
            //	if (Gc.IsGrounded)
            //		rb.velocity *= Vector2.up;
            //	return;
            //}

            if (flyMode)
            {
                transform.position += Time.fixedDeltaTime * maxSpeed * (Vector3)MoveInput;
            }
            else
            {
                if (Gc.IsGroundedBuffer)
                    jumpCount = 0;
                if (jumpBuffer) Jump();

                if (!Gc.IsGrounded)
                {
                    float jumpCurveX = (Time.time - jumpTime) / jumpCurveGraphScale.x;
                    if (jumpCurveX < 1f)
                        Rb.gravityScale = jumpGravityCurve.Evaluate(jumpCurveX) * jumpCurveGraphScale.y;
                }
            }
            Move();
        }
        private void OnValidate()
        {
            FlyMode = FlyMode;
        }

        private void Jump()
        {
            if (!Gc.IsGroundedBuffer)
                if (jumpCount >= maxAirJumps) return;
                else jumpCount++;

            if (Rb.velocity.y < 0f)
                Rb.velocity *= Vector2.right;
            else
                Rb.velocity *= new Vector2(1f, jumpVelocityAttenuation);

            Rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            Jumping = true;

            jumpBuffer.Reset();
            Gc.Lock();
        }
        private void Move()
        {
            float force = 1f;
            if (MoveInput.x == 0f)
            {
                if (Gc.IsGrounded)
                {
                    //Supposed to be for slopes but I'll keep it like this
                    Vector2 pVelocity = Vector3.Project(Rb.velocity, transform.right);
                    if (pVelocity.sqrMagnitude > decelerationThreshold * decelerationThreshold)
                    {
                        force *= neutralInputDeceleration;
                        Rb.AddForce(-pVelocity.normalized * force);
                    }
                    else
                    {
                        Rb.velocity *= Vector2.up;
                    }
                }
            }
            else
            {
                if (MathEx.MaxSpeed(Rb.velocity, maxSpeed * MoveInput * Vector2.right))
                {
                    force *= acceleration;
                    if (!Gc.IsGrounded) force *= airControl;
                    Rb.AddForce(new Vector2(MoveInput.x * force, 0f));
                }
            }
        }


        public void Kill() => Respawn();
        public void Respawn()
        {
            //transform.position = Checkpoint.SpawnPoint;
            transform.position = Vector2.zero;
            Rb.velocity = Vector2.zero;
        }

        private Vector2 MoveInput { get; set; }

        protected override IEnumerable<KeyValuePair<InputAction, InputCallback>> GetHandlers()
        {
            yield return GetValue<Vector2>("Move", v => MoveInput = v);
            yield return GetButton("Jump", () => jumpBuffer.Buffer(true));
            yield return GetButton("Pause", PauseMenu.Inst.TogglePauseMenu);
        }
    }
}