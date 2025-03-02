using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescMUEUIController : ElementUIController
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

    [Header("-- MainChip")]
    [SerializeField] private GameObject MainChipGO;
    [SerializeField] private Image RankLv3_LockerImg;
    [SerializeField] private Image RankLv5_LockerImg;
    [SerializeField] private Image RankLv1_MainChipImg;
    [SerializeField] private List<Image> RankLv1_MainChipAmountImgs;
    [SerializeField] private Image RankLv3_MainChipImg;
    [SerializeField] private List<Image> RankLv3_MainChipAmountImgs;
    [SerializeField] private Image RankLv5_MainChipImg;
    [SerializeField] private List<Image> RankLv5_MainChipAmountImgs;

    [Space(10)]
    [Header("=== Boost Lv")]
    [SerializeField] private Image BoostLvImg;
    [SerializeField] private ImgTxtAmountEUIController CurrentBoostLvMIAT;
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
        Color clr = PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, false);
        clr.a = 0f;
        BoostLvImg.color = clr;
        MainChipGO.gameObject.SetActive(false);

        CurrentBoostLvMIAT.Set_Amount(0);
        CurrentBoostLvMIAT.Set_Color(PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, false));
    }

    #endregion

    #region Desc

    public void SetOn_Desc(ModuleState _MS)
    {
        foreach (Transform child in this.transform)
        { child.gameObject.SetActive(true); }

        // Item
        ItemIconImg.color = new Color(1, 1, 1, 1);
        ItemIconImg.sprite = _MS.ThisItemData.ItemIcon;
        ItemNameTxt.text = _MS.ThisItemData.Name;
        ITemIntroTxt.text = _MS.ThisItemData.Description;

        // Rank
        CurrentRankImg.color = new Color(1, 1, 1, 1);
        CurrentRankImg.sprite = ModuleItemManager.Instance.Get_CorrectMUUIDescRankIcon(_MS);
        CurrentRankTxt.text = ExtraString_Rank;
        CurrentActualRankTxt.text = _MS.ThisItemData.Rank.ToString();

        // MainChip
        MainChipGO.gameObject.SetActive(true);
        RankLv1_MainChipImg.sprite =
            ModuleItemManager.Instance.Get_CorrectMainChip(_MS.ThisItemData.Rank1_ItemMainChipID).ThisIcon;
        RankLv3_MainChipImg.sprite =
            ModuleItemManager.Instance.Get_CorrectMainChip(_MS.ThisItemData.Rank3_ItemMainChipID).ThisIcon;
        RankLv5_MainChipImg.sprite =
            ModuleItemManager.Instance.Get_CorrectMainChip(_MS.ThisItemData.Rank5_ItemMainChipID).ThisIcon;

        RankLv3_LockerImg.gameObject.SetActive(true);
        RankLv5_LockerImg.gameObject.SetActive(true);


        RankLv1_MainChipAmountImgs[0].gameObject.SetActive(false);
        RankLv1_MainChipAmountImgs[1].gameObject.SetActive(false);
        RankLv1_MainChipAmountImgs[2].gameObject.SetActive(false);

        RankLv3_MainChipAmountImgs[0].gameObject.SetActive(false);
        RankLv3_MainChipAmountImgs[1].gameObject.SetActive(false);

        RankLv5_MainChipAmountImgs[0].gameObject.SetActive(false);

        if (_MS.ThisItemData.Rank >= 5)
        {
            RankLv3_LockerImg.gameObject.SetActive(false);
            RankLv5_LockerImg.gameObject.SetActive(false);

            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[2].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv3_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv5_MainChipAmountImgs[0].gameObject.SetActive(true);
        }
        else if (_MS.ThisItemData.Rank >= 3)
        {
            RankLv3_LockerImg.gameObject.SetActive(false);

            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
        }
        else
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
        }

        // Boost Lv
        Color clr = BoostLvImg.color;
        clr.a = (float)_MS.ThisItemData.BoostLv / (float)PlayerManager.Instance.PlayerController.MaxBoostLv;
        BoostLvImg.color = clr;
        CurrentBoostLvMIAT.Set_Amount(_MS.ThisItemData.BoostLv);
        CurrentBoostLvTxt.text = ExtraString_BoostLv;
        CurrentActualBoostLvTxt.text = _MS.ThisItemData.BoostLv.ToString();
    }

    public void SetOff_Desc()
    {
        // Item
        ItemIconImg.color = new Color(1, 1, 1, 0);
        ItemNameTxt.text = "-";
        ITemIntroTxt.text = "-";

        // Rank
        CurrentRankImg.color = new Color(1, 1, 1, 0);
        CurrentRankTxt.text = "-";
        CurrentActualRankTxt.text = "-";

        // MainChip
        MainChipGO.gameObject.SetActive(false);

        // Boost Lv
        Color clr = BoostLvImg.color;
        clr.a = 0;
        BoostLvImg.color = clr;

        CurrentBoostLvMIAT.Set_Amount(0);
        CurrentBoostLvTxt.text = "-";
        CurrentActualBoostLvTxt.text = "-";
    }

    #endregion
}
