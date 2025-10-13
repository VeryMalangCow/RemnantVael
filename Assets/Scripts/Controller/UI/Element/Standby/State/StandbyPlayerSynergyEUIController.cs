using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerSynergyEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Image IconImg;
    [SerializeField] private Image AmountImg;

    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text AmountTxt;

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
        IconImg.gameObject.SetActive(false);
        AmountImg.gameObject.SetActive(false);
        AmountTxt.gameObject.SetActive(false);

        NameTxt.text = "-";
    }

    public void SetOn(int _ID, int _Amount)
    {
        int rank = ModuleItemManager.Instance.Get_SynchronyRank(_Amount);

        IconImg.gameObject.SetActive(true);
        AmountImg.gameObject.SetActive(rank != 0);
        AmountTxt.gameObject.SetActive(true);

        IconImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_ID).ThisIcon;
        AmountImg.sprite = MainGameUIManager.Instance.ModuleUpgrade_UIController.SynergyTierFrames[rank];
        NameTxt.text = ResourceManager.Instance.Get_SynergyName(_ID);
        AmountTxt.text = _Amount.ToString();
    }

    public void Set_Color(Color _ImgClr, Color _TxtClr)
    {
        AmountImg.color = _ImgClr;
        NameTxt.color = _TxtClr;
        AmountTxt.color = _TxtClr;
    }

    #endregion
}
