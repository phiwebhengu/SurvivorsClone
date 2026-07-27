using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{ 
    [SerializeField]
    private EnemyData[] enemies;

    public EnemyData[] Enemies => enemies;
}
