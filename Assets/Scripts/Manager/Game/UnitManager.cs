using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public Material ModuleM_000_Explosion;
    [SerializeField] public Material ModuleM_000_Hitted;
    [SerializeField] public Material EnemyM_000_Explosion;

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator Player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator Build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator Enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator OnceTime_AnimGenerator;

    #endregion

    #region Generate Unit

    public static T Gen_Unit<T>(GameObject _GO, Transform _ParentTF)
    {
        GameObject SpawnedPlayerGO = Instantiate(_GO, _ParentTF);
        SpawnedPlayerGO.TryGetComponent(out T type);
        return type;
    }

    #endregion
}
