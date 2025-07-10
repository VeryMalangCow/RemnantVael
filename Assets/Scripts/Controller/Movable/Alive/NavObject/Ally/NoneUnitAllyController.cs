using UnityEngine;

public class NoneUnitAllyController : AllyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> None Unit")]

    [Space(10)]
    [Header("=== Comp")]

    #endregion

    #region - Hide

    // Spawn
    [HideInInspector] public static readonly Vector3 SpawnPos = new Vector3(20000, 20000, -11);

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        // base.OnEnable();
    }

    protected override void Update()
    {
        // base.Update();
    }

    #endregion

    #region Set

    public override void Set_SpawnFirst()
    {
        base.Set_SpawnFirst();

        transform.position = SpawnPos;
    }

    #endregion
}
