using UnityEngine;

public class EnemySpawnContoller : MonoBehaviour
{
    [SerializeField] private eEnemy enemyType;
    [SerializeField] private int spawnId;

    public eEnemy GetEnemyType()
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
