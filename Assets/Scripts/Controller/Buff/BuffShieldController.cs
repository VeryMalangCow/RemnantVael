using System.Collections.Generic;
using UnityEngine;

public class BuffShieldController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shield")]

    [Space(10)]
    [SerializeField] public Shield ThisShield;

    [Space(10)]
    [SerializeField] private List<Shield> ActivingBuff = new List<Shield>();

    #endregion

    #region Buff

    public override void GainBuff()
    {
        base.GainBuff();

        ThisShield.ShieldCurrentValue = ThisShield.ShieldMaxValue;
        PlayerManager.Instance.PlayerController.GainShield(ThisShield);
    }

    public override void EndBuff()
    {
        base.EndBuff();
    }

    #endregion
}
