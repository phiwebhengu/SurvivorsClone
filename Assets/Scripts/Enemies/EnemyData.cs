using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Type")]
    [SerializeField] private string enemyName;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage; 

    [Header("Spawning")]
    [SerializeField] private int spawnCost;
    [SerializeField] private int unlockLevel;
    [SerializeField] private int lockLevel = 999;    //player level when enemy type stop spawning
    [SerializeField] private float spawnPercent = 1f;  //so that each of the enemies spawn at different rates

    public string EnemyName => enemyName;

    public GameObject Prefab => prefab;

    public float MoveSpeed => moveSpeed;

    public float Damage => damage;

    public int SpawnCost => spawnCost;

    public int UnlockLevel => unlockLevel;

    public int LockLevel => lockLevel;

    public float SpawnPercent => spawnPercent;
}
