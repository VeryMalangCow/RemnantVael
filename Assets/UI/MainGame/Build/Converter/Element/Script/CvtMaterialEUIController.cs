using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ConverterUIController;

public class CvtMaterialEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Materal Part")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public RectTransform rt;

    [Space(4)]
    [SerializeField] private Image iconImg;
    [SerializeField] private Image conditionIconImg;

    [Space(4)]
    [SerializeField] private TMP_Text possessionTxt;
    [SerializeField] private TMP_Text necessaryTxt;

    [Space(4)]
    [SerializeField] private TMP_Text possessionAmountTxt;
    [SerializeField] private TMP_Text necessaryAmountTxt;

    [Space(4)]
    [SerializeField] private CanvasGroup lineCg;
    [SerializeField] private RectTransform arrowAngleRt;

    #endregion

    #region Init

    public void Init(CvtMaterialData cvtData)
    {
        rt.anchoredPosition = new Vector2(0, cvtData.posY);
        iconImg.sprite = cvtData.icon;
        iconImg.SetNativeSize();
        arrowAngleRt.rotation = Quaternion.Euler(0, 0, cvtData.rotateZ);
        Set_Language();
    }

    #endregion

    #region Offset

    public override void Offset()
    {
    }

    #endregion

    #region Txt

    public void Set_PossessionAmountTxt(string amount)
    {
        possessionAmountTxt.text = amount;
    }

    public void Set_NecessaryAmountTxt(string amount)
    {
        necessaryAmountTxt.text = $"-{amount}";
    }

    #endregion

    #region Play

    public void Play_Failure()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(necessaryAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(necessaryAmountTxt.transform.DOScale(1f, 0.25f));
    }

    public void Play_Convert()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(possessionAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(possessionAmountTxt.transform.DOScale(1f, 0.25f));
    }

    #endregion

    #region Condition

    public void Set_Condition(bool can)
    {
        conditionIconImg.sprite = StaticResourceManager.instance.BuildReso.cvtMaterialConditionIcon.Get_Special(can);
        lineCg.alpha = can ? 1f : 0.2f;
    }

    #endregion

    #region Language

    public void Set_Language()
    {
        var words = StaticResourceManager.instance.staticWords;
        possessionTxt.text = words.GetLanguage(121); 
        necessaryTxt.text = words.GetLanguage(122);
    }

    #endregion
}
