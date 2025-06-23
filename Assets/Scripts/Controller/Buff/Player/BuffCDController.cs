using UniRx;
using UnityEngine;

public class BuffCDController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> CD Add")]

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

        PlayerManager.Instance.PlayerController.BaseWeapon.CD.Gain_Buff(Multiple);
        PlayerManager.Instance.PlayerController.BaseWeapon.CD.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.Instance.PlayerController.BaseWeapon.CD.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.Instance.PlayerController.BaseWeapon.CD.Lose_Buff(Multiple);
        PlayerManager.Instance.PlayerController.BaseWeapon.CD.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float _Value)
    {
        Multiple.BaseValue = _Value;
    }

    #endregion
}
