using System.Collections.Generic;
using CloneGame.Player;
using CloneGame.UI;
using UnityEngine;

namespace CloneGame.Combat
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private LevelUpUI levelUpUI;

        private void OnEnable()
        {
            playerExperience.OnLevelUp += HandleLevelUp;
        }

        private void OnDisable()
        {
            playerExperience.OnLevelUp -= HandleLevelUp;
        }

        private void HandleLevelUp(int level)
        {
            List<WeaponUpgrade> upgrades =
                weaponManager.GetUpgradeChoices();

            levelUpUI.Show(upgrades, ApplyUpgrade);
        }

        private void ApplyUpgrade(WeaponUpgrade upgrade)
        {
            weaponManager.ApplyUpgrade(upgrade);
        }
    }
}