using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Progress Bar")]

    [Space(10)]
    [Header("=== Bar")]
    [SerializeField] public Image AfterImg;
    [SerializeField] public Image ActualImg;

    [Space(10)]
    [Header("=== Liner")]
    [SerializeField] public RectTransform ActualImgLiner;

    [Space(10)]
    [Header("=== Text")]
    [SerializeField] private TMP_Text Txt;

    [Space(10)]
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

        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        ActualImgLiner.localPosition = Get_LinerPos();
    }

    #endregion

    #region Set

    #region Max

    public void Set_MaxFillRT(float _SizeX, float _DurTime = 0.1f)
    {
        ThisRT.DOSizeDelta(new Vector2(_SizeX, ThisRT.sizeDelta.y), _DurTime);

        MiddleRT.DOSizeDelta(new Vector2(_SizeX + PlusSizeMiddleRTX, MiddleRT.sizeDelta.y), _DurTime);
        RightRT.DOAnchorPos(new Vector2(_SizeX + PlusPosRightRTX, RightRT.anchoredPosition.y), _DurTime);
    }

    #endregion

    #region Current

    // 부드럽게 변동
    public void Set_FillImgSmooth(float _CurrentValue, float _MaxValue)
    {

        DevTool.Set_KillTween(ActualImg.fillAmount);

        ActualImg.DOFillAmount(_CurrentValue / _MaxValue, 0.1f);

        if (isActiveAndEnabled)
            StartCoroutine(Set_FillImgSmooth_AfterImg_Cor());
        else
            AfterImg.fillAmount = 0;

        if (Txt != null)
        { Txt.text = (int)_CurrentValue + "<size=70%>/" + (int)_MaxValue + "</size>"; }
    }

    // After 이미지
    private IEnumerator Set_FillImgSmooth_AfterImg_Cor(float _DelayTime = 0.5f, float _DurTime = 0.2f)
    {
        DOTween.Kill(AfterImg.fillAmount);

        yield return new WaitForSeconds(_DelayTime);

        if (ActualImg.fillAmount >= AfterImg.fillAmount)
        {
            AfterImg.fillAmount = ActualImg.fillAmount;
        }
        else
        {
            AfterImg.DOFillAmount(ActualImg.fillAmount, _DurTime);
        }
    }


    // 최대로 채우기
    public void Set_FillFullImgSmooth(float _DurTime)
    {
        DevTool.Set_KillTween(ActualImg.fillAmount);

        ActualImg.DOFillAmount(1f, _DurTime)
            .SetEase(Ease.Linear)
            .OnComplete(() => { AfterImg.fillAmount = 1f; });
    }

    #endregion

    #endregion

    #region Unique -> Simplify

    public void Set_NoNum()
    {
        if (Txt == null) return;

        Txt.text = "";
    }

    #endregion

    #region Liner

    private Vector2 Get_LinerPos()
    {
        if (ThisRT == null) return Vector2.zero;

        return new Vector2(ThisRT.sizeDelta.x * ActualImg.fillAmount, 0f);
    }

    #endregion

}
