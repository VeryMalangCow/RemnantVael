using UnityEngine;

public class EnemySpawnContoller : MonoBehaviour
{
    [SerializeField] private eEnemy enemyType;
    [SerializeField] private int spawnId;

    public eEnemy Get_EnemyType()
    {
        return enemyType;
    }

    public int Get_SpawnID()
    {
        return spawnId;
    }
}
