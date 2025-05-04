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
    [SerializeField] private Transform BoxCellParentTF;
    [SerializeField] private Transform BoxConnectionParentTF;
    [SerializeField] private Transform InnerParentTF;

    [Space(10)]
    [Header("=== Selecting")]
    [SerializeField] private RectTransform SelectingSignRT;

    #endregion

    #region - Hide

    // EUI
    [HideInInspector] private List<BoxCellEUIController> AllBoxCellEUI;
    [HideInInspector] private List<BoxConnectionEUIController> AllBoxConnectionEUI;

    // Value
    [HideInInspector] private int CellAmount = 0;

    // RandomSet
    [HideInInspector] private HashSet<Vector2Int> OnDirBoxCell = new HashSet<Vector2Int>();

    // Dict
    [HideInInspector] private Dictionary<Vector2Int, BoxCellEUIController> BoxCellDirDict = new Dictionary<Vector2Int, BoxCellEUIController>();

    // Selecting
    [HideInInspector] private BoxCellEUIController SelectingBoxCellEUI = null;

    // Inner
    [HideInInspector] private List<Image> InnerList;


    #endregion

    #endregion

    #region Offset

    public override void Offset_FirstValue(PrisonController _Prison)
    {
        base.Offset_FirstValue(_Prison);

        CellAmount = 4 + _Prison.Rating;
        CurrentCountdown = BaseCountdown - _Prison.Rating;
    }

    public override void Offset()
    {
        base.Offset();

        AllBoxCellEUI = DevTool.Get_ChildList<BoxCellEUIController>(BoxCellParentTF);
        AllBoxConnectionEUI = DevTool.Get_ChildList<BoxConnectionEUIController>(BoxConnectionParentTF);
        InnerList = DevTool.Get_ChildList<Image>(InnerParentTF);

        for (int i = 0; i < AllBoxCellEUI.Count; i++)
        {
            AllBoxCellEUI[i].Offset();
            AllBoxCellEUI[i].OwnerUIController = this;
            AllBoxCellEUI[i].OwnerPuzzleUIController = this;
            BoxCellDirDict.Add(AllBoxCellEUI[i].ThisPos, AllBoxCellEUI[i]);
        }

        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
        {
            AllBoxConnectionEUI[i].Offset();
        }
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

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

        Set_AllInnerColor(UnlockedClr);

        // Selecting
        SelectingBoxCellEUI = null;
        SelectingSignRT.gameObject.SetActive(false);
    }

    #endregion

    #region Set (Unique)

    private void Set_AllInnerColor(Color _Clr)
    {
        // Cell
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
            AllBoxCellEUI[i].Set_InnerColor(_Clr);
        
        // Connection
        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
            AllBoxConnectionEUI[i].Set_InnerColor(_Clr);
        
        // Inner 
        for (int i = 0; i < InnerList.Count; i++)
            DevTool.Set_Color(_Clr, InnerList[i]);
    }

    private void Set_AllDefault()
    {
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
            AllBoxCellEUI[i].Set_Active(false);
        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
            AllBoxConnectionEUI[i].Set_Active(false);

        Set_AllInnerColor(LockedClr);

        // Selecting
        SelectingBoxCellEUI = null;
        SelectingSignRT.gameObject.SetActive(false);

    }

    private void Set_RandomPuzzleByRate()
    {
        if (OnDirBoxCell != null)
            OnDirBoxCell.Clear();
        
        // 처음은 랜덤으로 설정
        Vector2Int firstBoxCell = AllBoxCellEUI[Random.Range(0, AllBoxCellEUI.Count)].ThisPos;
        OnDirBoxCell.Add(firstBoxCell);

        while(true)
        {
            Vector2Int randomBoxCell = AllBoxCellEUI[Random.Range(0, AllBoxCellEUI.Count)].ThisPos;
            if (!OnDirBoxCell.Contains(randomBoxCell) &&
                DevTool.Get_RoundVec(OnDirBoxCell).Contains(randomBoxCell))
                OnDirBoxCell.Add(randomBoxCell);
            else
                continue;
            

            // 일정 수치를 채우면 종료
            if (OnDirBoxCell.Count >= CellAmount || OnDirBoxCell.Count >= AllBoxCellEUI.Count)
                break;
        }
    }

    private void SetOn_ByConditionToCell()
    {
        foreach (Vector2Int activeBoxCellVec in OnDirBoxCell)
        {
            // 셀을 활성화
            BoxCellDirDict[activeBoxCellVec].Set_Active(true);

            // 셀 내부의 방향 이미지 활성화
            List<Vector2Int> staticRoundVec = DevTool.Get_RoundVec(Vector2Int.zero);

            for (int i = 0; i < staticRoundVec.Count; i++)
                if (OnDirBoxCell.Contains(activeBoxCellVec + staticRoundVec[i]))
                    BoxCellDirDict[activeBoxCellVec].Get_CorrectDirGO(staticRoundVec[i]).gameObject.SetActive(true);
                else
                    BoxCellDirDict[activeBoxCellVec].Get_CorrectDirGO(staticRoundVec[i]).gameObject.SetActive(false);

            // 랜덤한 방향으로 회전 세팅
            BoxCellDirDict[activeBoxCellVec].Set_RandomAngle();
        }
    }

    private void SetOn_ByConditionToConnector()
    {
        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
        {
            AllBoxConnectionEUI[i].Set_ActiveByCondition(OnDirBoxCell);
        }
    }

    #endregion

    #region Select

    public void Set_BoxCellSelect(BoxCellEUIController _BoxCellEUI)
    {
        if (SelectingBoxCellEUI != _BoxCellEUI && IsInteractable)
        {
            SelectingBoxCellEUI = _BoxCellEUI;
            SelectingSignRT.gameObject.SetActive(true);
            SelectingSignRT.anchoredPosition = _BoxCellEUI.ThisRT.anchoredPosition;
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

    #region Can

    public override bool Can_Success()
    {
        bool success = true;

        foreach (Vector2Int vec2Int in OnDirBoxCell)
        {
            if (!BoxCellDirDict[vec2Int].Is_CorrectDir())
                success = false;
        }
        return success;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (Is_Interact_Roll(-90, 0.1f)) return;
    }

    public void Try_InteractSub()
    {
        if (Is_Interact_Roll(90, 0.1f)) return;
    }

    public void Try_InteractUnlock()
    {
        if (Is_Interact_TryUnlock()) return;
    }

    #endregion

    #region Intetact (Roll)

    private bool Is_Interact_Roll(float _PlusAngle, float _DurTime)
    {
        if (!(CurrentBtn is BoxCellEUIController boxCell) ||
            boxCell != SelectingBoxCellEUI ||
            !IsInteractable)
            return false;

        SelectingBoxCellEUI.Play_Roll(_PlusAngle, _DurTime);

        return true;
    }

    #endregion
}
