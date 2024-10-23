using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player")]
    [SerializeField] public TTypePooling<PlayerBulletController> PlayerBullet;
    [SerializeField] public TTypePooling<EnergyShrapnelController> EnergyShrapnel;
    [SerializeField] public TTypePooling<BetteryShrapnelController> BetteryShrapnel;
    [SerializeField] public TTypePooling<InteractItemController> InteractItems;
    [SerializeField] public TTypePooling<PlayerAttacker> PlayerAttackers;

    [Header("=== Skill")]
    [SerializeField] public TTypePooling<MissileBulletController> MissileBullet;

    [Header("=== Enemy")]
    [SerializeField] public TTypePooling<EnemyController> Enemy;

    [Header("=== Effect Img")]
    [SerializeField] public TTypePooling<SpriteRenderer> AfterImgs;
    [SerializeField] public TTypePooling<SpriteRenderer> ExplosionImgs;

    [Header("=== UI")]
    [SerializeField] public TTypePooling<ModifyEffectWorldTxt> DmgTxtCanvases;

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

    #endregion

    #region Player

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

    // Player Attacker
    public PlayerAttacker GetOP_PlayerAttacker()
    {
        return GetOP<PlayerAttacker>(PlayerAttackers.Prefab, PlayerAttackers.ParentTF, PlayerAttackers.Queue);
    }

    #endregion

    #region Missile

    // Missile
    public MissileBulletController GetOP_Missile()
    {
        return GetOP<MissileBulletController>(MissileBullet.Prefab, MissileBullet.ParentTF, MissileBullet.Queue);
    }

    #endregion

    #region Enemy

    public EnemyController GetOP_Enemy(GameObject _EC_Prefab)
    {
        if (_EC_Prefab != null)
        { return GetOP<EnemyController>(_EC_Prefab, Enemy.ParentTF, Enemy.Queue); }
        else
        { return GetOP<EnemyController>(Enemy.Prefab, Enemy.ParentTF, Enemy.Queue); }
    }

    #endregion

    #region Effect

    // After Image
    public SpriteRenderer GetOP_AfterImg()
    {
        return GetOP<SpriteRenderer>(AfterImgs.Prefab, AfterImgs.ParentTF, AfterImgs.Queue);
    }

    // After Image
    public SpriteRenderer GetOP_ExplosionImg()
    {
        return GetOP<SpriteRenderer>(ExplosionImgs.Prefab, ExplosionImgs.ParentTF, ExplosionImgs.Queue);
    }

    #endregion

    #region Dmg Txt

    // Damage Txt
    public ModifyEffectWorldTxt GetOP_DmgTxt()
    {
        return GetOP<ModifyEffectWorldTxt>(DmgTxtCanvases.Prefab, DmgTxtCanvases.ParentTF, DmgTxtCanvases.Queue);
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
