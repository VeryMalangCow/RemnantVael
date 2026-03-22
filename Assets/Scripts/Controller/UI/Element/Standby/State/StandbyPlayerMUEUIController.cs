using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StandbyPlayerMUEUIController : ElementUIController
{
    #region Value

    [FormerlySerializedAs("IconImg")][SerializeField] private Image iconImg;
    [FormerlySerializedAs("RankImg")][SerializeField] private Image rankImg;
    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;

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
        rankImg.gameObject.SetActive(false);

        nameTxt.text = "-";
    }

    public void SetOn(ModuleState moduleState)
    {
        iconImg.gameObject.SetActive(true);
        rankImg.gameObject.SetActive(true);

        iconImg.sprite = moduleState.thisItemData.itemIcon;
        rankImg.sprite = ResourceManager.instance.Get_RankIcon(moduleState.thisItemData.rank);

        nameTxt.text = ResourceManager.instance.Get_ModuleName(moduleState.thisItemData.id);
    }

    public void Set_Color(Color clr)
    {
        nameTxt.color = clr;
    }

    #endregion
}
