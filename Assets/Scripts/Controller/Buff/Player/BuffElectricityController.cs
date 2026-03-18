using UnityEngine;
using UnityEngine.Serialization;

public class BuffElectricityController : BuffController, IWhen_GetElectricity
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Electric")]

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
        BuffManager.instance.Active_GetElectricity();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        if (currentBuffCharge.Value <= 0)
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

        if (stackDmg != 0)
        { value += currentBuffCharge.Value * stackDmg; }

        if (constPoint != 0)
        { value += constPoint; }

        if (maxHpPercent != 0)
        { value += DevTool.Get_Percent(maxHpPercent, PlayerManager.instance.playerController.MaxEP.ActualState.Value); }

        value *= (AllyManager.instance.allAlly.Count + 1);

        return value;
    }

    public void Play_When(EnemyController _EC)
    {
        float dmg = Get_DmgValue();
        PlayerManager.instance.playerController.Take_Damaged(dmg, _HittedDir: Vector2.zero, _ShowHUDEffect: false);
        
        for (int i = 0; i < AllyManager.instance.allAlly.Count; i++)
            AllyManager.instance.allAlly[i].TakeDamage(dmg);
        
        this.Reduct_Buff();
    }

    #endregion
}
