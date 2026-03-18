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

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        ThisShield.ShieldCurrentValue = ThisShield.ShieldMaxValue;
        PlayerManager.Instance.playerController.Gain_Shield(ThisShield);
    }

    public override void End_Buff()
    {
        base.End_Buff();
    }

    #endregion
}
