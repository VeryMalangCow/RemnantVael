using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player Bullet")]
    [SerializeField] public TTypePooling<PlayerBulletController> PlayerBullet;

    [Header("=== Item")]
    [SerializeField] public TTypePooling<EnergyShrapnelController> EnergyShrapnel;
    [SerializeField] public TTypePooling<BetteryShrapnelController> BetteryShrapnel;
    [SerializeField] public TTypePooling<InteractItemController> InteractItems;

    [Header("=== After Img")]
    [SerializeField] public TTypePooling<SpriteRenderer> PlayerAfterImgs;

    [Header("=== Skill")]
    [SerializeField] public TTypePooling<MissileBulletController> MissileBullet;

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

    // Player Bullet
    public PlayerBulletController GetOP_PlayerBullet()
    {
        return GetOP<PlayerBulletController>(PlayerBullet.Prefab, PlayerBullet.ParentTF, PlayerBullet.Queue);
    }

    // Energy Shrapnel
    public EnergyShrapnelController GetOP_EnergyShrapnel()
    {
        return GetOP<EnergyShrapnelController>(EnergyShrapnel.Prefab, EnergyShrapnel.ParentTF, EnergyShrapnel.Queue);
    }

    // Bettery Shrapnel
    public BetteryShrapnelController GetOP_BetteryShrapnel()
    {
        return GetOP<BetteryShrapnelController>(BetteryShrapnel.Prefab, BetteryShrapnel.ParentTF, BetteryShrapnel.Queue);
    }

    // Interact Item For Each Kind
    public InteractItemController GetOP_InteractableItem()
    {
        return GetOP<InteractItemController>(InteractItems.Prefab, InteractItems.ParentTF, InteractItems.Queue);
    }

    // After Image
    public SpriteRenderer GetOP_AfterImg()
    {
        return GetOP<SpriteRenderer>(PlayerAfterImgs.Prefab, PlayerAfterImgs.ParentTF, PlayerAfterImgs.Queue);
    }
    #endregion

    #region Missile

    // Missile
    public MissileBulletController GetOP_Missile()
    {
        return GetOP<MissileBulletController>(MissileBullet.Prefab, MissileBullet.ParentTF, MissileBullet.Queue);
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
