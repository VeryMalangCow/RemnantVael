using UnityEngine;
using UnityEngine.Serialization;

public class BuffTickDmgController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dmg")]

    [Space(10)]
    [Header("=== Const Value")]
    [SerializeField] private float maxHpPercent = 10; // 최대 체력 비례
    [SerializeField] private float constPoint = 10; // 고정 수치

    [Space(10)]
    [Header("=== Stack")]
    [SerializeField] private float stackDmg = 1;

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

    }

    public override void Reduct_Buff()
    {
        PlayerManager.instance.playerController.Take_Damaged(Get_DmgValue(), _HittedDir: Vector2.zero, _ShowHUDEffect: true);

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

        if (stackDmg != 0)
        { value += currentBuffCharge.Value * stackDmg; }

        if (constPoint != 0)
        { value += constPoint; }

        if (maxHpPercent != 0)
        { value += DevTool.Get_Percent(maxHpPercent, PlayerManager.instance.playerController.MaxEP.ActualState.Value); }
        

        return value;
    }

    #endregion
}
