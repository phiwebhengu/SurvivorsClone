using System;
using UnityEngine;

namespace CloneGame.Player
{
    /// <summary>
    /// Tracks the player's experience and level. Enemies/gems call AddExperience().
    /// UI listens to OnExperienceChanged for the gauge and OnLevelUp to show upgrade choices.
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        [Header("Leveling Curve")]
        [Tooltip("XP required to go from level 1 to level 2.")]
        [SerializeField] private float baseXpToLevel = 10f;
        [Tooltip("Multiplier applied to the XP requirement each level (e.g. 1.2 = 20% more XP needed per level).")]
        [SerializeField] private float xpGrowthRate = 1.2f;

        public int CurrentLevel { get; private set; } = 1;
        public float CurrentXp { get; private set; }
        public float XpToNextLevel { get; private set; }

        /// <summary>(currentXp, xpToNextLevel) — for the gauge UI.</summary>
        public event Action<float, float> OnExperienceChanged;
        /// <summary>(newLevel) — for triggering the upgrade-choice screen.</summary>
        public event Action<int> OnLevelUp;

        private void Awake()
        {
            XpToNextLevel = baseXpToLevel;
        }

        public void AddExperience(float amount)
        {
            if (amount <= 0f) return;

            CurrentXp += amount;

            // Loop in case a big pickup crosses more than one level at once.
            while (CurrentXp >= XpToNextLevel)
            {
                CurrentXp -= XpToNextLevel;
                LevelUp();
            }

            OnExperienceChanged?.Invoke(CurrentXp, XpToNextLevel);
        }

        private void LevelUp()
        {
            CurrentLevel++;
            XpToNextLevel *= xpGrowthRate;
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }
}