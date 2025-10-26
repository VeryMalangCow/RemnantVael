using DG.Tweening;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleReadyPanelEUIController : ElementUIController
{
    #region Value

    #region - Inpector

    [Space(20)]
    [Header("<><><><><> Puzzle Ready Panel")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public CanvasGroup ReadyCG;
    [SerializeField] private TMP_Text ReadyAnnoTxt;
    [SerializeField] private TMP_Text ReadyTimeLimitTxt;
    [SerializeField] private TMP_Text ReadyKeyAnnoTxt;
    [SerializeField] private Image ReadyInputAnnoImg;

    [Space(10)]
    [Header("=== Rule")]
    [SerializeField] private TMP_Text RuleTxt;
    [SerializeField] private TMP_Text RuleDescTxt;

    [Space(10)]
    [Header("=== Warning")]
    [SerializeField] private TMP_Text WarningTxt;
    [SerializeField] private TMP_Text WarningDescTxt;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform RuleRT;
    [HideInInspector] private RectTransform WarningRT;

    [HideInInspector] private CoupleData<float> RuleWarningXRTPos;


    [HideInInspector] private RectTransform ReadyTimeLimitAnnoRT;
    [HideInInspector] private RectTransform ReadyKeyAnnoRT;

    [HideInInspector] private CoupleData<Vector2> ReadyTimeLimitAnnoRTPos;
    [HideInInspector] private CoupleData<Vector2> ReadykeyAnnoRTPos;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        RuleRT = DevTool.Get_ComponentTType(RuleTxt.gameObject, out RectTransform ruleRt) ? ruleRt : null;
        WarningRT = DevTool.Get_ComponentTType(WarningTxt.gameObject, out RectTransform warningRt) ? warningRt : null;

        RuleWarningXRTPos = new CoupleData<float>(900f, 3000f);

        ReadyTimeLimitAnnoRT = DevTool.Get_ComponentTType(ReadyTimeLimitTxt.gameObject, out RectTransform readyTimeRt) ? readyTimeRt : null;
        ReadyTimeLimitAnnoRTPos = new CoupleData<Vector2>(readyTimeRt.anchoredPosition, new Vector2(-1128f, -152f));

        ReadyKeyAnnoRT = DevTool.Get_ComponentTType(ReadyKeyAnnoTxt.gameObject, out RectTransform readyKeyRt) ? readyKeyRt : null;
        ReadykeyAnnoRTPos = new CoupleData<Vector2>(ReadyKeyAnnoRT.anchoredPosition, new Vector2(1128f, -580f));
    }

    #endregion

    #region Set

    public void Set_AllStart(float _CurrentCountdown, string _SecondString)
    {
        // Ready
        ReadyCG.alpha = 1f;
        ReadyCG.gameObject.SetActive(true);
        ReadyAnnoTxt.text = $"<< {ResourceManager.Instance.Get_StaticWord(90)} >>";
        ReadyTimeLimitTxt.text = $"{(int)_CurrentCountdown}{_SecondString}";
        ReadyInputAnnoImg.sprite = ResourceManager.Instance.SpaceBarSprite;
        ReadyKeyAnnoTxt.text = $"{ResourceManager.Instance.Get_StaticWord(88)} : {ResourceManager.Instance.Get_StaticWord(89)} & {ResourceManager.Instance.Get_StaticWord(85)}";

        WarningRT.anchoredPosition = new Vector2(RuleWarningXRTPos.TypeBase, WarningRT.anchoredPosition.y);
        RuleRT.anchoredPosition = new Vector2(-RuleWarningXRTPos.TypeBase, RuleRT.anchoredPosition.y);

        ReadyTimeLimitAnnoRT.anchoredPosition = ReadyTimeLimitAnnoRTPos.TypeBase;
        ReadyTimeLimitAnnoRT.localScale = Vector2.one;

        ReadyKeyAnnoRT.anchoredPosition = ReadykeyAnnoRTPos.TypeBase;
        ReadyKeyAnnoRT.localScale = Vector2.one;

        // Rule (Left)
        DevTool.Set_Color(ResourceManager.Instance.UnlockedClr, RuleTxt);
        DevTool.Set_Color(ResourceManager.Instance.UnlockedClr, RuleDescTxt);

        RuleTxt.text = $"< {ResourceManager.Instance.Get_StaticWord(94)} >";

        // Warning (Right)
        DevTool.Set_Color(ResourceManager.Instance.LockedClr, WarningTxt);
        DevTool.Set_Color(ResourceManager.Instance.LockedClr, WarningDescTxt);

        WarningTxt.text = $"< {ResourceManager.Instance.Get_StaticWord(93)} >";
        WarningDescTxt.text = ResourceManager.Instance.Get_StaticDesc(32).Replace("\\n", "\n");
    }

    public void Set_RuleDesc(string _Desc)
    {
        RuleDescTxt.text = _Desc.Replace("\\n", "\n");
    }

    #endregion

    #region Play

    public Tween Play_ReadyToStart(float _DurTime)
    {
        WarningRT.DOAnchorPosX(RuleWarningXRTPos.TypeSpecial, _DurTime);
        RuleRT.DOAnchorPosX(-RuleWarningXRTPos.TypeSpecial, _DurTime);

        ReadyTimeLimitAnnoRT.DOAnchorPos(ReadyTimeLimitAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyTimeLimitAnnoRT.DOScale(0.5f, _DurTime);
        ReadyKeyAnnoRT.DOAnchorPos(ReadykeyAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyKeyAnnoRT.DOScale(0.5f, _DurTime);

        return ReadyCG.DOFade(0f, _DurTime).SetEase(Ease.Linear);
    }

    #endregion
}
