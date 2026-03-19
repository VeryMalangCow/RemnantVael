using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuffShieldController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shield")]

    [Space(10)]
    [SerializeField] public Shield thisShield;

    [HideInInspector] private List<Shield> activingBuff = new List<Shield>();


    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        thisShield.shieldCurrentValue = thisShield.shieldMaxValue;
        PlayerManager.instance.playerController.Gain_Shield(thisShield);
    }

    public override void End_Buff()
    {
        base.End_Buff();
    }

    #endregion
}
