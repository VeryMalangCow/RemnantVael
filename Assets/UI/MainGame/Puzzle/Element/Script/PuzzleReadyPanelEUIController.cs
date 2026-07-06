using DG.Tweening;
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
    [SerializeField] public CanvasGroup readyCg;
    [SerializeField] private TMP_Text readyAnnoTxt;
    [SerializeField] private TMP_Text readyTimeLimitTxt;
    [SerializeField] private TMP_Text readyKeyAnnoTxt;
    [SerializeField] private Image readyInputAnnoImg;
    [SerializeField] private Transform keyAnnoParentTf;

    [Space(10)]
    [Header("=== Rule")]
    [SerializeField] private TMP_Text ruleTxt;
    [SerializeField] private TMP_Text ruleDescTxt;

    [Space(10)]
    [Header("=== Warning")]
    [SerializeField] private TMP_Text warningTxt;
    [SerializeField] private TMP_Text warningDescTxt;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform ruleRt;
    [HideInInspector] private RectTransform warningRt;

    [HideInInspector] private CoupleData<float> ruleWarningXRtPos;


    [HideInInspector] private RectTransform readyTimeLimitAnnoRt;
    [HideInInspector] private RectTransform readyKeyAnnoRt;

    [HideInInspector] private CoupleData<Vector2> readyTimeLimitAnnoRtPos;
    [HideInInspector] private CoupleData<Vector2> readykeyAnnoRtPos;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        ruleRt = DevTool.Get_ComponentTType(ruleTxt.gameObject, out RectTransform _ruleRt) ? _ruleRt : null;
        warningRt = DevTool.Get_ComponentTType(warningTxt.gameObject, out RectTransform _warningRt) ? _warningRt : null;

        ruleWarningXRtPos = new CoupleData<float>(900f, 3000f);

        readyTimeLimitAnnoRt = DevTool.Get_ComponentTType(readyTimeLimitTxt.gameObject, out RectTransform readyTimeRt) ? readyTimeRt : null;
        readyTimeLimitAnnoRtPos = new CoupleData<Vector2>(readyTimeRt.anchoredPosition, new Vector2(-1128f, -152f));

        readyKeyAnnoRt = DevTool.Get_ComponentTType(readyKeyAnnoTxt.gameObject, out RectTransform readyKeyRt) ? readyKeyRt : null;
        readykeyAnnoRtPos = new CoupleData<Vector2>(readyKeyAnnoRt.anchoredPosition, new Vector2(1128f, -580f));
    }

    #endregion

    #region Set

    public void Set_AllStart(float currentCountdown, string secondString)
    {
        var prefab = StaticResourceManager.instance.BuildReso.prisonPrefab;

        // Ready
        readyCg.alpha = 1f;
        readyCg.gameObject.SetActive(true);
        readyAnnoTxt.text = $"<< {ResourceManager.instance.Get_StaticWord(90)} >>";
        readyTimeLimitTxt.text = $"{(int)currentCountdown}{secondString}";
        readyInputAnnoImg.sprite = prefab.spaceBarSprite;
        readyKeyAnnoTxt.text = $"{ResourceManager.instance.Get_StaticWord(88)} : {ResourceManager.instance.Get_StaticWord(89)} & {ResourceManager.instance.Get_StaticWord(85)}";

        warningRt.anchoredPosition = new Vector2(ruleWarningXRtPos.typeBase, warningRt.anchoredPosition.y);
        ruleRt.anchoredPosition = new Vector2(-ruleWarningXRtPos.typeBase, ruleRt.anchoredPosition.y);

        readyTimeLimitAnnoRt.anchoredPosition = readyTimeLimitAnnoRtPos.typeBase;
        readyTimeLimitAnnoRt.localScale = Vector2.one;

        readyKeyAnnoRt.anchoredPosition = readykeyAnnoRtPos.typeBase;
        readyKeyAnnoRt.localScale = Vector2.one;

        // Rule (Left)
        Color unlockClr = PrisonBuild.unlockedClr;
        DevTool.SetColor(unlockClr, ruleTxt);
        DevTool.SetColor(unlockClr, ruleDescTxt);

        ruleTxt.text = $"< {ResourceManager.instance.Get_StaticWord(94)} >";

        // Warning (Right)
        Color lockClr = prefab.lockedClr;
        DevTool.SetColor(lockClr, warningTxt);
        DevTool.SetColor(lockClr, warningDescTxt);

        warningTxt.text = $"< {ResourceManager.instance.Get_StaticWord(93)} >";
        warningDescTxt.text = ResourceManager.instance.Get_StaticDesc(32).Replace("\\n", "\n");
    }

    public void Set_RuleDesc(string desc)
    {
        ruleDescTxt.text = desc.Replace("\\n", "\n");
    }

    public void Set_KeyAnno(Transform tf)
    {
        tf.SetParent(keyAnnoParentTf);
    }

    #endregion

    #region Play

    public Tween Play_ReadyToStart(float durTime)
    {
        warningRt.DOAnchorPosX(ruleWarningXRtPos.typeSpecial, durTime);
        ruleRt.DOAnchorPosX(-ruleWarningXRtPos.typeSpecial, durTime);

        readyTimeLimitAnnoRt.DOAnchorPos(readyTimeLimitAnnoRtPos.typeSpecial, durTime).SetEase(Ease.OutCubic);
        readyTimeLimitAnnoRt.DOScale(0.5f, durTime);
        readyKeyAnnoRt.DOAnchorPos(readykeyAnnoRtPos.typeSpecial, durTime).SetEase(Ease.OutCubic);
        readyKeyAnnoRt.DOScale(0.5f, durTime);

        return readyCg.DOFade(0f, durTime).SetEase(Ease.Linear);
    }

    #endregion
}
