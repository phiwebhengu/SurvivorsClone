using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnRadius = 15f;
    private Transform player;
    [SerializeField] private EnemyData testEnemy;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SpawnEnemy(testEnemy, GetSpawnPosition());
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

}
