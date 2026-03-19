using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class BuffCDController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> CD Add")]

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

        PlayerManager.instance.playerController.BaseWeapon.CD.Gain_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.CD.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.instance.playerController.BaseWeapon.CD.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.instance.playerController.BaseWeapon.CD.Lose_Buff(multiple);
        PlayerManager.instance.playerController.BaseWeapon.CD.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float _Value)
    {
        multiple.baseValue = _Value;
    }

    #endregion
}
