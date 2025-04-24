using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoxCellEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] public Vector2Int ThisPos;

    [Space(10)]
    [Header("=== GO")]

    [Space(5)]
    [Header("-- Main")]
    [SerializeField] private GameObject BackGO;
    [SerializeField] private GameObject FrontGO;

    [Space(5)]
    [Header("-- Dir")]
    [SerializeField] private GameObject UpLineGO;
    [SerializeField] private GameObject RightLineGO;
    [SerializeField] private GameObject DownLineGO;
    [SerializeField] private GameObject LeftLineGO;

    // Value
    [HideInInspector] private bool IsTweening = false;
    [HideInInspector] private bool IsInteractable = true;
    [HideInInspector] private Dictionary<Vector2Int, GameObject> DirGODict;

    // Inner
    [HideInInspector] private List<Image> FrameBackInnerList;
    [HideInInspector] private List<Image> AllInnerList = new List<Image>();

    // Owner
    [HideInInspector] public BoxLineConnectorUIController OwnerPuzzleUIController;

    // Comp
    [HideInInspector] private RectTransform FrontRT;

    #endregion

    #region Offset

    private void Offset_Value()
    {
        DirGODict = new Dictionary<Vector2Int, GameObject>
        {
            { Vector2Int.up, UpLineGO },
            { Vector2Int.right, RightLineGO },
            { Vector2Int.down, DownLineGO },
            { Vector2Int.left, LeftLineGO }
        };

        FrameBackInnerList = new List<Image>
        {
            DevTool.Get_ComponentTType<Image>(BackGO.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<Image>(BackGO.transform.GetChild(1).gameObject)
        };

        FrontRT = DevTool.Get_ComponentTType(FrontGO, out RectTransform rt) ? rt : null;

        AllInnerList.AddRange(FrameBackInnerList);
        AllInnerList.AddRange(DevTool.Get_ChildList<Image>(UpLineGO.transform));
        AllInnerList.AddRange(DevTool.Get_ChildList<Image>(RightLineGO.transform));
        AllInnerList.AddRange(DevTool.Get_ChildList<Image>(DownLineGO.transform));
        AllInnerList.AddRange(DevTool.Get_ChildList<Image>(LeftLineGO.transform));
    }

    public override void Offset()
    {
        base.Offset();

        Offset_Value();
    }

    #endregion

    #region Set

    public void Set_Active(bool _OnOff)
    {
        FrontGO.SetActive(_OnOff);

        if (_OnOff)
            SetOn_FrameBack();
        else
            SetOff_FrameBack();
    }

    private void SetOn_FrameBack()
    {
        FrameBackInnerList[0].gameObject.SetActive(true);
        IsInteractable = true;
    }

    private void SetOff_FrameBack()
    {
        FrameBackInnerList[0].gameObject.SetActive(false);
        IsInteractable = false;
    }

    public void Set_RandomAngle()
    {
        FrontRT.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 4) * 90f);
    }

    public void Set_InnerColor(Color _Clr)
    {
        DevTool.Set_Color(_Clr, AllInnerList);
    }


    #endregion

    #region Get

    public GameObject Get_CorrectDirGO(Vector2Int _Dir)
    {
        return DirGODict[_Dir];
    }

    public bool Is_CorrectDir()
    {
        if (DevTool.Is_InRange(FrontRT.transform.eulerAngles.z, 0f, 10f))
            return true;

        if ((UpLineGO.activeSelf && DownLineGO.activeSelf && !LeftLineGO.activeSelf && !RightLineGO.activeSelf) || 
            (RightLineGO.activeSelf && LeftLineGO.activeSelf && !UpLineGO.activeSelf && !DownLineGO.activeSelf))
        {
            if (DevTool.Is_InRange(FrontRT.transform.eulerAngles.z, 180f, 10f))
                return true;
        }

        if (UpLineGO.activeSelf && DownLineGO.activeSelf && LeftLineGO.activeSelf && RightLineGO.activeSelf)
            return true;

        return false;
    }

    #endregion

    #region Tween

    public void Play_Roll(float _PlusAngle, float _DurTime)
    {
        if (IsTweening || !IsInteractable) return;

        IsTweening = true;

        float targetAngle = FrontRT.rotation.eulerAngles.z + _PlusAngle;

        FrontRT.DOLocalRotate(new Vector3(0f, 0f, targetAngle), _DurTime)
            .OnComplete(() =>
            {
                FrontRT.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                IsTweening = false;

                OwnerPuzzleUIController.Check_CorrectLineSet();
            });

        Play_Seq(_DurTime);
    }

    public void Play_Seq(float _DurTime)
    {
        DevTool.Set_KillTween(FrameBackInnerList[1]);

        Sequence seq = DOTween.Sequence();

        DevTool.Set_AlphaColor(FrameBackInnerList[1], 0.01f);

        seq.Join(FrameBackInnerList[1].DOFade(0.5f, _DurTime / 2));
        seq.Join(FrameBackInnerList[1].DOFade(0.01f, _DurTime / 2));
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        if (OwnerPuzzleUIController != null) OwnerPuzzleUIController.Set_BoxCellSelect(this);
    }

    #endregion
}
