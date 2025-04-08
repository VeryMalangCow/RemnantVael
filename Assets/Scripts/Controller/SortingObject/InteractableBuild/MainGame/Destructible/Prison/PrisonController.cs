using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonController : DestructibleBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int DangerRating = 0;

    // Grade
    [HideInInspector] private int MaxRating = 4;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();

        base.Offset();
    }

    #endregion

    #region Set


    private void Set_AnimValue()
    {
        //OnOffAC = UnitManager.Instance.Vault_AC[DangerRank];
        //OnOffStateAC = UnitManager.Instance.Vault_StateAC;

        //BrokenAC = UnitManager.Instance.Vault_BrokenAC[DangerRank];
        //BrokenStateAC = UnitManager.Instance.Vault_StateAC.TypeBase;
    }


    #endregion
}
