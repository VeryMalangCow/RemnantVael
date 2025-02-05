using UnityEngine;
using UniRx;

public class BuffTakingDmgController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Taking mg Add")]

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

    public override void GainBuff()
    {
        base.GainBuff();

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.GainBuff(Multiple);
        PlayerManager.Instance.PlayerController.TakingDmgMultiple.SetBuffedState();
    }

    public override void ReductBuff()
    {
        base.ReductBuff();

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.SetBuffedState();
    }

    public override void EndBuff()
    {
        base.EndBuff();

        PlayerManager.Instance.PlayerController.TakingDmgMultiple.RemoveBuff(Multiple);
        PlayerManager.Instance.PlayerController.TakingDmgMultiple.SetBuffedState();
    }

    #endregion
}
