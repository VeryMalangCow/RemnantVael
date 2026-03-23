using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPanelEUIController : ElementUIController, IScrollHandler
{
    #region Value

    [Header("=== RT")]
    [SerializeField] public RectTransform panelRt;

    [Header("=== Scroll Bar")]
    [SerializeField] public Scrollbar tabScrollbar;
    [SerializeField] public RectTransform actualMovableRt;
    [SerializeField] protected float visibleY = 725;
    [HideInInspector] protected float actualAreaY;
    [HideInInspector] protected float movableY;

    IDisposable disposable = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_ScrollPanel(visibleY);
    }

    #endregion

    #region Framework

    public void OnEnable()
    {
        Reset_ScrollBar();
    }

    public void Reset_ScrollBar()
    {
        tabScrollbar.value = 0f;
    }

    #endregion

    #region Set

    public void Set_ScrollHeight(float height)
    {
        actualMovableRt.sizeDelta = new Vector2(0, height);
        Set_ScrollPanel();
    }

    public void Set_ScrollPanel(float visibleY)
    {
        if (disposable != null)
        {
            disposable.Dispose();
        }

        actualAreaY = actualMovableRt.rect.height;
        movableY = actualAreaY - visibleY;

        tabScrollbar.size = Mathf.Clamp((visibleY / actualAreaY), 0f, 1f);
        disposable = tabScrollbar.OnValueChangedAsObservable()
            .Subscribe(_Value =>
            {
                float targetY = movableY * _Value;
                actualMovableRt.anchoredPosition = new Vector2(actualMovableRt.anchoredPosition.x, targetY);
            });
    }

    public void Set_ScrollPanel()
    {
        Set_ScrollPanel(visibleY);
    }

    #endregion

    #region Wheel

    public void OnScroll(PointerEventData eventData)
    {
        tabScrollbar.value = Mathf.Clamp((tabScrollbar.value + (-eventData.scrollDelta.y * 0.1f)), 0f, 1f);
    }

    #endregion
}
