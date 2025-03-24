using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescMUEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Desc ModuleUpgrade")]

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

    [Space(5)]
    [Header("-- MainChip")]
    [SerializeField] private GameObject MainChipGO;

    [Space(3)]
    [Header("* Locker")]
    [SerializeField] private Image RankLv3_LockerImg;
    [SerializeField] private Image RankLv5_LockerImg;

    [Space(3)]
    [Header("* Rank")]
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

    #endregion

    #region Offset

    public override void Offset()
    {
        CurrentBoostLvMIAT.Offset();

        ItemIconImg.color = new Color(1, 1, 1, 0);
        CurrentRankImg.color = new Color(1, 1, 1, 0);
        Color clr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        clr.a = 0f;
        BoostLvImg.color = clr;
        MainChipGO.gameObject.SetActive(false);

        CurrentBoostLvMIAT.Set_Amount(0);

        DevTool.Set_Color(clr, CurrentBoostLvMIAT.AmountImgs);
    }

    #endregion

    #region Desc Element

    private void SetOn_Item(int _ID, string _Name, string _Desc)
    {
        ItemIconImg.color = new Color(1, 1, 1, 1);
        ItemIconImg.sprite = ModuleItemManager.Instance.Get_CorrectItemIcon(_ID);
        ItemNameTxt.text = _Name;
        ITemIntroTxt.text = _Desc;
    }

    private void SetOn_Rank(int _Rank)
    {
        CurrentRankImg.color = new Color(1, 1, 1, 1);
        CurrentRankImg.sprite = ModuleItemManager.Instance.Get_CorrectDescRankIcon(_Rank);
        CurrentRankTxt.text = ExtraString_Rank;
        CurrentActualRankTxt.text = _Rank.ToString();
    }

    private void SetOn_MainChip(int _Rank, int _R1_ID, int _R3_ID, int _R5_ID)
    {
        MainChipGO.gameObject.SetActive(true);

        RankLv1_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R1_ID).ThisIcon;
        RankLv3_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R3_ID).ThisIcon;
        RankLv5_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R5_ID).ThisIcon;

        SetOff_AllLocker();
        SetOff_AllMainChipImgs();

        SetOn_Locker(_Rank);
        SetOn_MainChipImgs(_Rank);
    }

    private void SetOn_MainChipImgs(int _Rank)
    {
        if (_Rank >= 5)
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[2].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv3_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv5_MainChipAmountImgs[0].gameObject.SetActive(true);
        }
        else if (_Rank >= 3)
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
        }
        else
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
        }
    }

    private void SetOff_AllMainChipImgs()
    {
        for (int i = 0; i < RankLv1_MainChipAmountImgs.Count; i++)
            RankLv1_MainChipAmountImgs[i].gameObject.SetActive(false);

        for (int i = 0; i < RankLv3_MainChipAmountImgs.Count; i++)
            RankLv3_MainChipAmountImgs[i].gameObject.SetActive(false);

        for (int i = 0; i < RankLv5_MainChipAmountImgs.Count; i++)
            RankLv5_MainChipAmountImgs[i].gameObject.SetActive(false);
    }

    private void SetOn_Locker(int _Rank)
    {
        if (_Rank >= 5)
        {
            RankLv3_LockerImg.gameObject.SetActive(false);
            RankLv5_LockerImg.gameObject.SetActive(false);
        }
        else if (_Rank >= 3)
        {
            RankLv3_LockerImg.gameObject.SetActive(false);
        }
    }

    private void SetOff_AllLocker()
    {
        RankLv3_LockerImg.gameObject.SetActive(true);
        RankLv5_LockerImg.gameObject.SetActive(true);
    }

    private void SetOn_BoostLv(int _BoostLv)
    {
        Color clr = BoostLvImg.color;
        clr.a = (float)_BoostLv / (float)PlayerController.MaxBoostLv;
        BoostLvImg.color = clr;
        CurrentBoostLvMIAT.Set_Amount(_BoostLv);
        CurrentBoostLvTxt.text = ExtraString_BoostLv;
        CurrentActualBoostLvTxt.text = _BoostLv.ToString();
    }

    #endregion

    #region Desc

    public void SetOn_Desc(ModuleState _MS)
    {
        if (_MS == null) return;

        foreach (Transform child in this.transform)
        { child.gameObject.SetActive(true); }

        // Item
        SetOn_Item(_MS.ThisItemData.ID, _MS.ThisItemData.Name, _MS.ThisItemData.Description);

        // Rank
        SetOn_Rank(_MS.ThisItemData.Rank);

        // MainChip
        SetOn_MainChip(_MS.ThisItemData.Rank,
            _MS.ThisItemData.R1_MainChipID, _MS.ThisItemData.R3_MainChipID, _MS.ThisItemData.R5_MainChipID);

        // Boost Lv
        SetOn_BoostLv(_MS.ThisItemData.BoostLv);
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

    #region Get

    public List<Component> Get_MainColorList()
    {
        List<Component> result = new List<Component>()
        {
            ItemNameTxt,
            CurrentActualRankTxt,
            CurrentActualBoostLvTxt
        };


        return result;
    }

    public List<Component> Get_SubColorList()
    {
        List<Component> result = new List<Component>()
        {
            ITemIntroTxt,
            CurrentRankTxt,
            CurrentBoostLvTxt
        };

        return result;
    }


    #endregion
}
