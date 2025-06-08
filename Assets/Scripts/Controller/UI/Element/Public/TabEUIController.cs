using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabEUIController : ElementUIController, IScrollHandler
{
    #region Value

    [Header("=== RT")]
    [SerializeField] public RectTransform ThisPanelRT;
    [SerializeField] public TabBtnEUIController ThisTabBtn;

    [Header("=== Scroll Bar")]
    [SerializeField] public Scrollbar ThisTabScrollbar;
    [SerializeField] private RectTransform ActualMovableRT;
    [SerializeField] private float VisibleY = 725;
    [HideInInspector] private float ActualAreaY;
    [HideInInspector] private float MovableY;

    IDisposable disposable = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisTabBtn.Offset();

        Set_ScrollPanel(VisibleY);
    }

    #endregion

    #region Scroll

    public void Set_ScrollPanel(float _VisibleY)
    {
        if (disposable != null)
        {
            disposable.Dispose();
        }

        ActualAreaY = ActualMovableRT.rect.height;
        MovableY = ActualAreaY - _VisibleY;

        ThisTabScrollbar.size = Mathf.Clamp((_VisibleY / ActualAreaY), 0f, 1f);
        disposable = ThisTabScrollbar.OnValueChangedAsObservable()
            .Subscribe(_Value =>
            {
                float targetY = MovableY * _Value;
                ActualMovableRT.anchoredPosition = new Vector2(ActualMovableRT.anchoredPosition.x, targetY);
            });
    }

    #endregion

    #region Framework

    public void OnEnable()
    {
        Reset_ScrollBar();
    }

    public void Reset_ScrollBar()
    {
        ThisTabScrollbar.value = 0f;
    }

    #endregion

    #region Wheel

    public void OnScroll(PointerEventData eventData)
    {
        ThisTabScrollbar.value = Mathf.Clamp((ThisTabScrollbar.value + (-eventData.scrollDelta.y * 0.1f)), 0f, 1f);
    }

    #endregion
}
