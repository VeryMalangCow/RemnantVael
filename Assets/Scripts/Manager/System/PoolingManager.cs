using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player Bullet")]
    [SerializeField] public TTypePooling<PlayerBulletController> PlayerBullet;

    [Header("=== Absorb Item")]
    [SerializeField] public TTypePooling<AbsorbItemController> EnergyParticle;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Get

    public T GetOP<T>(GameObject _SpawnGO, Transform _ParnetTF, Queue<T> _Queue)
    {
        // No Object
        if (_Queue.Count <= 0)
        {
            GameObject GenGO = Instantiate(_SpawnGO, _ParnetTF);
            GenGO.TryGetComponent(out T typeClass);
            GenGO.SetActive(false);
            return typeClass;
        }
        else
        {
            T getTypeClass = _Queue.Dequeue();
            return getTypeClass;
        }
    }

    public PlayerBulletController GetOP_PlayerBullet()
    {
        return GetOP<PlayerBulletController>(PlayerBullet.Prefab, PlayerBullet.ParentTF, PlayerBullet.Queue);
    }

    public AbsorbItemController GetOP_AbsorbItem()
    {
        return GetOP<AbsorbItemController>(EnergyParticle.Prefab, EnergyParticle.ParentTF, EnergyParticle.Queue);
    }

    #endregion
}

[System.Serializable]
public class TTypePooling<T>
{
    [SerializeField] public GameObject Prefab;
    [SerializeField] public Transform ParentTF;
    [SerializeField] public Queue<T> Queue = new Queue<T>();
}
