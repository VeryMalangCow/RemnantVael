using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>
{
    // Player's
    [SerializeField] private PoolSystem<PlayerBulletController> playerBulletPool;

    // Mono
    protected override void Awake()
    {
        base.Awake();

        playerBulletPool.Init(256);
    }

    // Centralized FixedUpdate
    private void FixedUpdate()
    {
        FlyPlayerBulletPool();
    }
    
    private void FlyPlayerBulletPool()
    {
        for (int i = playerBulletPool.activeIndices.Count - 1; i >= 0; i--)
        {
            playerBulletPool.objs[playerBulletPool.activeIndices[i]].PlayInAlive(Time.fixedDeltaTime);
        }
    }

    // Spawn
    public PlayerBulletController SpawnPlayerBullet()
    {
        return playerBulletPool.Dequeue();
    }

    // Spawn Many
    public void SpawnPlayerBullets(int amount, List<PlayerBulletController> list)
    {
        playerBulletPool.DequeueMany(amount, list);
    }

    // Remove
    public void RemovePlayerBullet(PlayerBulletController bullet)
    {
        playerBulletPool.Enqueue(bullet);
    }
}
