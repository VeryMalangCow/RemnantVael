using UniRx;
using UnityEngine;

public class BuffRofController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Rof Add")]

    [Space(10)]
    [SerializeField] private BuffState<float> multiple = new BuffState<float>();

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();

        currentBuffCharge
            .Subscribe(_Value =>
            {
                multiple.actualValue = multiple.baseValue * currentBuffCharge.Value;
            });
    }

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        PlayerManager.instance.playerController.BaseWeapon.rof.Gain_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.rof.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.instance.playerController.BaseWeapon.rof.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.instance.playerController.BaseWeapon.rof.Lose_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.rof.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float value)
    {
        multiple.baseValue = value;
    }

    public void Set_MaxChargeValue(int value)
    {
        maxBuffCharge = value;
    }

    public void Set_CoolTimeValue(float value)
    {
        maxDurTime = value;
    }

    #endregion
}
