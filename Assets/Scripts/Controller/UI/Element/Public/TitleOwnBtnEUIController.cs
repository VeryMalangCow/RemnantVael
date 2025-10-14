using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleOwnBtnEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool IsCanSelect = true;

    [Space(10)]
    [Header("=== Size")]
    [HideInInspector] public RectTransform ThisRT;

    // Owner
    [HideInInspector] public TitleLobbyUIController OwnerUIController;
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

        if (OwnerUIController != null)
        {
            OwnerUIController.Set_CurrentBtn(this);
            OwnerUIController.Set_CurrentMouseBtn(this);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        OwnerUIController.Set_CurrentMouseBtn(null);
    }


    #endregion

    #region Set

    public void Set_SelectOnThis(float _DurTime = 0.2f)
    {
        Set_SelectedThis(200f, _DurTime);
    }

    public void Set_SelectOffThis(float _DurTime = 0.2f)
    {
        Set_SelectedThis(180f, _DurTime);
    }

    private void Set_SelectedThis(float _Height, float _DurTime = 0.2f)
    {
        DevTool.Set_KillTween(ThisRT);

        ThisRT.DOSizeDelta(new Vector2(ThisRT.rect.width, _Height), _DurTime);
    }

    #endregion
}
