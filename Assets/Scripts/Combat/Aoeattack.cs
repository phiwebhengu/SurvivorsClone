using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    /// <summary>
    /// Second weapon type: on a cooldown, deals damage to every enemy within a radius around the player. No targeting or projectiles needed.
    /// Same upgrade pattern as AutoAttack, so the level-up UI can treat both weapons identically.
    /// </summary>
    public class AoEAttack : MonoBehaviour
    {
        [SerializeField] private AoEWeaponData weaponData;
        [SerializeField] private LayerMask enemyLayer;
        [Tooltip("If false, this weapon starts locked and disabled until Unlock() is called (e.g. from a level-up choice).")]
        [SerializeField] private bool startsUnlocked = false;

        public bool IsUnlocked { get; private set; }

        [Header("Upgrades")]
        [SerializeField] private List<WeaponUpgrade> availableUpgrades = new();

        // Runtime-only copies, same reasoning as AutoAttack: never mutate the shared asset.
        private float currentDamage;
        private float currentCooldown;
        private float currentRadius;

        private const float MinCooldown = 0.05f;
        private float cooldownTimer;

        // Reused buffer/filter to avoid allocating every pulse.
        private readonly List<Collider2D> hitBuffer = new(32);
        private ContactFilter2D contactFilter;

        private void Awake()
        {
            currentDamage = weaponData.damage;
            currentCooldown = weaponData.cooldown;
            currentRadius = weaponData.radius;

            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(enemyLayer);
            contactFilter.useTriggers = true; // enemies may use trigger colliders

            IsUnlocked = startsUnlocked;
            enabled = IsUnlocked; // disabled component means Update() never runs, no pulses fire
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

            Pulse();
            cooldownTimer = currentCooldown;
        }

        private void Pulse()
        {
            int count = Physics2D.OverlapCircle(transform.position, currentRadius, contactFilter, hitBuffer);
            for (int i = 0; i < count; i++)
            {
                if (hitBuffer[i].TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(currentDamage, this);
                }
            }

            SpawnPulseVisual();
        }

        private void SpawnPulseVisual()
        {
            GameObject vfx = new GameObject("AoEPulseVisual");
            vfx.transform.position = transform.position;
            vfx.transform.SetParent(transform);
            vfx.AddComponent<AoEPulseVisual>().Init(currentRadius);
        }

        /// <summary>
        /// Called by the level-up UI to get a set of random upgrade choices to display.
        /// The same upgrade CAN appear again in a future level-up — that's intentional,
        /// repeat picks scale in strength via the rank system in WeaponManager.
        /// </summary>
        public List<WeaponUpgrade> GetRandomUpgradeChoices(int count)
        {
            // Don't offer stat upgrades for a weapon the player hasn't unlocked yet.
            if (!IsUnlocked) return new List<WeaponUpgrade>();

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
        /// `rank` is how many times this specific upgrade has now been picked
        /// (1 = first time, 2 = second time, etc.) — scales the effect so picking
        /// the same upgrade again is meaningfully stronger than the last time.
        /// </summary>
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
                    currentRadius += upgrade.value * rank;
                    break;
                // ProjectileSpeedFlat has no meaning for an AoE weapon — just ignore it.
                // Keep upgrade pools per-weapon so this case never actually gets offered.
                default:
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (weaponData == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }
    }
}
