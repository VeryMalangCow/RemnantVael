using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CvtMaterialEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Materal Part")]

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("IconImg")][SerializeField] private Image iconImg;
    [FormerlySerializedAs("ConditionIconImg")][SerializeField] private Image conditionIconImg;

    [Space(4)]
    [FormerlySerializedAs("PossessionTxt")][SerializeField] private TMP_Text possessionTxt;
    [FormerlySerializedAs("NecessaryTxt")][SerializeField] private TMP_Text necessaryTxt;

    [Space(4)]
    [FormerlySerializedAs("PossessionAmountTxt")][SerializeField] private TMP_Text possessionAmountTxt;
    [FormerlySerializedAs("NecessaryAmountTxt")][SerializeField] private TMP_Text necessaryAmountTxt;

    [Space(4)]
    [FormerlySerializedAs("LineCG")][SerializeField] private CanvasGroup lineCg;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_Language();
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
        conditionIconImg.sprite = ResourceManager.instance.cvtMaterialConditionIcon.Get_Special(can);
        lineCg.alpha = can ? 1f : 0.2f;
    }

    #endregion

    #region Language

    public void Set_Language()
    {
        possessionTxt.text = ResourceManager.instance.Get_StaticWord(121);
        necessaryTxt.text = ResourceManager.instance.Get_StaticWord(122);
    }

    #endregion
}
