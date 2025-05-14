using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text UnlockAnnoTxt;
    [SerializeField] private TMP_Text SuccessAnnoTxt;
    [SerializeField] private TMP_Text FailureAnnoTxt;
    [SerializeField] public TMP_Text CountdownTxt;
    [SerializeField] public TMP_Text CountdownPaneltyTxt;

    #endregion

    #region - Hide

    // RT
    [HideInInspector] private RectTransform SuccessAnnoRT;
    [HideInInspector] private RectTransform FailureAnnoRT;
    [HideInInspector] private RectTransform CountdownPaneltyRT;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        FailureAnnoRT = DevTool.Get_ComponentTType(FailureAnnoTxt.gameObject, out RectTransform fRt) ? fRt : null;
        SuccessAnnoRT = DevTool.Get_ComponentTType(SuccessAnnoTxt.gameObject, out RectTransform sRt) ? sRt : null;

        CountdownPaneltyRT = DevTool.Get_ComponentTType(CountdownPaneltyTxt.gameObject, out RectTransform paneltyTimeRt) ? paneltyTimeRt : null;
    }

    #endregion

    #region Set

    public void Set_AllStart(float _CurrentCountdown, string _SecondString)
    {
        DevTool.Set_Color(UnitManager.Instance.LockedClr, FailureAnnoTxt);
        DevTool.Set_Color(UnitManager.Instance.UnlockedClr, SuccessAnnoTxt);

        DevTool.Set_Color(UnitManager.Instance.LockedClr, CountdownPaneltyTxt);

        DevTool.Set_Color(UnitManager.Instance.LockedClr, CountdownTxt);
        DevTool.Set_AlphaColor(CountdownPaneltyTxt, 0f);

        UnlockAnnoTxt.text = ResourceManager.Instance.Get_StaticDesc(28).Replace("\\n", "\n");
        SuccessAnnoTxt.text = ResourceManager.Instance.Get_StaticDesc(29).Replace("\\n", "\n");
        FailureAnnoTxt.text = ResourceManager.Instance.Get_StaticDesc(30).Replace("\\n", "\n");
        Set_CountdownTxt(_CurrentCountdown, _SecondString);
    }

    public void Set_CountdownTxt(float _CurrentCountdown, string _SecondString)
    {
        string countString = _CurrentCountdown < 4 ? _CurrentCountdown.ToString("0.0") : ((int)_CurrentCountdown).ToString();
        CountdownTxt.text = $"{countString}{_SecondString}";
    }


    public void Set_Panelty(float _PaneltyTime, string _SecondString)
    {
        CountdownPaneltyTxt.text = $"{_PaneltyTime.ToString("0.0")}{_SecondString}";

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

    #region Play

    public void Play_FailureAnno(float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
        Play_ExtraAnno(FailureAnnoRT, FailureAnnoTxt, _OnDurTime, _OffDurTime, _IntervalTime);
    }

    public void Play_SuccessAnno(float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
        Play_ExtraAnno(SuccessAnnoRT, SuccessAnnoTxt, _OnDurTime, _OffDurTime, _IntervalTime);
    }

    private void Play_ExtraAnno(RectTransform _RT, TMP_Text _Tmp, float _OnDurTime, float _OffDurTime, float _IntervalTime = 0.5f)
    {
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
}
