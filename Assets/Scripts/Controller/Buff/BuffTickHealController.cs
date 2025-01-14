using Unity.VisualScripting;
using UnityEngine;

public class BuffTickHealController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dmg Add")]

    [Space(10)]
    [SerializeField] private float HealPercent = 10;
    [SerializeField] private float HealPoint = 10;

    #endregion

    #region Framework


    #endregion

    #region Buff

    public override void GainBuff()
    {
        base.GainBuff();

    }

    public override void ReductBuff()
    {
        base.ReductBuff();

        PlayerManager.Instance.PlayerController.AddCurrentEP(GetHealValue());
    }

    public override void EndBuff()
    {
        base.EndBuff();

    }

    #endregion

    #region Unique

    private float GetHealValue()
    {
        return HealPoint + PlayerManager.Instance.PlayerController.PercentHP(HealPercent);
    }

    #endregion
}
