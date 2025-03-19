using UnityEngine;
using UniRx;

public class BuffTakingDmgController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Taking Dmg")]

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

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.Gain_Buff(Multiple);
        PlayerManager.Instance.PlayerController.TakingDmgMultiple.Set_BuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.Set_BuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.Lose_Buff(Multiple);
        PlayerManager.Instance.PlayerController.TakingDmgMultiple.Set_BuffedState();
    }

    #endregion
}
