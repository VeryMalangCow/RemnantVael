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
        playerBullet.queue.Clear();
        playerAttackers.queue.Clear();

        // Player_00
        missileBullet.queue.Clear();

        // Item
        joule.queue.Clear();
        betteryShard.queue.Clear();
        moduleShard.queue.Clear();
        overrider.queue.Clear();
        credit.queue.Clear();
        moduleItems.queue.Clear();

        // Ally Attack
        baseAllyBullet.queue.Clear();
        droppingAllyBullet.queue.Clear();

        // Enemy Attack
        enemyBullets.queue.Clear();
        enemyAttackers.queue.Clear();

        // Enemy
        for (int i = 0; i < currentStageEnemies.Count; i++)
            currentStageEnemies[i].queue.Clear();
        for (int i = 0; i < currentStageEliteEnemies.Count; i++)
            currentStageEliteEnemies[i].queue.Clear();
        for (int i = 0; i < currentStageBossEnemies.Count; i++)
            currentStageBossEnemies[i].queue.Clear();

        EnemyManager.instance.Remove_PoolingAllEnemy();

        // VFX
        afterImgs.queue.Clear();
        explosionImgs.queue.Clear();
        onlyOnceAnimators.queue.Clear();
        deadParticles.queue.Clear();

        // Module Sync
        moduleItem_000_Bullets.queue.Clear();
        moduleItem_001_Bullets.queue.Clear();
    }

    #endregion

    #region Get

    // Object (Single)
    public T Get_OP<T>(TTypePooling<T> pooling)
    {
        return Get_OP(pooling.prefab, pooling.queue);
    }

    private T Get_OP<T>(GameObject spawnGO, Queue<T> queue)
    {
        // No Object
        if (queue.Count <= 0)
        {
            GameObject GenGO = Instantiate(spawnGO);
            GenGO.TryGetComponent(out T typeClass);
            GenGO.SetActive(false);

            return typeClass;
        }
        else
        {
            T getTypeClass = queue.Dequeue();

            return getTypeClass;
        }
    }

    // List
    public List<T> Get_OP_List<T>(TTypePooling<T> pooling, int amount)
    {
        return Get_OP_List(pooling.prefab, pooling.queue, amount);
    }

    public List<T> Get_OP_List<T>(GameObject spawnGO, Queue<T> queue, int amount)
    {
        List<T> tTypeList = new List<T>();

        if (queue.Count < amount) // Queue 내에 오브젝트가 부족하다면
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject GenGO = Instantiate(spawnGO);
                GenGO.TryGetComponent(out T typeClass);
                GenGO.SetActive(false);

                tTypeList.Add(typeClass);
            }
        }
        else // Queue 내에 오브젝트가 충분하다면
        {
            for (int i = 0; i < amount; i++)
            {
                tTypeList.Add(queue.Dequeue());
            }
        }

        return tTypeList;
    }

    #endregion

    #region Ally

    public AllyBulletController Get_OP_AllyBullet()
        => Get_OP(baseAllyBullet.prefab, baseAllyBullet.queue);
    
    public AllyDroppingBombController Get_OP_DroppingAllyBullet()
        => Get_OP(droppingAllyBullet.prefab, droppingAllyBullet.queue);
    

    public AllyExplosionController Get_OP_AllyExplosion()
        => Get_OP(allyExplosions.prefab, allyExplosions.queue);
    

    public AllyTotemeController Get_OP_AllyToteme()
        => Get_OP(allyTotemes.prefab, allyTotemes.queue);
    

    #endregion

    #region Player

    // Player Bullet
    public PlayerBulletController Get_OP_PlayerBullet()
        => Get_OP(playerBullet.prefab, playerBullet.queue);
    
    public List<PlayerBulletController> Get_OP_PlayerBullet(int amount)
        => Get_OP_List(playerBullet.prefab, playerBullet.queue, amount);
    


    // Player Attacker
    public PlayerAttackerController Get_OP_PlayerAttacker()
        => Get_OP(playerAttackers.prefab, playerAttackers.queue);
    

    // Player Explosion
    public PlayerExplosionController Get_OP_PlayerExplosion()
        => Get_OP(playerExplosions.prefab, playerExplosions.queue);
    

    #endregion

    #region Item

    // Joule
    public JouleController Get_OP_Joule()
        => Get_OP(joule.prefab, joule.queue);
    

    // Bettery Shard
    public BetteryShardController Get_OP_BetteryShard()
        => Get_OP(betteryShard.prefab, betteryShard.queue);
    

    // Module Shard
    public ModuleShardController Get_OP_ModuleShard()
        => Get_OP(moduleShard.prefab, moduleShard.queue);
    

    // Overrider
    public OverriderController Get_OP_Overrider()
        => Get_OP(overrider.prefab, overrider.queue);
    

    // Credit
    public CreditController Get_OP_Credit()
        => Get_OP(credit.prefab, credit.queue);
    

    // Module Item
    public ModuleItemController Get_OP_ModuleItem()
        => Get_OP(moduleItems.prefab, moduleItems.queue);
    

    // Keycard Item
    public KeycardItemController Get_OP_KeycardItem()
        => Get_OP(keycardItems.prefab, keycardItems.queue);
    

    // Core Item
    public CoreItemController Get_OP_CoreItem()
        => Get_OP(coreItems.prefab, coreItems.queue);
    

    #endregion

    #region Missile

    // Missile
    public MissileBulletController Get_OP_Missile()
        => Get_OP(missileBullet.prefab, missileBullet.queue);
    

    #endregion

    #region Enemy

    // Enemy

    public EnemyController Get_OP_Enemy(eEnemy type, int enemyID)
    {
        if (type == eEnemy.Normal)
        {
            TTypePooling<NormalEnemyController> enemy = Get_CorrectEnemyQueue(enemyID);
            return Get_OP(enemy.prefab, enemy.queue);
        }
        else if (type == eEnemy.Elite)
        {
            TTypePooling<EliteEnemyController> enemy = Get_CorrectEliteEnemyQueue(enemyID);
            return Get_OP(enemy.prefab, enemy.queue);
        }
        else if (type == eEnemy.Boss)
        {
            TTypePooling<BossEnemyController> enemy = Get_CorrectBossEnemyQueue(enemyID);
            return Get_OP(enemy.prefab, enemy.queue);
        }

        return null;
    }

    public void Set_EnqueueEnemy(NormalEnemyController enemy)
    {
        Get_CorrectEnemyQueue(enemy.Get_ID()).Enqueue(enemy);
    }

    public void Set_EnqueueEliteEnemy(EliteEnemyController eliteEnemy)
    {
        Get_CorrectEliteEnemyQueue(eliteEnemy.Get_ID()).Enqueue(eliteEnemy);
    }

    public void Set_EnqueueBossEnemy(BossEnemyController bossEnemy)
    {
        Get_CorrectBossEnemyQueue(bossEnemy.Get_ID()).Enqueue(bossEnemy);
    }

    // Offset
    public void Offset_EnemiesPooling(List<GameObject> enemyGOs, List<GameObject> eliteEnemyGOs, List<GameObject> bossEnemyGOs)
    {
        currentStageEnemies = new List<TTypePooling<NormalEnemyController>>();
        for (int i = 0; i < enemyGOs.Count; i++)
            currentStageEnemies.Add(new TTypePooling<NormalEnemyController>(enemyGOs[i]));

        currentStageEliteEnemies = new List<TTypePooling<EliteEnemyController>>();
        for (int i = 0; i < eliteEnemyGOs.Count; i++)
            currentStageEliteEnemies.Add(new TTypePooling<EliteEnemyController>(eliteEnemyGOs[i]));

        currentStageBossEnemies = new List<TTypePooling<BossEnemyController>>();
        for (int i = 0; i < bossEnemyGOs.Count; i++)
            currentStageBossEnemies.Add(new TTypePooling<BossEnemyController>(bossEnemyGOs[i]));

    }

    // Find
    private TTypePooling<NormalEnemyController> Get_CorrectEnemyQueue(int enemyID)
    {
        for (int i = 0; i < currentStageEnemies.Count; i++)
        {
            if (currentStageEnemies[i].prefab.TryGetComponent(out NormalEnemyController enemy) && enemy.Get_ID() == enemyID)
            {
                return currentStageEnemies[i];
            }
        }
        return null;
    }

    private TTypePooling<EliteEnemyController> Get_CorrectEliteEnemyQueue(int eliteEnemyID)
    {
        for (int i = 0; i < currentStageEliteEnemies.Count; i++)
        {
            if (currentStageEliteEnemies[i].prefab.TryGetComponent(out EliteEnemyController EC) && EC.Get_ID() == eliteEnemyID)
            {
                return currentStageEliteEnemies[i];
            }
        }
        return null;
    }

    private TTypePooling<BossEnemyController> Get_CorrectBossEnemyQueue(int bossEnemyID)
    {
        for (int i = 0; i < currentStageBossEnemies.Count; i++)
        {
            if (currentStageBossEnemies[i].prefab.TryGetComponent(out BossEnemyController EC) && EC.Get_ID() == bossEnemyID)
            {
                return currentStageBossEnemies[i];
            }
        }
        return null;
    }

    // Enemy Bullet
    public EnemyBulletController Get_OP_EnemyBullet()
        => Get_OP(enemyBullets.prefab, enemyBullets.queue);
    

    // Enemy Attacker
    public EnemyAttackerController Get_OP_EnemyAttacker()
        => Get_OP(enemyAttackers.prefab, enemyAttackers.queue);
    

    // Enemy Explosion
    public EnemyExplosionController Get_OP_EnemyExplosion()
        => Get_OP(enemyExplosions.prefab, enemyExplosions.queue);
    

    #endregion

    #region UI

    // Damage Txt
    public WorldTxtEUIController Get_OP_DmgTxt()
        => Get_OP(dmgTxtCanvases.prefab, dmgTxtCanvases.queue);
    

    // Buff Icon UI
    public BuffIconEUIController Get_OP_BuffUI()
        => Get_OP(buffIcons.prefab, buffIcons.queue);
    

    #endregion

    #region Img Anim VFX

    // After Image
    public SpriteRenderer Get_OP_AfterImg()
        => Get_OP(afterImgs.prefab, afterImgs.queue);
    

    // After Image
    public SpriteRenderer Get_OP_ExplosionImg()
        => Get_OP(explosionImgs.prefab, explosionImgs.queue);
    

    // Hitted Animator
    public OnceTimeAnimController Get_OP_OnlyOnceAnimator()
        => Get_OP(onlyOnceAnimators.prefab, onlyOnceAnimators.queue);
    

    // Dead Particle
    public DeadParticleController Get_OP_DeadParticle()
        => Get_OP(deadParticles.prefab, deadParticles.queue);
    

    // Area Point
    public List<SpriteRenderer> Get_OP_AreaPointSRList(int amount)
        => Get_OP_List(areaPointSRs, amount);
    

    
    #endregion

    #region Module Item

    public PlayerBulletController Get_OP_MI_000_Bullets()
        => Get_OP(moduleItem_000_Bullets.prefab, moduleItem_000_Bullets.queue);
    

    public PlayerBulletController Get_OP_MI_001_Bullets()
        => Get_OP(moduleItem_001_Bullets.prefab, moduleItem_001_Bullets.queue);
    

    #endregion
}

[System.Serializable]
public class TTypePooling<T>
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public Queue<T> queue = new Queue<T>();

    public TTypePooling() { }

    public TTypePooling(GameObject prefab) 
    { 
        this.prefab = prefab;
    }

    public void Enqueue(T element)
    {
        if (!queue.Contains(element)) 
            queue.Enqueue(element);
    }
}
