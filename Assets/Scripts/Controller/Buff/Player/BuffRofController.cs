using UniRx;
using UnityEngine;

public class BuffRofController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Rof Add")]

    [Space(10)]
    [SerializeField] private BuffState<float> Multiple = new BuffState<float>();

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();

        CurrentBuffCharge
            .Subscribe(_Value =>
            {
                Multiple.ActualValue = Multiple.BaseValue * CurrentBuffCharge.Value;
            });
    }

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        PlayerManager.Instance.playerController.BaseWeapon.ROF.Gain_Buff(Multiple);
        PlayerManager.Instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.Instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.Instance.playerController.BaseWeapon.ROF.Lose_Buff(Multiple);
        PlayerManager.Instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float _Value)
    {
        Multiple.BaseValue = _Value;
    }

    public void Set_MaxChargeValue(int _Value)
    {
        MaxBuffCharge = _Value;
    }

    public void Set_CoolTimeValue(float _Value)
    {
        MaxDurTime = _Value;
    }

    #endregion
}
