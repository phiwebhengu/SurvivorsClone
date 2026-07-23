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
 
        [Header("Upgrades")]
        [SerializeField] private List<WeaponUpgrade> availableUpgrades = new();
 
        // Runtime-only copies, same reasoning as AutoAttack: never mutate the shared asset.
        private float currentDamage;
        private float currentCooldown;
        private float currentRadius;
 
        private const float MinCooldown = 0.05f;
        private float cooldownTimer;
 
        // Reused buffer to avoid allocating a new array every pulse.
        private readonly Collider2D[] hitBuffer = new Collider2D[32];
 
        private void Awake()
        {
            currentDamage = weaponData.damage;
            currentCooldown = weaponData.cooldown;
            currentRadius = weaponData.radius;
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
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, currentRadius, hitBuffer, enemyLayer);
            for (int i = 0; i < count; i++)
            {
                if (hitBuffer[i].TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(currentDamage, this);
                }
            }
        }
 
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
                    currentRadius += upgrade.value;
                    break;
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
