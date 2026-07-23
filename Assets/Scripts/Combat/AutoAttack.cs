using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class AutoAttack : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Upgrades")]
        [Tooltip("Pool of upgrades this weapon can offer on level up.")]
        [SerializeField] private List<WeaponUpgrade> availableUpgrades = new();

        // Runtime-only copies of the base stats. We never modify the WeaponData
        // asset itself, since that would permanently change the shared asset file.
        private float currentDamage;
        private float currentCooldown;
        private float currentRange;
        private float currentProjectileSpeed;

        private const float MinCooldown = 0.05f;

        private float cooldownTimer;

        private void Awake()
        {
            currentDamage = weaponData.damage;
            currentCooldown = weaponData.cooldown;
            currentRange = weaponData.range;
            currentProjectileSpeed = weaponData.projectileSpeed;
        }

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0f) return;

            Transform target = FindNearestEnemy();
            if (target == null) return;

            Attack(target);
            cooldownTimer = currentCooldown;
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);
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
                p.Init(dir, currentProjectileSpeed, currentDamage, gameObject);
            }
        }

        /// <summary>
        /// Called by the level-up UI to get a set of random upgrade choices to display.
        /// Returns up to `count` upgrades with no duplicates. Safe to call with an
        /// empty pool (returns an empty list).
        /// </summary>
        public List<WeaponUpgrade> GetRandomUpgradeChoices(int count)
        {
            var pool = new List<WeaponUpgrade>(availableUpgrades);
            var choices = new List<WeaponUpgrade>();

            count = Mathf.Min(count, pool.Count);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, pool.Count);
                choices.Add(pool[index]);
                pool.RemoveAt(index);
            }
            return choices;
        }

        /// <summary>
        /// Called by the level-up UI once the player picks an upgrade.
        /// </summary>
        public void ApplyUpgrade(WeaponUpgrade upgrade)
        {
            if (upgrade == null) return;

            switch (upgrade.type)
            {
                case UpgradeType.DamageFlat:
                    currentDamage += upgrade.value;
                    break;
                case UpgradeType.DamagePercent:
                    currentDamage *= 1f + (upgrade.value / 100f);
                    break;
                case UpgradeType.CooldownPercent:
                    currentCooldown *= 1f - (upgrade.value / 100f);
                    currentCooldown = Mathf.Max(MinCooldown, currentCooldown);
                    break;
                case UpgradeType.RangeFlat:
                    currentRange += upgrade.value;
                    break;
                case UpgradeType.ProjectileSpeedFlat:
                    currentProjectileSpeed += upgrade.value;
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (weaponData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentRange);
        }
    }
}