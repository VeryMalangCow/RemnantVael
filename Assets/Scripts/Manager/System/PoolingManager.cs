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
    [SerializeField] public TTypePooling<EnemyBulletController> EnemyBullets;
    [SerializeField] public TTypePooling<EnemyAttacker> EnemyAttackers;
    [HideInInspector] public List<TTypePooling<EnemyController>> CurrentStageEnemies;
    [SerializeField] public Transform EnemyParentTF;

    [Header("=== Effect Img")]
    [SerializeField] public TTypePooling<SpriteRenderer> AfterImgs;
    [SerializeField] public TTypePooling<SpriteRenderer> ExplosionImgs;
    [SerializeField] public TTypePooling<OnlyOnceTimeAnimation> OnlyOnceAnimators;

    [Header("=== UI")]
    [SerializeField] public TTypePooling<ModifyEffectWorldTxt> DmgTxtCanvases;
    [SerializeField] public TTypePooling<ModifyBuffIcon> BuffIcons;

    [Header("=== MI")]
    [SerializeField] public TTypePooling<PlayerBulletController> MI_000_Bullets;
    [SerializeField] public TTypePooling<PlayerBulletController> MI_001_Bullets;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Get

    private T Get_OP<T>(TTypePooling<T> _Pooling)
    {
        return Get_OP<T>(_Pooling.Prefab, _Pooling.ParentTF, _Pooling.Queue);
    }

    private T Get_OP<T>(GameObject _SpawnGO, Transform _ParnetTF, Queue<T> _Queue)
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
    public PlayerBulletController Get_OP_PlayerBullet()
    {
        return Get_OP<PlayerBulletController>(PlayerBullet.Prefab, PlayerBullet.ParentTF, PlayerBullet.Queue);
    }

    // Energy Shrapnel
    public EnergyShrapnelController Get_OP_EnergyShrapnel()
    {
        return Get_OP<EnergyShrapnelController>(EnergyShrapnel.Prefab, EnergyShrapnel.ParentTF, EnergyShrapnel.Queue);
    }

    // Bettery Shrapnel
    public BetteryShrapnelController Get_OP_BetteryShrapnel()
    {
        return Get_OP<BetteryShrapnelController>(BetteryShrapnel.Prefab, BetteryShrapnel.ParentTF, BetteryShrapnel.Queue);
    }

    // Module Shrapnel
    public ModuleShrapnelController Get_OP_ModuleShrapnel()
    {
        return Get_OP<ModuleShrapnelController>(ModuleShrapnel.Prefab, ModuleShrapnel.ParentTF, ModuleShrapnel.Queue);
    }

    // Interact Item For Each Kind
    public InteractItemController Get_OP_InteractableItem()
    {
        return Get_OP<InteractItemController>(InteractItems.Prefab, InteractItems.ParentTF, InteractItems.Queue);
    }

    // Player Attacker
    public PlayerAttacker Get_OP_PlayerAttacker()
    {
        return Get_OP<PlayerAttacker>(PlayerAttackers.Prefab, PlayerAttackers.ParentTF, PlayerAttackers.Queue);
    }

    #endregion

    #region Missile

    // Missile
    public MissileBulletController Get_OP_Missile()
    {
        return Get_OP<MissileBulletController>(MissileBullet.Prefab, MissileBullet.ParentTF, MissileBullet.Queue);
    }

    #endregion

    #region Enemy

    // Enemy
    public EnemyController Get_OP_Enemy(int _EnemyID)
    {
        TTypePooling<EnemyController> enemy = Get_CorrectEnemyQueue(_EnemyID);
        return Get_OP<EnemyController>(enemy.Prefab, enemy.ParentTF, enemy.Queue);
    }

    public void Set_EnqueueEnemy(EnemyController _Enemy)
    {
        Get_CorrectEnemyQueue(_Enemy.EnemyID).Queue.Enqueue(_Enemy);
    }

    // Offset
    public void Offset_EnemiesPooling(List<GameObject> _EnemyGOs)
    {
        CurrentStageEnemies = new List<TTypePooling<EnemyController>>();
        for (int i = 0; i < _EnemyGOs.Count; i++)
        {
            TTypePooling<EnemyController> enemy = new TTypePooling<EnemyController>(_EnemyGOs[i], EnemyParentTF);
            CurrentStageEnemies.Add(enemy);
        }
    }

    // Find
    private TTypePooling<EnemyController> Get_CorrectEnemyQueue(int _EnemyID)
    {
        for (int i = 0; i < CurrentStageEnemies.Count; i++)
        {
            if (CurrentStageEnemies[i].Prefab.TryGetComponent(out EnemyController EC) && EC.EnemyID == _EnemyID)
            {
                return CurrentStageEnemies[i];
            }
        }
#if UNITY_EDITOR
        Debug.Log("\'Enemy Queue\' cannot FIND!");
#endif
        return null;
    }


    // Enemy Bullet
    public EnemyBulletController Get_OP_EnemyBullet()
    {
        return Get_OP<EnemyBulletController>(EnemyBullets.Prefab, EnemyBullets.ParentTF, EnemyBullets.Queue);
    }

    // Enemy Attacker
    public EnemyAttacker Get_OP_EnemyAttacker()
    {
        return Get_OP<EnemyAttacker>(EnemyAttackers.Prefab, EnemyAttackers.ParentTF, EnemyAttackers.Queue);
    }

    #endregion

    #region UI

    // Damage Txt
    public ModifyEffectWorldTxt Get_OP_DmgTxt()
    {
        return Get_OP<ModifyEffectWorldTxt>(DmgTxtCanvases.Prefab, DmgTxtCanvases.ParentTF, DmgTxtCanvases.Queue);
    }

    // Buff Icon UI
    public ModifyBuffIcon Get_OP_BuffUI()
    {
        return Get_OP<ModifyBuffIcon>(BuffIcons.Prefab, BuffIcons.ParentTF, BuffIcons.Queue);
    }

    #endregion

    #region Img Anim VFX

    // After Image
    public SpriteRenderer Get_OP_AfterImg()
    {
        return Get_OP<SpriteRenderer>(AfterImgs.Prefab, AfterImgs.ParentTF, AfterImgs.Queue);
    }

    // After Image
    public SpriteRenderer Get_OP_ExplosionImg()
    {
        return Get_OP<SpriteRenderer>(ExplosionImgs.Prefab, ExplosionImgs.ParentTF, ExplosionImgs.Queue);
    }

    // Hitted Animator
    public OnlyOnceTimeAnimation Get_OP_OnlyOnceAnimator()
    {
        return Get_OP<OnlyOnceTimeAnimation>(OnlyOnceAnimators.Prefab, OnlyOnceAnimators.ParentTF, OnlyOnceAnimators.Queue);
    }

    #endregion

    #region Module Item

    public PlayerBulletController Get_OP_MI_000_Bullets()
    {
        return Get_OP<PlayerBulletController>(MI_000_Bullets.Prefab, MI_000_Bullets.ParentTF, MI_000_Bullets.Queue);
    }

    public PlayerBulletController Get_OP_MI_001_Bullets()
    {
        return Get_OP<PlayerBulletController>(MI_001_Bullets.Prefab, MI_001_Bullets.ParentTF, MI_001_Bullets.Queue);
    }

    #endregion
}

[System.Serializable]
public class TTypePooling<T>
{
    [SerializeField] public GameObject Prefab;
    [SerializeField] public Transform ParentTF;
    [SerializeField] public Queue<T> Queue = new Queue<T>();

    public TTypePooling() { }

    public TTypePooling(GameObject _Prefab, Transform _ParentTF) 
    { 
        Prefab = _Prefab;
        ParentTF = _ParentTF;
    }
}
