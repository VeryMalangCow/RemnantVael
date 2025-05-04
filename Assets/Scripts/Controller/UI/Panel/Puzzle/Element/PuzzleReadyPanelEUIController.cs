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
    [SerializeField] public CanvasGroup ReadyCG;
    [SerializeField] private TMP_Text ReadyAnnoTxt;
    [SerializeField] private TMP_Text ReadyTimeLimitTxt;
    [SerializeField] private TMP_Text ReadyKeyAnnoTxt;
    [SerializeField] private Image ReadyInputAnnoImg;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform ReadyTimeLimitAnnoRT;
    [HideInInspector] private RectTransform ReadyKeyAnnoRT;

    [HideInInspector] private CoupleData<Vector2> ReadyTimeLimitAnnoRTPos;
    [HideInInspector] private CoupleData<Vector2> ReadykeyAnnoRTPos;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        // Ready
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
        ReadyAnnoTxt.text = CSVManager.Instance.Get_StaticWord(90);
        ReadyTimeLimitTxt.text = $"{(int)_CurrentCountdown}{_SecondString}";
        ReadyInputAnnoImg.sprite = UnitManager.Instance.SpaceBarSprite;
        ReadyKeyAnnoTxt.text = $"{CSVManager.Instance.Get_StaticWord(88)} : {CSVManager.Instance.Get_StaticWord(89)} & {CSVManager.Instance.Get_StaticWord(85)}";

        ReadyTimeLimitAnnoRT.anchoredPosition = ReadyTimeLimitAnnoRTPos.TypeBase;
        ReadyTimeLimitAnnoRT.localScale = Vector2.one;

        ReadyKeyAnnoRT.anchoredPosition = ReadykeyAnnoRTPos.TypeBase;
        ReadyKeyAnnoRT.localScale = Vector2.one;
    }

    #endregion

    #region Play

    public Tween Play_ReadyToStart(float _DurTime)
    {
        ReadyTimeLimitAnnoRT.DOAnchorPos(ReadyTimeLimitAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyTimeLimitAnnoRT.DOScale(0.5f, _DurTime);
        ReadyKeyAnnoRT.DOAnchorPos(ReadykeyAnnoRTPos.TypeSpecial, _DurTime).SetEase(Ease.OutCubic);
        ReadyKeyAnnoRT.DOScale(0.5f, _DurTime);

        return ReadyCG.DOFade(0f, _DurTime).SetEase(Ease.Linear);
    }

    #endregion
}
