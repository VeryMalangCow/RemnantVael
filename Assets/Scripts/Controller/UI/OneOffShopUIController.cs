
using UnityEngine;

public class OneOffShopUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> One-Off Shop")]

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private ModifyTextAmountForBuy EpMaxUpgrade_MTAFB;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        EpMaxUpgrade_MTAFB.Offset();
    }

    protected override void Offset_UI()
    {
        
    }

    #endregion
}
