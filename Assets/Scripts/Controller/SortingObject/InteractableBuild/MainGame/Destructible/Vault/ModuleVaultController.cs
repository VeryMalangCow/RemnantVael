using System.Collections.Generic;
using UnityEngine;

public class ModuleVaultController : VaultController
{
    #region Value

    [HideInInspector] private readonly static Dictionary<int, List<float>> PercentByGrade = new Dictionary<int, List<float>>
    {
        { 0, new List<float> { 0.80f, 0.10f, 0.06f, 0.03f, 0.01f } },
        { 1, new List<float> { 0.60f, 0.20f, 0.10f, 0.07f, 0.03f } },
        { 2, new List<float> { 0.40f, 0.25f, 0.15f, 0.15f, 0.05f } },
        { 3, new List<float> { 0.20f, 0.20f, 0.20f, 0.25f, 0.15f } },
        { 4, new List<float> { 0.05f, 0.25f, 0.25f, 0.25f, 0.20f } }
    };

    private List<float> CurrentPercentByGrade;

    #endregion

    #region Offset

    protected override void Offset()
    {
        IconStateAnim.Set_Anim(new State_Anim(UnitManager.Instance.Vault_ModuleIconAC));
        CurrentPercentByGrade = PercentByGrade[CurrentGrade];

        base.Offset();
    }

    #endregion

    #region Set

    public override void Set_Grade(int _Grade)
    {
        base.Set_Grade(_Grade);

        CurrentPercentByGrade = PercentByGrade[CurrentGrade];
    }

    #endregion

    #region Item

    private void Gen_II_ByGrade()
    {
        Gen_II(DevTool.Get_Rank(CurrentPercentByGrade));
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_II_ByGrade();
    }

    public override void Gen_ItemWhenBreak()
    {
        Gen_II_ByGrade();
    }

    #endregion
}
