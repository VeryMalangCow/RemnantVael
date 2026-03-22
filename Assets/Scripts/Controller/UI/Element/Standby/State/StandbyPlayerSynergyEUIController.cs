using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StandbyPlayerSynergyEUIController : ElementUIController
{
    #region Value

    [FormerlySerializedAs("IconImg")][SerializeField] private Image iconImg;
    [FormerlySerializedAs("AmountImg")][SerializeField] private Image amountImg;

    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;
    [FormerlySerializedAs("AmountTxt")][SerializeField] private TMP_Text amountTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        SetOff();
    }

    #endregion

    #region Set

    public void SetOff()
    {
        iconImg.gameObject.SetActive(false);
        amountImg.gameObject.SetActive(false);
        amountTxt.gameObject.SetActive(false);

        nameTxt.text = "-";
    }

    public void SetOn(int id, int amount)
    {
        int rank = ModuleItemManager.instance.Get_SynchronyRank(amount);

        iconImg.gameObject.SetActive(true);
        amountImg.gameObject.SetActive(rank != 0);
        amountTxt.gameObject.SetActive(true);

        iconImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(id).thisIcon;
        amountImg.sprite = MainGameUIManager.instance.moduleUpgrade_UIController.synergyTierFrames[rank];
        nameTxt.text = ResourceManager.instance.Get_SynergyName(id);
        amountTxt.text = amount.ToString();
    }

    public void Set_Color(Color imgClr, Color txtClr)
    {
        amountImg.color = imgClr;
        nameTxt.color = txtClr;
        amountTxt.color = txtClr;
    }

    #endregion
}
