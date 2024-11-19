using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public Material ModuleMaterial_000;

    #endregion

    #region Generate Unit

    public static T GenerateUnit<T>(GameObject _GO, Transform _ParentTF)
    {
        GameObject SpawnedPlayerGO = Instantiate(_GO, _ParentTF);
        SpawnedPlayerGO.TryGetComponent(out T type);
        return type;
    }

    #endregion
}
