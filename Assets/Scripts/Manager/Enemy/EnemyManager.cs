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
    public EnemyController Get_ClosestEnemy(Vector2 _TargetVec)
    {
        if (CurrentEnemyList.Count == 0) 
        { return null; }

        float dis = 0f;
        EnemyController ec = null;

        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            if (!CurrentEnemyList[i].gameObject.activeSelf)
            { continue; }

            float currentDis = Vector2.Distance(CurrentEnemyList[i].transform.position, _TargetVec);
            if (dis > currentDis || dis == 0)
            {
                ec = CurrentEnemyList[i];
                dis = currentDis;
            }
        }

        return ec;
    }

    // 가장 먼 적 찾기
    public EnemyController Get_FurthestEnemy(Vector2 _TargetVec)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        float dis = 0f;
        EnemyController ec = null;

        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            if (!CurrentEnemyList[i].gameObject.activeSelf)
            { continue; }

            float currentDis = Vector2.Distance(CurrentEnemyList[i].transform.position, _TargetVec);
            if (dis < currentDis || dis == 0)
            {
                ec = CurrentEnemyList[i];
                dis = currentDis;
            }
        }

        return ec;
    }


    // 일정 구역 내 모든 적 찾기 (가까운 순서대로)
    public List<EnemyController> Get_CloserEnemies(Vector2 _TargetVec, float _TargetDis)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        List<EnemyController> closerEnemies = new List<EnemyController>();
        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            if (!CurrentEnemyList[i].gameObject.activeSelf)
            { continue; }

            if (_TargetDis >= Vector2.Distance(CurrentEnemyList[i].transform.position, _TargetVec))
            {
                closerEnemies.Add(CurrentEnemyList[i]);
            }
        }
        
        closerEnemies = closerEnemies.OrderBy(obj => Vector2.Distance(obj.transform.position, _TargetVec)).ToList();
        
        return closerEnemies;
    }

    // 일정 구역 외 모든 적 찾기 (가까운 순서대로)
    public List<EnemyController> Get_FurtherEnemies(Vector2 _TargetVec, float _TargetDis)
    {
        if (CurrentEnemyList.Count == 0)
        { return null; }

        List<EnemyController> furtherEnemies = new List<EnemyController>();
        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            if (!CurrentEnemyList[i].gameObject.activeSelf)
            { continue; }

            if (_TargetDis < Vector2.Distance(CurrentEnemyList[i].transform.position, _TargetVec))
            {
                furtherEnemies.Add(CurrentEnemyList[i]);
            }
        }

        furtherEnemies = furtherEnemies.OrderBy(obj => Vector2.Distance(obj.transform.position, _TargetVec)).ToList();

        return furtherEnemies;
    }



    #endregion

}