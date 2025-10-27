using System.Collections.Generic;
using UnityEngine;

public class JouleVaultController : VaultController
{
    #region Value

    [HideInInspector] private readonly static Dictionary<int, CoupleData<int>> AmountByGrade = new Dictionary<int, CoupleData<int>>
    {
        { 0, new CoupleData<int>(1, 1) },
        { 1, new CoupleData<int>(1, 2) },
        { 2, new CoupleData<int>(2, 3) },
        { 3, new CoupleData<int>(2, 4) },
        { 4, new CoupleData<int>(3, 5) }
    };

    CoupleData<int> CurrentAmountByGrade;

    #endregion

    #region Offset

    protected override void Offset()
    {
        IconStateAnim.Set_Anim(new State_Anim(ResourceManager.Instance.Vault_JIconAC));
        CurrentAmountByGrade = AmountByGrade[CurrentGrade];

        base.Offset();
    }

    #endregion

    #region Set

    public override void Set_Grade(int _Grade)
    {
        base.Set_Grade(_Grade);

        CurrentAmountByGrade = AmountByGrade[CurrentGrade];
    }

    #endregion

    #region Item
    private void Gen_J_ByGrade()
    {
        for (int i = 0; i < Random.Range(CurrentAmountByGrade.TypeBase, CurrentAmountByGrade.TypeSpecial + 1); i++)
            Gen_J(10 * PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value);
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_J_ByGrade();
    }

    public override void Gen_ItemWhenBreak()
    {
        Gen_J_ByGrade();
    }

    #endregion
}
