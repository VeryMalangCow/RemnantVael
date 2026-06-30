using System.Collections;
using UnityEngine;

public class ExplosionManager : Singleton<ExplosionManager>, IMainGameInitializer
{
    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;
    
    // Pool
    [SerializeField] private PoolSystem<PlayerExplosionController> playerExplosionPool;
    [SerializeField] private PoolSystem<AllyExplosionController> allyExplosionPool;
    [SerializeField] private PoolSystem<EnemyExplosionController> enemyExplosionPool;

    // Init
    public IEnumerator Initialize()
    {
        yield return playerExplosionPool.InitAsync(16, 8f);
        yield return allyExplosionPool.InitAsync(16, 8f);
        yield return enemyExplosionPool.InitAsync(16, 8f);

        enabled = true;
    }

    // Return
    public void ReturnAll()
    {
        playerExplosionPool.ReturnAll();
        allyExplosionPool.ReturnAll();
        enemyExplosionPool.ReturnAll();
    }


    #region Player

    public PlayerExplosionController SpawnPlayerExplosion() => playerExplosionPool.Dequeue();
    public void RemovePlayerExplosion(PlayerExplosionController explosion) => playerExplosionPool.Enqueue(explosion);

    #endregion

    #region Ally

    public AllyExplosionController SpawnAllyExplosion() => allyExplosionPool.Dequeue();
    public void RemoveAllyExplosion(AllyExplosionController explosion) => allyExplosionPool.Enqueue(explosion);

    #endregion

    #region Enemy

    public EnemyExplosionController SpawnEnemyExplosion() => enemyExplosionPool.Dequeue();
    public void RemoveEnemyExplosion(EnemyExplosionController explosion) => enemyExplosionPool.Enqueue(explosion);

    #endregion
}
