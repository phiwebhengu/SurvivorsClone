using UnityEngine;
using CloneGame.Combat;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    [Header("Drops")]
    [Tooltip("XP gem prefab spawned when this enemy dies.")]
    [SerializeField] private GameObject gemPrefab;

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

        if (gemPrefab != null)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }
}
