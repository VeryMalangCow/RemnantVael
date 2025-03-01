using UnityEngine;
using UnityEngine.UI;

public class ModifyEachInventoryItem : ModifyOwnEachBtn
{
    #region Value 

    [Header("=== RT")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(90, 90);

    [Header("=== Inner Component")]
    [SerializeField] public Image RankImg;
    [SerializeField] private ModifyImgAmountAndTxt BoostLvMIAAT;

    // Component
    [HideInInspector] public Image ThisImg;

    #endregion

    #region Offset

    public void Set_Data(Sprite _ThisIcon, Sprite _RankImg, int _BoostLv)
    {
        ThisImg.sprite = _ThisIcon;
        RankImg.sprite = _RankImg;
        RankImg.SetNativeSize();
        BoostLvMIAAT.Set_Amount(_BoostLv, 0.1f);
        BoostLvMIAAT.Set_Color(PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, false));
    }

    public override void Offset()
    {
        base.Offset();

        if (TryGetComponent(out Image img))
        { ThisImg = img; }

        ThisRT.sizeDelta = ThisSizeDelta;

        BoostLvMIAAT.Offset();
    }


    #endregion
}
