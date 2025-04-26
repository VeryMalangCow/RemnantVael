using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IOLCellEUIController : OwnBtnEUIController
{
    #region Value

    #region - Inpsector

    [Space(20)]
    [Header("<><><><><> IOL Cell")]

    [Space(10)]
    [Header("=== RT")]
    [SerializeField] private RectTransform OnPanelRT;
    [SerializeField] private RectTransform InnerParentRT;

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private RectTransform CenterRT;
    [SerializeField] private List<RectTransform> RoundLineRTList;

    #endregion

    #region - Hide

    // Owner
    [HideInInspector] public InOrderLockerUIController OnwerIOLUIController;

    // Value
    [HideInInspector] private CoupleData<Vector2> CenterSizeDelta;
    [HideInInspector] private CoupleData<Vector2> RoundLineSizeDelta;
    //[HideInInspector] public bool IsInteractable = false;
    [HideInInspector] public bool IsOn = false;
    [HideInInspector] private bool IsTweeing = false;

    // Inner
    [HideInInspector] public List<Image> InnerImgList;

    // CG
    [HideInInspector] public CanvasGroup OnPanelCG;
    [HideInInspector] public CanvasGroup InnerParentCG;

    // Seq
    [HideInInspector] private Sequence OnOffSeq = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        InnerImgList = DevTool.Get_ChildList<Image>(InnerParentRT);

        OnPanelCG = DevTool.Get_ComponentTType(OnPanelRT.gameObject, out CanvasGroup onPanelCg) ? onPanelCg : null;
        InnerParentCG = DevTool.Get_ComponentTType(InnerParentRT.gameObject, out CanvasGroup innerCg) ? innerCg : null;

        CenterSizeDelta = new CoupleData<Vector2>(new Vector2(92f, 92f), new Vector2(184f, 184f));
        RoundLineSizeDelta = new CoupleData<Vector2>(new Vector2(72f, 88f), new Vector2(42f, 42f));
    }

    #endregion

    #region Set (On/Off)

    public void Set_On(float _DurTime)
    {
        if (IsTweeing) return;

        DevTool.Set_KillTween(OnOffSeq);

        OnOffSeq = DOTween.Sequence();

        OnOffSeq.Join(OnPanelCG.DOFade(1f, _DurTime));
        OnOffSeq.Join(InnerParentCG.DOFade(1f, _DurTime));
        OnOffSeq.Join(CenterRT.DOSizeDelta(CenterSizeDelta.TypeSpecial, _DurTime));
        for (int i = 0; i < RoundLineRTList.Count; i++)
            OnOffSeq.Join(RoundLineRTList[i].DOSizeDelta(RoundLineSizeDelta.TypeSpecial, _DurTime));

        OnOffSeq.OnComplete(() =>
        {
            IsOn = true;
            IsTweeing = false; 
        });
        

    }

    public void Set_Off(float _DurTime)
    {
        if (IsTweeing) return;

        DevTool.Set_KillTween(OnOffSeq);

        OnOffSeq = DOTween.Sequence();

        OnOffSeq.Join(OnPanelCG.DOFade(0f, _DurTime));
        OnOffSeq.Join(InnerParentCG.DOFade(0.1f, _DurTime));
        OnOffSeq.Join(CenterRT.DOSizeDelta(CenterSizeDelta.TypeBase, _DurTime));
        for (int i = 0; i < RoundLineRTList.Count; i++)
            OnOffSeq.Join(RoundLineRTList[i].DOSizeDelta(RoundLineSizeDelta.TypeBase, _DurTime));

        OnOffSeq.OnComplete(() => 
        { 
            IsOn = false; 
            IsTweeing = false; 
        });
    }

    #endregion

    #region Set (Color)

    public void Set_Color(Color _Clr)
    {
        for (int i = 0; i < InnerImgList.Count; i++)
            DevTool.Set_Color(_Clr, InnerImgList[i]);
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        if (OnwerIOLUIController != null) OnwerIOLUIController.Set_CellSelect(this);
    }

    #endregion

}
