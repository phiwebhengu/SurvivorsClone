using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    /// <summary>
    /// New starting weapon: a close-range swing in whatever direction the player is currently facing — encourages the player to get close to enemies, unlike the ranged auto-attack and AoE pulse, which are now both unlocked later as upgrades.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class MeleeAttack : MonoBehaviour
    {
        [SerializeField] private MeleeWeaponData weaponData;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Upgrades")]
        [SerializeField] private List<WeaponUpgrade> availableUpgrades = new();

        private float currentDamage;
        private float currentCooldown;
        private float currentRange;
        private float currentWidth;

        private const float MinCooldown = 0.05f;
        private float cooldownTimer;

        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();

            currentDamage = weaponData.damage;
            currentCooldown = weaponData.cooldown;
            currentRange = weaponData.range;
            currentWidth = weaponData.width;
        }

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0f) return;

            Swing();
            cooldownTimer = currentCooldown;
        }

        private void Swing()
        {
            Vector2 dir = playerController.FacingDirection;
            if (dir.sqrMagnitude < 0.01f) dir = Vector2.down;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Vector2 center = (Vector2)transform.position + dir * (currentRange / 2f);
            Vector2 size = new Vector2(currentRange, currentWidth);

            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, enemyLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(currentDamage, this);
                }
            }

            SpawnSwingVisual(center, size, angle);
        }

        private void SpawnSwingVisual(Vector2 center, Vector2 size, float angle)
        {
            GameObject vfx = new GameObject("MeleeSwingVisual");
            vfx.transform.position = center;
            vfx.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            vfx.AddComponent<MeleeSwingVisual>().Init(size);
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
                default:
                    break; // ProjectileSpeedFlat has no meaning here
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (playerController == null || weaponData == null) return;
            Vector2 dir = Application.isPlaying ? playerController.FacingDirection : Vector2.down;
            float range = Application.isPlaying ? currentRange : weaponData.range;
            float width = Application.isPlaying ? currentWidth : weaponData.width;
            Vector2 center = (Vector2)transform.position + dir * (range / 2f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, new Vector2(range, width));
        }
    }
}
