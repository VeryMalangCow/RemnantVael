using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    #region Value

    [Space(10)]
    [Header("=== Enemies")]
    [SerializeField] public List<EnemyController> CurrentEnemyList = new List<EnemyController>();

    [Space(10)]
    [Header("=== Materal")]
    [SerializeField] public Material EnemySmokeMaterial;

    [Space(10)]
    [Header("=== Debuff Icon")]
    [SerializeField] public Sprite FlameIcon;
    [SerializeField] public Sprite ColdIcon;
    [SerializeField] public Sprite ElectricityIcon;
    [SerializeField] public Sprite CorrosionIcon;

    [SerializeField] public Sprite InfernoIcon;
    [SerializeField] public Sprite AbsoluteZeroIcon;
    [SerializeField] public Sprite PlasmaIcon;
    [SerializeField] public Sprite DecayIcon;

    [Space(10)]
    [Header("=== Buff Icon")]
    [SerializeField] public Sprite ShieldIcon;
    [SerializeField] public Sprite ATKIcon;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public AnimationClip HittedAC_0;
    [SerializeField] public AnimationClip HittedAC_1;
    [SerializeField] public AnimationClip HittedAC_2;

    #endregion

    #region Get

    // 가장 가까운 적 찾기
    public EnemyController Get_ClosestEnemy(GameObject _TargetGO)
    {
        if (CurrentEnemyList.Count == 0) 
        { return null; }

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_ClosetGO(
                DevTool.Get_GOList(CurrentEnemyList), _TargetGO));
    }

    // 가장 먼 적 찾기
    public EnemyController Get_FurthestEnemy(GameObject _TargetGO)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        return DevTool.Get_ComponentTType<EnemyController>(
            DevTool.Get_FurthestGO(
                DevTool.Get_GOList(CurrentEnemyList), _TargetGO));
    }


    // 일정 구역 내 모든 적 찾기 (가까운 순서대로)
    public List<EnemyController> Get_CloserEnemies(GameObject _TargetGO, float _MaxDis)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        return DevTool.Get_ComponentTTypeList<EnemyController>(
            DevTool.Get_CloserGOList(
                DevTool.Get_GOList(CurrentEnemyList), _TargetGO, _MaxDis));
    }

    // 일정 구역 외 모든 적 찾기 (먼 순서대로)
    public List<EnemyController> Get_FurtherEnemies(GameObject _TargetGO, float _MinDis)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        return DevTool.Get_ComponentTTypeList<EnemyController>(
           DevTool.Get_FurtherGOList(
               DevTool.Get_GOList(CurrentEnemyList), _TargetGO, _MinDis));
    }



    #endregion

}