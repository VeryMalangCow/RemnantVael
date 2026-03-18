using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CvtMaterialEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Materal Part")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image IconImg;
    [SerializeField] private Image ConditionIconImg;

    [Space(4)]
    [SerializeField] private TMP_Text PossessionTxt;
    [SerializeField] private TMP_Text NecessaryTxt;

    [Space(4)]
    [SerializeField] private TMP_Text PossessionAmountTxt;
    [SerializeField] private TMP_Text NecessaryAmountTxt;

    [Space(4)]
    [SerializeField] private CanvasGroup LineCG;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_Language();
    }

    #endregion

    #region Txt

    public void Set_PossessionAmountTxt(string _Amount)
    {
        PossessionAmountTxt.text = _Amount;
    }

    public void Set_NecessaryAmountTxt(string _Amount)
    {
        NecessaryAmountTxt.text = $"-{_Amount}";
    }

    #endregion

    #region Play

    public void Play_Failure()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(NecessaryAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(NecessaryAmountTxt.transform.DOScale(1f, 0.25f));
    }

    public void Play_Convert()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(PossessionAmountTxt.transform.DOScale(1.3f, 0.25f));
        seq.Append(PossessionAmountTxt.transform.DOScale(1f, 0.25f));
    }

    #endregion

    #region Condition

    public void Set_Condition(bool _Can)
    {
        ConditionIconImg.sprite = ResourceManager.instance.cvtMaterialConditionIcon.Get_Special(_Can);
        LineCG.alpha = _Can ? 1f : 0.2f;
    }

    #endregion

    #region Language

    public void Set_Language()
    {
        PossessionTxt.text = ResourceManager.instance.Get_StaticWord(121);
        NecessaryTxt.text = ResourceManager.instance.Get_StaticWord(122);
    }

    #endregion
}
