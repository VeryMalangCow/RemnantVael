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
    [SerializeField] public Image afterImg;
    [SerializeField] public Image actualImg;

    [Space(10)]
    [Header("=== Liner")]
    [SerializeField] public RectTransform actualImgLiner;

    [Space(10)]
    [Header("=== Text")]
    [SerializeField] private TMP_Text txt;

    [Space(10)]
    [Header("=== Extra")]
    [SerializeField] private RectTransform middleRT;
    [SerializeField] private float plusSizeMiddleRTX;
    [SerializeField] private RectTransform rightRt;
    [SerializeField] private float plusPosRightRtX;

    [HideInInspector] private RectTransform rt;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_FillImgSmooth(0, 1);

        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        actualImgLiner.localPosition = Get_LinerPos();
    }

    #endregion

    #region Set

    #region Max

    public void Set_MaxFillRT(float sizeX, float durTime = 0.1f)
    {
        rt.DOSizeDelta(new Vector2(sizeX, rt.sizeDelta.y), durTime);

        middleRT.DOSizeDelta(new Vector2(sizeX + plusSizeMiddleRTX, middleRT.sizeDelta.y), durTime);
        rightRt.DOAnchorPos(new Vector2(sizeX + plusPosRightRtX, rightRt.anchoredPosition.y), durTime);
    }

    #endregion

    #region Current

    // 부드럽게 변동
    public void Set_FillImgSmooth(float currentValue, float maxValue)
    {

        DevTool.Set_KillTween(actualImg.fillAmount);

        actualImg.DOFillAmount(currentValue / maxValue, 0.1f);

        if (isActiveAndEnabled)
            StartCoroutine(Set_FillImgSmooth_AfterImg_Cor());
        else
            afterImg.fillAmount = 0;

        if (txt != null)
        { txt.text = (int)currentValue + "<size=70%>/" + (int)maxValue + "</size>"; }
    }

    // After 이미지
    private IEnumerator Set_FillImgSmooth_AfterImg_Cor(float delayTime = 0.5f, float durTime = 0.2f)
    {
        DOTween.Kill(afterImg.fillAmount);

        yield return new WaitForSeconds(delayTime);

        if (actualImg.fillAmount >= afterImg.fillAmount)
        {
            afterImg.fillAmount = actualImg.fillAmount;
        }
        else
        {
            afterImg.DOFillAmount(actualImg.fillAmount, durTime);
        }
    }


    // 최대로 채우기
    public void Set_FillFullImgSmooth(float durTime)
    {
        DevTool.Set_KillTween(actualImg.fillAmount);

        actualImg.DOFillAmount(1f, durTime)
            .SetEase(Ease.Linear)
            .OnComplete(() => { afterImg.fillAmount = 1f; });
    }

    #endregion

    #endregion

    #region Unique -> Simplify

    public void Set_NoNum()
    {
        if (txt == null) return;

        txt.text = "";
    }

    #endregion

    #region Liner

    private Vector2 Get_LinerPos()
    {
        if (rt == null) return Vector2.zero;

        return new Vector2(rt.sizeDelta.x * actualImg.fillAmount, 0f);
    }

    #endregion

}
