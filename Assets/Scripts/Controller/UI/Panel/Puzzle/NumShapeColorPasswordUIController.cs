using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumShapeColorPasswordUIController : PuzzleUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> NSC")]

    [Space(10)]
    [Header("=== RT")]
    [SerializeField] private RectTransform AllNSCPanelEUIParentRT;
    [SerializeField] private RectTransform SelectingSignRT;

    [Space(10)]
    [Header("=== Ready KeyAnno")]
    [SerializeField] private Image DownRollInputImg;
    [SerializeField] private Image UpRollInputImg;

    #endregion

    #region - Hide

    // Value
    [HideInInspector] private int UnlockedAmount = 0;
    [HideInInspector] private int LockedAmount = 0;

    // EUI
    [HideInInspector] private List<NSCPanelEUIController> AllNSCPanelEUI;
    [HideInInspector] private List<NSCRollCellEUIController> AllNSCRollCellEUI = new List<NSCRollCellEUIController>();
    [HideInInspector] private List<int> LockedRollCellEUIIndexList;

    // Input
    [HideInInspector] private NSCRollCellEUIController SelectingRollCellEUI;

    #endregion

    #endregion

    #region Offset

    public override void Offset_FirstValue(PrisonController _Prison)
    {
        base.Offset_FirstValue(_Prison);

        Debug.Log(_Prison.Rating);
        UnlockedAmount = 4 + _Prison.Rating;
        CurrentCountdown = BaseCountdown - _Prison.Rating;
    }


    public override void Offset()
    {
        base.Offset();

        AllNSCPanelEUI = DevTool.Get_ChildList<NSCPanelEUIController>(AllNSCPanelEUIParentRT);

        for (int i = 0; i < AllNSCPanelEUI.Count; i++)
        {
            AllNSCPanelEUI[i].OwnerUIController = this;
            AllNSCPanelEUI[i].Offset();

            AllNSCRollCellEUI.AddRange(AllNSCPanelEUI[i].AllRollEUI);
        }

        DownRollInputImg.sprite = ResourceManager.Instance.MLBSprite;
        DownRollInputImg.SetNativeSize();
        UpRollInputImg.sprite = ResourceManager.Instance.MRBSprite;
        UpRollInputImg.SetNativeSize();
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        ReadyPanelEUI.Set_RuleDesc(ResourceManager.Instance.Get_StaticDesc(34));

        Set_AllNSCPanelEUI_DefaultAndRandom();
        Set_LockByRating();

        // First Check
        Check_CorrectLineSet();
    }


    protected override void Set_AllComplete()
    {
        base.Set_AllComplete();

        for (int i = 0; i < AllNSCPanelEUI.Count; i++)
        {
            AllNSCPanelEUI[i].Set_InnerColor(ResourceManager.Instance.UnlockedClr);
        }
    }

    #endregion

    #region Set (Unique)

    private void Set_AllNSCPanelEUI_DefaultAndRandom()
    {
        for (int i = 0; i < AllNSCPanelEUI.Count; i++)
        {
            AllNSCPanelEUI[i].Set_RollValueRandom();
            AllNSCPanelEUI[i].Set_RandomAnswer();

            AllNSCPanelEUI[i].Set_InnerColor(ResourceManager.Instance.LockedClr);
        }
    }

    private void Set_LockByRating()
    {
        LockedAmount = AllNSCRollCellEUI.Count - UnlockedAmount;
        LockedRollCellEUIIndexList = new List<int>();
        while (true)
        {
            int randomIndex = Random.Range(0, AllNSCRollCellEUI.Count);

            if (!LockedRollCellEUIIndexList.Contains(randomIndex))
                LockedRollCellEUIIndexList.Add(randomIndex);

            if (LockedRollCellEUIIndexList.Count >= LockedAmount)
                break;
        }

        for (int i = 0; i < LockedRollCellEUIIndexList.Count; i++)
             AllNSCRollCellEUI[LockedRollCellEUIIndexList[i]].Set_ImgByAnswerAndLock();
        
    }

    #endregion

    #region Set (Select)

    public void Set_RollCellSelect(NSCRollCellEUIController _RollCellEUI)
    {
        if (SelectingRollCellEUI != _RollCellEUI && IsInteractable)
        {
            SelectingRollCellEUI = _RollCellEUI;
            SelectingSignRT.gameObject.SetActive(true);
            SelectingSignRT.anchoredPosition = new Vector2(_RollCellEUI.ThisRT.anchoredPosition.x, _RollCellEUI.OwnerNSCPanelEUIController.ThisRT.anchoredPosition.y); 
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

    #region Interact

    public void Try_Interact()
    {
        InputManager.Instance.Play_MousePointerClick();

        if (Is_Interact_RollForDown()) return;
    }

    public void Try_InteractSub()
    {
        InputManager.Instance.Play_MousePointerClick();

        if (Is_Interact_RollForUp()) return;
    }

    public void Try_InteractUnlock()
    {
        if (Is_Interact_TryUnlock()) return;
    }

    #endregion

    #region Interact (Roll)

    private bool Is_Interact_RollForDown()
    {
        return Is_Interact_Roll(true);
    }

    private bool Is_Interact_RollForUp()
    {
        return Is_Interact_Roll(false);
    }

    private bool Is_Interact_Roll(bool _RollDown)
    {
        if (!(CurrentBtn is NSCRollCellEUIController rollCellEUI) ||
            rollCellEUI != SelectingRollCellEUI || 
            !IsInteractable)
            return false;

        SoundManager.Instance.Play_2D_SFX_UI("Click_01");

        if (_RollDown)
            SelectingRollCellEUI.Play_RollForDown();
        else
            SelectingRollCellEUI.Play_RollForUp();

        return true;
    }

    #endregion

    #region Success

    public override bool Can_Success()
    {
        for (int i = 0; i < AllNSCPanelEUI.Count; i++)
            if (!AllNSCPanelEUI[i].Is_Answer())
                return false;
            
        return true;
    }

    #endregion
}
