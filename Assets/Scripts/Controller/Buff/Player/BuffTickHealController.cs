using UnityEngine;
using UnityEngine.Serialization;

public class BuffTickHealController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Heal")]

    [Space(10)]
    [Header("=== Const Value")]
    [SerializeField] private float maxHpPercent = 10; // 최대 체력 비례
    [SerializeField] private float constPoint = 10; // 고정 수치

    [Space(10)]
    [Header("=== Stack")]
    [SerializeField] private float stackHeal = 1;

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

    }

    public override void Reduct_Buff()
    {
        PlayerManager.instance.playerController.Add_CurrentEP(Get_HealValue());

        base.Reduct_Buff();
    }

    public override void End_Buff()
    {
        base.End_Buff();

    }

    #endregion

    #region Unique

    private float Get_HealValue()
    {
        float value = 0;

        if (stackHeal != 0)
        { value += currentBuffCharge.Value * stackHeal; }

        if (constPoint != 0)
        { value += constPoint; }

        if (maxHpPercent != 0)
        { value += PlayerManager.instance.playerController.Get_PercentEP(maxHpPercent); }


        return value;
    }

    #endregion
}
