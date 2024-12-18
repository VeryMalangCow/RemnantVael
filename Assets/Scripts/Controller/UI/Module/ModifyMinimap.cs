using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ModifyMinimap : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject MinimapElement;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform MinimapFrameRT;
    [SerializeField] private Image InnerImg;


    [Space(10)]
    [Header("=== Normal")]
    [SerializeField] private RectTransform NormalMaskRT;
    [SerializeField] private RectTransform NormalMMEParentRT;
    [HideInInspector] private CanvasGroup NormalCG;
    [SerializeField] private Vector2 NormalSize_Frame = new Vector2(352, 336);

    [Header("-- Element")]
    [SerializeField] private List<ModifyMinimapElement> AllMMEs;


    [Space(10)]
    [Header("=== Interactable")]
    [SerializeField] private RectTransform InteractableMaskRT;
    [SerializeField] private RectTransform InteractableMMEParentRT;
    [HideInInspector] private CanvasGroup InteractableCG;
    [SerializeField] private Vector2 InteractableSize_Frame = new Vector2(728, 712);

    [Header("-- Element")]
    [SerializeField] private List<ModifyMinimapElement> AllIMMEs;

    Sequence TabSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        NormalMaskRT.gameObject.SetActive(true);
        InteractableMaskRT.gameObject.SetActive(false);

        MinimapFrameRT.sizeDelta = NormalSize_Frame;

        if (NormalMaskRT.gameObject.TryGetComponent(out CanvasGroup ncg))
        { NormalCG = ncg; }

        if (InteractableMaskRT.gameObject.TryGetComponent(out CanvasGroup icg))
        { InteractableCG = icg; }
    }

    #endregion

    #region Generate

    public void GenMinimap()
    {
        AllMMEs = new List<ModifyMinimapElement>();
        AllIMMEs = new List<ModifyMinimapElement>();
        List<RoomController> allRC = StageManager.Instance.GetAllRC();

        Color mainClr = InnerImg.color;
        mainClr.a = 1f;

        for (int i = 0; i < allRC.Count; i++)
        {
            GenMinimapElement(allRC[i], mainClr);
            GenMinimapElementInteractable(allRC[i], mainClr);
        }
    }

    private void GenMinimapElement(RoomController _ConnetedRoom, Color _Clr)
    {
        if (Instantiate(MinimapElement, NormalMMEParentRT.transform).TryGetComponent(out ModifyMinimapElement mme))
        {
            mme.gameObject.SetActive(false);
            mme.Offset(_ConnetedRoom, _Clr, true);
            AllMMEs.Add(mme);
        }
    }

    private void GenMinimapElementInteractable(RoomController _ConnetedRoom, Color _Clr)
    {
        if (Instantiate(MinimapElement, InteractableMMEParentRT.transform).TryGetComponent(out ModifyMinimapElement mme))
        {
            mme.gameObject.SetActive(false);
            mme.Offset(_ConnetedRoom, _Clr, false);
            AllIMMEs.Add(mme);
        }
    }

    #endregion

    #region Set State

    

    public void SetState()
    {
        RoomController CurrentRC = StageManager.Instance.CurrentRoomController;

        // 미니맵 위치 조정
        AnchorPosSet(CurrentRC.ThisMME, NormalMMEParentRT);
        AnchorPosSet(CurrentRC.ThisIMME, InteractableMMEParentRT);

        // PC가 있는 방
        SetActiveMME(CurrentRC.ThisMME);
        SetActiveMME(CurrentRC.ThisIMME);

        if (CurrentRC.RoomRuleController.RoomType == eRoomType.Completed)
        {
            CurrentRC.ThisMME.SetState_Complete();
            CurrentRC.ThisIMME.SetState_Complete();
        }
        else
        {
            CurrentRC.ThisMME.SetState_Uncomplete();
            CurrentRC.ThisIMME.SetState_Uncomplete();
        }

        // PC가 있는 방의 인접한 방
        List<RoomController> connectedAllRC = CurrentRC.GetConnectedRCList();
        for (int i = 0; i < connectedAllRC.Count; i++)
        {
            SetActiveMME(connectedAllRC[i].ThisMME);
            SetActiveMME(connectedAllRC[i].ThisIMME);

            if (connectedAllRC[i].RoomRuleController.RoomType != eRoomType.Completed)
            {
                connectedAllRC[i].ThisMME.SetState_Visible();
                connectedAllRC[i].ThisIMME.SetState_Visible();
            }
        }
    }

    // 미니맵 위치 조정
    private void AnchorPosSet(ModifyMinimapElement _MME, RectTransform _ParentRT)
    {
        if (_MME.gameObject.TryGetComponent(out RectTransform rt))
        {
            if (DOTween.IsTweening(_ParentRT))
            { DOTween.Kill(_ParentRT); }

            _ParentRT.DOAnchorPos(-rt.anchoredPosition, 0.5f);
        }
    }

    private void SetActiveMME(ModifyMinimapElement _MME)
    {
        if (!_MME.gameObject.activeSelf)
        {
            _MME.SetActiveOn();
        }
    }

    public void PlayEffect()
    {
        // 효과
        if (DOTween.IsTweening(InnerImg))
        { DOTween.Kill(InnerImg); }

        InnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                InnerImg.DOFade(0.25f, 0.2f);
            });
    }

    #endregion

    #region Tab Interactable

    public void OnTabInteract(float _DurTime)
    {
        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        TabSeq.Join(MinimapFrameRT.DOSizeDelta(InteractableSize_Frame, _DurTime));
        TabSeq.Join(NormalCG.DOFade(0, _DurTime));
        TabSeq.Join(InteractableCG.DOFade(1, _DurTime));

        TabSeq
            .OnStart(() =>
            {
                InteractableCG.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                NormalCG.gameObject.SetActive(false);
            });
    }

    public void OffTabInteract(float _DurTime)
    {
        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        TabSeq.Join(MinimapFrameRT.DOSizeDelta(NormalSize_Frame, _DurTime));
        TabSeq.Join(NormalCG.DOFade(1, _DurTime));
        TabSeq.Join(InteractableCG.DOFade(0, _DurTime));

        TabSeq
            .OnStart(() =>
            {
                NormalCG.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                InteractableCG.gameObject.SetActive(false);
            });
    }

    #endregion
}
