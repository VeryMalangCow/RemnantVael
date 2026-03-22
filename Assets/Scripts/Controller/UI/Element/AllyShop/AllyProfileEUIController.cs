using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AllyProfileEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Profile")]
    [FormerlySerializedAs("FaceImg")][SerializeField] private Image faceImg;
    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;

    [HideInInspector] private AllyController ally = null;
    [HideInInspector] private AllyShopUIController allyOwnerUIController;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        allyOwnerUIController = DevTool.Can_CastingTType(ownerUIController, out AllyShopUIController owner) ? owner : null;
    }

    #endregion

    #region Set

    public void Reset_Profile()
    {
        ally = null;
    }

    public void Set_Profile(AllyController ally, float yPos)
    {
        if (ally == null) return;

        this.ally = ally;
        faceImg.sprite = ally.Get_FrontFaceImg();
        nameTxt.text = ally.Get_Name();
        rt.anchoredPosition = new Vector2(0, yPos);
    }

    #endregion

    #region Get

    public AllyController Get_ThisAlly()
    {
        return ally;
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerUIController != null) allyOwnerUIController.Select_AllyProfile(this);
    }

    #endregion
}
