using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    #region Module

    #region Image

    protected void SetFillImgSmooth(Image _Img, float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(_Img.fillAmount);
        _Img.DOFillAmount(fillValue, 0.1f);
    }

    #endregion

    #endregion
}
