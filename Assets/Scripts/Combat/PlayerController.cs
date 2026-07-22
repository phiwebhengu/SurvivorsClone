using UnityEngine;
using UnityEngine.InputSystem;
using CloneGame.Combat;

namespace CloneGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private InputActionReference moveAction;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        public Vector2 FacingDirection { get; private set; } = Vector2.down;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
        }

        private void Update()
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
            if (moveInput.sqrMagnitude > 0.01f)
                FacingDirection = moveInput.normalized;
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}