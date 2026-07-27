using UnityEngine;
using System.Collections.Generic;

public class DifficultyManager : MonoBehaviour
{
    [Header("Difficulty Scaling")]
    [SerializeField] private int baseBudget = 3;
    [SerializeField] private int budgetPerLevel = 2;

    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float minimumSpawnInterval = 0.3f;
    [SerializeField] private float intervalReductionPerLevel = 0.1f;

    [Header("Enemy Pool")]
    [SerializeField] private EnemyDatabase enemyDatabase;

    [Header("Budget Calculations")]
    private int aliveBudget = 0;

    public int GetSpawnBudget(int playerLevel)
    {
        return baseBudget + playerLevel * budgetPerLevel;
    }

    public float GetSpawnInterval(int playerLevel)
    {
        return Mathf.Max(
            minimumSpawnInterval,
            baseSpawnInterval - playerLevel * intervalReductionPerLevel);
    }

    public List<EnemyData> GetAvailableEnemies(int playerLevel)
    {
        List<EnemyData> available = new();

        foreach (EnemyData enemy in enemyDatabase.Enemies)
        {
            if (playerLevel >= enemy.UnlockLevel &&
                playerLevel <= enemy.LockLevel)
            {
                available.Add(enemy);
            }
        }

        return available;
    }

    public void RegisterEnemySpawn(EnemyData enemy)
    {
        aliveBudget += enemy.SpawnCost;
    }

    public void RegisterEnemyDeath(EnemyData enemy)
    {
        aliveBudget -= enemy.SpawnCost;

        aliveBudget = Mathf.Max(0, aliveBudget);
    }

    public int GetMissingBudget(int playerLevel)
    {
        int targetBudget = GetSpawnBudget(playerLevel);

        return Mathf.Max(0, targetBudget - aliveBudget);
    }

    //Method to select a pool of enemy and calculate how many should spawn and what enemy types (based off of player level)

    public List<EnemyData> GenerateSpawnList(int playerLevel)
    {
        List<EnemyData> spawnList = new();

        int remainingBudget = GetMissingBudget(playerLevel);

        while (remainingBudget > 0)
        {
            List<EnemyData> affordableEnemies = new();

            foreach (EnemyData enemy in GetAvailableEnemies(playerLevel))
            {
                if (enemy.SpawnCost <= remainingBudget)
                    affordableEnemies.Add(enemy);
            }

            if (affordableEnemies.Count == 0)
                break;

            int totalWeight = 0;

            foreach (EnemyData enemy in affordableEnemies)
                totalWeight += enemy.SpawnPercent;

            int randomWeight = Random.Range(0, totalWeight);

            EnemyData selectedEnemy = null;

            foreach (EnemyData enemy in affordableEnemies)
            {
                randomWeight -= enemy.SpawnPercent;

                if (randomWeight < 0)
                {
                    selectedEnemy = enemy;
                    break;
                }
            }

            spawnList.Add(selectedEnemy);
            remainingBudget -= selectedEnemy.SpawnCost;
        }

        return spawnList;
    }
}
