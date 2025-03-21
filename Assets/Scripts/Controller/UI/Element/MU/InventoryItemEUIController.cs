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

    public void Set_Data(Sprite _ThisIcon, Sprite _RankImg, int _BoostLv)
    {
        ThisImg.sprite = _ThisIcon;
        RankImg.sprite = _RankImg;
        RankImg.SetNativeSize();

        BoostLvEUI.Offset();

        DevTool.Set_Color(
            PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false),
            BoostLvEUI.AmountImgs);

        BoostLvEUI.Set_Amount(_BoostLv, 0.1f);

    }

    public override void Offset()
    {
        base.Offset();

        if (TryGetComponent(out Image img))
        { ThisImg = img; }

        ThisRT.sizeDelta = ThisSizeDelta;

        BoostLvEUI.Offset();
    }


    #endregion
}
