using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerMUEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Image IconImg;
    [SerializeField] private Image RankImg;
    [SerializeField] private TMP_Text NameTxt;

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
        RankImg.gameObject.SetActive(false);

        NameTxt.text = "-";
    }

    public void SetOn(ModuleState _MS)
    {
        IconImg.gameObject.SetActive(true);
        RankImg.gameObject.SetActive(true);

        IconImg.sprite = _MS.ThisItemData.ItemIcon;
        RankImg.sprite = ModuleItemManager.Instance.Get_CorrectRankIcon(_MS.ThisItemData.Rank);

        NameTxt.text = ResourceManager.Instance.Get_ModuleName(_MS.ThisItemData.ID);
    }

    public void Set_Color(Color _Clr)
    {
        NameTxt.color = _Clr;
    }

    #endregion
}
