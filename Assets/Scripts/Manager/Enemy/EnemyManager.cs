using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>, IMainGameInitializer
{  
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(10)]
    [Header("=== Materal")]
    [SerializeField] public Material enemySmokeMaterial;

    [Space(10)]
    [Header("=== Debuff Icon")]
    [SerializeField] public Sprite flameIcon;
    [SerializeField] public Sprite coldIcon;
    [SerializeField] public Sprite electricityIcon;
    [SerializeField] public Sprite corrosionIcon;

    [SerializeField] public Sprite infernoIcon;
    [SerializeField] public Sprite absoluteZeroIcon;
    [SerializeField] public Sprite plasmaIcon;
    [SerializeField] public Sprite decayIcon;

    [Space(10)]
    [Header("=== Buff Icon")]
    [SerializeField] public Sprite shieldIcon;
    [SerializeField] public Sprite atkIcon;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public AnimationClip hittedAC_0;
    [SerializeField] public AnimationClip hittedAC_1;
    [SerializeField] public AnimationClip hittedAC_2;

    [Space(10)]
    [Header("=== Pool")]
    [SerializeField] private List<PoolSystem<NormalEnemyController>> normalEnemyPools;
    [SerializeField] private Transform normalEnemyParentTf;
    [SerializeField] private List<PoolSystem<EliteEnemyController>> eliteEnemyPools;
    [SerializeField] private Transform eliteEnemyParentTf;
    [SerializeField] private List<PoolSystem<BossEnemyController>> bossEnemyPools;
    [SerializeField] private Transform bossEnemyParentTf;

    // Current
    [HideInInspector] public List<EnemyController> currentEnemyList = new List<EnemyController>();

    [HideInInspector] private List<EliteEnemyController> currentEliteEnemyList = new List<EliteEnemyController>();
    [HideInInspector] private BossEnemyController currentBossEnemy = null;


    public IEnumerator Initialize()
    {
        for (int i = 0; i < normalEnemyPools.Count; i++)
            yield return normalEnemyPools[i].InitAsync(normalEnemyParentTf, 16, 8f);
        for (int i = 0; i < eliteEnemyPools.Count; i++)
            yield return eliteEnemyPools[i].InitAsync(eliteEnemyParentTf, 4, 8f);
        for (int i = 0; i < bossEnemyPools.Count; i++)
            yield return bossEnemyPools[i].InitAsync(bossEnemyParentTf, 2, 8f);

        yield return null;

        enabled = true;
    }

    // Centralized Update
    private void Update()
    {
        HandleChargeSkill();
    }

    private void HandleChargeSkill()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < normalEnemyPools.Count; i++)
            HandleChargeSkill(normalEnemyPools[i], dt);
        for (int i = 0; i < eliteEnemyPools.Count; i++)
            HandleChargeSkill(eliteEnemyPools[i], dt);
        for (int i = 0; i < bossEnemyPools.Count; i++)
            HandleChargeSkill(bossEnemyPools[i], dt);
    }

    private void HandleChargeSkill<T>(PoolSystem<T> pool, float dt) where T : EnemyController
    {
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
        {
            objs[activeIndices[i]].HandleChargeSkill(dt);
            objs[activeIndices[i]].HandleLookAtTarget();
        }
    }


    // Centralized FixedUpdate
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float fdt = Time.fixedDeltaTime;

        for (int i = 0; i < normalEnemyPools.Count; i++)
            HandleMovement(normalEnemyPools[i], fdt);
        for (int i = 0; i < eliteEnemyPools.Count; i++)
            HandleMovement(eliteEnemyPools[i], fdt);
        for (int i = 0; i < bossEnemyPools.Count; i++)
            HandleMovement(bossEnemyPools[i], fdt);
    }

    private void HandleMovement<T>(PoolSystem<T> pool, float fdt) where T : EnemyController
    {
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
        {
            objs[activeIndices[i]].HandleMovement(fdt);
        }
    }

    #region Spawn & Remove

    // Normal
    public NormalEnemyController SpawnNormalEnemy(int enemyId) => normalEnemyPools[enemyId].Dequeue();
    public void RemoveNormalEnemy(NormalEnemyController enemy, int enemyId) => normalEnemyPools[enemyId].Enqueue(enemy);

    // Elite
    public EliteEnemyController SpawnEliteEnemy(int enemyId) => eliteEnemyPools[enemyId].Dequeue();
    public void RemoveEliteEnemy(EliteEnemyController enemy, int enemyId) => eliteEnemyPools[enemyId].Enqueue(enemy);

    // Boss
    public BossEnemyController SpawnBossEnemy(int enemyId) => bossEnemyPools[enemyId].Dequeue();
    public void RemoveBossEnemy(BossEnemyController enemy, int enemyId) => bossEnemyPools[enemyId].Enqueue(enemy);


    public EnemyController SpawnEnemy(eEnemy type, int enemyId)
    {
        if (type == eEnemy.Normal)
        {
            Debug.Log($"color<red>EnemyDequeue : {type} : {enemyId}");
            return SpawnNormalEnemy(enemyId);
        }
        else if (type == eEnemy.Elite)
        {
            Debug.Log($"color<red>EnemyDequeue : {type} : {enemyId}");
            return SpawnEliteEnemy(enemyId);
        }
        else if (type == eEnemy.Boss)
        {
            Debug.Log($"color<red>EnemyDequeue : {type} : {enemyId}");
            return SpawnBossEnemy(enemyId);
        }

        return null;
    }

    public void RemoveEnemy(EnemyController enemy, eEnemy type, int enemyId)
    {
        if (type == eEnemy.Normal)
        {
            Debug.Log($"color<red>EnemyEnqueue : {type} : {enemyId}");
            NormalEnemyController normalEnemy = enemy as NormalEnemyController;
            if (normalEnemy != null) RemoveNormalEnemy(normalEnemy, enemyId);
        }
        else if (type == eEnemy.Elite)
        {
            Debug.Log($"color<red>EnemyEnqueue : {type} : {enemyId}");
            EliteEnemyController eliteEnemy = enemy as EliteEnemyController;
            if (eliteEnemy != null) RemoveEliteEnemy(eliteEnemy, enemyId);
        }
        else if (type == eEnemy.Boss)
        {
            Debug.Log($"color<red>EnemyEnqueue : {type} : {enemyId}");
            BossEnemyController bossEnemy = enemy as BossEnemyController;
            if (bossEnemy != null) RemoveBossEnemy(bossEnemy, enemyId);
        }
    }

    #endregion

    #region Prod

    public Sprite GetEliteProdSprite(int id) => eliteEnemyPools[id].Prefab.battleProdSprite;
    public Sprite GetBossProdSprite(int id) => bossEnemyPools[id].Prefab.battleProdSprite;

    #endregion

    #region Get

    // 가장 가까운 적 찾기
    public EnemyController Get_ClosestEnemy(GameObject targetGO)
    {
        if (currentEnemyList.Count == 0) return null; 

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_ClosetGO(
                DevTool.Get_GOList(currentEnemyList), targetGO));
    }

    public EnemyController Get_ClosestEnemy(GameObject targetGO, out float dis)
    {
        dis = 0f;
        if (currentEnemyList.Count == 0) return null;

        EnemyController result = DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_ClosetGO(
                DevTool.Get_GOList(currentEnemyList), targetGO));

        dis = Vector2.Distance(targetGO.transform.position, result.gameObject.transform.position);
        return result;
    }

    // 가장 먼 적 찾기
    public EnemyController Get_FurthestEnemy(GameObject targetGO)
    {
        if (currentEnemyList.Count == 0) return null; 

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_FurthestGO(
                DevTool.Get_GOList(currentEnemyList), targetGO));
    }


    // 일정 구역 내 모든 적 찾기 (가까운 순서대로)
    public List<EnemyController> Get_CloserEnemies(GameObject targetGO, float maxDis)
    {
        if (currentEnemyList.Count == 0) return null; 

        return DevTool.Get_ComponentTTypeList<EnemyController>(
            DevTool.Get_CloserGOList(
                DevTool.Get_GOList(currentEnemyList), targetGO, maxDis));
    }

    // 일정 구역 외 모든 적 찾기 (먼 순서대로)
    public List<EnemyController> Get_FurtherEnemies(GameObject targetGO, float minDis)
    {
        if (currentEnemyList.Count == 0) return null; 

        return DevTool.Get_ComponentTTypeList<EnemyController>(
           DevTool.Get_FurtherGOList(
               DevTool.Get_GOList(currentEnemyList), targetGO, minDis));
    }



    #endregion

    #region Elite

    public void Add_EliteEnemy(EliteEnemyController eliteEnemy)
    {
        DevTool.Add_InList(currentEliteEnemyList, eliteEnemy);
        Set_SpecialEnemyHUD();
    }

    public void Remove_EliteEnemy(EliteEnemyController eliteEnemy)
    {
        DevTool.Remove_InList(currentEliteEnemyList, eliteEnemy);
        Set_SpecialEnemyHUD();
    }

    #endregion

    #region Boss

    public void SetOn_BossEnemy(BossEnemyController bossEnemy)
    {
        currentBossEnemy = bossEnemy;
        Set_SpecialEnemyHUD();
    }
    
    public void SetOff_BossEnemy()
    {
        currentBossEnemy = null;
        Set_SpecialEnemyHUD();
    }

    #endregion

    #region HUD

    private void Set_SpecialEnemyHUD()
    {
        int index = 0;
        if (currentBossEnemy != null)
        {
            currentBossEnemy.Set_HUDPanelPos();
            index++;
        }
        
        for (int i = 0; i < currentEliteEnemyList.Count; i++)
        {
            currentEliteEnemyList[i].Set_HUDPanelPos(index); 
            index++;
        }
    }

    #endregion

    #region All Pattern Off

    public void SetOff_AllEnemyPattern()
    {
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            currentEnemyList[i].EndAll_Pattern();
        }
        for (int i = 0; i < currentEliteEnemyList.Count; i++)
        {
            currentEliteEnemyList[i].EndAll_Pattern();
        }
        if (currentBossEnemy != null)
        {
            currentBossEnemy.EndAll_Pattern();
        }
    }

    #endregion
}