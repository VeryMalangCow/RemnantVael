using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    #region Value

    #region - Inspector

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

    #endregion

    #region - Hide

    // Current
    [HideInInspector] public List<EnemyController> currentEnemyList = new List<EnemyController>();
    [HideInInspector] public List<EnemyController> poolingAllEnemyList = new List<EnemyController>();

    [HideInInspector] private List<EliteEnemyController> currentEliteEnemyList = new List<EliteEnemyController>();
    [HideInInspector] private BossEnemyController currentBossEnemy = null;

    #endregion

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

    #region Remove (AllEnemy)

    public void Remove_PoolingAllEnemy()
    {
        int amount = poolingAllEnemyList.Count;
        for (int i = amount - 1; i >= 0; i--)
        {
            Destroy(poolingAllEnemyList[i].gameObject);
            poolingAllEnemyList.RemoveAt(i);
        }

        poolingAllEnemyList.Clear();
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