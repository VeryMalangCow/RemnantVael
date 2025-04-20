using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Space(10)]
    [Header("=== Selecting")]
    [SerializeField] private RectTransform SelectingSignRT;

    [Space(10)]
    [Header("=== Right")]
    [SerializeField] private CanvasGroup SuccessCG;
    [SerializeField] private CanvasGroup FailureCG;
    [SerializeField] private TMP_Text TryUnlockTxt;
    [SerializeField] private RectTransform RollingRT;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] private Color LockedClr;
    [SerializeField] private Color UnlockedClr;


    #endregion

    #region - Hide

    // Txt
    [HideInInspector] private TMP_Text SuccessTxt;
    [HideInInspector] private TMP_Text FailureTxt;

    // EUI
    [HideInInspector] private List<BoxCellEUIController> AllBoxCellEUI;
    [HideInInspector] private List<BoxConnectionEUIController> AllBoxConnectionEUI;

    // Rate
    [HideInInspector] public int CellAmount = 0;

    // RandomSet
    [HideInInspector] private HashSet<Vector2Int> OnDirBoxCell = new HashSet<Vector2Int>();

    // Dict
    [HideInInspector] private Dictionary<Vector2Int, BoxCellEUIController> BoxCellDirDict = new Dictionary<Vector2Int, BoxCellEUIController>();

    // Selecting
    [HideInInspector] private BoxCellEUIController SelectingBoxCellEUI = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        AllBoxCellEUI = DevTool.Get_ChildList<BoxCellEUIController>(BoxCellParentTF);
        AllBoxConnectionEUI = DevTool.Get_ChildList<BoxConnectionEUIController>(BoxConnectionParentTF);

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

        SuccessTxt = DevTool.Get_ComponentTType(SuccessCG.transform.GetChild(0).gameObject, out TMP_Text sTxt) ? sTxt : null;
        FailureTxt = DevTool.Get_ComponentTType(FailureCG.transform.GetChild(0).gameObject, out TMP_Text fTxt) ? fTxt : null;

        TryUnlockTxt.text = CSVManager.Instance.Get_StaticWord(85);
        SuccessTxt.text = CSVManager.Instance.Get_StaticWord(86);
        FailureTxt.text = CSVManager.Instance.Get_StaticWord(87);
    }

    #endregion

    #region Set

    private void Set_AllStart()
    {
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
        {
            AllBoxCellEUI[i].Set_Active(false);
            AllBoxCellEUI[i].Set_InnerColor(LockedClr);
        }

        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
        {
            AllBoxConnectionEUI[i].Set_Active(false);
            AllBoxConnectionEUI[i].Set_InnerColor(LockedClr);
        }

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

    public void Set_BoxCellSelect(BoxCellEUIController _BoxCellEUI)
    {
        if (SelectingBoxCellEUI != _BoxCellEUI)
        {
            SelectingBoxCellEUI = _BoxCellEUI;
            SelectingSignRT.gameObject.SetActive(true);
            SelectingSignRT.anchoredPosition = _BoxCellEUI.ThisRT.anchoredPosition;
            Play_SelectingRT();
        }
    }

    #endregion

    #region Is & Can

    private bool Can_Success()
    {
        bool success = true;
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
        {
            if (!AllBoxCellEUI[i].Is_CorrectDir())
            {
                success = false;
            }
        }
        return success;
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Set_RandomPuzzleByRate();

        Set_AllStart();
        SetOn_ByConditionToCell();
        SetOn_ByConditionToConnector();

        Debug.Log(Can_Success());
    }

    #endregion

    #region Tween

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
        if (Is_Interact_Roll(-90, 0.1f)) return;
    }

    public void Try_InteractSub()
    {
        if (Is_Interact_Roll(90, 0.1f)) return;
    }

    #endregion

    #region Roll

    private bool Is_Interact_Roll(float _PlusAngle, float _DurTime)
    {
        if (!(CurrentBtn is BoxCellEUIController boxCell) ||
            boxCell != SelectingBoxCellEUI)
            return false;

        SelectingBoxCellEUI.Play_Roll(_PlusAngle, _DurTime);

        Debug.Log(Can_Success());
        return true;
    }

    #endregion
}
