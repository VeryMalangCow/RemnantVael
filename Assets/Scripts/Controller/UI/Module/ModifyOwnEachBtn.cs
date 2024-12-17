using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyOwnEachBtn : UIModule, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool IsCanSelect = true;

    [Space(10)]
    [Header("=== Size")]
    //[SerializeField] private float DurTime = 0.1f;
    [SerializeField] private Vector2 TargetScale = new Vector2(1.15f, 1.15f);
    [HideInInspector] private Vector2 DefScale;
    [HideInInspector] protected RectTransform ThisRT;

    // Owner

    [HideInInspector] public UIController OwnerUIController;
    [HideInInspector] public Button ThisBtn;

    #endregion

    #region Framework

    public override void Offset()
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
        if (!IsCanSelect)
        { return; }

        if (!TryGetComponent(out Button btn) || !btn.interactable)
        {
            return;
        }

        OwnerUIController.CurrentBtn = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect)
        { return; }

        if (!TryGetComponent(out Button btn) || !btn.interactable)
        {
            return;
        }

        OwnerUIController.CurrentBtn = null;
    }


    #endregion
}
