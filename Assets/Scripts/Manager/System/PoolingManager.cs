using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    #region Value

    [Header("=== Player")]
    [SerializeField] public TTypePooling<PlayerBulletController> playerBullet;
    [SerializeField] public TTypePooling<PlayerAttackerController> playerAttackers;
    [SerializeField] public TTypePooling<PlayerExplosionController> playerExplosions;

    [Header("=== Item")]
    [SerializeField] public TTypePooling<JouleController> joule;
    [SerializeField] public TTypePooling<BetteryShardController> betteryShard;
    [SerializeField] public TTypePooling<ModuleShardController> moduleShard;
    [SerializeField] public TTypePooling<OverriderController> overrider;
    [SerializeField] public TTypePooling<CreditController> credit;
    [SerializeField] public TTypePooling<ModuleItemController> moduleItems;
    [SerializeField] public TTypePooling<KeycardItemController> keycardItems;
    [SerializeField] public TTypePooling<CoreItemController> coreItems;

    [Header("=== Skill")]
    [SerializeField] public TTypePooling<MissileBulletController> missileBullet;

    [Header("=== Ally")]
    [SerializeField] public TTypePooling<AllyBulletController> baseAllyBullet;
    [SerializeField] public TTypePooling<AllyDroppingBombController> droppingAllyBullet;
    [SerializeField] public TTypePooling<AllyExplosionController> allyExplosions;
    [SerializeField] public TTypePooling<AllyTotemeController> allyTotemes;

    [Header("=== Enemy")]
    [SerializeField] public TTypePooling<EnemyBulletController> enemyBullets;
    [SerializeField] public TTypePooling<EnemyAttackerController> enemyAttackers;
    [HideInInspector] public List<TTypePooling<NormalEnemyController>> currentStageEnemies;
    [HideInInspector] public List<TTypePooling<EliteEnemyController>> currentStageEliteEnemies;
    [HideInInspector] public List<TTypePooling<BossEnemyController>> currentStageBossEnemies;
    [SerializeField] public Transform enemyParentTF;    
    [SerializeField] public TTypePooling<EnemyExplosionController> enemyExplosions;

    [Header("=== Effect Img")]
    [SerializeField] public TTypePooling<DeadParticleController> deadParticles;
    [SerializeField] public TTypePooling<SpriteRenderer> afterImgs;
    [SerializeField] public TTypePooling<SpriteRenderer> explosionImgs;
    [SerializeField] public TTypePooling<OnceTimeAnimController> onlyOnceAnimators;
    [SerializeField] public TTypePooling<SpriteRenderer> areaPointSRs;

    [Header("=== UI")]
    [SerializeField] public TTypePooling<WorldTxtEUIController> dmgTxtCanvases;
    [SerializeField] public TTypePooling<BuffIconEUIController> buffIcons;
    [SerializeField] public Transform poolingWorldUi;

    [Header("=== MI")]
    [SerializeField] public TTypePooling<PlayerBulletController> moduleItem_000_Bullets;
    [SerializeField] public TTypePooling<PlayerBulletController> moduleItem_001_Bullets;

    [Header("=== Missing")]
    [SerializeField] public Transform ifMissingTF;

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
        // Player
        playerBullet.Queue.Clear();
        playerAttackers.Queue.Clear();

        // Player_00
        missileBullet.Queue.Clear();

        // Item
        joule.Queue.Clear();
        betteryShard.Queue.Clear();
        moduleShard.Queue.Clear();
        overrider.Queue.Clear();
        credit.Queue.Clear();
        moduleItems.Queue.Clear();

        // Ally Attack
        baseAllyBullet.Queue.Clear();
        droppingAllyBullet.Queue.Clear();

        // Enemy Attack
        enemyBullets.Queue.Clear();
        enemyAttackers.Queue.Clear();

        // Enemy
        for (int i = 0; i < currentStageEnemies.Count; i++)
            currentStageEnemies[i].Queue.Clear();
        for (int i = 0; i < currentStageEliteEnemies.Count; i++)
            currentStageEliteEnemies[i].Queue.Clear();
        for (int i = 0; i < currentStageBossEnemies.Count; i++)
            currentStageBossEnemies[i].Queue.Clear();

        EnemyManager.Instance.Remove_PoolingAllEnemy();

        // VFX
        afterImgs.Queue.Clear();
        explosionImgs.Queue.Clear();
        onlyOnceAnimators.Queue.Clear();
        deadParticles.Queue.Clear();

        // Module Sync
        moduleItem_000_Bullets.Queue.Clear();
        moduleItem_001_Bullets.Queue.Clear();
    }

    #endregion

    #region Get

    // Object (Single)
    public T Get_OP<T>(TTypePooling<T> _Pooling)
    {
        return Get_OP(_Pooling.Prefab, _Pooling.Queue);
    }

    private T Get_OP<T>(GameObject _SpawnGO, Queue<T> _Queue)
    {
        // No Object
        if (_Queue.Count <= 0)
        {
            GameObject GenGO = Instantiate(_SpawnGO);
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
        return Get_OP_List(_Pooling.Prefab, _Pooling.Queue, _Amount);
    }

    public List<T> Get_OP_List<T>(GameObject _SpawnGO, Queue<T> _Queue, int _Amount)
    {
        List<T> tTypeList = new List<T>();

        if (_Queue.Count < _Amount) // Queue 내에 오브젝트가 부족하다면
        {
            for (int i = 0; i < _Amount; i++)
            {
                GameObject GenGO = Instantiate(_SpawnGO);
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
        => Get_OP(baseAllyBullet.Prefab, baseAllyBullet.Queue);
    
    public AllyDroppingBombController Get_OP_DroppingAllyBullet()
        => Get_OP(droppingAllyBullet.Prefab, droppingAllyBullet.Queue);
    

    public AllyExplosionController Get_OP_AllyExplosion()
        => Get_OP(allyExplosions.Prefab, allyExplosions.Queue);
    

    public AllyTotemeController Get_OP_AllyToteme()
        => Get_OP(allyTotemes.Prefab, allyTotemes.Queue);
    

    #endregion

    #region Player

    // Player Bullet
    public PlayerBulletController Get_OP_PlayerBullet()
        => Get_OP(playerBullet.Prefab, playerBullet.Queue);
    
    public List<PlayerBulletController> Get_OP_PlayerBullet(int _Amount)
        => Get_OP_List(playerBullet.Prefab, playerBullet.Queue, _Amount);
    


    // Player Attacker
    public PlayerAttackerController Get_OP_PlayerAttacker()
        => Get_OP(playerAttackers.Prefab, playerAttackers.Queue);
    

    // Player Explosion
    public PlayerExplosionController Get_OP_PlayerExplosion()
        => Get_OP(playerExplosions.Prefab, playerExplosions.Queue);
    

    #endregion

    #region Item

    // Joule
    public JouleController Get_OP_Joule()
        => Get_OP(joule.Prefab, joule.Queue);
    

    // Bettery Shard
    public BetteryShardController Get_OP_BetteryShard()
        => Get_OP(betteryShard.Prefab, betteryShard.Queue);
    

    // Module Shard
    public ModuleShardController Get_OP_ModuleShard()
        => Get_OP(moduleShard.Prefab, moduleShard.Queue);
    

    // Overrider
    public OverriderController Get_OP_Overrider()
        => Get_OP(overrider.Prefab, overrider.Queue);
    

    // Credit
    public CreditController Get_OP_Credit()
        => Get_OP(credit.Prefab, credit.Queue);
    

    // Module Item
    public ModuleItemController Get_OP_ModuleItem()
        => Get_OP(moduleItems.Prefab, moduleItems.Queue);
    

    // Keycard Item
    public KeycardItemController Get_OP_KeycardItem()
        => Get_OP(keycardItems.Prefab, keycardItems.Queue);
    

    // Core Item
    public CoreItemController Get_OP_CoreItem()
        => Get_OP(coreItems.Prefab, coreItems.Queue);
    

    #endregion

    #region Missile

    // Missile
    public MissileBulletController Get_OP_Missile()
        => Get_OP(missileBullet.Prefab, missileBullet.Queue);
    

    #endregion

    #region Enemy

    // Enemy

    public EnemyController Get_OP_Enemy(eEnemy _Type, int _EnemyID)
    {
        if (_Type == eEnemy.Normal)
        {
            TTypePooling<NormalEnemyController> enemy = Get_CorrectEnemyQueue(_EnemyID);
            return Get_OP(enemy.Prefab, enemy.Queue);
        }
        else if (_Type == eEnemy.Elite)
        {
            TTypePooling<EliteEnemyController> enemy = Get_CorrectEliteEnemyQueue(_EnemyID);
            return Get_OP(enemy.Prefab, enemy.Queue);
        }
        else if (_Type == eEnemy.Boss)
        {
            TTypePooling<BossEnemyController> enemy = Get_CorrectBossEnemyQueue(_EnemyID);
            return Get_OP(enemy.Prefab, enemy.Queue);
        }

        return null;
    }

    public void Set_EnqueueEnemy(NormalEnemyController _Enemy)
    {
        Get_CorrectEnemyQueue(_Enemy.Get_ID()).Enqueue(_Enemy);
    }

    public void Set_EnqueueEliteEnemy(EliteEnemyController _EliteEnemy)
    {
        Get_CorrectEliteEnemyQueue(_EliteEnemy.Get_ID()).Enqueue(_EliteEnemy);
    }

    public void Set_EnqueueBossEnemy(BossEnemyController _BossEnemy)
    {
        Get_CorrectBossEnemyQueue(_BossEnemy.Get_ID()).Enqueue(_BossEnemy);
    }

    // Offset
    public void Offset_EnemiesPooling(List<GameObject> _EnemyGOs, List<GameObject> _EliteEnemyGOs, List<GameObject> _BossEnemyGOs)
    {
        currentStageEnemies = new List<TTypePooling<NormalEnemyController>>();
        for (int i = 0; i < _EnemyGOs.Count; i++)
            currentStageEnemies.Add(new TTypePooling<NormalEnemyController>(_EnemyGOs[i]));

        currentStageEliteEnemies = new List<TTypePooling<EliteEnemyController>>();
        for (int i = 0; i < _EliteEnemyGOs.Count; i++)
            currentStageEliteEnemies.Add(new TTypePooling<EliteEnemyController>(_EliteEnemyGOs[i]));

        currentStageBossEnemies = new List<TTypePooling<BossEnemyController>>();
        for (int i = 0; i < _BossEnemyGOs.Count; i++)
            currentStageBossEnemies.Add(new TTypePooling<BossEnemyController>(_BossEnemyGOs[i]));

    }

    // Find
    private TTypePooling<NormalEnemyController> Get_CorrectEnemyQueue(int _EnemyID)
    {
        for (int i = 0; i < currentStageEnemies.Count; i++)
        {
            if (currentStageEnemies[i].Prefab.TryGetComponent(out NormalEnemyController EC) && EC.Get_ID() == _EnemyID)
            {
                return currentStageEnemies[i];
            }
        }
        return null;
    }

    private TTypePooling<EliteEnemyController> Get_CorrectEliteEnemyQueue(int _EliteEnemyID)
    {
        for (int i = 0; i < currentStageEliteEnemies.Count; i++)
        {
            if (currentStageEliteEnemies[i].Prefab.TryGetComponent(out EliteEnemyController EC) && EC.Get_ID() == _EliteEnemyID)
            {
                return currentStageEliteEnemies[i];
            }
        }
        return null;
    }

    private TTypePooling<BossEnemyController> Get_CorrectBossEnemyQueue(int _BossEnemyID)
    {
        for (int i = 0; i < currentStageBossEnemies.Count; i++)
        {
            if (currentStageBossEnemies[i].Prefab.TryGetComponent(out BossEnemyController EC) && EC.Get_ID() == _BossEnemyID)
            {
                return currentStageBossEnemies[i];
            }
        }
        return null;
    }

    // Enemy Bullet
    public EnemyBulletController Get_OP_EnemyBullet()
        => Get_OP(enemyBullets.Prefab, enemyBullets.Queue);
    

    // Enemy Attacker
    public EnemyAttackerController Get_OP_EnemyAttacker()
        => Get_OP(enemyAttackers.Prefab, enemyAttackers.Queue);
    

    // Enemy Explosion
    public EnemyExplosionController Get_OP_EnemyExplosion()
        => Get_OP(enemyExplosions.Prefab, enemyExplosions.Queue);
    

    #endregion

    #region UI

    // Damage Txt
    public WorldTxtEUIController Get_OP_DmgTxt()
        => Get_OP(dmgTxtCanvases.Prefab, dmgTxtCanvases.Queue);
    

    // Buff Icon UI
    public BuffIconEUIController Get_OP_BuffUI()
        => Get_OP(buffIcons.Prefab, buffIcons.Queue);
    

    #endregion

    #region Img Anim VFX

    // After Image
    public SpriteRenderer Get_OP_AfterImg()
        => Get_OP(afterImgs.Prefab, afterImgs.Queue);
    

    // After Image
    public SpriteRenderer Get_OP_ExplosionImg()
        => Get_OP(explosionImgs.Prefab, explosionImgs.Queue);
    

    // Hitted Animator
    public OnceTimeAnimController Get_OP_OnlyOnceAnimator()
        => Get_OP(onlyOnceAnimators.Prefab, onlyOnceAnimators.Queue);
    

    // Dead Particle
    public DeadParticleController Get_OP_DeadParticle()
        => Get_OP(deadParticles.Prefab, deadParticles.Queue);
    

    // Area Point
    public List<SpriteRenderer> Get_OP_AreaPointSRList(int _Amount)
        => Get_OP_List(areaPointSRs, _Amount);
    

    
    #endregion

    #region Module Item

    public PlayerBulletController Get_OP_MI_000_Bullets()
        => Get_OP(moduleItem_000_Bullets.Prefab, moduleItem_000_Bullets.Queue);
    

    public PlayerBulletController Get_OP_MI_001_Bullets()
        => Get_OP(moduleItem_001_Bullets.Prefab, moduleItem_001_Bullets.Queue);
    

    #endregion
}

[System.Serializable]
public class TTypePooling<T>
{
    [SerializeField] public GameObject Prefab;
    [SerializeField] public Queue<T> Queue = new Queue<T>();

    public TTypePooling() { }

    public TTypePooling(GameObject _Prefab) 
    { 
        Prefab = _Prefab;
    }

    public void Enqueue(T _Element)
    {
        if (!Queue.Contains(_Element)) 
            Queue.Enqueue(_Element);
    }
}
