using UniRx;
using UnityEngine;

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

        PlayerManager.instance.playerController.baseWeapon.cd.Gain_Buff(multiple);
        PlayerManager.instance.playerController.baseWeapon.cd.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.instance.playerController.baseWeapon.cd.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.instance.playerController.baseWeapon.cd.Lose_Buff(multiple);
        PlayerManager.instance.playerController.baseWeapon.cd.Set_BuffedState();
    }

    #endregion

    #region Set

    public void Set_Value(float value)
    {
        multiple.baseValue = value;
    }

    #endregion
}
