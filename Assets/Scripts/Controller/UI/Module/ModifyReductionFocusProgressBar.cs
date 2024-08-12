using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModifyReductionFocusProgressBar : MonoBehaviour
{
    #region Value

    [SerializeField] private Image AfterImageEP_Img;
    [SerializeField] private Image ActualEP_Img;

    #endregion

    #region FillAmount

    public void SetFillImgSmooth(float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(ActualEP_Img.fillAmount);
        ActualEP_Img.DOFillAmount(fillValue, 0.1f);

        StartCoroutine(SetFillImgSmooth_AfterImg());
    }

    private IEnumerator SetFillImgSmooth_AfterImg()
    {
        DOTween.Kill(AfterImageEP_Img.fillAmount);

        yield return new WaitForSeconds(0.5f);

        if (ActualEP_Img.fillAmount >= AfterImageEP_Img.fillAmount)
        {
            AfterImageEP_Img.fillAmount = ActualEP_Img.fillAmount;
        }
        else
        {
            AfterImageEP_Img.DOFillAmount(ActualEP_Img.fillAmount, 0.2f);
        }
    }

    #endregion
}
