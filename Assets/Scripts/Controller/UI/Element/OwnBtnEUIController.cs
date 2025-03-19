using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OwnBtnEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool IsCanSelect = true;

    [Space(10)]
    [Header("=== Size")]
    [HideInInspector] protected RectTransform ThisRT;

    // Owner

    [HideInInspector] public PanelUIController OwnerUIController;
    [HideInInspector] public Button ThisBtn;

    #endregion

    #region Framework

    public override void Offset()
    {
        if (TryGetComponent(out RectTransform thisRT))
        {
            ThisRT = thisRT;
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
