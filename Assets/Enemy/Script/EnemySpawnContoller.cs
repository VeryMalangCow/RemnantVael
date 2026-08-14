using UnityEngine;

public class EnemySpawnContoller : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int spawnId;

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public int Get_SpawnID()
    {
        return spawnId;
    }

    public void SetSpawnID(int spawnId)
    {
        this.spawnId = spawnId;
    }
}
