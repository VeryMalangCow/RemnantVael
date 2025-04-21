using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Space(10)]
    [Header("=== Selecting")]
    [SerializeField] private RectTransform SelectingSignRT;

    [Space(10)]
    [Header("=== Right")]
    [SerializeField] private CanvasGroup SuccessCG;
    [SerializeField] private CanvasGroup FailureCG;
    [SerializeField] private TMP_Text TryUnlockTxt;
    [SerializeField] private TMP_Text InputTxt;
    [SerializeField] private Image InputImg;
    [SerializeField] private RectTransform RollingRT;

    [Space(10)]
    [Header("=== Left")]
    [SerializeField] private TMP_Text UnlockAnnoTxt;
    [SerializeField] private TMP_Text SuccessAnnoTxt;
    [SerializeField] private TMP_Text FailureAnnoTxt;
    [SerializeField] private TMP_Text CountdownTxt;
    [SerializeField] private TMP_Text CountdownPaneltyTxt;

    [Space(10)]
    [Header("=== Ready Panel")]
    [SerializeField] private CanvasGroup ReadyCG;
    [SerializeField] private TMP_Text ReadyAnnoTxt;
    [SerializeField] private TMP_Text ReadyTimeLimitTxt;
    [SerializeField] private TMP_Text ReadyKeyAnnoTxt;
    [SerializeField] private Image ReadyInputAnnoImg;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] private Color LockedClr;
    [SerializeField] private Color UnlockedClr;


    #endregion

    #region - Hide

    // RT
    [HideInInspector] private RectTransform SuccessAnnoRT;
    [HideInInspector] private RectTransform FailureAnnoRT;
    [HideInInspector] private RectTransform ReadyTimeLimitAnnoRT;
    [HideInInspector] private RectTransform ReadyKeyAnnoRT;
    [HideInInspector] private RectTransform CountdownPaneltyRT;

    // Txt
    [HideInInspector] private TMP_Text SuccessTxt;
    [HideInInspector] private TMP_Text FailureTxt;

    // EUI
    [HideInInspector] private List<BoxCellEUIController> AllBoxCellEUI;
    [HideInInspector] private List<BoxConnectionEUIController> AllBoxConnectionEUI;

    // Value
    [HideInInspector] private int CellAmount = 0;
    [HideInInspector] private float CurrentCountdown = 0;

    // RandomSet
    [HideInInspector] private HashSet<Vector2Int> OnDirBoxCell = new HashSet<Vector2Int>();

    // Dict
    [HideInInspector] private Dictionary<Vector2Int, BoxCellEUIController> BoxCellDirDict = new Dictionary<Vector2Int, BoxCellEUIController>();

    // Selecting
    [HideInInspector] private BoxCellEUIController SelectingBoxCellEUI = null;

    // Success
    [HideInInspector] private bool CanSuccess = false;
    [HideInInspector] private bool IsInteractable = false;
    [HideInInspector] private bool IsReady = false;
    [HideInInspector] private bool IsStart = false;

    // Static Data
    [HideInInspector] private string SecondString = "<size=50%>s</size>";
    [HideInInspector] private CoupleData<Vector2> ReadyTimeLimitAnnoRTPos;
    [HideInInspector] private CoupleData<Vector2> ReadykeyAnnoRTPos;

    #endregion

    #endregion

    #region Offset

    public void Offset_FirstValue(int _Rate)
    {
        CellAmount = _Rate + 4;
        CurrentCountdown = 12f - _Rate;
    }

    public override void Offset()
    {
        base.Offset();

        UnlockedClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);

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
        InputTxt.text = CSVManager.Instance.Get_StaticWord(88);
        SuccessTxt.text = CSVManager.Instance.Get_StaticWord(86);
        FailureTxt.text = CSVManager.Instance.Get_StaticWord(87);

        FailureAnnoRT = DevTool.Get_ComponentTType(FailureAnnoTxt.gameObject, out RectTransform fRt) ? fRt : null;
        SuccessAnnoRT = DevTool.Get_ComponentTType(SuccessAnnoTxt.gameObject, out RectTransform sRt) ? sRt : null;

        DevTool.Set_Color(LockedClr, FailureTxt);
        DevTool.Set_Color(LockedClr, FailureAnnoTxt);

        DevTool.Set_Color(UnlockedClr, SuccessTxt);
        DevTool.Set_Color(UnlockedClr, SuccessAnnoTxt);

        DevTool.Set_Color(LockedClr, CountdownPaneltyTxt);

        InputImg.sprite = UnitManager.Instance.SpaceBarSprite;
        InputImg.SetNativeSize();

        ReadyTimeLimitAnnoRT = DevTool.Get_ComponentTType(ReadyTimeLimitTxt.gameObject, out RectTransform readyTimeRt) ? readyTimeRt : null;
        ReadyTimeLimitAnnoRTPos = new CoupleData<Vector2>(readyTimeRt.anchoredPosition, new Vector2(-1128f, -152f));

        ReadyKeyAnnoRT = DevTool.Get_ComponentTType(ReadyKeyAnnoTxt.gameObject, out RectTransform readyKeyRt) ? readyKeyRt : null;
        ReadykeyAnnoRTPos = new CoupleData<Vector2>(ReadyKeyAnnoRT.anchoredPosition, new Vector2(1128f, -580f));

        CountdownPaneltyRT = DevTool.Get_ComponentTType(CountdownPaneltyTxt.gameObject, out RectTransform paneltyTimeRt) ? paneltyTimeRt : null;
    }

    #endregion

    #region Framework

    private void Update()
    {
        Caculate_CountDown(Time.deltaTime);
    }

    #endregion

    #region Set

    private void Set_AllStart()
    {
        // txt
        UnlockAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(28).Replace("\\n", "\n");
        SuccessAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(29).Replace("\\n", "\n");
        FailureAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(30).Replace("\\n", "\n");

        // Cell
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
        {
            AllBoxCellEUI[i].Set_Active(false);
            AllBoxCellEUI[i].Set_InnerColor(LockedClr);
        }

        // Connection
        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
        {
            AllBoxConnectionEUI[i].Set_Active(false);
            AllBoxConnectionEUI[i].Set_InnerColor(LockedClr);
        }

        // Selecting
        SelectingBoxCellEUI = null;
        SelectingSignRT.gameObject.SetActive(false);

        // Right
        Play_LineSetChange();

        // Left
        Set_CountdownTxt();
        DevTool.Set_Color(LockedClr, CountdownTxt);
        DevTool.Set_AlphaColor(CountdownPaneltyTxt, 0f);

        // Ready
        ReadyCG.alpha = 1f;
        ReadyCG.gameObject.SetActive(true);
        ReadyAnnoTxt.text = CSVManager.Instance.Get_StaticWord(90);
        ReadyTimeLimitTxt.text = $"{(int)CurrentCountdown}{SecondString}";
        ReadyInputAnnoImg.sprite = UnitManager.Instance.SpaceBarSprite;
        ReadyKeyAnnoTxt.text = $"{CSVManager.Instance.Get_StaticWord(88)} : {CSVManager.Instance.Get_StaticWord(89)} & {CSVManager.Instance.Get_StaticWord(85)}";

        ReadyTimeLimitAnnoRT.anchoredPosition = ReadyTimeLimitAnnoRTPos.TypeBase;
        ReadyTimeLimitAnnoRT.localScale = Vector2.one;

        ReadyKeyAnnoRT.anchoredPosition = ReadykeyAnnoRTPos.TypeBase;
        ReadyKeyAnnoRT.localScale = Vector2.one;
    }

    private void Set_AllComplete()
    {
        // Cell
        for (int i = 0; i < AllBoxCellEUI.Count; i++)
        {
            AllBoxCellEUI[i].Set_InnerColor(UnlockedClr);
        }

        // Connection
        for (int i = 0; i < AllBoxConnectionEUI.Count; i++)
        {
            AllBoxConnectionEUI[i].Set_InnerColor(UnlockedClr);
        }

        // Selecting
        SelectingBoxCellEUI = null;
        SelectingSignRT.gameObject.SetActive(false);

        // Value
        IsInteractable = false;
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

    #endregion

    #region Check

    public void Check_CorrectLineSet()
    {
        bool jugeNow = Can_Success();
        Debug.Log("판별:" + jugeNow);
        if (jugeNow == CanSuccess) return;
        CanSuccess = jugeNow;

        Play_LineSetChange();
    }

    #endregion

    #region Is & Can

    public bool Can_Success()
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

    #region Ready

    private void Play_ReadyToStart(float _DurTime)
    {
        IsReady = false;

        ReadyTimeLimitAnnoRT.DOAnchorPos(ReadyTimeLimitAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyTimeLimitAnnoRT.DOScale(0.5f, _DurTime);
        ReadyKeyAnnoRT.DOAnchorPos(ReadykeyAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyKeyAnnoRT.DOScale(0.5f, _DurTime);

        ReadyCG.DOFade(0f, _DurTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                ReadyCG.gameObject.SetActive(false);
                IsStart = true;
            });
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        ThisCG.alpha = 1f;

        // Value
        IsReady = true;
        IsStart = false;
        IsInteractable = true;

        Set_RandomPuzzleByRate();

        Set_AllStart();
        SetOn_ByConditionToCell();
        SetOn_ByConditionToConnector();

        Check_CorrectLineSet();
    }

    #endregion

    #region Play

    private void Play_SelectingRT()
    {
        DevTool.Set_KillTween(SelectingSignRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(SelectingSignRT.DOScale(1.05f, 0.1f));
        seq.Append(SelectingSignRT.DOScale(1.0f, 0.1f));

    }

    private void Play_LineSetChange()
    {
        Set_SuccessPanel(0.5f);
        Set_FailurePanel(0.5f);

        Set_Roller(1f);
    }

    private void Play_FailureAnno(float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
        Play_ExtraAnno(FailureAnnoRT, FailureAnnoTxt, _OnDurTime, _OffDurTime, _IntervalTime);
    }

    private void Play_SuccessAnno(float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
        Play_ExtraAnno(SuccessAnnoRT, SuccessAnnoTxt, _OnDurTime, _OffDurTime, _IntervalTime);
    }

    private void Play_ExtraAnno(RectTransform _RT, TMP_Text _Tmp, float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
        if (!IsInteractable) return;

        DevTool.Set_KillTween(_RT);
        DevTool.Set_KillTween(_Tmp);

        Sequence ExtraAnnoSeq = DOTween.Sequence();

        ExtraAnnoSeq.OnStart(() =>
        {
            _RT.anchoredPosition = new Vector2(0f, 300f);
            DevTool.Set_AlphaColor(_Tmp, 0f);
        });

        ExtraAnnoSeq.Join(_RT.DOAnchorPosY(360f, _OnDurTime));
        ExtraAnnoSeq.Join(_Tmp.DOFade(1f, _OnDurTime));
        ExtraAnnoSeq.AppendInterval(_IntervalTime);
        ExtraAnnoSeq.Join(_Tmp.DOFade(0f, _OffDurTime));
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

    #region Roll

    private bool Is_Interact_Roll(float _PlusAngle, float _DurTime)
    {
        if (!(CurrentBtn is BoxCellEUIController boxCell) ||
            boxCell != SelectingBoxCellEUI)
            return false;

        SelectingBoxCellEUI.Play_Roll(_PlusAngle, _DurTime);

        return true;
    }

    #endregion

    #region Check Success Or Failure

    private void Set_SuccessPanel(float _DurTime)
    {
        DevTool.Set_KillTween(SuccessCG);

        SuccessCG.DOFade(CanSuccess ? 1f : 0.3f, _DurTime);
    }

    private void Set_FailurePanel(float _DurTime)
    {
        DevTool.Set_KillTween(FailureCG);

        FailureCG.DOFade(CanSuccess ? 0.3f : 1f, _DurTime);
    }

    private void Set_Roller(float _DurTime)
    {
        DevTool.Set_KillTween(RollingRT);

        float targetAngle = CanSuccess ? 0 : 180;
        Quaternion endQuatValue = Quaternion.Euler(0f, 0f, targetAngle);
        RollingRT.DORotateQuaternion(endQuatValue, _DurTime).SetEase(Ease.OutElastic);
    }

    #endregion

    #region Unlock

    private bool Is_Interact_TryUnlock()
    {
        if (IsReady)
        {
            Play_ReadyToStart(2f);

            return true;
        }

        if (!IsInteractable || !IsStart) 
            return false;

        if (CanSuccess)
        {
            Play_SuccessAnno(1f, 1f);
            DevTool.Set_Color(UnlockedClr, CountdownTxt);
            StartCoroutine(Play_Unlock_Cor());
            return true;
        }
        else
        {
            Play_FailureAnno(1f, 1f);
            Set_Panelty(-0.5f);
            return false;
        }
    }

    private IEnumerator Play_Unlock_Cor()
    {
        Set_AllComplete();

        ThisCG.DOFade(0f, 1.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);

        SetOff_ThisPanel();
    }

    #endregion

    #region CountDown

    private void Caculate_CountDown(float _DeltaTime)
    {
        if (!IsInteractable || !IsStart) return;

        if (CurrentCountdown > 0f)
        {
            CurrentCountdown -= _DeltaTime;
            Set_CountdownTxt();
        }
        else
        {
            IsInteractable = false;
            CurrentCountdown = 0f;
            Set_CountdownTxt();
        }
    }

    private void Set_CountdownTxt()
    {
        string countString = CurrentCountdown < 4 ? CurrentCountdown.ToString("0.0") : ((int)CurrentCountdown).ToString();
        CountdownTxt.text = $"{countString}{SecondString}";
    }

    #endregion

    #region Panelty

    private void Set_Panelty(float _PaneltyTime)
    {
        CurrentCountdown += _PaneltyTime;

        CountdownPaneltyTxt.text = $"{_PaneltyTime.ToString("0.0")}{SecondString}";

        DevTool.Set_KillTween(CountdownPaneltyRT);
        DevTool.Set_KillTween(CountdownPaneltyTxt);


        CountdownPaneltyRT.localScale = Vector2.one;
        DevTool.Set_AlphaColor(CountdownPaneltyTxt, 1f);

        Sequence seq = DOTween.Sequence();
        seq.Append(CountdownPaneltyRT.DOScale(1.1f, 0.05f));
        seq.Append(CountdownPaneltyRT.DOScale(0f, 1.95f));

        CountdownPaneltyTxt.DOFade(0f, 2f);
    }

    #endregion
}
