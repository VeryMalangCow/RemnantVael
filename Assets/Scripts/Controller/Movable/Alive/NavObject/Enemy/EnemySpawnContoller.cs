using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawnContoller : MonoBehaviour
{
    [FormerlySerializedAs("EnemyType")][SerializeField] private eEnemy enemyType;
    [FormerlySerializedAs("SpawnID")][SerializeField] private int spawnId;

    public eEnemy Get_EnemyType()
    {
        return enemyType;
    }

    public int Get_SpawnID()
    {
        return spawnId;
    }
}
