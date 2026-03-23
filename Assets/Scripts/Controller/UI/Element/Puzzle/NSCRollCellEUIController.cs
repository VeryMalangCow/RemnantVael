using DG.Tweening;
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
    [SerializeField] private RectTransform rollImgParentRt;
    [SerializeField] protected eNSCPuzzleType nscType;

    [Space(10)]
    [Header("=== Lock")]
    [SerializeField] private RectTransform lockedRt;

    #endregion

    #region - Hide

    // Comp
    [HideInInspector] protected List<Image> rollImgList;

    // Value
    [HideInInspector] private bool isLocked = false;
    [HideInInspector] protected int currentIndex = 0;
    [HideInInspector] public int answerIndex = 0;

    // Roll
    [HideInInspector] private static readonly float intervalY = 150f;
    [HideInInspector] private bool isRolling = false;

    // Owner
    [HideInInspector] public NSCPanelEUIController ownerNscPanelEuiController;
    [HideInInspector] public NumShapeColorPasswordUIController ownerNscUIController;
    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        rollImgList = DevTool.Get_ChildList<Image>(rollImgParentRt);
    }

    #endregion

    #region Set

    public abstract void Set_ImgByIndex(int index);

    public void Set_ImgByIndexRandom()
    {
        currentIndex = Random.Range(0, rollImgList.Count);
        Set_ImgByIndex(currentIndex);
    }

    public void Set_ImgByAnswerAndLock()
    {
        isLocked = true;
        lockedRt.gameObject.SetActive(true);

        currentIndex = (answerIndex + 3) % rollImgList.Count;
        Set_ImgByIndex(currentIndex);
    }

    public void Set_NoLock()
    {
        isLocked = false;
        lockedRt.gameObject.SetActive(false);
    }

    #endregion

    #region Get

    public int Get_Index()
    {
        return (currentIndex + 2) % rollImgList.Count;
    }

    public int Get_IndexAmount()
    {
        return rollImgList.Count;
    }

    #endregion

    #region Is

    public bool Is_AnswerIndex()
    {
        if (Get_Index() == answerIndex)
            return true;

        return false;
    }

    #endregion

    #region Play (Roll)

    public void Play_RollForDown()
    {
        if (isRolling || isLocked) return;

        isRolling = true;

        rollImgParentRt.DOAnchorPosY(intervalY, 0.1f)
            .OnComplete(() =>
            {
                currentIndex++;
                if (currentIndex > rollImgList.Count)
                    currentIndex -= rollImgList.Count;
                
                Set_ImgByIndex(currentIndex);
                rollImgParentRt.anchoredPosition = Vector2.zero;

                isRolling = false;

                ownerNscUIController.Check_CorrectLineSet();
            });
    }

    public void Play_RollForUp()
    {
        if (isRolling || isLocked) return;

        isRolling = true;

        rollImgParentRt.DOAnchorPosY(-intervalY, 0.1f)
            .OnComplete(() =>
            {
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex += rollImgList.Count;

                Set_ImgByIndex(currentIndex);
                rollImgParentRt.anchoredPosition = Vector2.zero;

                isRolling = false;

                ownerNscUIController.Check_CorrectLineSet();
            });
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerNscUIController != null) ownerNscUIController.Set_RollCellSelect(this);
    }

    #endregion
}
