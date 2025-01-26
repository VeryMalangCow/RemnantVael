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

    public override void GainBuff()
    {
        base.GainBuff();

    }

    public override void ReductBuff()
    {
        PlayerManager.Instance.PlayerController.TakeDamaged(GetDmgValue());

        base.ReductBuff();
    }

    public override void EndBuff()
    {
        base.EndBuff();

    }

    #endregion

    #region Unique

    private float GetDmgValue()
    {
        float value = 0;

        if (StackDmg != 0)
        { value += CurrentBuffCharge.Value * StackDmg; }

        if (ConstPoint != 0)
        { value += ConstPoint; }

        if (MaxHpPercent != 0)
        { value += PlayerManager.Instance.PlayerController.PercentHP(MaxHpPercent); }
        

        return value;
    }

    #endregion
}
