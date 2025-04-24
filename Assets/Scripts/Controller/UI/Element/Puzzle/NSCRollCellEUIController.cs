using DG.Tweening;
using LeTai.TrueShadow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class NSCRollCellEUIController : OwnBtnEUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Roll Img")]

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private RectTransform RollImgParentRT;
    [SerializeField] protected eNSCPuzzleType ThisNSCType;

    [Space(10)]
    [Header("=== Lock")]
    [SerializeField] private RectTransform LockedRT;

    #endregion

    #region - Hide

    // Comp
    [HideInInspector] protected List<Image> RollImgList;

    // Value
    [HideInInspector] private bool IsLocked = false;
    [HideInInspector] protected int CurrentIndex = 0;
    [HideInInspector] public int AnswerIndex = 0;

    // Roll
    [HideInInspector] private static readonly float Interval_Y = 150f;
    [HideInInspector] private bool IsRolling = false;

    // Owner
    [HideInInspector] public NSCPanelEUIController OwnerNSCPanelEUIController;
    [HideInInspector] public NumShapeColorPasswordUIController OwnerNSCUIController;
    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        RollImgList = DevTool.Get_ChildList<Image>(RollImgParentRT);
    }

    #endregion

    #region Set

    public abstract void Set_ImgByIndex(int _Index);

    public void Set_ImgByIndexRandom()
    {
        CurrentIndex = Random.Range(0, RollImgList.Count);
        Set_ImgByIndex(CurrentIndex);
    }

    public void Set_ImgByAnswerAndLock()
    {
        IsLocked = true;
        LockedRT.gameObject.SetActive(true);

        CurrentIndex = (AnswerIndex + 3) % RollImgList.Count;
        Set_ImgByIndex(CurrentIndex);
    }

    public void Set_NoLock()
    {
        IsLocked = false;
        LockedRT.gameObject.SetActive(false);
    }

    #endregion

    #region Get

    public int Get_Index()
    {
        return (CurrentIndex + 2) % RollImgList.Count;
    }

    public int Get_IndexAmount()
    {
        return RollImgList.Count;
    }

    #endregion

    #region Is

    public bool Is_AnswerIndex()
    {
        if (Get_Index() == AnswerIndex)
            return true;

        return false;
    }

    #endregion

    #region Play (Roll)

    public void Play_RollForDown()
    {
        if (IsRolling || IsLocked) return;

        IsRolling = true;

        RollImgParentRT.DOAnchorPosY(Interval_Y, 0.1f)
            .OnComplete(() =>
            {
                CurrentIndex++;
                if (CurrentIndex > RollImgList.Count)
                    CurrentIndex -= RollImgList.Count;
                
                Set_ImgByIndex(CurrentIndex);
                RollImgParentRT.anchoredPosition = Vector2.zero;

                IsRolling = false;

                OwnerNSCUIController.Check_CorrectLineSet();
            });
    }

    public void Play_RollForUp()
    {
        if (IsRolling || IsLocked) return;

        IsRolling = true;

        RollImgParentRT.DOAnchorPosY(-Interval_Y, 0.1f)
            .OnComplete(() =>
            {
                CurrentIndex--;
                if (CurrentIndex < 0)
                    CurrentIndex += RollImgList.Count;

                Set_ImgByIndex(CurrentIndex);
                RollImgParentRT.anchoredPosition = Vector2.zero;

                IsRolling = false;

                OwnerNSCUIController.Check_CorrectLineSet();
            });
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        if (OwnerNSCUIController != null) OwnerNSCUIController.Set_RollCellSelect(this);
    }

    #endregion
}
