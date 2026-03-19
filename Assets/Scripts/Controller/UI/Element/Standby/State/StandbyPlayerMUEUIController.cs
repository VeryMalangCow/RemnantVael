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

        IconImg.sprite = _MS.thisItemData.itemIcon;
        RankImg.sprite = ResourceManager.instance.Get_RankIcon(_MS.thisItemData.rank);

        NameTxt.text = ResourceManager.instance.Get_ModuleName(_MS.thisItemData.id);
    }

    public void Set_Color(Color _Clr)
    {
        NameTxt.color = _Clr;
    }

    #endregion
}
