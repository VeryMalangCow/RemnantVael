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
    [SerializeField] private Image itemIconImg;
    [SerializeField] public TMP_Text itemNameTxt;
    [SerializeField] public TMP_Text itemIntroTxt;

    [Space(10)]
    [Header("=== Rank")]
    [SerializeField] private Image currentRankImg;
    [SerializeField] public TMP_Text currentRankTxt;
    [SerializeField] public TMP_Text currentActualRankTxt;

    [Space(5)]
    [Header("-- MainChip")]
    [SerializeField] private GameObject mainChipGo;

    [Space(3)]
    [Header("* Locker")]
    [SerializeField] private Image rankLv3_LockerImg;
    [SerializeField] private Image rankLv5_LockerImg;

    [Space(3)]
    [Header("* Rank")]
    [SerializeField] private Image rankLv1_MainChipImg;
    [SerializeField] private List<Image> rankLv1_MainChipAmountImgs;
    [SerializeField] private TMP_Text rankLv1_Name;

    [SerializeField] private Image rankLv3_MainChipImg;
    [SerializeField] private List<Image> rankLv3_MainChipAmountImgs;
    [SerializeField] private TMP_Text rankLv3_Name;

    [SerializeField] private Image rankLv5_MainChipImg;
    [SerializeField] private List<Image> rankLv5_MainChipAmountImgs;
    [SerializeField] private TMP_Text rankLv5_Name;

    #endregion

    #region - Hide

    [HideInInspector] private string extraString_Rank;

    #endregion

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        // string
        extraString_Rank = ResourceManager.instance.Get_StaticWord(25);
    }

    #endregion

    #region Offset

    public override void Offset()
    {

        itemIconImg.color = new Color(1, 1, 1, 0);
        currentRankImg.color = new Color(1, 1, 1, 0);

        mainChipGo.gameObject.SetActive(false);
    }

    #endregion

    #region Desc Element

    private void SetOn_Item(int id, string name, string desc)
    {
        itemIconImg.color = new Color(1, 1, 1, 1);
        itemIconImg.sprite = ModuleItemManager.instance.Get_CorrectItemIcon(id);
        itemNameTxt.text = name;
        itemIntroTxt.text = desc;
    }

    private void SetOn_Rank(int rank)
    {
        currentRankImg.color = new Color(1, 1, 1, 1);
        currentRankImg.sprite = ResourceManager.instance.Get_DescRankIcon(rank);
        currentRankTxt.text = extraString_Rank;
        currentActualRankTxt.text = rank.ToString();
    }

    private void SetOn_MainChip(int rank, int r1Id, int r3Id, int r5Id)
    {
        mainChipGo.gameObject.SetActive(true);

        rankLv1_MainChipImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(r1Id).thisIcon;
        rankLv3_MainChipImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(r3Id).thisIcon;
        rankLv5_MainChipImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(r5Id).thisIcon;

        rankLv1_Name.text = ModuleItemManager.instance.Get_CorrectMainChip(r1Id).name;
        rankLv3_Name.text = ModuleItemManager.instance.Get_CorrectMainChip(r3Id).name;
        rankLv5_Name.text = ModuleItemManager.instance.Get_CorrectMainChip(r5Id).name;

        SetOff_AllLocker();
        SetOff_AllMainChipImgsTxts();

        SetOn_Locker(rank);
        SetOn_MainChipImgsTxts(rank);
    }

    private void SetOn_MainChipImgsTxts(int rank)
    {
        if (rank >= 5)
        {
            rankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            rankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);
            rankLv1_MainChipAmountImgs[2].gameObject.SetActive(true);

            rankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);
            rankLv3_MainChipAmountImgs[1].gameObject.SetActive(true);

            rankLv5_MainChipAmountImgs[0].gameObject.SetActive(true);

            rankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";
            rankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";
            rankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";

            DevTool.Set_AlphaColor(rankLv1_Name, 1f);
            DevTool.Set_AlphaColor(rankLv3_Name, 1f);
            DevTool.Set_AlphaColor(rankLv5_Name, 1f);

        }
        else if (rank >= 3)
        {
            rankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);
            rankLv1_MainChipAmountImgs[1].gameObject.SetActive(true);

            rankLv3_MainChipAmountImgs[0].gameObject.SetActive(true);

            rankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";
            rankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";
                                               
            rankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(55)})</color></size>";

            DevTool.Set_AlphaColor(rankLv1_Name, 1f);
            DevTool.Set_AlphaColor(rankLv3_Name, 1f);
        }
        else
        {
            rankLv1_MainChipAmountImgs[0].gameObject.SetActive(true);

            rankLv1_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(54)})</color></size>";
                                               
            rankLv3_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(55)})</color></size>";
            rankLv5_Name.text += $"\n<size=70%><color=#FFFFFF>({ResourceManager.instance.Get_StaticWord(55)})</color></size>";

            DevTool.Set_AlphaColor(rankLv1_Name, 1f);
        }
    }

    private void SetOff_AllMainChipImgsTxts()
    {
        for (int i = 0; i < rankLv1_MainChipAmountImgs.Count; i++)
        {
            rankLv1_MainChipAmountImgs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < rankLv3_MainChipAmountImgs.Count; i++)
        {
            rankLv3_MainChipAmountImgs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < rankLv5_MainChipAmountImgs.Count; i++)
        {
            rankLv5_MainChipAmountImgs[i].gameObject.SetActive(false);
        }


        DevTool.Set_AlphaColor(rankLv1_Name, 0.3f);
        DevTool.Set_AlphaColor(rankLv3_Name, 0.3f);
        DevTool.Set_AlphaColor(rankLv5_Name, 0.3f);
    }

    private void SetOn_Locker(int rank)
    {
        if (rank >= 5)
        {
            rankLv3_LockerImg.gameObject.SetActive(false);
            rankLv5_LockerImg.gameObject.SetActive(false);
        }
        else if (rank >= 3)
        {
            rankLv3_LockerImg.gameObject.SetActive(false);
        }
    }

    private void SetOff_AllLocker()
    {
        rankLv3_LockerImg.gameObject.SetActive(true);
        rankLv5_LockerImg.gameObject.SetActive(true);
    }

    #endregion

    #region Desc

    public void SetOn_Desc(ModuleState moduleState)
    {
        if (moduleState == null) return;

        foreach (Transform child in this.transform)
        { child.gameObject.SetActive(true); }

        // Item
        SetOn_Item(moduleState.thisItemData.id, moduleState.thisItemData.name, moduleState.thisItemData.desc);

        // Rank
        SetOn_Rank(moduleState.thisItemData.rank);

        // MainChip
        SetOn_MainChip(moduleState.thisItemData.rank,
            moduleState.thisItemData.r1_MainChipID, moduleState.thisItemData.r3_MainChipID, moduleState.thisItemData.r5_MainChipID);
    }

    public void SetOff_Desc()
    {
        // Item
        itemIconImg.color = new Color(1, 1, 1, 0);
        itemNameTxt.text = "-";
        itemIntroTxt.text = "-";

        // Rank
        currentRankImg.color = new Color(1, 1, 1, 0);
        currentRankTxt.text = "-";
        currentActualRankTxt.text = "-";

        // MainChip
        mainChipGo.gameObject.SetActive(false);
    }

    #endregion

    #region Get

    public List<Component> Get_MainColorList()
    {
        List<Component> result = new List<Component>()
        {
            itemNameTxt,
            currentActualRankTxt,
            rankLv1_Name,
            rankLv3_Name,
            rankLv5_Name
        };


        return result;
    }

    public List<Component> Get_SubColorList()
    {
        List<Component> result = new List<Component>()
        {
            itemIntroTxt,
            currentRankTxt
        };

        return result;
    }


    #endregion
}
