using UnityEngine;
using UnityEngine.UI;

public class InventoryItemEUIController : OwnBtnEUIController
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Partner")]
    [SerializeField] public InventorySlotEUIController ThisSlot = null;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(90, 90);

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Image ThisImg;
    [SerializeField] public Image RankImg;
    [SerializeField] private ImgTxtAmountEUIController BoostLvEUI;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisRT.sizeDelta = ThisSizeDelta;
        BoostLvEUI.Offset();
    }

    #endregion

    #region

    public void Set_Data(ItemData_UIVisual _State)
    {
        // Set Visual
        ThisImg.sprite = _State.Icon;
        RankImg.sprite = _State.RankIcon;
        RankImg.SetNativeSize();

        BoostLvEUI.Set_Amount(_State.BoostLv, 0.1f);


        // Color Set
        DevTool.Set_Color(
            PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false),
            BoostLvEUI.AmountImgs);

    }



    #endregion
}
