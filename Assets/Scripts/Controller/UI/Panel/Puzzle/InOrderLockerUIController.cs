using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOrderLockerUIController : PuzzleUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> In Order Locker")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform AllIOLCellParentTF;
    [SerializeField] private RectTransform SelectingSignRT;

    #endregion

    #region - Hide

    // EUI
    [HideInInspector] private List<IOLCellEUIController> AllIOLCell;

    // Value
    [HideInInspector] private int CellAmount = 0;

    // Selecting
    [HideInInspector] private IOLCellEUIController SelectingIOLCellEUI;

    #endregion

    #endregion

    #region Offset

    public override void Offset_FirstValue(PrisonController _Prison)
    {
        base.Offset_FirstValue(_Prison);

        Debug.Log(_Prison.Rating);
        CellAmount = 5 + _Prison.Rating;
        CurrentCountdown = BaseCountdown;
    }

    public override void Offset()
    {
        base.Offset();

        AllIOLCell = DevTool.Get_ChildList<IOLCellEUIController>(AllIOLCellParentTF);

        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].OwnerUIController = this;
            AllIOLCell[i].OnwerIOLUIController = this;
            AllIOLCell[i].Offset();
        }
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (Is_Interact_On()) return;
    }

    public void Try_InteractUnlock()
    {
        if (Is_Interact_TryUnlock()) return;
    }

    #endregion

    #region Interact (On)

    private bool Is_Interact_On()
    {
        if (!(CurrentBtn is IOLCellEUIController cellEUI) ||
            cellEUI != SelectingIOLCellEUI ||
            !IsInteractable)
            return false;

        if (SelectingIOLCellEUI.IsOn)
            SelectingIOLCellEUI.Set_Off(0.1f);
        else
            SelectingIOLCellEUI.Set_On(0.1f);

        return true;
    }

    #endregion

    #region Can

    public override bool Can_Success()
    {
        return false;
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        Set_AllDefault();
        Set_InnerColor(LockedClr);
    }

    protected override void Set_AllComplete()
    {
        base.Set_AllComplete();

        Set_InnerColor(UnlockedClr);
    }

    #endregion

    #region Set (Unique)

    private void Set_AllDefault()
    {
        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].Set_Off(0.1f);
        }
    }

    private void Set_InnerColor(Color _Clr)
    {
        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].Set_Color(_Clr);
        }
    }

    #endregion

    #region Ser (Select)

    public void Set_CellSelect(IOLCellEUIController _CellEUI)
    {
        if (SelectingIOLCellEUI != _CellEUI)
        {
            SelectingIOLCellEUI = _CellEUI;
            SelectingSignRT.gameObject.SetActive(true);
            SelectingSignRT.anchoredPosition = _CellEUI.ThisRT.anchoredPosition;
            Play_SelectingRT();
        }
    }

    private void Play_SelectingRT()
    {
        DevTool.Set_KillTween(SelectingSignRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(SelectingSignRT.DOScale(1.05f, 0.1f));
        seq.Append(SelectingSignRT.DOScale(1.0f, 0.1f));

    }

    #endregion
}
