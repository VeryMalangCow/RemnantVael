using DG.Tweening;
using TMPro;
using UnityEngine;

public class PuzzleTimePanelEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Puzzle Time")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text unlockAnnoTxt;
    [SerializeField] private TMP_Text successAnnoTxt;
    [SerializeField] private TMP_Text failureAnnoTxt;
    [SerializeField] public TMP_Text countdownTxt;
    [SerializeField] public TMP_Text countdownPaneltyTxt;

    #endregion

    #region - Hide

    // RT
    [HideInInspector] private RectTransform successAnnoRt;
    [HideInInspector] private RectTransform failureAnnoRt;
    [HideInInspector] private RectTransform countdownPaneltyRt;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        failureAnnoRt = DevTool.Get_ComponentTType(failureAnnoTxt.gameObject, out RectTransform fRt) ? fRt : null;
        successAnnoRt = DevTool.Get_ComponentTType(successAnnoTxt.gameObject, out RectTransform sRt) ? sRt : null;

        countdownPaneltyRt = DevTool.Get_ComponentTType(countdownPaneltyTxt.gameObject, out RectTransform paneltyTimeRt) ? paneltyTimeRt : null;
    }

    #endregion

    #region Set

    public void Set_AllStart(float currentCountdown, string secondString)
    {
        DevTool.Set_Color(ResourceManager.instance.lockedClr, failureAnnoTxt);
        DevTool.Set_Color(ResourceManager.instance.unlockedClr, successAnnoTxt);

        DevTool.Set_Color(ResourceManager.instance.lockedClr, countdownPaneltyTxt);

        DevTool.Set_Color(ResourceManager.instance.lockedClr, countdownTxt);
        DevTool.Set_AlphaColor(countdownPaneltyTxt, 0f);

        unlockAnnoTxt.text = ResourceManager.instance.Get_StaticDesc(28).Replace("\\n", "\n");
        successAnnoTxt.text = ResourceManager.instance.Get_StaticDesc(29).Replace("\\n", "\n");
        failureAnnoTxt.text = ResourceManager.instance.Get_StaticDesc(30).Replace("\\n", "\n");
        Set_CountdownTxt(currentCountdown, secondString);
    }

    public void Set_CountdownTxt(float currentCountdown, string secondString)
    {
        string countString = currentCountdown < 4 ? currentCountdown.ToString("0.0") : ((int)currentCountdown).ToString();
        countdownTxt.text = $"{countString}{secondString}";
    }


    public void Set_Panelty(float paneltyTime, string secondString)
    {
        countdownPaneltyTxt.text = $"{paneltyTime.ToString("0.0")}{secondString}";

        DevTool.Set_KillTween(countdownPaneltyRt);
        DevTool.Set_KillTween(countdownPaneltyTxt);

        countdownPaneltyRt.localScale = Vector2.one;
        DevTool.Set_AlphaColor(countdownPaneltyTxt, 1f);

        Sequence seq = DOTween.Sequence();
        seq.Append(countdownPaneltyRt.DOScale(1.1f, 0.05f));
        seq.Append(countdownPaneltyRt.DOScale(0f, 1.95f));

        countdownPaneltyTxt.DOFade(0f, 2f);
    }

    #endregion

    #region Play

    public void Play_FailureAnno(float onDurTime, float offDurTime, float intervalTime = 0.5f)
    {
        Play_ExtraAnno(failureAnnoRt, failureAnnoTxt, onDurTime, offDurTime, intervalTime);
    }

    public void Play_SuccessAnno(float onDurTime, float offDurTime, float intervalTime = 0.5f)
    {
        Play_ExtraAnno(successAnnoRt, successAnnoTxt, onDurTime, offDurTime, intervalTime);
    }

    private void Play_ExtraAnno(RectTransform rt, TMP_Text tmp, float onDurTime, float offDurTime, float intervalTime = 0.5f)
    {
        DevTool.Set_KillTween(rt);
        DevTool.Set_KillTween(tmp);

        Sequence ExtraAnnoSeq = DOTween.Sequence();

        ExtraAnnoSeq.OnStart(() =>
        {
            rt.anchoredPosition = new Vector2(0f, 300f);
            DevTool.Set_AlphaColor(tmp, 0f);
        });

        ExtraAnnoSeq.Join(rt.DOAnchorPosY(360f, onDurTime));
        ExtraAnnoSeq.Join(tmp.DOFade(1f, onDurTime));
        ExtraAnnoSeq.AppendInterval(intervalTime);
        ExtraAnnoSeq.Join(tmp.DOFade(0f, offDurTime));
    }

    #endregion
}
