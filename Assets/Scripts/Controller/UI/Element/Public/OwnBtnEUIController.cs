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

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ThisBtn = DevTool.Get_ComponentTType(gameObject, out Button btn) ? btn : null;
    }

    #endregion

    #region Pointer

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return; 

        if (OwnerUIController != null) OwnerUIController.CurrentBtn = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return; 

        if (OwnerUIController != null) OwnerUIController.CurrentBtn = null;
    }


    #endregion
}
