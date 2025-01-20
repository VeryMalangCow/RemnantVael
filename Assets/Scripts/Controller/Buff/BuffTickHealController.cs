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

        float value = GetHealValue();
        if (value >= 0)
        {
            PlayerManager.Instance.PlayerController.AddCurrentEP(value);
        }
        else
        {
            PlayerManager.Instance.PlayerController.TakeDamaged(-value);
        }
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
