using UnityEngine;

public class BuffTickDmgController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dmg")]

    [Space(10)]
    [Header("=== Const Value")]
    [SerializeField] private float MaxHpPercent = 10; // 최대 체력 비례
    [SerializeField] private float ConstPoint = 10; // 고정 수치

    [Space(10)]
    [Header("=== Stack")]
    [SerializeField] private float StackDmg = 1;

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

    }

    public override void Reduct_Buff()
    {
        PlayerManager.Instance.PlayerController.Take_Damaged(Get_DmgValue());

        base.Reduct_Buff();
    }

    public override void End_Buff()
    {
        base.End_Buff();

    }

    #endregion

    #region Unique

    private float Get_DmgValue()
    {
        float value = 0;

        if (StackDmg != 0)
        { value += CurrentBuffCharge.Value * StackDmg; }

        if (ConstPoint != 0)
        { value += ConstPoint; }

        if (MaxHpPercent != 0)
        { value += PlayerManager.Instance.PlayerController.Get_PercentHP(MaxHpPercent); }
        

        return value;
    }

    #endregion
}
