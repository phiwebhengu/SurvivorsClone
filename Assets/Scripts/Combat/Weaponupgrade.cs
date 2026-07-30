using UnityEngine;

namespace CloneGame.Combat
{
    public enum UpgradeType
    {
        DamageFlat,          // +value damage
        DamagePercent,       // +value% damage
        CooldownPercent,     // -value% cooldown (faster attacks)
        RangeFlat,           // +value targeting range
        ProjectileSpeedFlat, // +value projectile speed
        UnlockAoEWeapon      // legacy label; unlocking is now detected by asset reference, not type
    }

    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "CloneGame/Weapon Upgrade")]
    public class WeaponUpgrade : ScriptableObject
    {
        public string upgradeName = "Upgrade";
        [TextArea] public string description = "Describe what this upgrade does.";
        public UpgradeType type;
        public float value = 10f;

        [Tooltip("Max number of times this upgrade can be picked in one run. 0 or less = unlimited.")]
        public int maxRank = 3;
    }
}
