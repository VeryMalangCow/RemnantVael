using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PuzzleUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Puzzle UI")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected float BaseCountdown = 15f;

    [Space(10)]
    [Header("=== Ready Panel")]
    [SerializeField] private CanvasGroup ReadyCG;
    [SerializeField] private TMP_Text ReadyAnnoTxt;
    [SerializeField] private TMP_Text ReadyTimeLimitTxt;
    [SerializeField] private TMP_Text ReadyKeyAnnoTxt;
    [SerializeField] private Image ReadyInputAnnoImg;

    [Space(10)]
    [Header("=== Left")]
    [SerializeField] private TMP_Text UnlockAnnoTxt;
    [SerializeField] private TMP_Text SuccessAnnoTxt;
    [SerializeField] private TMP_Text FailureAnnoTxt;
    [SerializeField] private TMP_Text CountdownTxt;
    [SerializeField] private TMP_Text CountdownPaneltyTxt;

    [Space(10)]
    [Header("=== Right")]
    [SerializeField] private CanvasGroup SuccessCG;
    [SerializeField] private CanvasGroup FailureCG;
    [SerializeField] private TMP_Text TryUnlockTxt;
    [SerializeField] private TMP_Text InputTxt;
    [SerializeField] private Image InputImg;
    [SerializeField] private RectTransform RollingRT;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] protected Color LockedClr;
    [SerializeField] protected Color UnlockedClr;

    #endregion

    #region - Hide

    // Canvas Group
    [HideInInspector] protected CanvasGroup ThisCG;

    // RT
    [HideInInspector] private RectTransform SuccessAnnoRT;
    [HideInInspector] private RectTransform FailureAnnoRT;
    [HideInInspector] private RectTransform CountdownPaneltyRT;
    [HideInInspector] private RectTransform ReadyTimeLimitAnnoRT;
    [HideInInspector] private RectTransform ReadyKeyAnnoRT;

    // Txt
    [HideInInspector] private TMP_Text SuccessTxt;
    [HideInInspector] private TMP_Text FailureTxt;

    // Static Data
    [HideInInspector] private static string SecondString = "<size=50%>s</size>";
    [HideInInspector] private CoupleData<Vector2> ReadyTimeLimitAnnoRTPos;
    [HideInInspector] private CoupleData<Vector2> ReadykeyAnnoRTPos;

    // Success
    [HideInInspector] protected bool CanSuccess = false;
    [HideInInspector] protected bool IsInteractable = false;
    [HideInInspector] private bool IsReady = false;
    [HideInInspector] private bool IsStart = false;

    // Value
    [HideInInspector] protected float CurrentCountdown = 0;

    // Prison
    [HideInInspector] private PrisonController UsingPrison = null;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset_FirstValue(PrisonController _Prison)
    {
        UsingPrison = _Prison;
    }

    public override void Offset()
    {
        base.Offset();
        
        // Color
        UnlockedClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);

        // CG
        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;

        // Left
        FailureAnnoRT = DevTool.Get_ComponentTType(FailureAnnoTxt.gameObject, out RectTransform fRt) ? fRt : null;
        SuccessAnnoRT = DevTool.Get_ComponentTType(SuccessAnnoTxt.gameObject, out RectTransform sRt) ? sRt : null;

        DevTool.Set_Color(LockedClr, FailureAnnoTxt);
        DevTool.Set_Color(UnlockedClr, SuccessAnnoTxt);

        DevTool.Set_Color(LockedClr, CountdownPaneltyTxt);

        // Right
        FailureTxt = DevTool.Get_ComponentTType(FailureCG.transform.GetChild(0).gameObject, out TMP_Text fTxt) ? fTxt : null;
        SuccessTxt = DevTool.Get_ComponentTType(SuccessCG.transform.GetChild(0).gameObject, out TMP_Text sTxt) ? sTxt : null;

        DevTool.Set_Color(LockedClr, FailureTxt);
        DevTool.Set_Color(UnlockedClr, SuccessTxt);

        CountdownPaneltyRT = DevTool.Get_ComponentTType(CountdownPaneltyTxt.gameObject, out RectTransform paneltyTimeRt) ? paneltyTimeRt : null;

        // Ready
        ReadyTimeLimitAnnoRT = DevTool.Get_ComponentTType(ReadyTimeLimitTxt.gameObject, out RectTransform readyTimeRt) ? readyTimeRt : null;
        ReadyTimeLimitAnnoRTPos = new CoupleData<Vector2>(readyTimeRt.anchoredPosition, new Vector2(-1128f, -152f));

        ReadyKeyAnnoRT = DevTool.Get_ComponentTType(ReadyKeyAnnoTxt.gameObject, out RectTransform readyKeyRt) ? readyKeyRt : null;
        ReadykeyAnnoRTPos = new CoupleData<Vector2>(ReadyKeyAnnoRT.anchoredPosition, new Vector2(1128f, -580f));

        // Txt
        TryUnlockTxt.text = CSVManager.Instance.Get_StaticWord(85);
        InputTxt.text = CSVManager.Instance.Get_StaticWord(88);
        SuccessTxt.text = CSVManager.Instance.Get_StaticWord(86);
        FailureTxt.text = CSVManager.Instance.Get_StaticWord(87);

        // Key Img
        InputImg.sprite = UnitManager.Instance.SpaceBarSprite;
        InputImg.SetNativeSize();
    }

    #endregion

    #region Framework

    private void Update()
    {
        Caculate_CountDown(Time.deltaTime);
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

        Set_AllStart();
    }

    #endregion

    #region Set (Start & Complete)

    protected virtual void Set_AllStart()
    {
        // txt
        UnlockAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(28).Replace("\\n", "\n");
        SuccessAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(29).Replace("\\n", "\n");
        FailureAnnoTxt.text = CSVManager.Instance.Get_StaticDesc(30).Replace("\\n", "\n");

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

    protected virtual void Set_AllComplete()
    {
        // Value
        IsInteractable = false;
    }

    #endregion

    #region Left

    #region Set (Panelty)

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

    #region Anno

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

    #region Countdown

    private void Set_CountdownTxt()
    {
        string countString = CurrentCountdown < 4 ? CurrentCountdown.ToString("0.0") : ((int)CurrentCountdown).ToString();
        CountdownTxt.text = $"{countString}{SecondString}";
    }

    #endregion

    #endregion

    #region Right

    #region Preview

    public void Check_CorrectLineSet()
    {
        bool jugeNow = Can_Success();
        if (jugeNow == CanSuccess) return;
        CanSuccess = jugeNow;

        Play_LineSetChange();
    }

    private void Play_LineSetChange()
    {
        Set_SuccessPanel(0.5f);
        Set_FailurePanel(0.5f);

        Set_Roller(1f);
    }

    #endregion

    #region Anno

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

    #region Can

    public abstract bool Can_Success();

    #endregion

    #region Caculate

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
            StartCoroutine(Play_Unlock_Failure_Cor());
            IsInteractable = false;
            CurrentCountdown = 0f;
            Set_CountdownTxt();
        }
    }
    #endregion

    #region Interact

    protected bool Is_Interact_TryUnlock()
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
            StartCoroutine(Play_Unlock_Complete_Cor());

            return true;
        }
        else
        {
            Play_FailureAnno(1f, 1f);
            Set_Panelty(-0.5f);
            return false;
        }
    }

    private IEnumerator Play_Unlock_Complete_Cor()
    {
        Set_AllComplete();

        ThisCG.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        UsingPrison.Set_Unlock();
        UsingPrison = null;

        yield return new WaitForSeconds(1f);

        SetOff_ThisPanel();
    }

    private IEnumerator Play_Unlock_Failure_Cor()
    {
        ThisCG.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(2f);

        SetOff_ThisPanel();
    }

    #endregion
}
