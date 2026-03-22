using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BoxCellEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Value")]
    [FormerlySerializedAs("ThisPos")][SerializeField] public Vector2Int pos;

    [Space(10)]
    [Header("=== GO")]

    [Space(5)]
    [Header("-- Main")]
    [FormerlySerializedAs("BackGO")][SerializeField] private GameObject backGo;
    [FormerlySerializedAs("FrontGO")][SerializeField] private GameObject frontGo;

    [Space(5)]
    [Header("-- Dir")]
    [FormerlySerializedAs("UpLineGO")][SerializeField] private GameObject upLineGo;
    [FormerlySerializedAs("RightLineGO")][SerializeField] private GameObject rightLineGo;
    [FormerlySerializedAs("DownLineGO")][SerializeField] private GameObject downLineGo;
    [FormerlySerializedAs("LeftLineGO")][SerializeField] private GameObject leftLineGo;

    // Value
    [HideInInspector] private bool isTweening = false;
    [HideInInspector] private bool isInteractable = true;
    [HideInInspector] private Dictionary<Vector2Int, GameObject> dirGoDict;

    // Inner
    [HideInInspector] private List<Image> frameBackInnerList;
    [HideInInspector] private List<Image> allInnerList = new List<Image>();

    // Owner
    [HideInInspector] public BoxLineConnectorUIController ownerPuzzleUIController;

    // Comp
    [HideInInspector] private RectTransform frontRt;

    #endregion

    #region Offset

    private void Offset_Value()
    {
        dirGoDict = new Dictionary<Vector2Int, GameObject>
        {
            { Vector2Int.up, upLineGo },
            { Vector2Int.right, rightLineGo },
            { Vector2Int.down, downLineGo },
            { Vector2Int.left, leftLineGo }
        };

        frameBackInnerList = new List<Image>
        {
            DevTool.Get_ComponentTType<Image>(backGo.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<Image>(backGo.transform.GetChild(1).gameObject)
        };

        frontRt = DevTool.Get_ComponentTType(frontGo, out RectTransform rt) ? rt : null;

        allInnerList.AddRange(frameBackInnerList);
        allInnerList.AddRange(DevTool.Get_ChildList<Image>(upLineGo.transform));
        allInnerList.AddRange(DevTool.Get_ChildList<Image>(rightLineGo.transform));
        allInnerList.AddRange(DevTool.Get_ChildList<Image>(downLineGo.transform));
        allInnerList.AddRange(DevTool.Get_ChildList<Image>(leftLineGo.transform));
    }

    public override void Offset()
    {
        base.Offset();

        Offset_Value();
    }

    #endregion

    #region Set

    public void Set_Active(bool onOff)
    {
        frontGo.SetActive(onOff);

        if (onOff)
            SetOn_FrameBack();
        else
            SetOff_FrameBack();
    }

    private void SetOn_FrameBack()
    {
        frameBackInnerList[0].gameObject.SetActive(true);
        isInteractable = true;
    }

    private void SetOff_FrameBack()
    {
        frameBackInnerList[0].gameObject.SetActive(false);
        isInteractable = false;
    }

    public void Set_RandomAngle()
    {
        frontRt.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 4) * 90f);
    }

    public void Set_InnerColor(Color clr)
    {
        DevTool.Set_Color(clr, allInnerList);
    }


    #endregion

    #region Get

    public GameObject Get_CorrectDirGO(Vector2Int dir)
    {
        return dirGoDict[dir];
    }

    public bool Is_CorrectDir()
    {
        if (DevTool.Is_InRange(frontRt.transform.eulerAngles.z, 0f, 10f))
            return true;

        if ((upLineGo.activeSelf && downLineGo.activeSelf && !leftLineGo.activeSelf && !rightLineGo.activeSelf) || 
            (rightLineGo.activeSelf && leftLineGo.activeSelf && !upLineGo.activeSelf && !downLineGo.activeSelf))
        {
            if (DevTool.Is_InRange(frontRt.transform.eulerAngles.z, 180f, 10f))
                return true;
        }

        if (upLineGo.activeSelf && downLineGo.activeSelf && leftLineGo.activeSelf && rightLineGo.activeSelf)
            return true;

        return false;
    }

    #endregion

    #region Tween

    public void Play_Roll(float plusAngle, float durTime)
    {
        if (isTweening || !isInteractable) return;

        isTweening = true;

        float targetAngle = frontRt.rotation.eulerAngles.z + plusAngle;

        frontRt.DOLocalRotate(new Vector3(0f, 0f, targetAngle), durTime)
            .OnComplete(() =>
            {
                frontRt.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                isTweening = false;

                ownerPuzzleUIController.Check_CorrectLineSet();
            });

        Play_Seq(durTime);
    }

    public void Play_Seq(float durTime)
    {
        DevTool.Set_KillTween(frameBackInnerList[1]);

        Sequence seq = DOTween.Sequence();

        DevTool.Set_AlphaColor(frameBackInnerList[1], 0.01f);

        seq.Join(frameBackInnerList[1].DOFade(0.5f, durTime / 2));
        seq.Join(frameBackInnerList[1].DOFade(0.01f, durTime / 2));
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerPuzzleUIController != null) ownerPuzzleUIController.Set_BoxCellSelect(this);
    }

    #endregion
}
