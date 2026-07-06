using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerMUEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Image iconImg;
    [SerializeField] private Image rankImg;
    [SerializeField] private TMP_Text nameTxt;
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
        rankImg.gameObject.SetActive(false);

        nameTxt.text = "-";
    }

    public void SetOn(ModuleState moduleState)
    {
        iconImg.gameObject.SetActive(true);
        rankImg.gameObject.SetActive(true);

        iconImg.sprite = moduleState.thisItemData.itemIcon;
        rankImg.sprite = StaticResourceManager.instance.ItemReso.rankIcons[moduleState.thisItemData.rank - 1];

        nameTxt.text = ResourceManager.instance.Get_ModuleName(moduleState.thisItemData.id);
    }

    public void SetColor(Color clr)
    {
        nameTxt.color = clr;
    }

    #endregion
}
