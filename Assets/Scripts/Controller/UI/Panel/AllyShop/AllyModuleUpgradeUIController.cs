using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyModuleUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Inventory")]
    [SerializeField] private ScrollPanelEUIController InventoryScrollPanelEUI;
    [SerializeField] private InventoryEUIController InventoryEUI;

    [Space(10)]
    [Header("=== Picked Item")]
    [SerializeField] private OwnBtnEUIController ToggleBtn;

    [Space(5)]
    [Header("-- Panel (Player)")]
    [SerializeField] private GameObject PickedModulePanel_PlayerGO;
    [SerializeField] private TMP_Text PlayerSyncNameTxt;

    [Space(2)]
    [Header("* Off")]
    [SerializeField] private GameObject PlayerSynergyEmptyGO;

    [Space(2)]
    [Header("* On")]
    [SerializeField] private GameObject PlayerSynergyExsitGO;
    [SerializeField] private Transform PlayerSynergySlotParentTF;
    [SerializeField] private Image PlayerSynergyDescImg;
    [SerializeField] private TMP_Text PlayerSynergyDescTxt;

    [Space(5)]
    [Header("-- Panel (Ally)")]
    [SerializeField] private GameObject PickedModulePanel_AllyGO;

    [Space(5)]
    [Header("-- Off")]
    [SerializeField] private GameObject PickedOffGO;

    [Space(5)]
    [Header("-- On")]
    [SerializeField] private GameObject PickedOnGO;

    [Space(2)]
    [Header("* Item & Slot")]
    [SerializeField] private InventorySlotEUIController PickedPanelSlotEUI;
    [SerializeField] private TMP_Text PickedPanelItemNameTxt;
    [SerializeField] private TMP_Text PickedPanelItemRankTxt;
    [SerializeField] private Image PickedPanelItemLockImg;

    [Space(2)]
    [Header("* Synergy")]
    [SerializeField] private List<AllySynergySlotEUIController> PickedPanelSynergyEUIList;

    [Space(5)]
    [Header("-- Buy")]
    [SerializeField] private OwnCGBtnEUIController BuyBtnEUI;
    [SerializeField] private GameObject CanBuyArrowGO;
    [SerializeField] private RectTransform ModuleDetailExtraRT;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] private TMP_Text ChargeBetteryTxt;
    [SerializeField] private TMP_Text ChargeBetteryUseTxt;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text ModuleInventoryTxt;
    [SerializeField] private TMP_Text ModuleDetailTxt;

    #endregion

    #region - Hide

    // Inven Data
    [HideInInspector] private List<List<CoupleData<int>>> IndexData;
    [HideInInspector] private Dictionary<CoupleData<int>, CopyModuleState> CurrentData;

    // Picked
    [HideInInspector] private InventoryItemEUIController PickedItemEUI;
    [HideInInspector] private CopyModuleState PickedModule;
    [HideInInspector] private List<int> PickedModuleMainChipID;
    [HideInInspector] private int NeedChargedBettery = 0;

    // Tuner Detail
    [HideInInspector] private Vector2 ModuleDetailExtraRTOpen;

    // Player Sync
    [SerializeField] private List<SynergySlotEUIController> PlayerSyncSlotEUIList;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_EUI();
        Offset_Subscribe();

        Set_LanguageTxt();
    }

    private void Offset_EUI()
    {
        // Inven
        InventoryScrollPanelEUI.Offset();
        InventoryEUI.Offset();
        InventoryEUI.Gen_AllSlotAndItem(this);

        // Picked Panel
        PickedPanelSlotEUI.Offset();
        PickedPanelSlotEUI.OwnerUIController = this;

        PickedPanelSlotEUI.ThisItem.Offset();
        PickedPanelSlotEUI.ThisItem.OwnerUIController = this;

        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            PickedPanelSynergyEUIList[i].OwnerUIController = this;
            PickedPanelSynergyEUIList[i].Offset();
        }

        ModuleDetailExtraRTOpen = ModuleDetailExtraRT.sizeDelta;

        // Toggle
        ToggleBtn.OwnerUIController = this;
        ToggleBtn.Offset();

        // Player Sync
        PlayerSyncSlotEUIList = new List<SynergySlotEUIController>();
        for (int i = 0; i < PlayerSynergySlotParentTF.childCount; i++)
        {
            if (PlayerSynergySlotParentTF.GetChild(i).TryGetComponent(out SynergySlotEUIController ssEui))
            {
                ssEui.OwnerUIController = this;
                ssEui.Offset();
                PlayerSyncSlotEUIList.Add(ssEui);
            }
        }

        // Buy
        BuyBtnEUI.OwnerUIController = this;
        BuyBtnEUI.Offset();
    }

    private void Offset_Subscribe()
    {
        PlayerManager.Instance.PlayerController.CurrentChargedBettery
            .Subscribe(_Value =>
            {
                Set_ChargedBetteryUI(_Value, NeedChargedBettery);
            });
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;
        if (Is_Interact_ToggleBtn()) return true;
        if (Is_Interact_ModuleInInventory()) return true;
        if (Is_Interact_ModulePickSynergy()) return true;
        if (Is_Interact_PlayerSync()) return true;
        if (Try_Interact_Buy()) return true;

        return false;
    }

    #endregion

    #region Interact (Picked)

    private bool Is_Interact_ModulePickSynergy()
    {
        if (CurrentBtn is AllySynergySlotEUIController eui)
        {
            if (!PickedModule.IsEquipped)
            {
                eui.Set_SelectChange();
                Set_BuyBtn();           
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Picked Ally Or Player)

    private bool Is_Interact_ToggleBtn()
    {
        if (CurrentBtn == ToggleBtn)
        {
            Set_ToggleAllyPlayerSyncPanel();

            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Player Sync)

    private bool Is_Interact_PlayerSync()
    {
        if (CurrentBtn is SynergySlotEUIController ssEui && PlayerSyncSlotEUIList.Contains(ssEui))
        {
            Debug.Log("맞음");
            SetOn_PlayerSynergyDesc(ssEui.ID);
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Inven)

    private bool Is_Interact_ModuleInInventory()
    {
        if (CurrentBtn is InventoryItemEUIController eui && eui == CurrentItemBtn)
        {
            if (PickedItemEUI != eui)
            {
                Set_Picked(eui);
                Set_BuyBtn();
            }
            
            return true;
        }        

        return false;
    }

    #endregion

    #region Interact (Buy)

    private bool Try_Interact_Buy()
    {
        if (CurrentBtn == BuyBtnEUI)
        {
            if (Can_Buy(out int goods))
            {
                Buy();
                Set_Picked(null);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Set (Profile)

    protected override void Pick_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (IsTweening) return;

        base.Pick_AllyProfile(_EUI);

        Set_BuyBtn();
    }

    #endregion

    #region Buy

    private void Set_BuyBtn()
    {
        bool can = Can_Buy(out int goods);
        BuyBtnEUI.ThisCG.alpha = can ? 1f : 0.5f;
        CanBuyArrowGO.gameObject.SetActive(can);

        DevTool.Set_KillTween(ModuleDetailExtraRT);
        ModuleDetailExtraRT.DOSizeDelta(can ? ModuleDetailExtraRTOpen : new Vector2(ModuleDetailExtraRTOpen.x, 0), 0.2f);

        NeedChargedBettery = can ? goods : 0;
        Set_ChargedBetteryUI(PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value, NeedChargedBettery);
    }

    private bool Can_Buy(out int _Goods)
    {
        _Goods = -2;

        if (PickedItemEUI == null) return false;

        bool isExist = false;
        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            if (PickedPanelSynergyEUIList[i].Get_IsOn())
            {
                isExist = true;
                _Goods += 2;
            }
        }

        return isExist && 
            PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value >= _Goods &&
            CurrentPickedProfileEUI != null &&
            PickedModulePanel_AllyGO.activeSelf;
    }

    private void Buy()
    {
        // 데이터
        PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value -= NeedChargedBettery;
        ModuleItemManager.Instance.Remove_ModuleState(Get_CorrectMS(PickedItemEUI.ThisSlot).OriginalIndex);

        CurrentPickedAlly.Add_Sync(Get_PickedSyncList());

        // 소비 효과
        Play_UseTxt(ChargeBetteryUseTxt, NeedChargedBettery, 30f);

        // Extra 창에 State UI
        Set_AllyState(CurrentPickedAlly);

        // Extra 창에 Sync UI
        Set_AllySync(CurrentPickedAlly);

        // 구매한 모듈로 재세팅
        Set_Inventory();
        Set_Picked(null);
        Set_BuyBtn();
    }

    private List<int> Get_PickedSyncList()
    {
        List<int> result = new List<int>();

        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            if (PickedPanelSynergyEUIList[i].Get_IsOn())
            {
                result.Add(PickedPanelSynergyEUIList[i].Get_ID());
            }
        }

        return result;
    }

    #endregion

    #region Pay

    private void Set_ChargedBetteryUI(int _Amount, int _NeedAmount = 0)
    {
        if (_NeedAmount == 0)
        {
            ChargeBetteryTxt.text = _Amount.ToString();
        }
        else
        {
            ChargeBetteryTxt.text = $"{_Amount} <color=#933C8E>- {_NeedAmount}</color>";
        }
    }

    #endregion

    #region Set (Inventory)

    private void Set_Inventory()
    {
        // 존재하는 모듈 중, 장착 중인 모듈
        List<CopyModuleState> equippedMsList = 
            ModuleItemManager.Instance.Get_EquippedModuleState()
            .OrderByDescending(obj => obj.MS.ThisItemData.Rank).ToList();

        // 존재하는 모듈 중, 장착 중이지 않은 모듈
        List<CopyModuleState> unEquippedMsList = 
            ModuleItemManager.Instance.Get_ExistModuleState(equippedMsList)
            .OrderByDescending(obj => obj.MS.ThisItemData.Rank).ToList();

        Set_CopyAllyShopMSInventory(unEquippedMsList, equippedMsList);
        Set_Picked(null);
    }

    private void Set_CopyAllyShopMSInventory(List<CopyModuleState> _UnEq, List<CopyModuleState> _Eq)
    {
        IndexData = new List<List<CoupleData<int>>>();
        CurrentData = new Dictionary<CoupleData<int>, CopyModuleState>();

        // 순서대로, Row Col 로 이중 리스트로 사용
        List<List<CopyModuleState>> combineData = DevTool.Get_RowColumeList(DevTool.Get_CombineList(_UnEq, _Eq), ModuleItemManager.RowAmount);
        
        for (int i = 0; i < combineData.Count; i++)
        {
            IndexData.Add(new List<CoupleData<int>>());
            for (int j = 0; j < combineData[i].Count; j++)
            {
                CoupleData<int> indexData = new CoupleData<int>(i, j);
                IndexData[i].Add(indexData);
                CurrentData.Add(indexData, combineData[i][j]);
            }
        }

        // UI Set
        InventoryEUI.SetOff_AllInventoryEquipedUI();

        InventoryEUI.Set_InventoryUI(combineData);

        return;
    }

    #endregion

    #region Set (Picked)

    private void Set_PickedOnOffPanel(bool _OnOff)
    {
        PickedOnGO.SetActive(_OnOff);
        PickedOffGO.SetActive(!_OnOff);
    }

    private void Set_Picked(InventoryItemEUIController _ItemEUI)
    {
        if (_ItemEUI == null)
        {
            Set_PickedOnOffPanel(false);

            PickedItemEUI = null;
            PickedModule = null;
            PickedModuleMainChipID = null;

            InventoryEUI.SetOff_AllInventoryForgeSelectedUI();
        }
        else
        {
            Set_PickedOnOffPanel(true);

            PickedItemEUI = _ItemEUI; // 아이템 EUI
            PickedModule = Get_CorrectMS(PickedItemEUI.ThisSlot); // MS
            PickedModuleMainChipID = ModuleItemManager.Instance.Get_MainChipIDData(PickedModule.MS); // MainChip

            Set_PickedInventoryUI(); // Inventory UI
            Set_PickedModuleUI(); // Module UI
            Set_PickedSynergyUI(); // Synergy UI
        }
    }

    private void Set_PickedInventoryUI()
    {
        InventoryEUI.SetOff_AllInventoryForgeSelectedUI();
        PickedItemEUI.ThisSlot.Set_ForgeSelectedTxt(true);
    }

    private void Set_PickedModuleUI()
    {
        string name = $"[ {PickedModule.MS.ThisItemData.Name} ]";
        if (PickedModule.IsEquipped) name += $" <size=75%><color=#7F7F7F>({ResourceManager.Instance.Get_StaticWord(112)})</size></color>";
        PickedPanelItemNameTxt.text = name;
        PickedPanelItemRankTxt.text = $"<size=70%>(R: {PickedModule.MS.ThisItemData.Rank})</size>";
        PickedPanelItemRankTxt.color = UnitManager.Instance.AllyCardColorList[PickedModule.MS.ThisItemData.Rank - 1];
        PickedPanelSlotEUI.Set_EquipedTxt_NoneNum(PickedModule.IsEquipped);
        
        PickedPanelSlotEUI.ThisItem.Set_Data(new ItemData_UIVisual(
            PickedModule.MS.ThisItemData.ItemIcon,
            PickedModule.MS.ThisItemData.Rank));

        PickedPanelItemLockImg.gameObject.SetActive(PickedModule.IsEquipped);
    }

    private void Set_PickedSynergyUI()
    {
        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            MainChipData MDC = ModuleItemManager.Instance.Get_CorrectMainChip(PickedModuleMainChipID[i]);
            PickedPanelSynergyEUIList[i].Set_SynergySlot(MDC.ID, MDC.ThisIcon, ResourceManager.Instance.Get_MainChipBaseDesc(MDC.ID));
            PickedPanelSynergyEUIList[i].Set_PlayerSynergyTxt(ModuleItemManager.Instance.Get_MainChipAmount(MDC.ID));
            PickedPanelSynergyEUIList[i].Set_Select(false);
            PickedPanelSynergyEUIList[i].Set_Lock(PickedModule.IsEquipped);
        }
    }

    #endregion

    #region Set (Picked Ally Or Player)

    private void Set_ToggleAllyPlayerSyncPanel()
    {
        if (PickedModulePanel_AllyGO.activeSelf)
            SetOn_ToggleAllySyncPanel(false);
        else
            SetOn_ToggleAllySyncPanel(true);
    }

    
    private void SetOn_ToggleAllySyncPanel(bool _IsOn)
    {
        PickedModulePanel_AllyGO.SetActive(_IsOn);
        PickedModulePanel_PlayerGO.SetActive(!_IsOn);
    }

    private void Set_PlayerSyncState()
    {
        SetOff_PlayerSyncState();

        int orderIndex = 0;
        Dictionary<int, int> playerSync = ModuleItemManager.Instance.Get_CurrentMainChipData();

        if (playerSync.Count <= 0)
        {
            PlayerSynergyEmptyGO.gameObject.SetActive(true);
            PlayerSynergyExsitGO.gameObject.SetActive(false);
        }
        else
        {
            PlayerSynergyEmptyGO.gameObject.SetActive(false);
            PlayerSynergyExsitGO.gameObject.SetActive(true);

            foreach (KeyValuePair<int, int> sync in playerSync)
            {
                MainChipData MDC = ModuleItemManager.Instance.Get_CorrectMainChip(sync.Key);
                PlayerSyncSlotEUIList[orderIndex].SetOn_SynergySlot(sync.Key, MDC.ThisIcon, sync.Value);
                orderIndex++;
            }
        }   
    }

    private void SetOff_PlayerSyncState()
    {
        for (int i = 0; i < PlayerSyncSlotEUIList.Count; i++)
            PlayerSyncSlotEUIList[i].SetOff_SynergySlot();
    }

    #endregion

    #region Set (Player Sync Desc)

    private void SetOff_PlayerSynergyDesc()
    {
        PlayerSynergyDescImg.gameObject.SetActive(false);
    }

    private void SetOn_PlayerSynergyDesc(int _ID)
    {
        PlayerSynergyDescImg.gameObject.SetActive(true);

        PlayerSynergyDescImg.sprite = ModuleItemManager.Instance.Get_CorrectMainChip(_ID).ThisIcon;
        PlayerSynergyDescTxt.text = ResourceManager.Instance.Get_MainChipBaseDesc(_ID);
    }

    #endregion

    #region Get (MS)

    private CopyModuleState Get_CorrectMS(InventorySlotEUIController _SlotBtn)
    {
        return CurrentData[IndexData[_SlotBtn.Col][_SlotBtn.Row]];
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        ThisDurEUI.Set_Dur(AllyModuleUpgradeController.UsingShop.CurrentDur);

        Set_Inventory();
        Set_BuyBtn();

        // Toggle
        Set_PlayerSyncState();
        SetOn_ToggleAllySyncPanel(true);

        SetOff_PlayerSynergyDesc();
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        AllyModuleUpgradeController.UsingShop = null;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(95) + " " + ResourceManager.Instance.Get_StaticWord(27) + " " + ResourceManager.Instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tuner
        ModuleInventoryTxt.text = ResourceManager.Instance.Get_StaticWord(110);
        ModuleDetailTxt.text = ResourceManager.Instance.Get_StaticWord(111);

        // Buy
        BuyBtnEUI.ThisTxt.text = ResourceManager.Instance.Get_StaticWord(47) + " & " + ResourceManager.Instance.Get_StaticWord(105);

        // Player Sync
        PlayerSyncNameTxt.text = $"[ {ResourceManager.Instance.Get_StaticWord(113)} {ResourceManager.Instance.Get_StaticWord(50)} ]";
        base.Set_LanguageTxt();
    }

    #endregion
}

