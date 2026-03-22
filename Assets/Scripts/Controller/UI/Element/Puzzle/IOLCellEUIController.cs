using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IOLCellEUIController : OwnBtnEUIController
{
    #region Value

    #region - Inpsector

    [Space(20)]
    [Header("<><><><><> IOL Cell")]

    [Space(10)]
    [Header("=== RT")]
    [FormerlySerializedAs("OnPanelRT")][SerializeField] private RectTransform onPanelRt;
    [FormerlySerializedAs("InnerParentRT")][SerializeField] private RectTransform innerParentRt;

    [FormerlySerializedAs("InteractablePanelRT")][SerializeField] private Transform interactablePanelRt;
    [FormerlySerializedAs("UninteractablePanelRT")][SerializeField] private Transform uninteractablePanelRt;

    [Space(10)]
    [Header("=== Inner")]
    [FormerlySerializedAs("CenterRT")][SerializeField] private RectTransform centerRt;
    [FormerlySerializedAs("RoundLineRTList")][SerializeField] private List<RectTransform> roundLineRtList;

    [Space(10)]
    [Header("=== Txt")]
    [FormerlySerializedAs("IndexTxt")][SerializeField] private TMP_Text indexTxt;

    #endregion

    #region - Hide

    // Owner
    [HideInInspector] public InOrderLockerUIController ownerIolUIController;

    // Value
    [HideInInspector] private CoupleData<Vector2> centerSizeDelta;
    [HideInInspector] private CoupleData<Vector2> roundLineSizeDelta;
    [HideInInspector] public bool isInteractable = false;
    [HideInInspector] public bool isOn = false;
    [HideInInspector] private bool isTweeing = false;

    // Inner
    [HideInInspector] public List<Image> innerImgList;

    // CG
    [HideInInspector] public CanvasGroup onPanelCg;
    [HideInInspector] public CanvasGroup innerParentCg;

    // Seq
    [HideInInspector] private Sequence onOffSeq = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        innerImgList = DevTool.Get_ChildList<Image>(innerParentRt);
        innerImgList.AddRange(DevTool.Get_ChildList<Image>(interactablePanelRt));
        innerImgList.AddRange(DevTool.Get_ChildList<Image>(uninteractablePanelRt));

        onPanelCg = DevTool.Get_ComponentTType(onPanelRt.gameObject, out CanvasGroup _onPanelCg) ? _onPanelCg : null;
        innerParentCg = DevTool.Get_ComponentTType(innerParentRt.gameObject, out CanvasGroup innerCg) ? innerCg : null;

        centerSizeDelta = new CoupleData<Vector2>(new Vector2(92f, 92f), new Vector2(184f, 184f));
        roundLineSizeDelta = new CoupleData<Vector2>(new Vector2(72f, 88f), new Vector2(72f, 42f));
    }

    #endregion

    #region Set (On/Off)

    public void Set_Interactable(bool onOff)
    {
        interactablePanelRt.gameObject.SetActive(onOff);
        uninteractablePanelRt.gameObject.SetActive(!onOff);
        isInteractable = onOff;
    }

    public void Set_Default()
    {
        onPanelCg.alpha = 0.0f;
        innerParentCg.alpha = 0.1f;

        centerRt.sizeDelta = centerSizeDelta.typeBase;
        for (int i = 0; i < roundLineRtList.Count; i++)
            roundLineRtList[i].sizeDelta = roundLineSizeDelta.typeBase;

        isOn = false;
        isTweeing = false;
    }

    public void Set_NumIndexTxt(bool onOff, int index = 0)
    {
        indexTxt.gameObject.SetActive(onOff);
        indexTxt.text = (index + 1).ToString();
    }

    public void Set_On(float durTime)
    {
        if (isTweeing || !isInteractable || isOn) return;

        DevTool.Set_KillTween(onOffSeq);

        onOffSeq = DOTween.Sequence();

        onOffSeq.Join(onPanelCg.DOFade(1f, durTime));
        onOffSeq.Join(innerParentCg.DOFade(1f, durTime));
        onOffSeq.Join(centerRt.DOSizeDelta(centerSizeDelta.typeSpecial, durTime));
        for (int i = 0; i < roundLineRtList.Count; i++)
            onOffSeq.Join(roundLineRtList[i].DOSizeDelta(roundLineSizeDelta.typeSpecial, durTime));

        onOffSeq.OnComplete(() =>
        {
            isOn = true;
            isTweeing = false;
            ownerIolUIController.Check_CorrectLineSet();
        });
    }

    public void Set_Off(float durTime)
    {
        if (isTweeing || !isInteractable || !isOn) return;

        DevTool.Set_KillTween(onOffSeq);

        onOffSeq = DOTween.Sequence();

        onOffSeq.Join(onPanelCg.DOFade(0f, durTime));
        onOffSeq.Join(innerParentCg.DOFade(0.1f, durTime));
        onOffSeq.Join(centerRt.DOSizeDelta(centerSizeDelta.typeBase, durTime));
        for (int i = 0; i < roundLineRtList.Count; i++)
            onOffSeq.Join(roundLineRtList[i].DOSizeDelta(roundLineSizeDelta.typeBase, durTime));

        onOffSeq.OnComplete(() => 
        { 
            isOn = false; 
            isTweeing = false; 
        });
    }

    #endregion

    #region Set (Color)

    public void Set_Color(Color clr)
    {
        for (int i = 0; i < innerImgList.Count; i++)
            DevTool.Set_Color(clr, innerImgList[i]);
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerIolUIController != null) ownerIolUIController.Set_CellSelect(this);
    }

    #endregion

}
