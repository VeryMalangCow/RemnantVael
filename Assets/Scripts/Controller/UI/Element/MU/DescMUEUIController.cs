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
    [SerializeField] public TMP_Text ItemIntroTxt;

    [Space(10)]
    [Header("=== Rank")]
    [SerializeField] private Image CurrentRankImg;
    [SerializeField] public TMP_Text CurrentRankTxt;
    [SerializeField] public TMP_Text CurrentActualRankTxt;

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
    [SerializeField] private TMP_Text RankLv1_Name;
    [SerializeField] private Image RankLv3_MainChipImg;
    [SerializeField] private List<Image> RankLv3_MainChipAmountImgs;
    [SerializeField] private TMP_Text RankLv3_Name;
    [SerializeField] private Image RankLv5_MainChipImg;
    [SerializeField] private List<Image> RankLv5_MainChipAmountImgs;
    [SerializeField] private TMP_Text RankLv5_Name;

    #endregion

    #region - Hide

    [HideInInspector] private string ExtraString_Rank;

    #endregion

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        // string
        ExtraString_Rank = ResourceManager.Instance.Get_StaticWord(25);
    }

    #endregion

    #region Offset

    public override void Offset()
    {

        ItemIconImg.color = new Color(1, 1, 1, 0);
        CurrentRankImg.color = new Color(1, 1, 1, 0);

        MainChipGO.gameObject.SetActive(false);
    }

    #endregion

    #region Desc Element

    private void SetOn_Item(int _ID, string _Name, string _Desc)
    {
        ItemIconImg.color = new Color(1, 1, 1, 1);
        ItemIconImg.sprite = ModuleItemManager.Instance.Get_CorrectItemIcon(_ID);
        ItemNameTxt.text = _Name;
        ItemIntroTxt.text = _Desc;
    }

    private void SetOn_Rank(int _Rank)
    {
        CurrentRankImg.color = new Color(1, 1, 1, 1);
        CurrentRankImg.sprite = ResourceManager.Instance.Get_DescRankIcon(_Rank);
        CurrentRankTxt.text = ExtraString_Rank;
        CurrentActualRankTxt.text = _Rank.ToString();
    }

    private void SetOn_MainChip(int _Rank, int _R1_ID, int _R3_ID, int _R5_ID)
    {
        MainChipGO.gameObject.SetActive(true);

        RankLv1_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R1_ID).ThisIcon;
        RankLv3_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R3_ID).ThisIcon;
        RankLv5_MainChipImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_R5_ID).ThisIcon;

        RankLv1_Name.text = ModuleItemManager.Instance.Get_CorrectMainChip(_R1_ID).Name;
        RankLv3_Name.text = ModuleItemManager.Instance.Get_CorrectMainChip(_R3_ID).Name;
        RankLv5_Name.text = ModuleItemManager.Instance.Get_CorrectMainChip(_R5_ID).Name;

        SetOff_AllLocker();
        SetOff_AllMainChipImgsTxts();

        SetOn_Locker(_Rank);
        SetOn_MainChipImgsTxts(_Rank);
    }

    private void SetOn_MainChipImgsTxts(int _Rank)
    {
        if (_Rank >= 5)
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[2].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv3_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv5_MainChipAmountImgs[0].gameObject.SetActive(true);

            RankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";
            RankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";
            RankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";

            DevTool.Set_AlphaColor(RankLv1_Name, 1f);
            DevTool.Set_AlphaColor(RankLv3_Name, 1f);
            DevTool.Set_AlphaColor(RankLv5_Name, 1f);

        }
        else if (_Rank >= 3)
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            RankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);

            RankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);

            RankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";
            RankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";
                                               
            RankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(55)})</color></size>";

            DevTool.Set_AlphaColor(RankLv1_Name, 1f);
            DevTool.Set_AlphaColor(RankLv3_Name, 1f);
        }
        else
        {
            RankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);

            RankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(54)})</color></size>";
                                               
            RankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(55)})</color></size>";
            RankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.Instance.Get_StaticWord(55)})</color></size>";

            DevTool.Set_AlphaColor(RankLv1_Name, 1f);
        }
    }

    private void SetOff_AllMainChipImgsTxts()
    {
        for (int i = 0; i < RankLv1_MainChipAmountImgs.Count; i++)
        {
            RankLv1_MainChipAmountImgs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < RankLv3_MainChipAmountImgs.Count; i++)
        {
            RankLv3_MainChipAmountImgs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < RankLv5_MainChipAmountImgs.Count; i++)
        {
            RankLv5_MainChipAmountImgs[i].gameObject.SetActive(false);
        }


        DevTool.Set_AlphaColor(RankLv1_Name, 0.3f);
        DevTool.Set_AlphaColor(RankLv3_Name, 0.3f);
        DevTool.Set_AlphaColor(RankLv5_Name, 0.3f);
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
    }

    public void SetOff_Desc()
    {
        // Item
        ItemIconImg.color = new Color(1, 1, 1, 0);
        ItemNameTxt.text = "-";
        ItemIntroTxt.text = "-";

        // Rank
        CurrentRankImg.color = new Color(1, 1, 1, 0);
        CurrentRankTxt.text = "-";
        CurrentActualRankTxt.text = "-";

        // MainChip
        MainChipGO.gameObject.SetActive(false);
    }

    #endregion

    #region Get

    public List<Component> Get_MainColorList()
    {
        List<Component> result = new List<Component>()
        {
            ItemNameTxt,
            CurrentActualRankTxt,
            RankLv1_Name,
            RankLv3_Name,
            RankLv5_Name
        };


        return result;
    }

    public List<Component> Get_SubColorList()
    {
        List<Component> result = new List<Component>()
        {
            ItemIntroTxt,
            CurrentRankTxt
        };

        return result;
    }


    #endregion
}
