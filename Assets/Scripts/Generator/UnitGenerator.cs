using UnityEngine;

public class UnitGenerator : Singleton<UnitGenerator>
{
    public T GenerateUnit<T>(GameObject _GO, Transform _ParentTF)
    {
        GameObject SpawnedPlayerGO = Instantiate(_GO, _ParentTF);
        SpawnedPlayerGO.TryGetComponent(out T type);
        return type;
    }
}
