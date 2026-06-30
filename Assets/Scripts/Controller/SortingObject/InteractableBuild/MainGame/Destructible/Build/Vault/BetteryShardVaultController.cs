using System.Collections.Generic;
using UnityEngine;

public class BetteryShardVaultController : VaultController
{
    #region Value

    [HideInInspector] private readonly static Dictionary<int, CoupleData<int>> amountByGrade = new Dictionary<int, CoupleData<int>>
    {
        { 0, new CoupleData<int>(1, 1) },
        { 1, new CoupleData<int>(1, 2) },
        { 2, new CoupleData<int>(2, 3) },
        { 3, new CoupleData<int>(2, 4) },
        { 4, new CoupleData<int>(3, 5) }
    };

    CoupleData<int> currentAmountByGrade;

    #endregion

    #region Offset

    protected override void Offset()
    {

        iconStateAnim.Set_Anim(new State_Anim(StaticResourceManager.instance.BuildPrefab.vaultPrefab.betteryShardIconAnimation));
        currentAmountByGrade = amountByGrade[currentGrade];

        base.Offset();
    }

    #endregion

    #region Set

    public override void Set_Grade(int grade)
    {
        base.Set_Grade(grade);

        currentAmountByGrade = amountByGrade[currentGrade];
    }

    #endregion

    #region Item

    private void Gen_BS_ByGrade()
    {
        for (int i = 0; i < Random.Range(currentAmountByGrade.typeBase, currentAmountByGrade.typeSpecial + 1); i++) 
            Gen_BS(1);
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_BS_ByGrade();
    }

    public override void Gen_ItemWhenBreak()
    {
        Gen_BS_ByGrade();
    }

    #endregion
}
