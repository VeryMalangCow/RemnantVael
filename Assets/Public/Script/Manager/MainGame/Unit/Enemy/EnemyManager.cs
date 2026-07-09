using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyPoolSet<T> where T : EnemyController, IPoolable
{
    public Dictionary<int, PoolSystem<T>> dict = new Dictionary<int, PoolSystem<T>>();
    public List<PoolSystem<T>> updateList = new List<PoolSystem<T>>();
    public Transform parentTf;

    // 생성
    public IEnumerator SetEnemyPoolAsync(T[] prefabs, List<int> enemyIndices, int size, float limitMsPerFrame = 8f)
    {
        for (int i = 0; i < enemyIndices.Count; i++)
        {
            int index = enemyIndices[i];
            if (index == -1)
                continue;

            T prefab = prefabs[index];
            PoolSystem<T> pool = new PoolSystem<T>();

            yield return pool.InitAsync(prefab, parentTf, size, limitMsPerFrame);
            dict.Add(index, pool);
            updateList.Add(pool);
            yield return null;
        }
    }

    // 제거
    public IEnumerator DestoryEnemyPoolAsync()
    {
        // 프리펩 모두 삭제
        foreach (var enemyPool in dict)
            yield return enemyPool.Value.DestroyAsync();

        dict.Clear();
        updateList.Clear();
        
    }

    // Update
    public void HandleUpdate(float dt)
    {
        for (int i = 0; i < updateList.Count; i++)
        {
            var pool = updateList[i];
            var objs = pool.objs;
            var activeIndices = pool.activeIndices;

            if (activeIndices == null) 
                return;

            for (int j = activeIndices.Count - 1; j >= 0; j--)
            {
                // Skill
                objs[activeIndices[j]].HandleChargeSkill(dt);
                // Look Player
                objs[activeIndices[j]].HandleLookAtTarget();
            }
        }
    }

    public void HandleFixedUpdate(float dt)
    {
        for (int i = 0; i < updateList.Count; i++)
        {
            var pool = updateList[i];
            var objs = pool.objs;
            var activeIndices = pool.activeIndices;

            for (int j = activeIndices.Count - 1; j >= 0; j--)
            {
                // Movement
                objs[activeIndices[j]].HandleMovement(dt);
            }
        }
    }

}

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
    [Header("=== PoolSet")]
    [SerializeField] private EnemyPoolSet<NormalEnemyController> normalEnemyPoolSet;
    [SerializeField] private EnemyPoolSet<EliteEnemyController> eliteEnemyPoolSet;
    [SerializeField] private EnemyPoolSet<BossEnemyController> bossEnemyPoolSet;


    [Space(10)]
    [Header("=== Prefab (Before Addressable)")]
    [SerializeField] private NormalEnemyController[] normalEnemyPrefabs;
    [SerializeField] private EliteEnemyController[] eliteEnemyPrefabs;
    [SerializeField] private BossEnemyController[] bossEnemyPrefabs;

    // Current
    [HideInInspector] public List<EnemyController> currentEnemies = new List<EnemyController>();

    [HideInInspector] private List<EliteEnemyController> currentEliteEnemies = new List<EliteEnemyController>();
    [HideInInspector] private BossEnemyController currentBossEnemy = null;


    public IEnumerator Initialize()
    {
        yield return null;

        enabled = true;
    }

    // Pool Dict 객체 값 -> 삽입 및 생성
    public IEnumerator SetEnemyPoolsAsync(List<int> normalEnemyIndices, List<int> eliteEnemyIndices, List<int> bossEnemyIndices)
    {
        yield return normalEnemyPoolSet.SetEnemyPoolAsync(normalEnemyPrefabs, normalEnemyIndices, 8);
        yield return eliteEnemyPoolSet.SetEnemyPoolAsync(eliteEnemyPrefabs, eliteEnemyIndices, 4);
        yield return bossEnemyPoolSet.SetEnemyPoolAsync(bossEnemyPrefabs, bossEnemyIndices, 2);
    }

    // Pool Dict 객체 값 -> 삭제 및 데이터 초기화
    public IEnumerator DestoryEnemyPoolsAsync()
    {
        currentEnemies.Clear();
        currentEliteEnemies.Clear();
        currentBossEnemy = null;
        yield return normalEnemyPoolSet.DestoryEnemyPoolAsync();
        yield return eliteEnemyPoolSet.DestoryEnemyPoolAsync();
        yield return bossEnemyPoolSet.DestoryEnemyPoolAsync();
    }


    // Centralized
    private void Update()
    {
        float dt = Time.deltaTime;
        normalEnemyPoolSet.HandleUpdate(dt);
        eliteEnemyPoolSet.HandleUpdate(dt);
        bossEnemyPoolSet.HandleUpdate(dt);
    }

    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;
        normalEnemyPoolSet.HandleFixedUpdate(fdt);
        eliteEnemyPoolSet.HandleFixedUpdate(fdt);
        bossEnemyPoolSet.HandleFixedUpdate(fdt);
    }


    #region Spawn & Remove

    // Normal
    public NormalEnemyController SpawnNormalEnemy(int enemyId) => normalEnemyPoolSet.dict[enemyId].Dequeue();
    public void RemoveNormalEnemy(NormalEnemyController enemy, int enemyId) => normalEnemyPoolSet.dict[enemyId].Enqueue(enemy);

    // Elite
    public EliteEnemyController SpawnEliteEnemy(int enemyId) => eliteEnemyPoolSet.dict[enemyId].Dequeue();
    public void RemoveEliteEnemy(EliteEnemyController enemy, int enemyId) => eliteEnemyPoolSet.dict[enemyId].Enqueue(enemy);

    // Boss
    public BossEnemyController SpawnBossEnemy(int enemyId) => bossEnemyPoolSet.dict[enemyId].Dequeue();
    public void RemoveBossEnemy(BossEnemyController enemy, int enemyId) => bossEnemyPoolSet.dict[enemyId].Enqueue(enemy);


    public EnemyController SpawnEnemy(eEnemy type, int enemyId)
    {
        if (type == eEnemy.Normal)
        {
            Debug.Log($"<color=red>EnemyDequeue : {type} : {enemyId}</color>");
            return SpawnNormalEnemy(enemyId);
        }
        else if (type == eEnemy.Elite)
        {
            Debug.Log($"<color=red>EnemyDequeue : {type} : {enemyId}</color>");
            return SpawnEliteEnemy(enemyId);
        }
        else if (type == eEnemy.Boss)
        {
            Debug.Log($"<color=red>EnemyDequeue : {type} : {enemyId}</color>");
            return SpawnBossEnemy(enemyId);
        }

        return null;
    }

    public void RemoveEnemy(EnemyController enemy, eEnemy type, int enemyId)
    {
        if (type == eEnemy.Normal)
        {
            Debug.Log($"<color=red>EnemyEnqueue</color> : {type} : {enemyId}");
            NormalEnemyController normalEnemy = enemy as NormalEnemyController;
            if (normalEnemy != null) RemoveNormalEnemy(normalEnemy, enemyId);
        }
        else if (type == eEnemy.Elite)
        {
            Debug.Log($"<color=red>EnemyEnqueue</color> : {type} : {enemyId}");
            EliteEnemyController eliteEnemy = enemy as EliteEnemyController;
            if (eliteEnemy != null) RemoveEliteEnemy(eliteEnemy, enemyId);
        }
        else if (type == eEnemy.Boss)
        {
            Debug.Log($"<color=red>EnemyEnqueue</color> : {type} : {enemyId}");
            BossEnemyController bossEnemy = enemy as BossEnemyController;
            if (bossEnemy != null) RemoveBossEnemy(bossEnemy, enemyId);
        }
    }

    #endregion

    #region Prod

    public Sprite GetEliteProdSprite(int id) => eliteEnemyPoolSet.dict[id].Prefab.battleProdSprite;
    public Sprite GetBossProdSprite(int id) => bossEnemyPoolSet.dict[id].Prefab.battleProdSprite;

    #endregion

    #region Get

    // 가장 가까운 적 찾기
    public EnemyController Get_ClosestEnemy(GameObject targetGO)
    {
        if (currentEnemies.Count == 0) return null;

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_ClosetGO(
                DevTool.Get_GOList(currentEnemies), targetGO));
    }

    public EnemyController Get_ClosestEnemy(GameObject targetGO, out float dis)
    {
        dis = 0f;
        if (currentEnemies.Count == 0) return null;

        EnemyController result = DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_ClosetGO(
                DevTool.Get_GOList(currentEnemies), targetGO));

        dis = Vector2.Distance(targetGO.transform.position, result.gameObject.transform.position);
        return result;
    }

    // 가장 먼 적 찾기
    public EnemyController Get_FurthestEnemy(GameObject targetGO)
    {
        if (currentEnemies.Count == 0) return null;

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_FurthestGO(
                DevTool.Get_GOList(currentEnemies), targetGO));
    }


    // 일정 구역 내 모든 적 찾기 (가까운 순서대로)
    public List<EnemyController> Get_CloserEnemies(GameObject targetGO, float maxDis)
    {
        if (currentEnemies.Count == 0) return null;

        return DevTool.Get_ComponentTTypeList<EnemyController>(
            DevTool.Get_CloserGOList(
                DevTool.Get_GOList(currentEnemies), targetGO, maxDis));
    }

    // 일정 구역 외 모든 적 찾기 (먼 순서대로)
    public List<EnemyController> Get_FurtherEnemies(GameObject targetGO, float minDis)
    {
        if (currentEnemies.Count == 0) return null;

        return DevTool.Get_ComponentTTypeList<EnemyController>(
           DevTool.Get_FurtherGOList(
               DevTool.Get_GOList(currentEnemies), targetGO, minDis));
    }



    #endregion

    #region Elite

    public void Add_EliteEnemy(EliteEnemyController eliteEnemy)
    {
        DevTool.Add_InList(currentEliteEnemies, eliteEnemy);
        Set_SpecialEnemyHUD();
    }

    public void Remove_EliteEnemy(EliteEnemyController eliteEnemy)
    {
        DevTool.Remove_InList(currentEliteEnemies, eliteEnemy);
        Set_SpecialEnemyHUD();
    }

    public void ClearEliteEnemy()
    {
        currentEliteEnemies.Clear();

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

        for (int i = 0; i < currentEliteEnemies.Count; i++)
        {
            currentEliteEnemies[i].Set_HUDPanelPos(index);
            index++;
        }
    }

    #endregion

    #region All Pattern Off

    public void SetOff_AllEnemyPattern()
    {
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            currentEnemies[i].EndAll_Pattern();
        }
        for (int i = 0; i < currentEliteEnemies.Count; i++)
        {
            currentEliteEnemies[i].EndAll_Pattern();
        }
        if (currentBossEnemy != null)
        {
            currentBossEnemy.EndAll_Pattern();
        }
    }

    #endregion
}