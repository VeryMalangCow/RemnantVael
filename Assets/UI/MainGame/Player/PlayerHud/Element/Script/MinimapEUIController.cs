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
    [SerializeField] private GameObject minimapElement;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform minimapFrameRt;
    [SerializeField] public Image innerImg;
    [HideInInspector] private Color mainClr;


    [Space(10)]
    [Header("=== Normal")]
    [SerializeField] private RectTransform normalMaskRt;
    [SerializeField] private RectTransform normalMmeParentRt;
    [SerializeField] private Vector2 normalSize_Frame = new Vector2(352, 336);

    [Header("-- Element")]
    [SerializeField] private RectTransform normalPoint;


    [Space(10)]
    [Header("=== Interactable")]
    [SerializeField] private RectTransform interactableMaskRt;
    [SerializeField] private RectTransform interactableMmeParentRt;
    [SerializeField] private Vector2 interactableSize_Frame = new Vector2(728, 712);

    [Header("-- Element")]
    [SerializeField] private RectTransform interactablePoint;
    [SerializeField] private RectTransform interactingPoint;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public List<MinimapCellEUIController> allMmcEui = new List<MinimapCellEUIController>();

    // CG
    [HideInInspector] private CanvasGroup normalCg;
    [HideInInspector] private CanvasGroup interactableCg;

    // Book
    [HideInInspector] private GateController interactingBookGate;
    [HideInInspector] private RoomController minimapSelectedElementRoom;

    [HideInInspector] private bool canInteractable = false;
    [HideInInspector] private Sequence tabSeq;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        // Color
        Color mainClr = innerImg.color;
        mainClr.a = 1f;
        this.mainClr = mainClr;

        // Frame
        minimapFrameRt.sizeDelta = normalSize_Frame;

        // Canvas Group
        normalCg = DevTool.Get_ComponentTType(normalMaskRt.gameObject, out CanvasGroup nCg) ? nCg : null;
        normalCg.alpha = 1f;

        interactableCg = DevTool.Get_ComponentTType(interactableMaskRt.gameObject, out CanvasGroup iCg) ? iCg : null;
        interactableCg.alpha = 0f;
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
        List<RoomController> allRC = StageManager.instance.stageObjectGenerator.currentAllRoomController;

        for (int i = 0; i < allRC.Count; i++)
        {
            Gen_EachMinimapElement(allRC[i], normalMmeParentRt.transform, isNormal: true);
            Gen_EachMinimapElement(allRC[i], interactableMmeParentRt.transform, isNormal: false);
        }
    }

    private void Gen_EachMinimapElement(RoomController connetedRoom, Transform parentTf, bool isNormal)
    {
        if (DevTool.Get_ComponentTType(Instantiate(minimapElement, parentTf), out MinimapCellEUIController mmc))
        {
            allMmcEui.Add(mmc);
            mmc.gameObject.SetActive(false);
            mmc.Offset();
            mmc.Offset(connetedRoom, isNormal);
        }
    }

    #endregion

    #region Remove


    public void Remove_AllMinimapCell()
    {
        int amount = allMmcEui.Count;
        for (int i = amount - 1; i >= 0; i--)
        {
            Destroy(allMmcEui[i].gameObject);
            allMmcEui.RemoveAt(i);
        }

        allMmcEui.Clear();
    }

    #endregion

    #region Set State

    public void Set_State()
    {
        RoomController CurrentRC = StageManager.instance.currentRoomController;

        Set_AnchorPos(CurrentRC.thisMME, normalMmeParentRt, 0.3f);
        Set_AnchorPos(CurrentRC.thisIMME, interactableMmeParentRt, 0.3f);

        //Set_Point(NormalPoint, CurrentRC.ThisMME);
        //Set_Point(InteractablePoint, CurrentRC.ThisIMME);

        Set_ActiveMME(CurrentRC.thisMME);
        Set_ActiveMME(CurrentRC.thisIMME);

        if (CurrentRC.roomRule.roomType == eRoomType.Completed)
        {
            CurrentRC.thisMME.Set_Complete(mainClr);
            CurrentRC.thisIMME.Set_Complete(mainClr);
        }
        else
        {
            CurrentRC.thisMME.Set_Uncomplete();
            CurrentRC.thisIMME.Set_Uncomplete();
        }

        List<RoomController> connectedAllRC = CurrentRC.GetConnectedRooms();

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
    private void Set_AnchorPos(MinimapCellEUIController mme, RectTransform parentRt, float durTime)
    {
        if (DevTool.Get_ComponentTType(mme.gameObject, out RectTransform rt))
        {
            DevTool.SetKillTween(parentRt);

            parentRt.DOAnchorPos(-rt.anchoredPosition, durTime);
        }
    }

    // 켜지는 MME
    private void Set_ActiveMME(MinimapCellEUIController mme)
    {
        if (!mme.gameObject.activeSelf)
        {
            mme.Set_ActiveOn();
        }
    }

    // 이펙트
    public void Play_Effect()
    {
        // 효과
        DevTool.SetKillTween(innerImg);

        innerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                innerImg.DOFade(0.25f, 0.2f);
            });
    }

    #endregion

    #region Tab Interactable

    
    private Sequence Play_MinimapTween(
        Vector2 targetSize, 
        float normalMinimapAlpha, float interactMinimapAlpha, 
        float durTime,
        Dele startDele, Dele completeDele)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(minimapFrameRt.DOSizeDelta(targetSize, durTime));
        seq.Join(normalCg.DOFade(normalMinimapAlpha, durTime));
        seq.Join(interactableCg.DOFade(interactMinimapAlpha, durTime));

        seq.OnStart(() => { startDele(); })
            .OnComplete(() => { completeDele(); });

        return seq; 
    }


    public void SetOn_TabInteract(float durTime)
    {
        DevTool.Set_KillTween(tabSeq);

        tabSeq = Play_MinimapTween(
            targetSize: interactableSize_Frame,
            normalMinimapAlpha: 0f,
            interactMinimapAlpha: 1f, 
            durTime,
            Set_Start_OnInteract, 
            Set_Complete_OnInteract);
    }

    private void Set_Start_OnInteract()
    {
        minimapSelectedElementRoom = null;
        interactingBookGate = null;
    }

    private void Set_Complete_OnInteract()
    {
        interactingPoint.gameObject.SetActive(true);

        InputManager.instance.inputArrowDir = Vector2Int.zero;
        canInteractable = true;

        minimapSelectedElementRoom = StageManager.instance.currentRoomController;
    }


    public void SetOff_TabInteract(float durTime)
    {
        DevTool.Set_KillTween(tabSeq);
        tabSeq = Play_MinimapTween(
            targetSize: normalSize_Frame,
            normalMinimapAlpha: 1f,
            interactMinimapAlpha: 0f,
            durTime,
            Set_Start_OffInteract,
            Set_Complete_OffInteract);
    }

    private void Set_Start_OffInteract()
    {
        interactingPoint.gameObject.SetActive(false);

        InputManager.instance.inputArrowDir = Vector2Int.zero;
        canInteractable = false;
    }
    private void Set_Complete_OffInteract()
    {
        if (interactingBookGate != null && 
            StageManager.instance.currentRoomController != interactingBookGate.parterGate.thisRoom)
        {
            interactingBookGate.PlayInteract();
        }
    }

    #endregion

    #region Set

    private void Set_BookRoom()
    {
        if (canInteractable && InputManager.instance.inputArrowDir != Vector2Int.zero)
        {
            GateController gc = minimapSelectedElementRoom.GetMinimapInteractShortcutGate(InputManager.instance.inputArrowDir);
            if (gc != null)
            {
                interactingBookGate = gc.parterGate;
                minimapSelectedElementRoom = gc.thisRoom;
                Set_AnchorPos(minimapSelectedElementRoom.thisIMME, interactableMmeParentRt, 0.15f);
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
