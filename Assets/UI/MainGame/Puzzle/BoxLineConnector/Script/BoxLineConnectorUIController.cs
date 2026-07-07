using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BoxLineConnectorUIController : PuzzleUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Box Line Connector")]

    [Space(10)]
    [Header("=== Box Cell")]
    [SerializeField] private BoxCellEUIController boxCellEuiPrefab;
    [SerializeField] private Transform boxCellParentTf;
    [SerializeField] private Vector2 boxInterval;
    private BoxCellEUIController[] allBoxCellEui;

    [Space(10)]
    [Header("=== Box Connector")]
    [SerializeField] private BoxConnectionEUIController boxConnectorEuiPrefab;
    [SerializeField] private Transform boxConnectionParentTf;
    private BoxConnectionEUIController[] allBoxConnectionEui;

    [Space(10)]
    [Header("=== Selecting")]
    [SerializeField] private RectTransform selectingSignRt;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] clrImgs;

    // EUI

    // Value
    [HideInInspector] private int cellAmount = 0;

    // RandomSet
    [HideInInspector] private HashSet<Vector2Int> onDirBoxCell = new HashSet<Vector2Int>();

    // Dict
    [HideInInspector] private Dictionary<Vector2Int, BoxCellEUIController> boxCellDirDict = new Dictionary<Vector2Int, BoxCellEUIController>();

    // Selecting
    [HideInInspector] private BoxCellEUIController selectingBoxCellEui = null;

    #endregion

    #region Init

    public override IEnumerator InitAsync()
    {
        yield return base.InitAsync();

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        string ms = "";
#endif
        allBoxCellEui = new BoxCellEUIController[9];
        Vector2Int gridPos;
        Vector2 pos;
        for (int i = 0; i < allBoxCellEui.Length; i++)
        {
            allBoxCellEui[i] = Instantiate(boxCellEuiPrefab, boxCellParentTf);
            allBoxCellEui[i].Offset();
            gridPos = new Vector2Int((i % 3) - 1, (i / 3) - 1);
            pos = new Vector2(gridPos.x * boxInterval.x, gridPos.y * boxInterval.y);
            allBoxCellEui[i].Init(this, gridPos, pos);

            boxCellDirDict.Add(allBoxCellEui[i].pos, allBoxCellEui[i]);

            if (i % 3 == 2)
            {
#if UNITY_EDITOR
                sw.Stop();
                ms += $"{(float)sw.Elapsed.TotalMilliseconds:F2} / ";
#endif
                yield return null;
#if UNITY_EDITOR
                sw.Restart();
#endif
            }
        }
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Box Cell</color> : <color=red>{ms:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        allBoxConnectionEui = new BoxConnectionEUIController[12];

        int k = 0;
        Vector2Int vecA, vecB;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vecA = new Vector2Int(j - 1, i - 1);
                if (j % 3 != 2) // 가장 오른쪽이 아니라면
                {
                    allBoxConnectionEui[k] = Instantiate(boxConnectorEuiPrefab, boxConnectionParentTf);
                    allBoxConnectionEui[k].Offset();
                    vecB = new Vector2Int(vecA.x + 1, vecA.y);
                    Vector2 vec = new Vector2((vecA.x + vecB.x) * 0.5f, (vecA.y + vecB.y) * 0.5f);
                    vec *= boxInterval;
                    allBoxConnectionEui[k].Init(vecA, vecB, vec, true);
                    k++;
                }
                if (i != 2)
                {
                    allBoxConnectionEui[k] = Instantiate(boxConnectorEuiPrefab, boxConnectionParentTf);
                    allBoxConnectionEui[k].Offset();
                    vecB = new Vector2Int(vecA.x, vecA.y + 1);
                    Vector2 vec = new Vector2((vecA.x + vecB.x) * 0.5f, (vecA.y + vecB.y) * 0.5f);
                    vec *= boxInterval;
                    allBoxConnectionEui[k].Init(vecA, vecB, vec, false);
                    k++;
                }
            }
        }

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Box Connector</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    #endregion

    #region Prison

    public override void SetPrison(PrisonController prison)
    {
        base.SetPrison(prison);

        cellAmount = 4 + prison.rating;
        currentCountdown = baseCountdown - prison.rating;
    }

    #endregion

    #region Set (Start & Complete)

    protected override void Set_AllStart()
    {
        base.Set_AllStart();

        readyPanelEui.Set_RuleDesc(StaticResourceManager.instance.staticDescs.GetLanguage(33));

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

        Set_AllInnerColor(PrisonBuild.unlockedClr);

        // Selecting
        selectingBoxCellEui = null;
        selectingSignRt.gameObject.SetActive(false);
    }

    #endregion

    #region Set (Unique)

    private void Set_AllInnerColor(Color clr)
    {
        DevTool.SetColorImgs(clr, clrImgs);

        // Cell
        for (int i = 0; i < allBoxCellEui.Length; i++)
            allBoxCellEui[i].Set_InnerColor(clr);
        
        // Connection
        for (int i = 0; i < allBoxConnectionEui.Length; i++)
            allBoxConnectionEui[i].SetInnerColor(clr);
    }

    private void Set_AllDefault()
    {
        for (int i = 0; i < allBoxCellEui.Length; i++)
            allBoxCellEui[i].Set_Active(false);
        for (int i = 0; i < allBoxConnectionEui.Length; i++)
            allBoxConnectionEui[i].SetActive(false);

        Set_AllInnerColor(StaticResourceManager.instance.BuildReso.prisonPrefab.lockedClr);

        // Selecting
        selectingBoxCellEui = null;
        selectingSignRt.gameObject.SetActive(false);

    }

    private void Set_RandomPuzzleByRate()
    {
        if (onDirBoxCell != null)
            onDirBoxCell.Clear();
        
        // 처음은 랜덤으로 설정
        Vector2Int firstBoxCell = allBoxCellEui[Random.Range(0, allBoxCellEui.Length)].pos;
        onDirBoxCell.Add(firstBoxCell);

        while(true)
        {
            Vector2Int randomBoxCell = allBoxCellEui[Random.Range(0, allBoxCellEui.Length)].pos;
            if (!onDirBoxCell.Contains(randomBoxCell) &&
                DevTool.Get_RoundVec(onDirBoxCell).Contains(randomBoxCell))
                onDirBoxCell.Add(randomBoxCell);
            else
                continue;
            

            // 일정 수치를 채우면 종료
            if (onDirBoxCell.Count >= cellAmount || onDirBoxCell.Count >= allBoxCellEui.Length)
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
        for (int i = 0; i < allBoxConnectionEui.Length; i++)
        {
            allBoxConnectionEui[i].SetActiveByCondition(onDirBoxCell);
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
        DevTool.SetKillTween(selectingSignRt);

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

        SoundManager.instance.PlayUiSfx("Click01");

        selectingBoxCellEui.Play_Roll(plusAngle, durTime);

        return true;
    }

    #endregion
}
