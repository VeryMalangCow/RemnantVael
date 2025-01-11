using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player")]
    [SerializeField] public TTypePooling<PlayerBulletController> PlayerBullet;
    [SerializeField] public TTypePooling<EnergyShrapnelController> EnergyShrapnel;
    [SerializeField] public TTypePooling<BetteryShrapnelController> BetteryShrapnel;
    [SerializeField] public TTypePooling<ModuleShrapnelController> ModuleShrapnel;
    [SerializeField] public TTypePooling<InteractItemController> InteractItems;
    [SerializeField] public TTypePooling<PlayerAttacker> PlayerAttackers;

    [Header("=== Skill")]
    [SerializeField] public TTypePooling<MissileBulletController> MissileBullet;

    [Header("=== Enemy")]
    [SerializeField] public TTypePooling<EnemyController> Enemy;
    [SerializeField] public TTypePooling<EnemyBulletController> EnemyBullets;
    [SerializeField] public TTypePooling<EnemyAttacker> EnemyAttackers;

    [Header("=== Effect Img")]
    [SerializeField] public TTypePooling<SpriteRenderer> AfterImgs;
    [SerializeField] public TTypePooling<SpriteRenderer> ExplosionImgs;
    [SerializeField] public TTypePooling<OnlyOnceTimeAnimation> OnlyOnceAnimators;

    [Header("=== UI")]
    [SerializeField] public TTypePooling<ModifyEffectWorldTxt> DmgTxtCanvases;
    [SerializeField] public TTypePooling<ModifyBuffIcon> BuffIcons;

    [Header("=== MI")]
    [SerializeField] public TTypePooling<PlayerBulletController> MI_000_Bullets;

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

    // Module Shrapnel
    public ModuleShrapnelController GetOP_ModuleShrapnel()
    {
        return GetOP<ModuleShrapnelController>(ModuleShrapnel.Prefab, ModuleShrapnel.ParentTF, ModuleShrapnel.Queue);
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

    // Enemy
    public EnemyController GetOP_Enemy(GameObject _EC_Prefab)
    {
        if (_EC_Prefab != null)
        { return GetOP<EnemyController>(_EC_Prefab, Enemy.ParentTF, Enemy.Queue); }
        else
        { return GetOP<EnemyController>(Enemy.Prefab, Enemy.ParentTF, Enemy.Queue); }
    }

    // Enemy Bullet
    public EnemyBulletController GetOP_EnemyBullet()
    {
        return GetOP<EnemyBulletController>(EnemyBullets.Prefab, EnemyBullets.ParentTF, EnemyBullets.Queue);
    }

    // Enemy Attacker
    public EnemyAttacker GetOP_EnemyAttacker()
    {
        return GetOP<EnemyAttacker>(EnemyAttackers.Prefab, EnemyAttackers.ParentTF, EnemyAttackers.Queue);
    }

    #endregion

    #region VFX things

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

    // Hitted Animator
    public OnlyOnceTimeAnimation GetOP_OnlyOnceAnimator()
    {
        return GetOP<OnlyOnceTimeAnimation>(OnlyOnceAnimators.Prefab, OnlyOnceAnimators.ParentTF, OnlyOnceAnimators.Queue);
    }

    #endregion

    #region UI

    // Damage Txt
    public ModifyEffectWorldTxt GetOP_DmgTxt()
    {
        return GetOP<ModifyEffectWorldTxt>(DmgTxtCanvases.Prefab, DmgTxtCanvases.ParentTF, DmgTxtCanvases.Queue);
    }

    // Buff Icon UI
    public ModifyBuffIcon GetOP_BuffUI()
    {
        return GetOP<ModifyBuffIcon>(BuffIcons.Prefab, BuffIcons.ParentTF, BuffIcons.Queue);
    }

    #endregion

    #region Module Item

    public PlayerBulletController GetOP_MI_000_Bullets()
    {
        return GetOP<PlayerBulletController>(MI_000_Bullets.Prefab, MI_000_Bullets.ParentTF, MI_000_Bullets.Queue);
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
