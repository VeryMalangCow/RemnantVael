using UniRx;
using UnityEngine;

public class BuffElectricController : BuffController, IWhen_Hitted
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Electric")]

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
        base.ReductBuff();

        if (CurrentBuffCharge.Value <= 0)
        {
            EndBuff();
        }
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

        value = value * (AllyManager.Instance.AllAllies.Count + 1);

        return value;
    }

    public void When(EnemyController _EC)
    {
        float dmg = GetDmgValue();
        PlayerManager.Instance.PlayerController.TakeExtraDamage(dmg);
        for (int i = 0; i < AllyManager.Instance.AllAllies.Count; i++)
        {
            AllyManager.Instance.AllAllies[i].TakeDamage(dmg);
        }

        this.ReductBuff();
    }

    #endregion
}
