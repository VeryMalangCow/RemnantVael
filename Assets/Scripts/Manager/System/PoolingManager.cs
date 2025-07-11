using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player")]
    [SerializeField] public TTypePooling<PlayerBulletController> PlayerBullet;
    [SerializeField] public TTypePooling<JouleController> Joule;
    [SerializeField] public TTypePooling<BetteryShardController> BetteryShard;
    [SerializeField] public TTypePooling<ModuleShardController> ModuleShard;
    [SerializeField] public TTypePooling<OverriderController> Overrider;
    [SerializeField] public TTypePooling<CreditController> Credit;
    [SerializeField] public TTypePooling<InteractItemController> InteractItems;
    [SerializeField] public TTypePooling<PlayerAttackerController> PlayerAttackers;
    [SerializeField] public TTypePooling<PlayerExplosionController> PlayerExplosions;

    [Header("=== Skill")]
    [SerializeField] public TTypePooling<MissileBulletController> MissileBullet;

    [Header("=== Ally")]
    [SerializeField] public TTypePooling<AllyBulletController> BaseAllyBullet;
    [SerializeField] public TTypePooling<AllyDroppingBulletController> DroppingAllyBullet;

    [Header("=== Enemy")]
    [SerializeField] public TTypePooling<EnemyBulletController> EnemyBullets;
    [SerializeField] public TTypePooling<EnemyAttackerController> EnemyAttackers;
    [HideInInspector] public List<TTypePooling<EnemyController>> CurrentStageEnemies;
    [SerializeField] public Transform EnemyParentTF;
    [SerializeField] public TTypePooling<EnemyExplosionController> EnemyExplosions;

    [Header("=== Effect Img")]
    [SerializeField] public TTypePooling<DeadParticleController> DeadParticles;
    [SerializeField] public TTypePooling<SpriteRenderer> AfterImgs;
    [SerializeField] public TTypePooling<SpriteRenderer> ExplosionImgs;
    [SerializeField] public TTypePooling<OnceTimeAnimController> OnlyOnceAnimators;

    [Header("=== UI")]
    [SerializeField] public TTypePooling<WorldTxtEUIController> DmgTxtCanvases;
    [SerializeField] public TTypePooling<BuffIconEUIController> BuffIcons;

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

    #region Remove

    public void Remove_AllQueue()
    {
        PlayerBullet.Queue.Clear();
        Joule.Queue.Clear();
        BetteryShard.Queue.Clear();
        ModuleShard.Queue.Clear();
        Overrider.Queue.Clear();
        Credit.Queue.Clear();
        InteractItems.Queue.Clear();
        PlayerAttackers.Queue.Clear();

        MissileBullet.Queue.Clear();

        BaseAllyBullet.Queue.Clear();
        DroppingAllyBullet.Queue.Clear();

        EnemyBullets.Queue.Clear();
        EnemyAttackers.Queue.Clear();
        for (int i = 0; i < CurrentStageEnemies.Count; i++)
            CurrentStageEnemies[i].Queue.Clear();
        EnemyManager.Instance.Remove_PoolingAllEnemy();

        AfterImgs.Queue.Clear();
        ExplosionImgs.Queue.Clear();
        OnlyOnceAnimators.Queue.Clear();

        MI_000_Bullets.Queue.Clear();
        MI_001_Bullets.Queue.Clear();
    }

    #endregion

    #region Get

    // Object (Single)
    public T Get_OP<T>(TTypePooling<T> _Pooling)
    {
        return Get_OP(_Pooling.Prefab, _Pooling.ParentTF, _Pooling.Queue);
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

    // List
    public List<T> Get_OP_List<T>(TTypePooling<T> _Pooling, int _Amount)
    {
        return Get_OP_List(_Pooling.Prefab, _Pooling.ParentTF, _Pooling.Queue, _Amount);
    }

    public List<T> Get_OP_List<T>(GameObject _SpawnGO, Transform _ParnetTF, Queue<T> _Queue, int _Amount)
    {
        List<T> tTypeList = new List<T>();

        if (_Queue.Count < _Amount) // Queue 내에 오브젝트가 부족하다면
        {
            for (int i = 0; i < _Amount; i++)
            {
                GameObject GenGO = Instantiate(_SpawnGO, _ParnetTF);
                GenGO.TryGetComponent(out T typeClass);
                GenGO.SetActive(false);

                tTypeList.Add(typeClass);
            }
        }
        else // Queue 내에 오브젝트가 충분하다면
        {
            for (int i = 0; i < _Amount; i++)
            {
                tTypeList.Add(_Queue.Dequeue());
            }
        }

        return tTypeList;
    }

    #endregion

    #region Ally

    public AllyBulletController Get_OP_AllyBullet()
    {
        return Get_OP<AllyBulletController>(BaseAllyBullet.Prefab, BaseAllyBullet.ParentTF, BaseAllyBullet.Queue);
    }
    public AllyDroppingBulletController Get_OP_DroppingAllyBullet()
    {
        return Get_OP<AllyDroppingBulletController>(DroppingAllyBullet.Prefab, DroppingAllyBullet.ParentTF, DroppingAllyBullet.Queue);
    }

    #endregion

    #region Player

    // Player Bullet
    public PlayerBulletController Get_OP_PlayerBullet()
    {
        return Get_OP<PlayerBulletController>(PlayerBullet.Prefab, PlayerBullet.ParentTF, PlayerBullet.Queue);
    }
    public List<PlayerBulletController> Get_OP_PlayerBullet(int _Amount)
    {
        return Get_OP_List<PlayerBulletController>(PlayerBullet.Prefab, PlayerBullet.ParentTF, PlayerBullet.Queue, _Amount);
    }

    // Joule
    public JouleController Get_OP_Joule()
    {
        return Get_OP<JouleController>(Joule.Prefab, Joule.ParentTF, Joule.Queue);
    }

    // Bettery Shard
    public BetteryShardController Get_OP_BetteryShard()
    {
        return Get_OP<BetteryShardController>(BetteryShard.Prefab, BetteryShard.ParentTF, BetteryShard.Queue);
    }

    // Module Shard
    public ModuleShardController Get_OP_ModuleShard()
    {
        return Get_OP<ModuleShardController>(ModuleShard.Prefab, ModuleShard.ParentTF, ModuleShard.Queue);
    }
    
    // Overrider
    public OverriderController Get_OP_Overrider()
    {
        return Get_OP<OverriderController>(Overrider.Prefab, Overrider.ParentTF, Overrider.Queue);
    }

    // Credit
    public CreditController Get_OP_Credit()
    {
        return Get_OP<CreditController>(Credit.Prefab, Credit.ParentTF, Credit.Queue);
    }

    // Interact Item For Each Kind
    public InteractItemController Get_OP_InteractableItem()
    {
        return Get_OP<InteractItemController>(InteractItems.Prefab, InteractItems.ParentTF, InteractItems.Queue);
    }

    // Player Attacker
    public PlayerAttackerController Get_OP_PlayerAttacker()
    {
        return Get_OP<PlayerAttackerController>(PlayerAttackers.Prefab, PlayerAttackers.ParentTF, PlayerAttackers.Queue);
    }

    // Player Explosion
    public PlayerExplosionController Get_OP_PlayerExplosion()
    {
        return Get_OP<PlayerExplosionController>(PlayerExplosions.Prefab, PlayerExplosions.ParentTF, PlayerExplosions.Queue);
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
        Get_CorrectEnemyQueue(_Enemy.Get_ID()).Queue.Enqueue(_Enemy);
    }

    // Offset
    public void Offset_EnemiesPooling(List<GameObject> _EnemyGOs)
    {
        CurrentStageEnemies = new List<TTypePooling<EnemyController>>();

        for (int i = 0; i < _EnemyGOs.Count; i++)
            CurrentStageEnemies.Add(new TTypePooling<EnemyController>(_EnemyGOs[i], EnemyParentTF));
    }

    // Find
    private TTypePooling<EnemyController> Get_CorrectEnemyQueue(int _EnemyID)
    {
        for (int i = 0; i < CurrentStageEnemies.Count; i++)
        {
            if (CurrentStageEnemies[i].Prefab.TryGetComponent(out EnemyController EC) && EC.Get_ID() == _EnemyID)
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
    public EnemyAttackerController Get_OP_EnemyAttacker()
    {
        return Get_OP<EnemyAttackerController>(EnemyAttackers.Prefab, EnemyAttackers.ParentTF, EnemyAttackers.Queue);
    }

    // Enemy Explosion
    public EnemyExplosionController Get_OP_EnemyExplosion()
    {
        return Get_OP<EnemyExplosionController>(EnemyExplosions.Prefab, EnemyExplosions.ParentTF, EnemyExplosions.Queue);
    }

    #endregion

    #region UI

    // Damage Txt
    public WorldTxtEUIController Get_OP_DmgTxt()
    {
        return Get_OP<WorldTxtEUIController>(DmgTxtCanvases.Prefab, DmgTxtCanvases.ParentTF, DmgTxtCanvases.Queue);
    }

    // Buff Icon UI
    public BuffIconEUIController Get_OP_BuffUI()
    {
        return Get_OP<BuffIconEUIController>(BuffIcons.Prefab, BuffIcons.ParentTF, BuffIcons.Queue);
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
    public OnceTimeAnimController Get_OP_OnlyOnceAnimator()
    {
        return Get_OP<OnceTimeAnimController>(OnlyOnceAnimators.Prefab, OnlyOnceAnimators.ParentTF, OnlyOnceAnimators.Queue);
    }

    // Dead Particle
    public DeadParticleController Get_OP_DeadParticle()
    {
        return Get_OP<DeadParticleController>(DeadParticles.Prefab, DeadParticles.ParentTF, DeadParticles.Queue);
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
