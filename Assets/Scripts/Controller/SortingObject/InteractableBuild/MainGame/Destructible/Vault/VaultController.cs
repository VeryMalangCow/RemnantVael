using System.Collections.Generic;
using UnityEngine;

public class VaultController : DestructibleBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected StateAnimController IconStateAnim;

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] protected int CurrentGrade = 0;


    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();

        base.Offset();
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        IconStateAnim.ThisSR.sortingOrder = _SortingOrder - 1;
    }

    #endregion

    #region Set

    public virtual void Set_Grade(int _Grade)
    {
        CurrentGrade = _Grade;

        Set_AnimValue();
        Set_StateAnim();
    }

    private void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Vault_AC[CurrentGrade];
        OnOffStateAC = UnitManager.Instance.Vault_StateAC;

        BrokenAC = UnitManager.Instance.Vault_BrokenAC[CurrentGrade];
        BrokenStateAC = UnitManager.Instance.Vault_StateAC.TypeBase;
    }

    #endregion

    #region Change

    public void Change_ToOtherVault(VaultController _OtherVault)
    {
        IsOn = true;
        ThisAnimator = _OtherVault.ThisAnimator;

        OnOffAC = _OtherVault.OnOffAC;
        OnOffStateAC = _OtherVault.OnOffStateAC;

        AOC = _OtherVault.AOC;

        int MaxDur = _OtherVault.MaxDur;
        int CurrentDur = _OtherVault.CurrentDur;

        BrokenAC = _OtherVault.BrokenAC;
        BrokenStateAC = _OtherVault.BrokenStateAC;

        DurFrameSRList = _OtherVault.DurFrameSRList;
        DurInnerSRList = _OtherVault.DurInnerSRList;
    }

    #endregion
}
