using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject MinimapElement;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform MinimapFrameRT;
    [SerializeField] public Image InnerImg;
    [HideInInspector] private Color MainColor;


    [Space(10)]
    [Header("=== Normal")]
    [SerializeField] private RectTransform NormalMaskRT;
    [SerializeField] private RectTransform NormalMMEParentRT;
    [SerializeField] private Vector2 NormalSize_Frame = new Vector2(352, 336);

    [Header("-- Element")]
    [SerializeField] private RectTransform NormalPoint;


    [Space(10)]
    [Header("=== Interactable")]
    [SerializeField] private RectTransform InteractableMaskRT;
    [SerializeField] private RectTransform InteractableMMEParentRT;
    [SerializeField] private Vector2 InteractableSize_Frame = new Vector2(728, 712);

    [Header("-- Element")]
    [SerializeField] private RectTransform InteractablePoint;
    [SerializeField] private RectTransform InteractingPoint;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public List<MinimapCellEUIController> AllMMCEUI = new List<MinimapCellEUIController>();

    // CG
    [HideInInspector] private CanvasGroup NormalCG;
    [HideInInspector] private CanvasGroup InteractableCG;

    // Book
    [HideInInspector] private GateController InteractingBookGate;
    [HideInInspector] private RoomController MinimapSelectedElementRC;

    [HideInInspector] private bool CanInteractable = false;
    [HideInInspector] private Sequence TabSeq;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        // Color
        Color mainClr = InnerImg.color;
        mainClr.a = 1f;
        MainColor = mainClr;

        // Frame
        MinimapFrameRT.sizeDelta = NormalSize_Frame;

        // Canvas Group
        NormalCG = DevTool.Get_ComponentTType(NormalMaskRT.gameObject, out CanvasGroup nCg) ? nCg : null;
        NormalCG.alpha = 1f;

        InteractableCG = DevTool.Get_ComponentTType(InteractableMaskRT.gameObject, out CanvasGroup iCg) ? iCg : null;
        InteractableCG.alpha = 0f;
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
        List<RoomController> allRC = StageManager.instance.Get_AllRoom();

        for (int i = 0; i < allRC.Count; i++)
        {
            Gen_EachMinimapElement(allRC[i], NormalMMEParentRT.transform, _IsNormal: true);
            Gen_EachMinimapElement(allRC[i], InteractableMMEParentRT.transform, _IsNormal: false);
        }
    }

    private void Gen_EachMinimapElement(RoomController _ConnetedRoom, Transform _ParentTF, bool _IsNormal)
    {
        if (DevTool.Get_ComponentTType(Instantiate(MinimapElement, _ParentTF), out MinimapCellEUIController mmc))
        {
            AllMMCEUI.Add(mmc);
            mmc.gameObject.SetActive(false);
            mmc.Offset();
            mmc.Offset(_ConnetedRoom, _IsNormal);
        }
    }

    #endregion

    #region Remove


    public void Remove_AllMinimapCell()
    {
        int amount = AllMMCEUI.Count;
        for (int i = amount - 1; i >= 0; i--)
        {
            Destroy(AllMMCEUI[i].gameObject);
            AllMMCEUI.RemoveAt(i);
        }

        AllMMCEUI.Clear();
    }

    #endregion

    #region Set State

    public void Set_State()
    {
        RoomController CurrentRC = StageManager.instance.currentRoomController;

        Set_AnchorPos(CurrentRC.thisMME, NormalMMEParentRT, 0.3f);
        Set_AnchorPos(CurrentRC.thisIMME, InteractableMMEParentRT, 0.3f);

        //Set_Point(NormalPoint, CurrentRC.ThisMME);
        //Set_Point(InteractablePoint, CurrentRC.ThisIMME);

        Set_ActiveMME(CurrentRC.thisMME);
        Set_ActiveMME(CurrentRC.thisIMME);

        if (CurrentRC.roomRule.roomType == eRoomType.Completed)
        {
            CurrentRC.thisMME.Set_Complete(MainColor);
            CurrentRC.thisIMME.Set_Complete(MainColor);
        }
        else
        {
            CurrentRC.thisMME.Set_Uncomplete();
            CurrentRC.thisIMME.Set_Uncomplete();
        }

        List<RoomController> connectedAllRC = CurrentRC.Get_ConnectedRooms();

        for (int i = 0; i < connectedAllRC.Count; i++)
        {
            Set_ActiveMME(connectedAllRC[i].thisMME);
            Set_ActiveMME(connectedAllRC[i].thisIMME);

            if (connectedAllRC[i].roomRule.roomType != eRoomType.Completed)
            {
                connectedAllRC[i].thisMME.Set_Visible();
                connectedAllRC[i].thisIMME.Set_Visible();
            }
        }
    }

    // 미니맵 위치 조정
    private void Set_AnchorPos(MinimapCellEUIController _MME, RectTransform _ParentRT, float _DurTime)
    {
        if (DevTool.Get_ComponentTType(_MME.gameObject, out RectTransform rt))
        {
            DevTool.Set_KillTween(_ParentRT);

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

    // 이펙트
    public void Play_Effect()
    {
        // 효과
        DevTool.Set_KillTween(InnerImg);

        InnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                InnerImg.DOFade(0.25f, 0.2f);
            });
    }

    #endregion

    #region Tab Interactable

    
    private Sequence Play_MinimapTween(
        Vector2 _TargetSize, 
        float _NormalMinimapAlpha, float _InteractMinimapAlpha, 
        float _DurTime,
        Dele _StartDele, Dele _CompleteDele)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(MinimapFrameRT.DOSizeDelta(_TargetSize, _DurTime));
        seq.Join(NormalCG.DOFade(_NormalMinimapAlpha, _DurTime));
        seq.Join(InteractableCG.DOFade(_InteractMinimapAlpha, _DurTime));

        seq.OnStart(() => { _StartDele(); })
            .OnComplete(() => { _CompleteDele(); });

        return seq; 
    }


    public void SetOn_TabInteract(float _DurTime)
    {
        DevTool.Set_KillTween(TabSeq);

        TabSeq = Play_MinimapTween(
            _TargetSize: InteractableSize_Frame,
            _NormalMinimapAlpha: 0f,
            _InteractMinimapAlpha: 1f, 
            _DurTime,
            Set_Start_OnInteract, 
            Set_Complete_OnInteract);
    }

    private void Set_Start_OnInteract()
    {
        MinimapSelectedElementRC = null;
        InteractingBookGate = null;
    }

    private void Set_Complete_OnInteract()
    {
        InteractingPoint.gameObject.SetActive(true);

        InputManager.instance.inputArrowDir = Vector2Int.zero;
        CanInteractable = true;

        MinimapSelectedElementRC = StageManager.instance.currentRoomController;
    }


    public void SetOff_TabInteract(float _DurTime)
    {
        DevTool.Set_KillTween(TabSeq);
        TabSeq = Play_MinimapTween(
            _TargetSize: NormalSize_Frame,
            _NormalMinimapAlpha: 1f,
            _InteractMinimapAlpha: 0f,
            _DurTime,
            Set_Start_OffInteract,
            Set_Complete_OffInteract);
    }

    private void Set_Start_OffInteract()
    {
        InteractingPoint.gameObject.SetActive(false);

        InputManager.instance.inputArrowDir = Vector2Int.zero;
        CanInteractable = false;
    }
    private void Set_Complete_OffInteract()
    {
        if (InteractingBookGate != null && 
            StageManager.instance.currentRoomController != InteractingBookGate.parterGate.thisRoom)
        {
            InteractingBookGate.Play_Interact();
        }
    }

    #endregion

    #region Set

    private void Set_BookRoom()
    {
        if (CanInteractable && InputManager.instance.inputArrowDir != Vector2Int.zero)
        {
            GateController gc = MinimapSelectedElementRC.Get_MinimapInteract_ShortcutGate(InputManager.instance.inputArrowDir);
            if (gc != null)
            {
                InteractingBookGate = gc.parterGate;
                MinimapSelectedElementRC = gc.thisRoom;
                Set_AnchorPos(MinimapSelectedElementRC.thisIMME, InteractableMMEParentRT, 0.15f);
            }
            InputManager.instance.inputArrowDir = Vector2Int.zero;
        }
    }

    public void Reset_BookRoom()
    {
        Set_Start_OnInteract(); 
        Set_Complete_OnInteract();
    }

    #endregion
}
