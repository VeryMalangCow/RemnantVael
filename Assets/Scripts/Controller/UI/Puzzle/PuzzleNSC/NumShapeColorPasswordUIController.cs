using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    [SerializeField] private RectTransform allNscPanelEuiParentRt;
    [SerializeField] private RectTransform selectingSignRt;

    [Space(10)]
    [Header("=== Ready KeyAnno")]
    [SerializeField] private Image downRollInputImg;
    [SerializeField] private Image upRollInputImg;

    #endregion

    #region - Hide

    // Value
    [HideInInspector] private int unlockedAmount = 0;
    [HideInInspector] private int lockedAmount = 0;

    // EUI
    [HideInInspector] private List<NSCPanelEUIController> allNscPanelEui;
    [HideInInspector] private List<NSCRollCellEUIController> allNscRollCellEui = new List<NSCRollCellEUIController>();
    [HideInInspector] private List<int> lockedRollCellEuiIndexList;

    // Input
    [HideInInspector] private NSCRollCellEUIController selectingRollCellEui;

    #endregion

    #endregion

    #region Init

    public override IEnumerator InitAsync()
    {
        yield return base.InitAsync();

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        allNscPanelEui = DevTool.Get_ChildList<NSCPanelEUIController>(allNscPanelEuiParentRt);

        for (int i = 0; i < allNscPanelEui.Count; i++)
        {
            allNscPanelEui[i].ownerUIController = this;
            allNscPanelEui[i].Offset();

            allNscRollCellEui.AddRange(allNscPanelEui[i].allRollEui);
        }

        downRollInputImg.sprite = ResourceManager.instance.mlbSprite;
        downRollInputImg.SetNativeSize();
        upRollInputImg.sprite = ResourceManager.instance.mrbSprite;
        upRollInputImg.SetNativeSize();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=FFFF7F>NumShapeColorPassword Puzzle</color> : DataSet : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms");
#endif
        yield return null;
    }

    #endregion

    #region Prison

    public override void SetPrison(PrisonController prison)
    {
        base.SetPrison(prison);

        unlockedAmount = 4 + prison.rating;
        currentCountdown = baseCountdown - prison.rating;
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        readyPanelEui.Set_RuleDesc(ResourceManager.instance.Get_StaticDesc(34));

        Set_AllNSCPanelEUI_DefaultAndRandom();
        Set_LockByRating();

        // First Check
        Check_CorrectLineSet();
    }


    protected override void Set_AllComplete()
    {
        base.Set_AllComplete();

        for (int i = 0; i < allNscPanelEui.Count; i++)
        {
            allNscPanelEui[i].Set_InnerColor(ResourceManager.instance.unlockedClr);
        }
    }

    #endregion

    #region Set (Unique)

    private void Set_AllNSCPanelEUI_DefaultAndRandom()
    {
        for (int i = 0; i < allNscPanelEui.Count; i++)
        {
            allNscPanelEui[i].Set_RollValueRandom();
            allNscPanelEui[i].Set_RandomAnswer();

            allNscPanelEui[i].Set_InnerColor(ResourceManager.instance.lockedClr);
        }
    }

    private void Set_LockByRating()
    {
        lockedAmount = allNscRollCellEui.Count - unlockedAmount;
        lockedRollCellEuiIndexList = new List<int>();
        while (true)
        {
            int randomIndex = Random.Range(0, allNscRollCellEui.Count);

            if (!lockedRollCellEuiIndexList.Contains(randomIndex))
                lockedRollCellEuiIndexList.Add(randomIndex);

            if (lockedRollCellEuiIndexList.Count >= lockedAmount)
                break;
        }

        for (int i = 0; i < lockedRollCellEuiIndexList.Count; i++)
             allNscRollCellEui[lockedRollCellEuiIndexList[i]].Set_ImgByAnswerAndLock();
        
    }

    #endregion

    #region Set (Select)

    public void Set_RollCellSelect(NSCRollCellEUIController rollCellEui)
    {
        if (selectingRollCellEui != rollCellEui && isInteractable)
        {
            selectingRollCellEui = rollCellEui;
            selectingSignRt.gameObject.SetActive(true);
            selectingSignRt.anchoredPosition = new Vector2(rollCellEui.rt.anchoredPosition.x, rollCellEui.ownerNscPanelEuiController.rt.anchoredPosition.y); 
            Play_SelectingRT();
        }
    }

    private void Play_SelectingRT()
    {
        DevTool.SetKillTween(selectingSignRt);

        Sequence seq = DOTween.Sequence();
        seq.Append(selectingSignRt.DOScale(1.05f, 0.1f));
        seq.Append(selectingSignRt.DOScale(1.0f, 0.1f));

    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_RollForDown()) return;
    }

    public void Try_InteractSub()
    {
        InputManager.instance.Play_MousePointerClick();

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

    private bool Is_Interact_Roll(bool rollDown)
    {
        if (!(currentBtn is NSCRollCellEUIController rollCellEUI) ||
            rollCellEUI != selectingRollCellEui || 
            !isInteractable)
            return false;

        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        if (rollDown)
            selectingRollCellEui.Play_RollForDown();
        else
            selectingRollCellEui.Play_RollForUp();

        return true;
    }

    #endregion

    #region Success

    public override bool Can_Success()
    {
        for (int i = 0; i < allNscPanelEui.Count; i++)
            if (!allNscPanelEui[i].Is_Answer())
                return false;
            
        return true;
    }

    #endregion
}
