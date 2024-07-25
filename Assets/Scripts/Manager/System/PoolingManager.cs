using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player Bullet")]
    [SerializeField] private GameObject PlayerBulletPrefab;
    [SerializeField] private Transform PlayerBulletParentTF;
    [SerializeField] public Queue<PlayerBulletController> PlayerBulletQueue = new Queue<PlayerBulletController>();

    [Header("=== Absorb Item")]
    [SerializeField] private GameObject AbsorbItemPrefab;
    [SerializeField] private Transform AbsorbItemParentTF;
    [SerializeField] public Queue<AbsorbItemController> AbsorbItemQueue = new Queue<AbsorbItemController>();

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Get

    public T GetOP<T>(GameObject spawnGO, Transform parnetTF, Queue<T> queue)
    {
        // No Object
        if (PlayerBulletQueue.Count <= 0)
        {
            GameObject GenGO = Instantiate(spawnGO, parnetTF);
            GenGO.TryGetComponent(out T typeClass);
            GenGO.SetActive(false);
            return typeClass;
        }

        T getTypeClass = queue.Dequeue();
        return getTypeClass;
    }

    public PlayerBulletController GetOP_PlayerBullet()
    {
        return GetOP<PlayerBulletController>(PlayerBulletPrefab, PlayerBulletParentTF, PlayerBulletQueue);
    }

    #endregion
}
