using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OwnBtnEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool isCanSelect = true;

    [Space(10)]
    [Header("=== Size")]
    [HideInInspector] public RectTransform rt;

    // Owner
    [HideInInspector] public SinglePanelUIController ownerUIController;
    [HideInInspector] public Button btn;

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        btn = DevTool.Get_ComponentTType(gameObject, out Button _btn) ? _btn : null;
    }

    #endregion

    #region Pointer

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("In");
        if (!isCanSelect || btn == null || !btn.interactable) return;
        Debug.Log("Able");
        if (ownerUIController != null) ownerUIController.currentBtn = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return; 

        if (ownerUIController != null) ownerUIController.currentBtn = null;
    }


    #endregion
}
