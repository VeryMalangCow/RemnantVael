using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern_SectorFormMelee : EnemyPattern_Melee
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private List<float> angleList;

    #endregion

    #region Pattern

    protected override void Play_ActualPattern(Vector2 targetDir)
    {
        float centerAngle = DevTool.Get_AngleFromDir(targetDir);
        for (int i = 0; i < angleList.Count; i++)
        {
            float currentAngle = centerAngle + angleList[i];
            Vector2 currentDir = DevTool.Get_DirFromAngle(currentAngle);
            base.Play_ActualPattern(currentDir);
        }
    }

    #endregion
}
