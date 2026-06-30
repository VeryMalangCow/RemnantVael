using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>, IMainGameInitializer
{
    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    // Player's
    [SerializeField] private PoolSystem<PlayerBulletController> playerBulletPool;
    [SerializeField] private PoolSystem<MissileBulletController> playerMissilePool;

    // Enemy's
    [SerializeField] private PoolSystem<EnemyBulletController> enemyBulletPool;

    // Ally's
    [SerializeField] private PoolSystem<AllyBulletController> allyBulletPool;
    [SerializeField] private PoolSystem<AllyDroppingBombController> allyDroppingBombPool;

    // Module
    [SerializeField] private PoolSystem<PlayerBulletController> module000BulletPool;
    [SerializeField] private PoolSystem<PlayerBulletController> module001BulletPool;

    // Init
    public IEnumerator Initialize()
    {
        yield return playerBulletPool.InitAsync(256, 8f);
        yield return playerMissilePool.InitAsync(32, 8f);

        yield return enemyBulletPool.InitAsync(256, 8f);

        yield return allyBulletPool.InitAsync(256, 8f);
        yield return allyDroppingBombPool.InitAsync(128, 8f);

        yield return module000BulletPool.InitAsync(128, 8f);
        yield return module001BulletPool.InitAsync(128, 8f);

        enabled = true;
    }

    // Centralized Update
    private void Update()
    {
        HandleMissileGuiding();
    }

    private void HandleMissileGuiding()
    {
        var pool = playerMissilePool;
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].SetGuide();
    }

    // Centralized FixedUpdate
    private void FixedUpdate()
    {
        HandleBulletFlying(playerBulletPool);
        HandleBulletFlying(playerMissilePool);
        HandleBulletFlying(enemyBulletPool);
        HandleBulletFlying(allyBulletPool);
    }
    
    private void HandleBulletFlying<T>(PoolSystem<T> pool) where T : BulletController
    {
        var activeIndices = pool.activeIndices;
        var objs = pool.objs;
        var dt = Time.fixedDeltaTime;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].PlayInAlive(dt);
    }

    // Return All
    public void ReturnAll()
    {
        playerBulletPool.ReturnAll();
        playerMissilePool.ReturnAll();

        enemyBulletPool.ReturnAll();

        allyBulletPool.ReturnAll();
        allyDroppingBombPool.ReturnAll();

        module000BulletPool.ReturnAll();
        module001BulletPool.ReturnAll();
    }

    #region Player Bullet

    public PlayerBulletController SpawnPlayerBullet() => playerBulletPool.Dequeue();
    public void SpawnPlayerBullets(int amount, List<PlayerBulletController> list) => playerBulletPool.DequeueMany(amount, list);
    public void RemovePlayerBullet(PlayerBulletController bullet) => playerBulletPool.Enqueue(bullet);
    
    #endregion

    #region Player Missile

    public MissileBulletController SpawnPlayerMissile() => playerMissilePool.Dequeue();
    public void RemovePlayerMissile(MissileBulletController bullet) => playerMissilePool.Enqueue(bullet);
    
    #endregion

    #region Enemy Bullet

    public EnemyBulletController SpawnEnemyBullet() => enemyBulletPool.Dequeue();
    public void RemoveEnemyBullet(EnemyBulletController bullet) => enemyBulletPool.Enqueue(bullet);

    #endregion

    #region ally Bullet

    public AllyBulletController SpawnAllyBullet() => allyBulletPool.Dequeue();
    public void RemoveAllyBullet(AllyBulletController bullet) => allyBulletPool.Enqueue(bullet);

    public AllyDroppingBombController SpawnAllyDroppingBomb() => allyDroppingBombPool.Dequeue();
    public void RemoveAllyDroppingBomb(AllyDroppingBombController bullet) => allyDroppingBombPool.Enqueue(bullet);

    #endregion

    #region Module

    public PlayerBulletController SpawnModule000Bullet() => module000BulletPool.Dequeue();
    public void RemoveModule000Bullet(PlayerBulletController bullet) => module000BulletPool.Enqueue(bullet);

    #endregion
}
