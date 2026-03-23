using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleOwnBtnEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool isCanSelect = true;

    [Space(10)]
    [Header("=== Size")]
    [HideInInspector] public RectTransform rt;

    // Owner
    [HideInInspector] public TitleLobbyUIController ownerUIController;
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
        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerUIController != null)
        {
            ownerUIController.Set_CurrentBtn(this);
            ownerUIController.Set_CurrentMouseBtn(this);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return;

        ownerUIController.Set_CurrentMouseBtn(null);
    }


    #endregion

    #region Set

    public void Set_SelectOnThis(float durTime = 0.2f)
    {
        Set_SelectedThis(200f, durTime);
    }

    public void Set_SelectOffThis(float durTime = 0.2f)
    {
        Set_SelectedThis(180f, durTime);
    }

    private void Set_SelectedThis(float height, float durTime = 0.2f)
    {
        DevTool.Set_KillTween(rt);

        rt.DOSizeDelta(new Vector2(rt.rect.width, height), durTime);
    }

    #endregion
}
