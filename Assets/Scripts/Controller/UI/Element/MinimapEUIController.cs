using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject MinimapElement;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform MinimapFrameRT;
    [SerializeField] public Image InnerImg;


    [Space(10)]
    [Header("=== Normal")]
    [SerializeField] private RectTransform NormalMaskRT;
    [SerializeField] private RectTransform NormalMMEParentRT;
    [HideInInspector] private CanvasGroup NormalCG;
    [SerializeField] private Vector2 NormalSize_Frame = new Vector2(352, 336);

    [Header("-- Element")]
    [SerializeField] private List<MinimapCellEUIController> AllMMEs;
    [SerializeField] private RectTransform NormalPoint;


    [Space(10)]
    [Header("=== Interactable")]
    [SerializeField] private RectTransform InteractableMaskRT;
    [SerializeField] private RectTransform InteractableMMEParentRT;
    [HideInInspector] private CanvasGroup InteractableCG;
    [SerializeField] private Vector2 InteractableSize_Frame = new Vector2(728, 712);

    [Header("-- Element")]
    [SerializeField] private List<MinimapCellEUIController> AllIMMEs;
    [SerializeField] private RectTransform InteractablePoint;
    [SerializeField] private RectTransform InteractingPoint;

    [HideInInspector] private GateController InteractingBookGate;
    [HideInInspector] private RoomController MinimapSelectedElementRC;

    [HideInInspector] private bool CanInteractable = false;
    Sequence TabSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        MinimapFrameRT.sizeDelta = NormalSize_Frame;

        if (NormalMaskRT.gameObject.TryGetComponent(out CanvasGroup ncg))
        { NormalCG = ncg; NormalCG.alpha = 1f; }

        if (InteractableMaskRT.gameObject.TryGetComponent(out CanvasGroup icg))
        { InteractableCG = icg; InteractableCG.alpha = 0f; }
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Set_BookRoom();
    }

    #endregion

    #region Generate

    public void Gen_Minimap()
    {
        AllMMEs = new List<MinimapCellEUIController>();
        AllIMMEs = new List<MinimapCellEUIController>();
        List<RoomController> allRC = StageManager.Instance.GetAllRC();

        Color mainClr = InnerImg.color;
        mainClr.a = 1f;

        for (int i = 0; i < allRC.Count; i++)
        {
            Gen_MinimapElement(allRC[i], mainClr);
            Gen_MinimapInteractableElement(allRC[i], mainClr);
        }
    }

    private void Gen_MinimapElement(RoomController _ConnetedRoom, Color _Clr)
    {
        if (Instantiate(MinimapElement, NormalMMEParentRT.transform).TryGetComponent(out MinimapCellEUIController mme))
        {
            mme.gameObject.SetActive(false);
            mme.Offset(_ConnetedRoom, _Clr, true);
            AllMMEs.Add(mme);
        }
    }

    private void Gen_MinimapInteractableElement(RoomController _ConnetedRoom, Color _Clr)
    {
        if (Instantiate(MinimapElement, InteractableMMEParentRT.transform).TryGetComponent(out MinimapCellEUIController mme))
        {
            mme.gameObject.SetActive(false);
            mme.Offset(_ConnetedRoom, _Clr, false);
            AllIMMEs.Add(mme);
        }
    }

    #endregion

    #region Set State

    public void Set_State()
    {
        RoomController CurrentRC = StageManager.Instance.CurrentRoomController;

        // 미니맵 위치 조정
        Set_AnchorPos(CurrentRC.ThisMME, NormalMMEParentRT, 0.3f);
        Set_AnchorPos(CurrentRC.ThisIMME, InteractableMMEParentRT, 0.3f);

        // 포인트
        Set_Point(NormalPoint, CurrentRC.ThisMME);
        Set_Point(InteractablePoint, CurrentRC.ThisIMME);

        // PC가 있는 방
        Set_ActiveMME(CurrentRC.ThisMME);
        Set_ActiveMME(CurrentRC.ThisIMME);

        if (CurrentRC.RoomRuleController.RoomType == eRoomType.Completed)
        {
            CurrentRC.ThisMME.Set_Complete();
            CurrentRC.ThisIMME.Set_Complete();
        }
        else
        {
            CurrentRC.ThisMME.Set_Uncomplete();
            CurrentRC.ThisIMME.Set_Uncomplete();
        }

        // PC가 있는 방의 인접한 방
        List<RoomController> connectedAllRC = CurrentRC.Get_ConnectedRoomList();
        for (int i = 0; i < connectedAllRC.Count; i++)
        {
            Set_ActiveMME(connectedAllRC[i].ThisMME);
            Set_ActiveMME(connectedAllRC[i].ThisIMME);

            if (connectedAllRC[i].RoomRuleController.RoomType != eRoomType.Completed)
            {
                connectedAllRC[i].ThisMME.Set_Visible();
                connectedAllRC[i].ThisIMME.Set_Visible();
            }
        }
    }

    // 미니맵 위치 조정
    private void Set_AnchorPos(MinimapCellEUIController _MME, RectTransform _ParentRT, float _DurTime)
    {
        if (_MME.gameObject.TryGetComponent(out RectTransform rt))
        {
            if (DOTween.IsTweening(_ParentRT))
            { DOTween.Kill(_ParentRT); }

            _ParentRT.DOAnchorPos(-rt.anchoredPosition, _DurTime);
        }
    }

    // 켜지는 MME
    private void Set_ActiveMME(MinimapCellEUIController _MME)
    {
        if (!_MME.gameObject.activeSelf)
        {
            _MME.Set_ActiveOn();
        }
    }

    // 중앙 포인터
    private void Set_Point(RectTransform _Point, MinimapCellEUIController _MME)
    {
        _Point.transform.SetParent(_MME.transform);
        if (_MME.TryGetComponent(out RectTransform rt))
        {
            Vector2 pivot = Vector2.one - rt.pivot;
            _Point.pivot = pivot;
            _Point.anchoredPosition = Vector2.zero;
        }
    }

    // 이펙트
    public void Play_Effect()
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

    public void SetOn_TabInteract(float _DurTime)
    {
        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        TabSeq.Join(MinimapFrameRT.DOSizeDelta(InteractableSize_Frame, _DurTime));
        TabSeq.Join(NormalCG.DOFade(0, _DurTime));
        TabSeq.Join(InteractableCG.DOFade(1, _DurTime));

        TabSeq
            .OnComplete(() =>
            {
                // 미니맵 이동을 위한 Reset
                InteractingPoint.gameObject.SetActive(true);

                InputManager.Instance.InputArrowDir = Vector2Int.zero;
                CanInteractable = true;
                
                MinimapSelectedElementRC = StageManager.Instance.CurrentRoomController;
            });
    }

    public void SetOff_TabInteract(float _DurTime)
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
                InteractingPoint.gameObject.SetActive(false);

                InputManager.Instance.InputArrowDir = Vector2Int.zero;
                CanInteractable = false;
            })
            .OnComplete(() =>
            {
                if (InteractingBookGate != null && StageManager.Instance.CurrentRoomController != InteractingBookGate.ParterGate.ThisRoom)
                {
                    InteractingBookGate.Play_Interact();
                }
            });
    }

    #endregion

    #region Move In Interactable Condition

    private void Set_BookRoom()
    {
        if (CanInteractable && InputManager.Instance.InputArrowDir != Vector2Int.zero)
        {
            GateController gc = MinimapSelectedElementRC.Get_CollectGate(InputManager.Instance.InputArrowDir);
            if (gc != null)
            {
                InteractingBookGate = gc.ParterGate;
                MinimapSelectedElementRC = gc.ThisRoom;
                Set_AnchorPos(MinimapSelectedElementRC.ThisIMME, InteractableMMEParentRT, 0.15f);
            }

            InputManager.Instance.InputArrowDir = Vector2Int.zero;
        }
    }

    #endregion
}
