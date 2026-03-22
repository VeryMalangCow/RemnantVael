using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SynergySlotEUIController : OwnBtnEUIController
{
    #region Value

    [HideInInspector] public int id;
    [HideInInspector] public Image img;
    [HideInInspector] public Image tierImg;
    [HideInInspector] public TMP_Text txt;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        img = DevTool.Get_ComponentTType(gameObject, out Image _img) ? _img : null;
        tierImg = DevTool.Get_ComponentTType(gameObject.transform.GetChild(0).gameObject, out Image TierImg) ? TierImg : null;
        txt = DevTool.Get_ComponentTType(gameObject.transform.GetChild(1).gameObject, out TMP_Text tmpt) ? tmpt : null;

        SetOff_SynergySlot();
    }

    #endregion

    #region Set

    public void SetOff_SynergySlot()
    {
        this.gameObject.SetActive(false);
    }

    public void SetOn_SynergySlot(int id, Sprite icon, int amalgamation)
    {
        this.gameObject.SetActive(true);

        this.id = id;
        img.sprite = icon;
        txt.text = amalgamation.ToString();

        int rank = ModuleItemManager.instance.Get_SynchronyRank(amalgamation);
        tierImg.gameObject.SetActive(rank != 0);
        tierImg.sprite = MainGameUIManager.instance.moduleUpgrade_UIController.synergyTierFrames[rank];
    }

    #endregion
}
