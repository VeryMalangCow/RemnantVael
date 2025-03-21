using UnityEngine;
using UnityEngine.UI;

public class InventoryItemEUIController : OwnBtnEUIController
{
    #region Value 

    [Header("=== RT")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(90, 90);

    [Header("=== Inner Component")]
    [SerializeField] public Image RankImg;
    [SerializeField] private ImgTxtAmountEUIController BoostLvEUI;

    // Component
    [HideInInspector] public Image ThisImg;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;

        ThisRT.sizeDelta = ThisSizeDelta;

        BoostLvEUI.Offset();
    }

    #endregion

    #region

    public void Set_Data(State_ItemData _State)
    {
        // Set Visual
        ThisImg.sprite = ModuleItemManager.Instance.Get_CorrectItemIcon(_State.ID);

        RankImg.sprite = ModuleItemManager.Instance.Get_CorrectRankIcon(_State.Rank);
        RankImg.SetNativeSize();

        BoostLvEUI.Set_Amount(_State.BoostLv, 0.1f);


        // Color Set
        DevTool.Set_Color(
            PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false),
            BoostLvEUI.AmountImgs);

    }



    #endregion
}
