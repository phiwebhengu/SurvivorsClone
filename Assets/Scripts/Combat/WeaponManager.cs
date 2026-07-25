using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private AutoAttack autoAttack;
        [SerializeField] private AoEAttack aoeAttack;
        [SerializeField] private WeaponUpgrade unlockAoEWeaponUpgrade;

        public List<WeaponUpgrade> GetUpgradeChoices()
        {
            var upgrades = new List<WeaponUpgrade>();
            bool aoeLocked = aoeAttack != null && !aoeAttack.IsUnlocked;

            int remainingSlots = 3;

            if (aoeLocked && unlockAoEWeaponUpgrade != null)
            {
                upgrades.Add(unlockAoEWeaponUpgrade);
                remainingSlots--;
            }

            var pool = new List<WeaponUpgrade>();
            if (autoAttack != null)
                pool.AddRange(autoAttack.GetRandomUpgradeChoices(remainingSlots));
            if (aoeAttack != null)
                pool.AddRange(aoeAttack.GetRandomUpgradeChoices(remainingSlots));

            Shuffle(pool);
            for (int i = 0; i < remainingSlots && i < pool.Count; i++)
                upgrades.Add(pool[i]);

            Shuffle(upgrades); // so the unlock card isn't always shown first
            return upgrades;
        }

        public void ApplyUpgrade(WeaponUpgrade upgrade)
        {
            if (upgrade == null) return;

            // Special case: this "upgrade" unlocks a whole new weapon rather than modifying a stat, so it doesn't go through the normal ApplyUpgrade path.
            if (upgrade.type == UpgradeType.UnlockAoEWeapon)
            {
                if (aoeAttack != null) aoeAttack.Unlock();
                return;
            }

            if (autoAttack != null)
                autoAttack.ApplyUpgrade(upgrade);

            if (aoeAttack != null)
                aoeAttack.ApplyUpgrade(upgrade);
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int random = Random.Range(0, i + 1);
                (list[i], list[random]) = (list[random], list[i]);
            }
        }
    }
}