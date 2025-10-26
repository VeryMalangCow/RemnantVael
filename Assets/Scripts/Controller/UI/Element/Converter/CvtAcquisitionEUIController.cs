using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CvtAcquisitionEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Acquisition Part")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public OwnBtnEUIController ConvertBtn;
    [SerializeField] public OwnBtnEUIController MaxBtn;
    [SerializeField] public OwnBtnEUIController More10Btn;
    [SerializeField] public OwnBtnEUIController More1Btn;
    [SerializeField] public OwnBtnEUIController Less1Btn;
    [SerializeField] public OwnBtnEUIController Less10Btn;
    [SerializeField] public OwnBtnEUIController MinBtn;

    [Space(5)]
    [SerializeField] private TMP_Text PossessionTxt;
    [SerializeField] private TMP_Text AcquisitionTxt;

    [Space(5)]
    [SerializeField] private TMP_Text PossessionAmountTxt;
    [SerializeField] private TMP_Text AcquisitionAmountTxt;

    [Space(5)]
    [SerializeField] private Image ConvertInnerImg;
    [SerializeField] private CanvasGroup VisualCG;
    [SerializeField] private RectTransform Cog0RT;
    [SerializeField] private RectTransform Cog1RT;
    [SerializeField] private Image ProgressBarImg;
    [SerializeField] private Image AfterIconImg;


    [HideInInspector] private Sequence AfterIconSeq = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        ConvertBtn.Offset();
        MaxBtn.Offset();
        More10Btn.Offset();
        More1Btn.Offset();
        Less1Btn.Offset();
        Less10Btn.Offset();
        MinBtn.Offset();
    }

    public void Offset_Owner(SinglePanelUIController _OwnerUI)
    {
        ConvertBtn.OwnerUIController = _OwnerUI;
        MaxBtn.OwnerUIController = _OwnerUI;
        More10Btn.OwnerUIController = _OwnerUI;
        More1Btn.OwnerUIController = _OwnerUI;
        Less1Btn.OwnerUIController = _OwnerUI;
        Less10Btn.OwnerUIController = _OwnerUI;
        MinBtn.OwnerUIController = _OwnerUI;
    }

    #endregion

    #region Txt

    public void Set_PossessionAmountTxt(string _Amount)
    {
        PossessionAmountTxt.text = _Amount;
    }

    public void Set_AcquisitionAmountTxt(string _Amount)
    {
        AcquisitionAmountTxt.text = $"+{_Amount}";
    }

    #endregion

    #region Language

    public void Set_Language(bool _CanConvert)
    {
        if (ConvertBtn.gameObject.TryGetComponent(out TMP_Text convertTxt))
        {
            if (_CanConvert) 
                convertTxt.text = ResourceManager.Instance.Get_StaticWord(127);
            else
                convertTxt.text = ResourceManager.Instance.Get_StaticWord(134);
        }

        if (MaxBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text maxTxt))
            maxTxt.text = ResourceManager.Instance.Get_StaticWord(128);
        if (More10Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text more10Txt))
            more10Txt.text = ResourceManager.Instance.Get_StaticWord(129);
        if (More1Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text more1Txt))
            more1Txt.text = ResourceManager.Instance.Get_StaticWord(130);
        if (Less1Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text less1Txt))
            less1Txt.text = ResourceManager.Instance.Get_StaticWord(131);
        if (Less10Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text less10Txt))
            less10Txt.text = ResourceManager.Instance.Get_StaticWord(132);
        if (MinBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text minTxt))
            minTxt.text = ResourceManager.Instance.Get_StaticWord(133);

        PossessionTxt.text = ResourceManager.Instance.Get_StaticWord(121);
        AcquisitionTxt.text = ResourceManager.Instance.Get_StaticWord(126);
    }

    #endregion

    #region Visual

    public void Set_AbleConvertVisual(bool _Can)
    {
        ConvertInnerImg.gameObject.SetActive(_Can);
    }

    // 2s
    public void Play_VisualComp()
    {
        Sequence seq = DOTween.Sequence();

        VisualCG.gameObject.SetActive(true);

        Cog0RT.transform.rotation = Quaternion.identity;
        Cog1RT.transform.rotation = Quaternion.identity;
        ProgressBarImg.fillAmount = 0f;

        Cog0RT.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360);
        Cog1RT.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360);
        ProgressBarImg.DOFillAmount(1f, 1.5f);

        seq.Append(VisualCG.DOFade(1f, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(VisualCG.DOFade(0f, 0.5f));
        seq.OnComplete(() =>
            {
                VisualCG.gameObject.SetActive(false);
            });
    }

    public void Play_SuccessComp()
    {
        AfterIconImg.sprite = ResourceManager.Instance.CvtMaterialConditionIcon.Get_Special(true);
        Play_AfterImg();

        Sequence seq = DOTween.Sequence();

        seq.Append(PossessionAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(PossessionAmountTxt.transform.DOScale(1f, 0.25f));
    }

    public void Play_FailComp()
    {
        AfterIconImg.sprite = ResourceManager.Instance.CvtMaterialConditionIcon.Get_Special(false);
        Play_AfterImg();
    }

    // 1f
    private void Play_AfterImg()
    {
        DevTool.Set_CompleteTween(AfterIconSeq);
        AfterIconSeq = DOTween.Sequence();

        AfterIconImg.transform.localScale = Vector2.zero;
        AfterIconImg.gameObject.SetActive(true);

        AfterIconSeq.Append(AfterIconImg.transform.DOScale(1.5f, 0.25f));
        AfterIconSeq.Append(AfterIconImg.transform.DOScale(0f, 0.25f));
        AfterIconSeq.OnComplete(() => AfterIconImg.gameObject.SetActive(false));
    }

    #endregion
}
