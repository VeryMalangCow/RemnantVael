using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyEachTab : MonoBehaviour, IScrollHandler
{
    #region Value

    [Header("=== RT")]
    [SerializeField] public RectTransform ThisPanelRT;
    [SerializeField] public Vector2 ThisOriginalPanelSize;
    [SerializeField] public Button ThisTabBtn;

    [Header("=== Scroll Bar")]
    [SerializeField] private Scrollbar ThisTabScrollbar;
    [SerializeField] private RectTransform ActualMovableRT;
    [SerializeField] private float VisibleY = 725;
    [HideInInspector] private float ActualAreaY;
    [HideInInspector] private float MovableY;

    #endregion

    #region Framework

    private void Start()
    {
        ActualAreaY = ActualMovableRT.rect.height;
        MovableY = ActualAreaY - VisibleY;

        ThisTabScrollbar.size = Mathf.Clamp((VisibleY / ActualAreaY), 0f, 1f);
        ThisTabScrollbar.OnValueChangedAsObservable()
            .Subscribe(_Value =>
            {
                float targetY = MovableY * _Value;
                ActualMovableRT.anchoredPosition = new Vector2(ActualMovableRT.anchoredPosition.x, targetY);
            });
    }

    public void OnEnable()
    {
        OnReset();
    }

    public void OnReset()
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
