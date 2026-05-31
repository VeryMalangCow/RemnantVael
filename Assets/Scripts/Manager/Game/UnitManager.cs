using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    #region - Generator

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator onceTime_AnimGenerator;

    #endregion

    #endregion

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] testGO;
    [SerializeField] private GameObject ultraModeGO;
    public void Test_Cor()
    {
        for (int i = 0; i < testGO.Length; i++)
            testGO[i].gameObject.SetActive(true);
    }

    public void Set_UltraModeGO(bool onOff)
    {
        ultraModeGO.SetActive(onOff);
    }

}
