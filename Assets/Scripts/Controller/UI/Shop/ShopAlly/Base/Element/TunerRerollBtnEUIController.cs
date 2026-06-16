using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TunerRerollBtnEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] private TMP_Text txt;
    [SerializeField] private TMP_Text needOverriderAmountTxt;

    #endregion

    #region Init

    public void Init(SinglePanelUIController ui, Vector2 pos)
    {
        ownerUIController = ui;

        rt.anchoredPosition = pos;
    }

    #endregion

    #region Set (UI)

    public void Set_UI(int needOverrider)
    {
        needOverriderAmountTxt.text = $"- {needOverrider}";
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return;

        base.OnPointerEnter(eventData);

        DevTool.SetKillTween(rt);
        rt.DOScale(1.05f, 0.05f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return;

        base.OnPointerExit(eventData);

        DevTool.SetKillTween(rt);
        rt.DOScale(1f, 0.05f);
    }


    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        txt.text = ResourceManager.instance.Get_StaticWord(57);
    }

    #endregion
}
