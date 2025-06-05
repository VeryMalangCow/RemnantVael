using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern_SectorFormMelee : EnemyPattern_Melee
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private List<float> AngleList;

    #endregion

    #region Pattern

    protected override void Play_ActualPattern(Vector2 _TargetDir)
    {
        float centerAngle = DevTool.Get_AngleFromDir(_TargetDir);
        for (int i = 0; i < AngleList.Count; i++)
        {
            float currentAngle = centerAngle + AngleList[i];
            Vector2 currentDir = DevTool.Get_DirFromAngle(currentAngle);
            base.Play_ActualPattern(currentDir);
        }
    }

    #endregion
}
