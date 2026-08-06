using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern_SectorFormRange : EnemyPattern_Range
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Range")]

    [Space(10)]
    [Header("=== State")]
   [SerializeField] private List<float> angleList;

    #endregion

    #region Pattern

    protected override void Play_ActualPattern(Vector2 targetDir)
    {
        float centerAngle = DevTool.GetAngleFromDir(targetDir);
        for (int i = 0; i < angleList.Count; i++)
        {
            float currentAngle = centerAngle + angleList[i];
            Vector2 currentDir = DevTool.GetDirFromAngle(currentAngle);
            base.Play_ActualPattern(currentDir);
        }
    }

    #endregion
}
