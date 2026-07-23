using System.Collections.Generic;
using UnityEngine;
using CloneGame.Combat;

namespace CloneGame.Player
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private AutoAttack autoAttack;
        [SerializeField] private AoEAttack aoeAttack;

        public List<WeaponUpgrade> GetUpgradeChoices()
        {
            List<WeaponUpgrade> upgrades = new();

            if (autoAttack != null)
                upgrades.AddRange(autoAttack.GetRandomUpgradeChoices(2));

            if (aoeAttack != null)
                upgrades.AddRange(aoeAttack.GetRandomUpgradeChoices(2));

            Shuffle(upgrades);

            while (upgrades.Count > 3)
                upgrades.RemoveAt(upgrades.Count - 1);

            return upgrades;
        }

        public void ApplyUpgrade(WeaponUpgrade upgrade)
        {
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