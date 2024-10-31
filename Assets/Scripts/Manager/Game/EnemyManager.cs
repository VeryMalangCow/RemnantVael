using System.Collections.Generic;
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

    #endregion

    #region Set

    public EnemyController GetClosestEnemy(Vector2 _TargetVec)
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

    #endregion

}