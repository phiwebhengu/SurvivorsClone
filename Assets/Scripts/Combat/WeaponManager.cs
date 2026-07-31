using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private MeleeAttack meleeAttack;
        [SerializeField] private AutoAttack autoAttack;
        [SerializeField] private AoEAttack aoeAttack;

        [Tooltip("Special upgrade card that unlocks the ranged auto-attack weapon.")]
        [SerializeField] private WeaponUpgrade unlockAutoAttackUpgrade;
        [Tooltip("Special upgrade card that unlocks the AoE pulse weapon.")]
        [SerializeField] private WeaponUpgrade unlockAoEWeaponUpgrade;

        // Tracks how many times each upgrade asset has been picked. Picking the same
        // upgrade again scales its effect (rank 1 = base value, rank 2 = 2x, etc.),
        // up to that upgrade's own maxRank, after which it stops being offered.
        private readonly Dictionary<WeaponUpgrade, int> upgradeRanks = new();

        public List<WeaponUpgrade> GetUpgradeChoices()
        {
            var pool = new List<WeaponUpgrade>();
            var seen = new HashSet<WeaponUpgrade>();

            void AddUnique(IEnumerable<WeaponUpgrade> source)
            {
                foreach (var u in source)
                {
                    if (u == null || seen.Contains(u) || IsMaxedOut(u)) continue;
                    seen.Add(u);
                    pool.Add(u);
                }
            }

            // Melee is always active, so its upgrades are always in the running.
            if (meleeAttack != null)
                AddUnique(meleeAttack.GetRandomUpgradeChoices(int.MaxValue));

            // Locked weapons contribute their "unlock" card to the same general pool
            // instead of a guaranteed slot — so it competes randomly with everything
            // else, rather than showing up (or dominating) every single level-up.
            if (autoAttack != null && autoAttack.IsUnlocked)
                AddUnique(autoAttack.GetRandomUpgradeChoices(int.MaxValue));
            else if (unlockAutoAttackUpgrade != null && seen.Add(unlockAutoAttackUpgrade))
                pool.Add(unlockAutoAttackUpgrade);

            if (aoeAttack != null && aoeAttack.IsUnlocked)
                AddUnique(aoeAttack.GetRandomUpgradeChoices(int.MaxValue));
            else if (unlockAoEWeaponUpgrade != null && seen.Add(unlockAoEWeaponUpgrade))
                pool.Add(unlockAoEWeaponUpgrade);

            Shuffle(pool);

            var choices = new List<WeaponUpgrade>();
            for (int i = 0; i < 3 && i < pool.Count; i++)
                choices.Add(pool[i]);

            return choices;
        }

        private bool IsMaxedOut(WeaponUpgrade upgrade)
        {
            if (upgrade.maxRank <= 0) return false; // 0 or less = unlimited
            int rank = upgradeRanks.TryGetValue(upgrade, out int r) ? r : 0;
            return rank >= upgrade.maxRank;
        }

        public void ApplyUpgrade(WeaponUpgrade upgrade)
        {
            if (upgrade == null) return;

            if (upgrade == unlockAutoAttackUpgrade)
            {
                if (autoAttack != null) autoAttack.Unlock();
                return;
            }

            if (upgrade == unlockAoEWeaponUpgrade)
            {
                if (aoeAttack != null) aoeAttack.Unlock();
                return;
            }

            int rank = upgradeRanks.TryGetValue(upgrade, out int currentRank) ? currentRank + 1 : 1;
            upgradeRanks[upgrade] = rank;

            if (meleeAttack != null) meleeAttack.ApplyUpgrade(upgrade, rank);
            if (autoAttack != null) autoAttack.ApplyUpgrade(upgrade, rank);
            if (aoeAttack != null) aoeAttack.ApplyUpgrade(upgrade, rank);
        }

        /// <summary>
        /// Current rank of a given upgrade (0 if never picked yet). Useful for UI
        /// that wants to show "Rank 2/3" or grey out maxed-out upgrades.
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
