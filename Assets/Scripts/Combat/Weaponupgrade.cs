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
        UnlockAoEWeapon      // special: unlocks the AoE weapon instead of modifying a stat
    }

    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "CloneGame/Weapon Upgrade")]
    public class WeaponUpgrade : ScriptableObject
    {
        public string upgradeName = "Upgrade";
        [TextArea] public string description = "Describe what this upgrade does.";
        public UpgradeType type;
        public float value = 10f;
    }
}