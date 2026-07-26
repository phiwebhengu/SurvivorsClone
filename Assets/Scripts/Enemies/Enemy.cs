using UnityEngine;
using CloneGame.Combat;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private Health health;

    public EnemyData Data => enemyData;

    private void Awake()
    {
        health = GetComponent<Health>();

        health.OnDied += HandleDeath;

        Initialize();
    }

    private void Initialize()
    {

    }

    private void HandleDeath()
    {
        Debug.Log(enemyData.EnemyName + " died.");

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }
}