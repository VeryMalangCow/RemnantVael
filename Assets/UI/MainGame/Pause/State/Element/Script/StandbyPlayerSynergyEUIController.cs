using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerSynergyEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Image iconImg;
    [SerializeField] private Image amountImg;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text amountTxt;

    [SerializeField] public RectTransform rt;

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
        amountImg.sprite = MainGameUIManager.instance.muUi.synergyTierFrames[rank];
        nameTxt.text = ResourceManager.instance.Get_SynergyName(id);
        amountTxt.text = amount.ToString();
    }

    public void SetColor(Color imgClr, Color txtClr)
    {
        amountImg.color = imgClr;
        nameTxt.color = txtClr;
        amountTxt.color = txtClr;
    }

    #endregion
}
