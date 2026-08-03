using CloneGame.Player;
using UnityEngine;

namespace CloneGame.Animation
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (playerController == null)
                playerController = GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            if (playerController == null)
                return;

            Vector2 direction = playerController.FacingDirection;

            bool isMoving = playerController.GetComponent<Rigidbody2D>().linearVelocity.sqrMagnitude > 0.01f;
            animator.SetBool("IsMoving", isMoving);

            if (direction.x > 0.1f)
                spriteRenderer.flipX = true;
            else if (direction.x < -0.1f)
                spriteRenderer.flipX = false;
        }

        public void Attack()
        {
            animator.SetTrigger("Attack");
        }

        public void Hurt()
        {
            animator.SetTrigger("Hurt");
        }

        public void Die()
        {
            animator.SetTrigger("Death");
        }
    }
}