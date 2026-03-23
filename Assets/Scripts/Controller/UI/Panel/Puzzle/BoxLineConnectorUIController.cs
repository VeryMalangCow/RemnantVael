using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxLineConnectorUIController : PuzzleUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Box Line Connector")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform boxCellParentTf;
    [SerializeField] private Transform boxConnectionParentTf;
    [SerializeField] private Transform innerParentTf;

    [Space(10)]
    [Header("=== Selecting")]
    [SerializeField] private RectTransform selectingSignRt;

    [Space(10)]
    [Header("=== Ready KeyAnno")]
    [SerializeField] private Image leftRollInputImg;
    [SerializeField] private Image rightRollInputImg;

    #endregion

    #region - Hide

    // EUI
    [HideInInspector] private List<BoxCellEUIController> allBoxCellEui;
    [HideInInspector] private List<BoxConnectionEUIController> allBoxConnectionEui;

    // Value
    [HideInInspector] private int cellAmount = 0;

    // RandomSet
    [HideInInspector] private HashSet<Vector2Int> onDirBoxCell = new HashSet<Vector2Int>();

    // Dict
    [HideInInspector] private Dictionary<Vector2Int, BoxCellEUIController> boxCellDirDict = new Dictionary<Vector2Int, BoxCellEUIController>();

    // Selecting
    [HideInInspector] private BoxCellEUIController selectingBoxCellEui = null;

    // Inner
    [HideInInspector] private List<Image> innerList;


    #endregion

    #endregion

    #region Offset

    public override void Offset_FirstValue(PrisonController prison)
    {
        base.Offset_FirstValue(prison);

        cellAmount = 4 + prison.rating;
        currentCountdown = baseCountdown - prison.rating;
    }

    public override void Offset()
    {
        base.Offset();

        allBoxCellEui = DevTool.Get_ChildList<BoxCellEUIController>(boxCellParentTf);
        allBoxConnectionEui = DevTool.Get_ChildList<BoxConnectionEUIController>(boxConnectionParentTf);
        innerList = DevTool.Get_ChildList<Image>(innerParentTf);

        for (int i = 0; i < allBoxCellEui.Count; i++)
        {
            allBoxCellEui[i].Offset();
            allBoxCellEui[i].ownerUIController = this;
            allBoxCellEui[i].ownerPuzzleUIController = this;
            boxCellDirDict.Add(allBoxCellEui[i].pos, allBoxCellEui[i]);
        }

        for (int i = 0; i < allBoxConnectionEui.Count; i++)
        {
            allBoxConnectionEui[i].Offset();
        }

        rightRollInputImg.sprite = ResourceManager.instance.mlbSprite;
        rightRollInputImg.SetNativeSize();
        leftRollInputImg.sprite = ResourceManager.instance.mrbSprite;
        leftRollInputImg.SetNativeSize();
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        readyPanelEui.Set_RuleDesc(ResourceManager.instance.Get_StaticDesc(33));

        Set_AllDefault();
        Set_RandomPuzzleByRate();
        SetOn_ByConditionToCell();
        SetOn_ByConditionToConnector();

        // Check First
        Check_CorrectLineSet();
    }

    protected override void Set_AllComplete()
    {
        base.Set_AllComplete();

        Set_AllInnerColor(ResourceManager.instance.unlockedClr);

        // Selecting
        selectingBoxCellEui = null;
        selectingSignRt.gameObject.SetActive(false);
    }

    #endregion

    #region Set (Unique)

    private void Set_AllInnerColor(Color clr)
    {
        // Cell
        for (int i = 0; i < allBoxCellEui.Count; i++)
            allBoxCellEui[i].Set_InnerColor(clr);
        
        // Connection
        for (int i = 0; i < allBoxConnectionEui.Count; i++)
            allBoxConnectionEui[i].Set_InnerColor(clr);
        
        // Inner 
        for (int i = 0; i < innerList.Count; i++)
            DevTool.Set_Color(clr, innerList[i]);
    }

    private void Set_AllDefault()
    {
        for (int i = 0; i < allBoxCellEui.Count; i++)
            allBoxCellEui[i].Set_Active(false);
        for (int i = 0; i < allBoxConnectionEui.Count; i++)
            allBoxConnectionEui[i].Set_Active(false);

        Set_AllInnerColor(ResourceManager.instance.lockedClr);

        // Selecting
        selectingBoxCellEui = null;
        selectingSignRt.gameObject.SetActive(false);

    }

    private void Set_RandomPuzzleByRate()
    {
        if (onDirBoxCell != null)
            onDirBoxCell.Clear();
        
        // 처음은 랜덤으로 설정
        Vector2Int firstBoxCell = allBoxCellEui[Random.Range(0, allBoxCellEui.Count)].pos;
        onDirBoxCell.Add(firstBoxCell);

        while(true)
        {
            Vector2Int randomBoxCell = allBoxCellEui[Random.Range(0, allBoxCellEui.Count)].pos;
            if (!onDirBoxCell.Contains(randomBoxCell) &&
                DevTool.Get_RoundVec(onDirBoxCell).Contains(randomBoxCell))
                onDirBoxCell.Add(randomBoxCell);
            else
                continue;
            

            // 일정 수치를 채우면 종료
            if (onDirBoxCell.Count >= cellAmount || onDirBoxCell.Count >= allBoxCellEui.Count)
                break;
        }
    }

    private void SetOn_ByConditionToCell()
    {
        foreach (Vector2Int activeBoxCellVec in onDirBoxCell)
        {
            // 셀을 활성화
            boxCellDirDict[activeBoxCellVec].Set_Active(true);

            // 셀 내부의 방향 이미지 활성화
            List<Vector2Int> staticRoundVec = DevTool.Get_RoundVec(Vector2Int.zero);

            for (int i = 0; i < staticRoundVec.Count; i++)
                if (onDirBoxCell.Contains(activeBoxCellVec + staticRoundVec[i]))
                    boxCellDirDict[activeBoxCellVec].Get_CorrectDirGO(staticRoundVec[i]).gameObject.SetActive(true);
                else
                    boxCellDirDict[activeBoxCellVec].Get_CorrectDirGO(staticRoundVec[i]).gameObject.SetActive(false);

            // 랜덤한 방향으로 회전 세팅
            boxCellDirDict[activeBoxCellVec].Set_RandomAngle();
        }
    }

    private void SetOn_ByConditionToConnector()
    {
        for (int i = 0; i < allBoxConnectionEui.Count; i++)
        {
            allBoxConnectionEui[i].Set_ActiveByCondition(onDirBoxCell);
        }
    }

    #endregion

    #region Select

    public void Set_BoxCellSelect(BoxCellEUIController boxCellEui)
    {
        if (selectingBoxCellEui != boxCellEui && isInteractable)
        {
            selectingBoxCellEui = boxCellEui;
            selectingSignRt.gameObject.SetActive(true);
            selectingSignRt.anchoredPosition = boxCellEui.rt.anchoredPosition;
            Play_SelectingRT();
        }
    }
    private void Play_SelectingRT()
    {
        DevTool.Set_KillTween(selectingSignRt);

        Sequence seq = DOTween.Sequence();
        seq.Append(selectingSignRt.DOScale(1.05f, 0.1f));
        seq.Append(selectingSignRt.DOScale(1.0f, 0.1f));

    }

    #endregion

    #region Can

    public override bool Can_Success()
    {
        bool success = true;

        foreach (Vector2Int vec2Int in onDirBoxCell)
        {
            if (!boxCellDirDict[vec2Int].Is_CorrectDir())
                success = false;
        }
        return success;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Roll(-90, 0.1f)) return;
    }

    public void Try_InteractSub()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Roll(90, 0.1f)) return;
    }

    public void Try_InteractUnlock()
    {
        if (Is_Interact_TryUnlock()) return;
    }

    #endregion

    #region Intetact (Roll)

    private bool Is_Interact_Roll(float plusAngle, float durTime)
    {
        if (!(currentBtn is BoxCellEUIController boxCell) ||
            boxCell != selectingBoxCellEui ||
            !isInteractable)
            return false;

        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        selectingBoxCellEui.Play_Roll(plusAngle, durTime);

        return true;
    }

    #endregion
}
