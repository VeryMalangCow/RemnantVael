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
    [SerializeField] public OwnBtnEUIController convertBtn;
    [SerializeField] public OwnBtnEUIController maxBtn;
    [SerializeField] public OwnBtnEUIController more10Btn;
    [SerializeField] public OwnBtnEUIController more1Btn;
    [SerializeField] public OwnBtnEUIController less1Btn;
    [SerializeField] public OwnBtnEUIController less10Btn;
    [SerializeField] public OwnBtnEUIController minBtn;

    [Space(5)]
    [SerializeField] private TMP_Text possessionTxt;
    [SerializeField] private TMP_Text acquisitionTxt;

    [Space(5)]
    [SerializeField] private TMP_Text possessionAmountTxt;
    [SerializeField] private TMP_Text acquisitionAmountTxt;

    [Space(5)]
    [SerializeField] private Image convertInnerImg;
    [SerializeField] private CanvasGroup visualCg;
    [SerializeField] private RectTransform cog0Rt;
    [SerializeField] private RectTransform cog1Rt;
    [SerializeField] private Image progressBarImg;
    [SerializeField] private Image afterIconImg;


    [HideInInspector] private Sequence afterIconSeq = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        convertBtn.Offset();
        maxBtn.Offset();
        more10Btn.Offset();
        more1Btn.Offset();
        less1Btn.Offset();
        less10Btn.Offset();
        minBtn.Offset();
    }

    public void Offset_Owner(SinglePanelUIController ownerUI)
    {
        convertBtn.ownerUIController = ownerUI;
        maxBtn.ownerUIController = ownerUI;
        more10Btn.ownerUIController = ownerUI;
        more1Btn.ownerUIController = ownerUI;
        less1Btn.ownerUIController = ownerUI;
        less10Btn.ownerUIController = ownerUI;
        minBtn.ownerUIController = ownerUI;
    }

    #endregion

    #region Txt

    public void Set_PossessionAmountTxt(string amount)
    {
        possessionAmountTxt.text = amount;
    }

    public void Set_AcquisitionAmountTxt(string amount)
    {
        acquisitionAmountTxt.text = $"+{amount}";
    }

    #endregion

    #region Language

    public void Set_Language(bool canConvert)
    {
        if (convertBtn.gameObject.TryGetComponent(out TMP_Text convertTxt))
        {
            if (canConvert) 
                convertTxt.text = ResourceManager.instance.Get_StaticWord(127);
            else
                convertTxt.text = ResourceManager.instance.Get_StaticWord(134);
        }

        if (maxBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text maxTxt))
            maxTxt.text = ResourceManager.instance.Get_StaticWord(128);
        if (more10Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text more10Txt))
            more10Txt.text = ResourceManager.instance.Get_StaticWord(129);
        if (more1Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text more1Txt))
            more1Txt.text = ResourceManager.instance.Get_StaticWord(130);
        if (less1Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text less1Txt))
            less1Txt.text = ResourceManager.instance.Get_StaticWord(131);
        if (less10Btn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text less10Txt))
            less10Txt.text = ResourceManager.instance.Get_StaticWord(132);
        if (minBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text minTxt))
            minTxt.text = ResourceManager.instance.Get_StaticWord(133);

        possessionTxt.text = ResourceManager.instance.Get_StaticWord(121);
        acquisitionTxt.text = ResourceManager.instance.Get_StaticWord(126);
    }

    #endregion

    #region Visual

    public void Set_AbleConvertVisual(bool can)
    {
        convertInnerImg.gameObject.SetActive(can);
    }

    // 2s
    public void Play_VisualComp()
    {
        Sequence seq = DOTween.Sequence();

        visualCg.gameObject.SetActive(true);

        cog0Rt.transform.rotation = Quaternion.identity;
        cog1Rt.transform.rotation = Quaternion.identity;
        progressBarImg.fillAmount = 0f;

        cog0Rt.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360);
        cog1Rt.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360);
        progressBarImg.DOFillAmount(1f, 1.5f);

        seq.Append(visualCg.DOFade(1f, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(visualCg.DOFade(0f, 0.5f));
        seq.OnComplete(() =>
            {
                visualCg.gameObject.SetActive(false);
            });
    }

    public void Play_SuccessComp()
    {
        afterIconImg.sprite = ResourceManager.instance.cvtMaterialConditionIcon.Get_Special(true);
        Play_AfterImg();

        Sequence seq = DOTween.Sequence();

        seq.Append(possessionAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(possessionAmountTxt.transform.DOScale(1f, 0.25f));
    }

    public void Play_FailComp()
    {
        afterIconImg.sprite = ResourceManager.instance.cvtMaterialConditionIcon.Get_Special(false);
        Play_AfterImg();
    }

    // 1f
    private void Play_AfterImg()
    {
        DevTool.Set_CompleteTween(afterIconSeq);
        afterIconSeq = DOTween.Sequence();

        afterIconImg.transform.localScale = Vector2.zero;
        afterIconImg.gameObject.SetActive(true);

        afterIconSeq.Append(afterIconImg.transform.DOScale(1.5f, 0.25f));
        afterIconSeq.Append(afterIconImg.transform.DOScale(0f, 0.25f));
        afterIconSeq.OnComplete(() => afterIconImg.gameObject.SetActive(false));
    }

    #endregion
}
