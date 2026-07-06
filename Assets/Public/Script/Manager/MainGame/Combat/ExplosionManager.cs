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
    [Space(10)]
    [SerializeField] private Transform playerExplosionTf;
    [SerializeField] private Transform alluExplosionTf;
    [SerializeField] private Transform enemyExplosionTf;

    private PoolSystem<PlayerExplosionController> playerExplosionPool = new PoolSystem<PlayerExplosionController>();
    private PoolSystem<AllyExplosionController> allyExplosionPool = new PoolSystem<AllyExplosionController>();
    private PoolSystem<EnemyExplosionController> enemyExplosionPool = new PoolSystem<EnemyExplosionController>();

    // Init
    public IEnumerator Initialize()
    {
        var prefab = StaticResourceManager.instance.ExplosionReso;
        yield return playerExplosionPool.InitAsync(prefab.playerEplosionPrefab, playerExplosionTf, 16, 8f);
        yield return allyExplosionPool.InitAsync(prefab.allyEplosionPrefab, alluExplosionTf, 16, 8f);
        yield return enemyExplosionPool.InitAsync(prefab.enemyEplosionPrefab, enemyExplosionTf, 16, 8f);

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
