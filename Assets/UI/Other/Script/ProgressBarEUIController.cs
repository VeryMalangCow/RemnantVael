using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarEUIController : ElementUIController
{
    [Space(20)]
    [Header("<><><><><> Progress Bar")]

    [SerializeField] protected RectTransform rt;
    [SerializeField] private Image barImg;
    [SerializeField] private RectTransform liner;
    [SerializeField] private TMP_Text txt;
    [SerializeField] protected float ratio;

    public override void Offset()
    {
        SetFillImg(0, 1);
    }

    public void SetFillImg(float currentValue, float maxValue)
    {
        barImg.fillAmount = currentValue / maxValue;
        liner.anchoredPosition = rt == null ? Vector2.zero : new Vector2(rt.sizeDelta.x * barImg.fillAmount, 0f);

        if (txt != null)
        { txt.text = (int)currentValue + "<size=70%>/" + (int)maxValue + "</size>"; }
    }

    public virtual void SetMaxUI(float maxValue)
    {
        rt.sizeDelta = new Vector2(maxValue * ratio, rt.sizeDelta.y);
    }

    public void SetNoNum()
    { 
        if (txt != null) txt.text = ""; 
    }
}
