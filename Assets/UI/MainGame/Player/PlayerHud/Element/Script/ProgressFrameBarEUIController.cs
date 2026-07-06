using UnityEngine;

public class ProgressFrameBarEUIController : ProgressBarEUIController
{
    [Space(20)]
    [Header("<><><><><> Frame Progress Bar")]
    [SerializeField] private RectTransform middleRt;
    [SerializeField] private float baseMiddleWidth;
    [SerializeField] private RectTransform rightRt;
    [SerializeField] private float baseRightPos;

    public override void SetMaxUI(float maxValue)
    {
        middleRt.sizeDelta = new Vector2(baseMiddleWidth + (maxValue * ratio), middleRt.sizeDelta.y);
        rightRt.anchoredPosition = new Vector2(baseRightPos + (maxValue * ratio), middleRt.anchoredPosition.y);
    }
}
