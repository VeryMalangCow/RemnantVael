using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InOrderLockerUIController : PuzzleUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> In Order Locker")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform AllIOLCellParentTF;
    [SerializeField] private Transform InnerParentTF;
    [SerializeField] private RectTransform SelectingSignRT;

    [Space(10)]
    [SerializeField] private TMP_Text AnswerIndexTxt;
    [SerializeField] private TMP_Text AnswerCurrentSetTxt;

    [Space(10)]
    [Header("=== Ready KeyAnno")]
    [SerializeField] private Image SelectInputImg;

    #endregion

    #region - Hide

    // EUI
    [HideInInspector] private List<IOLCellEUIController> AllIOLCell;

    // Inner
    [HideInInspector] private List<Image> InnerImgList;

    // Value
    [HideInInspector] private int CellAmount = 0;
    [HideInInspector] private List<int> AnswerIntList = new List<int>();
    [HideInInspector] private List<int> SelectingIntList = new List<int>();
    [HideInInspector] private int NeedNextSelectEUIIndex = -1;
     
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
        CurrentCountdown = BaseCountdown + (_Prison.Rating * 4);
    }

    public override void Offset()
    {
        base.Offset();

        AllIOLCell = DevTool.Get_ChildList<IOLCellEUIController>(AllIOLCellParentTF);

        InnerImgList = DevTool.Get_ChildList<Image>(InnerParentTF);

        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].OwnerUIController = this;
            AllIOLCell[i].OwnerIOLUIController = this;
            AllIOLCell[i].Offset();
        }

        SelectInputImg.sprite = ResourceManager.instance.mlbSprite;
        SelectInputImg.SetNativeSize();
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

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

        if (SelectingIOLCellEUI.IsInteractable && !SelectingIOLCellEUI.IsOn)
            Set_SelectingIncludeValue(SelectingIOLCellEUI);

        return true;
    }

    #endregion

    #region Can

    public override bool Can_Success()
    {
        if (AnswerIntList.Count == SelectingIntList.Count)
            return true;

        return false;
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        ReadyPanelEUI.Set_RuleDesc(ResourceManager.instance.Get_StaticDesc(35));

        Set_AllDefault();
        Set_InnerColor(ResourceManager.instance.lockedClr);
        Set_InteractableAmount(CellAmount);
    }

    protected override void Set_AllComplete()
    {
        base.Set_AllComplete();

        Set_InnerColor(ResourceManager.instance.unlockedClr);
    }

    #endregion

    #region Set (Unique)

    private void Set_AllDefault()
    {
        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].Set_Default();
            AllIOLCell[i].Set_Interactable(false);
            AllIOLCell[i].Set_NumIndexTxt(false);
        }

        NeedNextSelectEUIIndex = -1;
    }

    private void Set_InnerColor(Color _Clr)
    {
        for (int i = 0; i < InnerImgList.Count; i++)
        {
            DevTool.Set_Color(_Clr, InnerImgList[i]);
        }
        for (int i = 0; i < AllIOLCell.Count; i++)
        {
            AllIOLCell[i].Set_Color(_Clr);
        }
    }

    private void Set_InteractableAmount(int _Amount)
    {
        AnswerIntList.Clear();

        string answerIndexString = "";
        for (int i = 0; i < _Amount; i++)
        {
            AllIOLCell[i].Set_Interactable(true);
            AnswerIntList.Add(i);
            answerIndexString += i != _Amount - 1 ? $"<size=200%>{i + 1}</size>\t" : $"<size=200%>{i + 1}</size>";
        }
        AnswerIndexTxt.text = answerIndexString;
        AnswerCurrentSetTxt.text = "";

        AnswerIntList = DevTool.Get_ShuffledList(AnswerIntList);
    }

    #endregion

    #region Set (Selecting List)

    private void Set_SelectingIncludeValue(IOLCellEUIController _CellEUI)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        int targetCellEUIIndex = AllIOLCell.IndexOf(_CellEUI);

        // 처음 선택
        if (NeedNextSelectEUIIndex == -1 ||
            NeedNextSelectEUIIndex != targetCellEUIIndex)
        {
            SelectingIntList.Clear();
        }

        SelectingIntList.Add(targetCellEUIIndex);
        NeedNextSelectEUIIndex = Get_NextTargetIndex(targetCellEUIIndex);

        Set_SelectingValueTxt(SelectingIntList);
    }

    private void Set_SelectingValueTxt(List<int> _Value)
    {
        List<int> numList = new List<int>();
        for (int i = 0; i < _Value.Count; i++)
            numList.Add(AnswerIntList.IndexOf(_Value[i]));
        
        string answerIndexString = "";
        for (int i = 0; i < AnswerIntList.Count; i++)
        {
            // On Off
            if (_Value.Contains(i))
            {
                AllIOLCell[i].Set_On(0.1f);
                AllIOLCell[i].Set_NumIndexTxt(true, AnswerIntList.IndexOf(i));
            }
            else
            {
                AllIOLCell[i].Set_Off(0.1f);
                AllIOLCell[i].Set_NumIndexTxt(false);
            }

            // Num
            if (numList.Contains(i))
            {
                answerIndexString += i != AnswerIntList.Count - 1 ? "<size=150%>^</size>\t" : "<size=150%>^</size>";
            }
            else
            {
                answerIndexString += i != AnswerIntList.Count - 1 ? "<size=150%> </size>\t" : "<size=150%> </size>";
            }
        }
        AnswerCurrentSetTxt.text = answerIndexString;
    }


    #endregion

    #region Set (Select)

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

    #region Get (Selecting List)

    private int Get_NextTargetIndex(int _FirstInt)
    {
        for (int i = 0; i < AnswerIntList.Count; i++)
        {
            if (AnswerIntList[i] == _FirstInt)
            {
                if (i == AnswerIntList.Count - 1)
                    return AnswerIntList[0];
                else
                    return AnswerIntList[i + 1];
            }
        }

        return -1;
    }

    #endregion
}
