using UnityEngine;

public class EnemySpawnContoller : MonoBehaviour
{
    [SerializeField] private eEnemy EnemyType;
    [SerializeField] private int SpawnID;

    public eEnemy Get_EnemyType()
    {
        return EnemyType;
    }

    public int Get_SpawnID()
    {
        return SpawnID;
    }
}
