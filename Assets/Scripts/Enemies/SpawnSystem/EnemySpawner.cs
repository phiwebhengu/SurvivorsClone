using CloneGame.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnRadius = 15f;
    private Transform player;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private PlayerExperience level;
    private float spawnTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnMissingEnemies();

            spawnTimer = difficultyManager.GetSpawnInterval(level.CurrentLevel);
        }
    }

    public Enemy SpawnEnemy(EnemyData enemyData, Vector2 position)
    {
        GameObject enemyObject =
            Instantiate(enemyData.Prefab, position, Quaternion.identity);

        return enemyObject.GetComponent<Enemy>();
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        return (Vector2)player.position + direction * spawnRadius;
    }

    private void SpawnMissingEnemies()
    {
        List<EnemyData> enemies =
            difficultyManager.GenerateSpawnList(level.CurrentLevel);

        foreach (EnemyData enemyData in enemies)
        {
            Enemy enemy = SpawnEnemy(enemyData, GetSpawnPosition());

            enemy.OnEnemyDied += HandleEnemyDeath;

            difficultyManager.RegisterEnemySpawn(enemyData);
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        difficultyManager.RegisterEnemyDeath(enemy.Data);

        enemy.OnEnemyDied -= HandleEnemyDeath;
    }

}
