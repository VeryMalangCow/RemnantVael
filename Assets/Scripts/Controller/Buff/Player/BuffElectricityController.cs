using UnityEngine;

public class BuffElectricityController : BuffController, IWhen_GetElectricity
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

    public override void Gain_Buff()
    {
        base.Gain_Buff();
        BuffManager.Instance.Active_GetElectricity();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        if (CurrentBuffCharge.Value <= 0)
        {
            End_Buff();
        }
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
        { value += DevTool.Get_Percent(MaxHpPercent, PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value); }

        value *= (AllyManager.Instance.AllAllies.Count + 1);

        return value;
    }

    public void Play_When(EnemyController _EC)
    {
        float dmg = Get_DmgValue();
        PlayerManager.Instance.PlayerController.Take_Damaged(dmg, _HittedDir: Vector2.zero, _ShowHUDEffect: false);
        
        for (int i = 0; i < AllyManager.Instance.AllAllies.Count; i++)
            AllyManager.Instance.AllAllies[i].TakeDamage(dmg);
        
        this.Reduct_Buff();
    }

    #endregion
}
