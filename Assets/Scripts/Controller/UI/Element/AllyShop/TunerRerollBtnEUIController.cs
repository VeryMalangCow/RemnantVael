using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TunerRerollBtnEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] private TMP_Text ThisTxt;
    [SerializeField] private TMP_Text NeedOverriderAmountTxt;

    #endregion

    #region Set (UI)

    public void Set_UI(int _NeedOverrider)
    {
        NeedOverriderAmountTxt.text = $"- {_NeedOverrider}";
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerEnter(eventData);

        DevTool.Set_KillTween(ThisRT);
        ThisRT.DOScale(1.05f, 0.05f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerExit(eventData);

        DevTool.Set_KillTween(ThisRT);
        ThisRT.DOScale(1f, 0.05f);
    }


    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        ThisTxt.text = ResourceManager.instance.Get_StaticWord(57);
    }

    #endregion
}
