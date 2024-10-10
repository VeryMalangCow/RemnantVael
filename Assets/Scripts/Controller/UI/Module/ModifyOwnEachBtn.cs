using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyOwnEachBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Size")]
    [SerializeField] private float DurTime = 0.1f;
    [SerializeField] private Vector2 TargetScale = new Vector2(1.15f, 1.15f);
    [HideInInspector] private Vector2 DefScale;
    [HideInInspector] private RectTransform ThisRT;

    // Owner

    [HideInInspector] public UIController OwnerUIController;
    [HideInInspector] public Button ThisBtn;

    #endregion

    #region Framework

    private void OnEnable()
    {
        if (TryGetComponent(out RectTransform thisRT))
        {
            ThisRT = thisRT;
            DefScale = ThisRT.localScale;
        }
        if (TryGetComponent(out Button thisBtn))
        {
            ThisBtn = thisBtn;
        }
    }

    #endregion

    #region Pointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!TryGetComponent(out Button btn) || !btn.interactable)
        {
            return;
        }

        if(DOTween.IsTweening(ThisRT))
        { DOTween.Kill(ThisRT); }

        ThisRT.DOScale(TargetScale, DurTime);

        if (OwnerUIController is BaseUpgradeUIController BU)
        { BU.CurrentBtn = this; }
        if (OwnerUIController is ModuleUpgradeUIController MU)
        { MU.CurrentBtn = this; }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DOTween.IsTweening(ThisRT))
        { DOTween.Kill(ThisRT); }

        ThisRT.DOScale(DefScale, DurTime);

        if (OwnerUIController is BaseUpgradeUIController BU)
        { BU.CurrentBtn = null; }
        if (OwnerUIController is ModuleUpgradeUIController MU)
        { MU.CurrentBtn = null; }
    }

    #endregion
}
