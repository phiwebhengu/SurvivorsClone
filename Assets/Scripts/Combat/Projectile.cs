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
        private GameObject owner;
        [SerializeField] private float lifetime = 3f;

        public void Init(Vector2 dir, float spd, float dmg, GameObject shooter = null)
        {
            direction = dir;
            speed = spd;
            damage = dmg;
            owner = shooter;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Never damage whoever fired this projectile, even if it spawned
            // overlapping them for a frame.
            if (owner != null && other.gameObject == owner) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage, this);
                Destroy(gameObject);
            }
        }
    }
}