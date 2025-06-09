using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllyProfileEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Profile")]
    [SerializeField] private Image FaceImg;
    [SerializeField] private TMP_Text NameTxt;

    [HideInInspector] private AllyController ThisAlly = null;
    [HideInInspector] private AllyShopUIController AllyOwnerUIController;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        AllyOwnerUIController = DevTool.Can_CastingTType(OwnerUIController, out AllyShopUIController owner) ? owner : null;
    }

    #endregion

    #region Set

    public void Reset_Profile()
    {
        ThisAlly = null;
    }

    public void Set_Profile(AllyController _Ally, float _YPos)
    {
        if (_Ally == null) return;

        ThisAlly = _Ally;
        FaceImg.sprite = _Ally.Get_FrontFaceImg();
        NameTxt.text = _Ally.Get_Name();
        ThisRT.anchoredPosition = new Vector2(0, _YPos);
    }

    #endregion

    #region Get

    public AllyController Get_ThisAlly()
    {
        return ThisAlly;
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        if (OwnerUIController != null) AllyOwnerUIController.Select_AllyProfile(this);
    }

    #endregion
}
