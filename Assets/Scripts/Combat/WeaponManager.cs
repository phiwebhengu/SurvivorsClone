using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private AutoAttack autoAttack;
        [SerializeField] private AoEAttack aoeAttack;

        [Tooltip("The special 'Unlock AoE Weapon' upgrade asset. Guaranteed to appear " +
                 "as one of the level-up choices while the AoE weapon is still locked.")]
        [SerializeField] private WeaponUpgrade unlockAoEWeaponUpgrade;

        // Tracks how many times each upgrade asset has been picked. Picking the same
        // upgrade again scales its effect (rank 1 = base value, rank 2 = 2x, etc.),
        // similar to how repeat-picking the same item in Vampire Survivors levels it up.
        private readonly Dictionary<WeaponUpgrade, int> upgradeRanks = new();

        public List<WeaponUpgrade> GetUpgradeChoices()
        {
            var upgrades = new List<WeaponUpgrade>();
            bool aoeLocked = aoeAttack != null && !aoeAttack.IsUnlocked;

            int remainingSlots = 3;

            // Guaranteed slot: keep offering the unlock card every level-up until picked.
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

            // Special case: this "upgrade" unlocks a whole new weapon rather than
            // modifying a stat, so it doesn't go through rank tracking at all.
            if (upgrade.type == UpgradeType.UnlockAoEWeapon)
            {
                if (aoeAttack != null) aoeAttack.Unlock();
                return;
            }

            int rank = upgradeRanks.TryGetValue(upgrade, out int currentRank) ? currentRank + 1 : 1;
            upgradeRanks[upgrade] = rank;

            if (autoAttack != null)
                autoAttack.ApplyUpgrade(upgrade, rank);

            if (aoeAttack != null)
                aoeAttack.ApplyUpgrade(upgrade, rank);
        }

        /// <summary>
        /// Current rank of a given upgrade (0 if never picked yet). Useful for UI
        /// that wants to show "Rank 2" or preview the next tier before picking.
        /// </summary>
        public int GetUpgradeRank(WeaponUpgrade upgrade)
        {
            return upgradeRanks.TryGetValue(upgrade, out int rank) ? rank : 0;
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
