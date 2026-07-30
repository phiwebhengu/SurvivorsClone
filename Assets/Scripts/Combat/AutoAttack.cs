using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class AutoAttack : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private LayerMask enemyLayer;
        [Tooltip("If false, this weapon starts locked and disabled until Unlock() is called (e.g. from a level-up choice).")]
        [SerializeField] private bool startsUnlocked = false;

        public bool IsUnlocked { get; private set; }

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

            IsUnlocked = startsUnlocked;
            enabled = IsUnlocked; // disabled component means Update() never runs
        }

        /// <summary>
        /// Called when the player picks the "unlock this weapon" level-up choice.
        /// Safe to call more than once.
        /// </summary>
        public void Unlock()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            enabled = true;
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
        /// Returns the entire available pool shuffled, no duplicates within the call.
        /// WeaponManager handles final random selection and cross-weapon deduping.
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

        public void ApplyUpgrade(WeaponUpgrade upgrade, int rank = 1)
        {
            if (upgrade == null) return;
            rank = Mathf.Max(1, rank);

            switch (upgrade.type)
            {
                case UpgradeType.DamageFlat:
                    currentDamage += upgrade.value * rank;
                    break;
                case UpgradeType.DamagePercent:
                    currentDamage *= 1f + (upgrade.value * rank / 100f);
                    break;
                case UpgradeType.CooldownPercent:
                    currentCooldown *= 1f - (upgrade.value * rank / 100f);
                    currentCooldown = Mathf.Max(MinCooldown, currentCooldown);
                    break;
                case UpgradeType.RangeFlat:
                    currentRange += upgrade.value * rank;
                    break;
                case UpgradeType.ProjectileSpeedFlat:
                    currentProjectileSpeed += upgrade.value * rank;
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
