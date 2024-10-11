using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public static T GenerateUnit<T>(GameObject _GO, Transform _ParentTF)
    {
        GameObject SpawnedPlayerGO = Instantiate(_GO, _ParentTF);
        SpawnedPlayerGO.TryGetComponent(out T type);
        return type;
    }
}
