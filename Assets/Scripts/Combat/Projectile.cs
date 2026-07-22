using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private float damage;
        [SerializeField] private float lifetime = 3f;

        public void Init(Vector2 dir, float spd, float dmg)
        {
            direction = dir;
            speed = spd;
            damage = dmg;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage, this);
                Destroy(gameObject);
            }
        }
    }
}