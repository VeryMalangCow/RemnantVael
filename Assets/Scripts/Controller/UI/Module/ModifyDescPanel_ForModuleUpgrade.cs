using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyDescPanel_ForModuleUpgrade : UIModule
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Desc _ For ModuleUpgrade")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] private Image ItemIconImg;
    [SerializeField] public TMP_Text ItemNameTxt;
    [SerializeField] public TMP_Text ITemIntroTxt;

    [Space(10)]
    [Header("=== Rank")]
    [SerializeField] private Image CurrentRankImg;
    [SerializeField] public TMP_Text CurrentRankTxt;
    [SerializeField] public TMP_Text CurrentActualRankTxt;
    [SerializeField] private string ExtraString_Rank;

    [Space(10)]
    [Header("=== Boost Lv")]
    [SerializeField] private Image BoostLvImg;
    [SerializeField] private ModifyImgAmountAndTxt CurrentBoostLvMIAT;
    [SerializeField] public TMP_Text CurrentBoostLvTxt;
    [SerializeField] public TMP_Text CurrentActualBoostLvTxt;
    [SerializeField] private string ExtraString_BoostLv;



    #endregion

    #region Offset

    public override void Offset()
    {
        CurrentBoostLvMIAT.Offset();

        ItemIconImg.color = new Color(1, 1, 1, 0);
        CurrentRankImg.color = new Color(1, 1, 1, 0);
        Color clr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, false);
        clr.a = 0f;
        BoostLvImg.color = clr;

        CurrentBoostLvMIAT.SetAmount(0);
        CurrentBoostLvMIAT.SetColor(PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, false));
    }

    #endregion

    #region Desc

    public void SetDesc(PassiveSkill _PS)
    {
        foreach (Transform child in this.transform)
        { child.gameObject.SetActive(true); }

        // Item
        ItemIconImg.color = new Color(1, 1, 1, 1);
        ItemIconImg.sprite = _PS.ThisItemData.Sprite;
        ItemNameTxt.text = _PS.ThisItemData.Name;
        ITemIntroTxt.text = _PS.ThisItemData.Description;

        // Rank
        CurrentRankImg.color = new Color(1, 1, 1, 1);
        CurrentRankImg.sprite = BoostItemManager.Instance.GetCorrectMUUIDescRankIcon(_PS);
        CurrentRankTxt.text = ExtraString_Rank;
        CurrentActualRankTxt.text = _PS.ThisItemData.Rank.ToString();

        // Boost Lv
        
        Color clr = BoostLvImg.color;
        clr.a = (float)_PS.ThisItemData.BoostLv / (float)PlayerManager.Instance.PlayerController.MaxBoostLv;
        BoostLvImg.color = clr;
        CurrentBoostLvMIAT.SetAmount(_PS.ThisItemData.BoostLv);
        CurrentBoostLvTxt.text = ExtraString_BoostLv;
        CurrentActualBoostLvTxt.text = _PS.ThisItemData.BoostLv.ToString();
    }

    public void SetDescOff()
    {
        // Item
        ItemIconImg.color = new Color(1, 1, 1, 0);
        ItemNameTxt.text = "-";
        ITemIntroTxt.text = "-";

        // Rank
        CurrentRankImg.color = new Color(1, 1, 1, 0);
        CurrentRankTxt.text = "-";
        CurrentActualRankTxt.text = "-";

        // Boost Lv
        Color clr = BoostLvImg.color;
        clr.a = 0;
        BoostLvImg.color = clr;

        CurrentBoostLvMIAT.SetAmount(0);
        CurrentBoostLvTxt.text = "-";
        CurrentActualBoostLvTxt.text = "-";
    }

    #endregion
}
