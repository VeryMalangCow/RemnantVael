using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SynergySlotEUIController : OwnBtnEUIController
{
    #region Value

    [HideInInspector] public int ID;
    [HideInInspector] public Image ThisImg;
    [HideInInspector] public Image ThisTierImg;
    [HideInInspector] public TMP_Text ThisTxt;


    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
        ThisTierImg = DevTool.Get_ComponentTType(gameObject.transform.GetChild(0).gameObject, out Image TierImg) ? TierImg : null;
        ThisTxt = DevTool.Get_ComponentTType(gameObject.transform.GetChild(1).gameObject, out TMP_Text tmpt) ? tmpt : null;

        SetOff_SynergySlot();
    }

    #endregion

    #region Set

    public void SetOff_SynergySlot()
    {
        this.gameObject.SetActive(false);
    }

    public void SetOn_SynergySlot(int _ID, Sprite _Icon, int _Amalgamation)
    {
        this.gameObject.SetActive(true);

        ID = _ID;
        ThisImg.sprite = _Icon;
        ThisTxt.text = _Amalgamation.ToString();

        // 1~6 / 7~12 / 13~18
        int tier = (_Amalgamation - 1) > 0 ? _Amalgamation / ModuleItemManager.SynchoronyOneTierRange : 0;

        ThisTierImg.sprite = MainGameUIManager.Instance.ModuleUpgrade_UIController.SynergyTierFrames[tier];

    }

    #endregion
}
