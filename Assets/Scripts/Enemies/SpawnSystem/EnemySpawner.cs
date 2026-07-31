using CloneGame.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float despawnRadius = 30f;
    private Transform player;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private PlayerExperience level;
    private float spawnTimer;
    public int killCount;

    private readonly List<Enemy> activeEnemies = new(); //stores active enemies to fix our spawning problem 
    [SerializeField] private float despawnCheckInterval = 0.25f;


    private float despawnTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        killCount = 0;
    }

    private void Update()
    {
        despawnTimer -= Time.deltaTime;

        if (despawnTimer <= 0f)
        {
            CheckForDistantEnemies();

            despawnTimer = despawnCheckInterval;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnMissingEnemies();

            spawnTimer = difficultyManager.GetSpawnInterval(level.CurrentLevel);
        }
    }

    private void CheckForDistantEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = activeEnemies[i];
            float distance =Vector2.Distance(player.position,enemy.transform.position);

            if (distance > despawnRadius)
            {
                RemoveEnemy(enemy);
            }
        }
    }

    private void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);

        difficultyManager.RegisterEnemyDeath(enemy.Data);

        enemy.OnEnemyDied -= HandleEnemyDeath;

        Destroy(enemy.gameObject);
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
            activeEnemies.Add(enemy);

            difficultyManager.RegisterEnemySpawn(enemyData);
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        killCount++;
        activeEnemies.Remove(enemy);
        difficultyManager.RegisterEnemyDeath(enemy.Data);

        enemy.OnEnemyDied -= HandleEnemyDeath;
    }

}
