using System.Collections.Generic;
using UnityEngine;

public class ModuleVaultController : VaultController
{
    #region Value

    [HideInInspector] private readonly static Dictionary<int, List<int>> PercentByGrade = new Dictionary<int, List<int>>
    {
        { 0, new List<int> { 80, 10, 6, 3, 1 } },
        { 1, new List<int> { 60, 20, 10, 7, 3 } },
        { 2, new List<int> { 40, 25, 15, 15, 5 } },
        { 3, new List<int> { 20, 20, 20, 25, 15 } },
        { 4, new List<int> { 5, 25, 25, 25, 20 } }
    };

    private List<int> CurrentPercentByGrade;

    #endregion

    #region Offset

    protected override void Offset()
    {
        IconStateAnim.Set_Anim(new State_Anim(ResourceManager.Instance.vault_ModuleIconAC));
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
        Gen_ModuleItem(DevTool.Get_Rank(CurrentPercentByGrade));
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
