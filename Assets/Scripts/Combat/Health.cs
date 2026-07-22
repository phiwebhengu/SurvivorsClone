using System;
using UnityEngine;

namespace CloneGame.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float invincibilityDuration = 0.5f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;
        public bool IsInvincible { get; private set; }

        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDied;

        private float invincibilityTimer;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void Update()
        {
            if (invincibilityTimer > 0f)
            {
                invincibilityTimer -= Time.deltaTime;
                if (invincibilityTimer <= 0f)
                    IsInvincible = false;
            }
        }

        public void TakeDamage(float amount, object source = null)
        {
            if (!IsAlive || IsInvincible || amount <= 0f) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0f)
            {
                OnDied?.Invoke();
            }
            else
            {
                StartInvincibility();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void StartInvincibility()
        {
            if (invincibilityDuration <= 0f) return;
            IsInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }
}