using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class AutoAttack : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private LayerMask enemyLayer;

        private float cooldownTimer;

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0f) return;

            Transform target = FindNearestEnemy();
            if (target == null) return;

            Attack(target);
            cooldownTimer = weaponData.cooldown;
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData.range, enemyLayer);
            Transform nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                float dist = (hit.transform.position - transform.position).sqrMagnitude;
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = hit.transform;
                }
            }
            return nearest;
        }

        private void Attack(Transform target)
        {
            if (weaponData.projectilePrefab == null) return;

            GameObject proj = Instantiate(weaponData.projectilePrefab, transform.position, Quaternion.identity);
            if (proj.TryGetComponent<Projectile>(out var p))
            {
                Vector2 dir = (target.position - transform.position).normalized;
                p.Init(dir, weaponData.projectileSpeed, weaponData.damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (weaponData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, weaponData.range);
        }
    }
}