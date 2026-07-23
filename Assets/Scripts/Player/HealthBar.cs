using UnityEngine;
using UnityEngine.UI;
using CloneGame.Combat;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        if (health == null)
            health = GetComponentInParent<Health>();

        UpdateBar(health.CurrentHealth, health.MaxHealth);

        health.OnHealthChanged += UpdateBar;
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}