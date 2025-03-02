using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Bar")]
    [SerializeField] public Image AfterImg;

    [SerializeField] public Image ActualImg;
    [SerializeField] public RectTransform ActualImgLiner;

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
        Set_FillImgSmooth(0, 1);

        TryGetComponent(out RectTransform thisRT);
        ThisRT = thisRT;
    }

    private void LateUpdate()
    {
        ActualImgLiner.localPosition = Get_LinerPos();
    }

    #endregion

    #region Unique -> Max

    public void Set_MaxFillRT(float _SizeX)
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

    public void Set_FillImgSmooth(float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(ActualImg.fillAmount);
        ActualImg.DOFillAmount(fillValue, 0.1f);

        StartCoroutine(Set_FillImgSmooth_AfterImg_Cor());
        if (Txt != null)
        { Txt.text = (int)_CurrentValue + "<size=70%>/" + (int)_MaxValue + "</size>"; }
    }

    private IEnumerator Set_FillImgSmooth_AfterImg_Cor()
    {
        DOTween.Kill(AfterImg.fillAmount);

        yield return new WaitForSeconds(0.5f);

        if (ActualImg.fillAmount >= AfterImg.fillAmount)
        {
            AfterImg.fillAmount = ActualImg.fillAmount;
        }
        else
        {
            AfterImg.DOFillAmount(ActualImg.fillAmount, 0.2f);
        }
    }


    public void Set_FillFullImgSmooth(float _DurTime)
    {
        DOTween.Kill(ActualImg.fillAmount);
        ActualImg.DOFillAmount(1f, _DurTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                AfterImg.fillAmount = 1f;
            });
    }

    #endregion

    #region Unique -> Simplify

    public void Set_NoNum()
    {
        if (Txt != null)
        { Txt.text = ""; }
    }

    #endregion

    #region Liner

    private Vector2 Get_LinerPos()
    {
        if (ThisRT != null)
        {
            float targetX = ThisRT.sizeDelta.x * ActualImg.fillAmount;
            return new Vector2(targetX, 0f);
        }

        return Vector2.zero;
    }

    #endregion

}
