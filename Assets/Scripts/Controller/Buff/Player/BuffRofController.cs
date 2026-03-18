using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

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
                multiple.ActualValue = multiple.BaseValue * currentBuffCharge.Value;
            });
    }

    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        PlayerManager.instance.playerController.BaseWeapon.ROF.Gain_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.instance.playerController.BaseWeapon.ROF.Lose_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.ROF.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float _Value)
    {
        multiple.BaseValue = _Value;
    }

    public void Set_MaxChargeValue(int _Value)
    {
        maxBuffCharge = _Value;
    }

    public void Set_CoolTimeValue(float _Value)
    {
        maxDurTime = _Value;
    }

    #endregion
}
