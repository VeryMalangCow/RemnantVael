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
    [SerializeField] private Transform allIolCellParentTf;
    [SerializeField] private Transform innerParentTf;
    [SerializeField] private RectTransform selectingSignRT;

    [Space(10)]
    [SerializeField] private TMP_Text answerIndexTxt;
    [SerializeField] private TMP_Text answerCurrentSetTxt;

    [Space(10)]
    [Header("=== Ready KeyAnno")]
    [SerializeField] private Image selectInputImg;

    #endregion

    #region - Hide

    // EUI
    [HideInInspector] private List<IOLCellEUIController> allIolCell;

    // Inner
    [HideInInspector] private List<Image> innerImgList;

    // Value
    [HideInInspector] private int cellAmount = 0;
    [HideInInspector] private List<int> answerIntList = new List<int>();
    [HideInInspector] private List<int> selectingIntList = new List<int>();
    [HideInInspector] private int needNextSelectEUIIndex = -1;
     
    // Selecting
    [HideInInspector] private IOLCellEUIController selectingIolCellEui;

    #endregion

    #endregion

    #region Offset

    public override void Offset_FirstValue(PrisonController prison)
    {
        base.Offset_FirstValue(prison);

        Debug.Log(prison.rating);
        cellAmount = 5 + prison.rating;
        currentCountdown = baseCountdown + (prison.rating * 4);
    }

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        allIolCell = DevTool.Get_ChildList<IOLCellEUIController>(allIolCellParentTf);

        innerImgList = DevTool.Get_ChildList<Image>(innerParentTf);

        for (int i = 0; i < allIolCell.Count; i++)
        {
            allIolCell[i].ownerUIController = this;
            allIolCell[i].ownerIolUIController = this;
            allIolCell[i].Offset();
        }

        selectInputImg.sprite = ResourceManager.instance.mlbSprite;
        selectInputImg.SetNativeSize();
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
        if (!(currentBtn is IOLCellEUIController cellEUI) ||
            cellEUI != selectingIolCellEui ||
            !isInteractable)
            return false;

        if (selectingIolCellEui.isInteractable && !selectingIolCellEui.isOn)
            Set_SelectingIncludeValue(selectingIolCellEui);

        return true;
    }

    #endregion

    #region Can

    public override bool Can_Success()
    {
        if (answerIntList.Count == selectingIntList.Count)
            return true;

        return false;
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        readyPanelEui.Set_RuleDesc(ResourceManager.instance.Get_StaticDesc(35));

        Set_AllDefault();
        Set_InnerColor(ResourceManager.instance.lockedClr);
        Set_InteractableAmount(cellAmount);
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
        for (int i = 0; i < allIolCell.Count; i++)
        {
            allIolCell[i].Set_Default();
            allIolCell[i].Set_Interactable(false);
            allIolCell[i].Set_NumIndexTxt(false);
        }

        needNextSelectEUIIndex = -1;
    }

    private void Set_InnerColor(Color clr)
    {
        for (int i = 0; i < innerImgList.Count; i++)
        {
            DevTool.Set_Color(clr, innerImgList[i]);
        }
        for (int i = 0; i < allIolCell.Count; i++)
        {
            allIolCell[i].Set_Color(clr);
        }
    }

    private void Set_InteractableAmount(int amount)
    {
        answerIntList.Clear();

        string answerIndexString = "";
        for (int i = 0; i < amount; i++)
        {
            allIolCell[i].Set_Interactable(true);
            answerIntList.Add(i);
            answerIndexString += i != amount - 1 ? $"<size=200%>{i + 1}</size>\t" : $"<size=200%>{i + 1}</size>";
        }
        answerIndexTxt.text = answerIndexString;
        answerCurrentSetTxt.text = "";

        answerIntList = DevTool.Get_ShuffledList(answerIntList);
    }

    #endregion

    #region Set (Selecting List)

    private void Set_SelectingIncludeValue(IOLCellEUIController cellEui)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        int targetCellEUIIndex = allIolCell.IndexOf(cellEui);

        // 처음 선택
        if (needNextSelectEUIIndex == -1 ||
            needNextSelectEUIIndex != targetCellEUIIndex)
        {
            selectingIntList.Clear();
        }

        selectingIntList.Add(targetCellEUIIndex);
        needNextSelectEUIIndex = Get_NextTargetIndex(targetCellEUIIndex);

        Set_SelectingValueTxt(selectingIntList);
    }

    private void Set_SelectingValueTxt(List<int> value)
    {
        List<int> numList = new List<int>();
        for (int i = 0; i < value.Count; i++)
            numList.Add(answerIntList.IndexOf(value[i]));
        
        string answerIndexString = "";
        for (int i = 0; i < answerIntList.Count; i++)
        {
            // On Off
            if (value.Contains(i))
            {
                allIolCell[i].Set_On(0.1f);
                allIolCell[i].Set_NumIndexTxt(true, answerIntList.IndexOf(i));
            }
            else
            {
                allIolCell[i].Set_Off(0.1f);
                allIolCell[i].Set_NumIndexTxt(false);
            }

            // Num
            if (numList.Contains(i))
            {
                answerIndexString += i != answerIntList.Count - 1 ? "<size=150%>^</size>\t" : "<size=150%>^</size>";
            }
            else
            {
                answerIndexString += i != answerIntList.Count - 1 ? "<size=150%> </size>\t" : "<size=150%> </size>";
            }
        }
        answerCurrentSetTxt.text = answerIndexString;
    }


    #endregion

    #region Set (Select)

    public void Set_CellSelect(IOLCellEUIController cellEui)
    {
        if (selectingIolCellEui != cellEui)
        {
            selectingIolCellEui = cellEui;
            selectingSignRT.gameObject.SetActive(true);
            selectingSignRT.anchoredPosition = cellEui.rt.anchoredPosition;
            Play_SelectingRT();
        }
    }

    private void Play_SelectingRT()
    {
        DevTool.Set_KillTween(selectingSignRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(selectingSignRT.DOScale(1.05f, 0.1f));
        seq.Append(selectingSignRT.DOScale(1.0f, 0.1f));

    }

    #endregion

    #region Get (Selecting List)

    private int Get_NextTargetIndex(int firstInt)
    {
        for (int i = 0; i < answerIntList.Count; i++)
        {
            if (answerIntList[i] == firstInt)
            {
                if (i == answerIntList.Count - 1)
                    return answerIntList[0];
                else
                    return answerIntList[i + 1];
            }
        }

        return -1;
    }

    #endregion
}
