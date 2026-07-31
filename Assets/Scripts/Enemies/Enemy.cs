using UnityEngine;
using CloneGame.Combat;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private Health health;

    public EnemyData Data => enemyData;

    // Fired whenever this enemy dies.
    public event Action<Enemy> OnEnemyDied;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.OnDied += HandleDeath;
        }

        Initialize();
    }

    private void Initialize()
    {
      
    }

    private void HandleDeath()
    {
        Debug.Log(enemyData.EnemyName + " died.");

        OnEnemyDied?.Invoke(this);

        if (enemyData.XPGemPrefab != null)
        {
            Instantiate(enemyData.XPGemPrefab, transform.position, Quaternion.identity);
        }

        if (UnityEngine.Random.value <= enemyData.HealthDropChance)
        {
            Instantiate(enemyData.HealthPickupPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }
}