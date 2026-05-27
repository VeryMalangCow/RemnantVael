using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>
{
    // Player's
    [SerializeField] private PoolSystem<PlayerBulletController> playerBulletPool;
    [SerializeField] private PoolSystem<MissileBulletController> playerMissilePool;

    // Enemy's
    [SerializeField] private PoolSystem<EnemyBulletController> enemyBulletPool;

    // Ally's
    [SerializeField] private PoolSystem<AllyBulletController> allyBulletPool;

    // Mono
    protected override void Awake()
    {
        base.Awake();

        playerBulletPool.Init(256);
        playerMissilePool.Init(32);

        enemyBulletPool.Init(256);

        allyBulletPool.Init(256);
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

    #region Player Bullet

    // Spawn
    public PlayerBulletController SpawnPlayerBullet()
        => playerBulletPool.Dequeue();
    
    // Spawn Many
    public void SpawnPlayerBullets(int amount, List<PlayerBulletController> list)
        => playerBulletPool.DequeueMany(amount, list);
    
    // Remove
    public void RemovePlayerBullet(PlayerBulletController bullet)
       => playerBulletPool.Enqueue(bullet);
    
    #endregion

    #region Player Missile

    // Spawn
    public MissileBulletController SpawnPlayerMissile()
        => playerMissilePool.Dequeue();
    
    // Remove
    public void RemovePlayerMissile(MissileBulletController bullet)
        => playerMissilePool.Enqueue(bullet);
    
    #endregion

    #region Enemy Bullet

    // Spawn
    public EnemyBulletController SpawnEnemyBullet()
        => enemyBulletPool.Dequeue();
    
    // Remove
    public void RemoveEnemyBullet(EnemyBulletController bullet)
        => enemyBulletPool.Enqueue(bullet);

    #endregion

    #region ally Bullet

    // Spawn
    public AllyBulletController SpawnAllyBullet()
        => allyBulletPool.Dequeue();

    // Remove
    public void RemoveAllyBullet(AllyBulletController bullet)
        => allyBulletPool.Enqueue(bullet);

    #endregion
}
