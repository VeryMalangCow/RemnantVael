using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModifyReductionFocusProgressBar : MonoBehaviour
{
    [SerializeField] private Image AfterImageEP_Img;
    [SerializeField] private Image ActualEP_Img;



    #region FillAmount

    protected void SetFillImgSmooth(Image _Img, float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(_Img.fillAmount);
        _Img.DOFillAmount(fillValue, 0.1f);
    }

    public void SetFillImgSmooth_EP()
    {
        SetFillImgSmooth(
            ActualEP_Img,
            PlayerManager.Instance.PlayerController.CurrentEP.Value,
            PlayerManager.Instance.LifeState.MaxEP);
    }

    #endregion
}
