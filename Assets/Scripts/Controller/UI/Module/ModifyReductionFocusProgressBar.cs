using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyReductionFocusProgressBar : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Bar")]
    [SerializeField] private Image AfterImageEP_Img;

    [SerializeField] private Image ActualEP_Img;
    [SerializeField] private RectTransform ActualEP_ImgLiner;

    [Header("=== Text")]
    [SerializeField] private TMP_Text Txt;

    [Header("=== Extra")]
    [SerializeField] private RectTransform MiddleRT;
    [SerializeField] private float PlusSizeMiddleRTX;
    [SerializeField] private RectTransform RightRT;
    [SerializeField] private float PlusPosRightRTX;

    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        SetFillImgSmooth(0, 1);

        TryGetComponent(out RectTransform thisRT);
        ThisRT = thisRT;
    }

    private void LateUpdate()
    {
        ActualEP_ImgLiner.localPosition = GetLinerPos();
    }

    #endregion

    #region Unique -> Max

    public void SetMaxFillRT(float _SizeX)
    {
        if(ThisRT == null && TryGetComponent(out RectTransform thisRT))
        {
            ThisRT = thisRT;
        }

        ThisRT.DOSizeDelta(new Vector2(_SizeX, ThisRT.sizeDelta.y), 0.1f);

        MiddleRT.DOSizeDelta(new Vector2(_SizeX + PlusSizeMiddleRTX, MiddleRT.sizeDelta.y), 0.1f);
        RightRT.DOAnchorPos(new Vector2(_SizeX + PlusPosRightRTX, RightRT.anchoredPosition.y), 0.1f);
    }

    #endregion

    #region Unique -> Current

    public void SetFillImgSmooth(float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(ActualEP_Img.fillAmount);
        ActualEP_Img.DOFillAmount(fillValue, 0.1f);

        StartCoroutine(SetFillImgSmooth_AfterImg());
        if (Txt != null)
        { Txt.text = (int)_CurrentValue + "<size=70%>/" + (int)_MaxValue + "</size>"; }
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


    public void SetFillFullImgSmooth(float _DurTime)
    {
        DOTween.Kill(ActualEP_Img.fillAmount);
        ActualEP_Img.DOFillAmount(1f, _DurTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                AfterImageEP_Img.fillAmount = 1f;
            });
    }

    #endregion

    #region Liner

    private Vector2 GetLinerPos()
    {
        if (ThisRT != null)
        {
            float targetX = ThisRT.sizeDelta.x * ActualEP_Img.fillAmount;
            return new Vector2(targetX, 0f);
        }

        return Vector2.zero;
    }

    #endregion

}
