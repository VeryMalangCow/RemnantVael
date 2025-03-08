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

    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

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
