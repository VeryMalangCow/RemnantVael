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
    [SerializeField] private GameObject NoneSyncArrowPanelGO;
    [SerializeField] private OwnBtnEUIController NoneSyneToggleBtn;

    [Space(10)]
    [Header("-- Panel (Inven Or NoneSyne)")]
    [SerializeField] private ScrollPanelEUIController NoneSyncScrollPanelEUI;
    [SerializeField] private GameObject InventoryPanel_InventoryGO;
    [SerializeField] private GameObject InventoryPanel_NoneSyneGO;

    [Space(10)]
    [Header("* Panel (NoneSyne)")]
    [SerializeField] private GameObject NoneSyncPanel_EmptyGO;
    [SerializeField] private GameObject NoneSyncPanel_ExistGO;
    [SerializeField] private Transform NoneSyncItemParentTF;
    [SerializeField] private GameObject NoneSyncItemPrefab;
    [SerializeField] private OwnBtnEUIController NoneSyneBuyBtn;
    [SerializeField] private GameObject NoneSyncCanBuyGO;
    [SerializeField] private TMP_Text NoneSyncCanBuyTxt;
    [SerializeField] private GameObject NoneSyncCannotBuyGO;


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
    [SerializeField] private TMP_Text NoneSyncTxt;
    [SerializeField] private TMP_Text NoneSyncUseTxt;

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

    // None Sync
    [HideInInspector] private List<AllyNoneSynergySlotEUIController> NoneSyncItemEUIList;
    [HideInInspector] private List<AllyNoneSynergySlotEUIController> UsingNoneSyncItemEUIList;
    [HideInInspector] private List<int> SelectedNoneSyncIDList;

    [HideInInspector] private static readonly Vector2 NoneSyncItemOffset = new Vector2(78, -80);
    [HideInInspector] private static readonly float NoneSyncItemInterval = 134;
    [HideInInspector] private static readonly int NoneSyncRowAmount = 4;

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

        NoneSyncScrollPanelEUI.Offset();

        // Picked Panel
        PickedPanelSlotEUI.Offset();
        PickedPanelSlotEUI.ownerUIController = this;

        PickedPanelSlotEUI.item.Offset();
        PickedPanelSlotEUI.item.ownerUIController = this;

        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            PickedPanelSynergyEUIList[i].ownerUIController = this;
            PickedPanelSynergyEUIList[i].Offset();
        }

        ModuleDetailExtraRTOpen = ModuleDetailExtraRT.sizeDelta;

        // Toggle
        ToggleBtn.ownerUIController = this;
        ToggleBtn.Offset();

        NoneSyneToggleBtn.ownerUIController = this;
        NoneSyneToggleBtn.Offset();

        // Player Sync
        PlayerSyncSlotEUIList = new List<SynergySlotEUIController>();
        for (int i = 0; i < PlayerSynergySlotParentTF.childCount; i++)
        {
            if (PlayerSynergySlotParentTF.GetChild(i).TryGetComponent(out SynergySlotEUIController ssEui))
            {
                ssEui.ownerUIController = this;
                ssEui.Offset();
                PlayerSyncSlotEUIList.Add(ssEui);
            }
        }

        // Buy
        BuyBtnEUI.ownerUIController = this;
        BuyBtnEUI.Offset();

        NoneSyncItemEUIList = new List<AllyNoneSynergySlotEUIController>();
        UsingNoneSyncItemEUIList = new List<AllyNoneSynergySlotEUIController>();
        SelectedNoneSyncIDList = new List<int>();

        // None Sync
        NoneSyncCanBuyTxt.text = ResourceManager.instance.Get_StaticWord(115);
        NoneSyneBuyBtn.ownerUIController = this;
        NoneSyneBuyBtn.Offset();
    }

    private void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.currentChargedBettery
            .Subscribe(_Value =>
            {
                Set_ChargedBetteryUI(_Value, NeedChargedBettery);
            });
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (Is_Interact_Msg()) return true;

        if (CurrentBtn == null || AllyModuleUpgradeController.UsingShop == null) return true;

        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;
        if (Is_Interact_ToggleBtn()) return true;
        if (Is_Interact_NoneSyneToggleBtn()) return true;
        if (Is_Interact_ModuleInInventory()) return true;
        if (Is_Interact_ModulePickSynergy()) return true;
        if (Is_Interact_PlayerSync()) return true;
        if (Try_Interact_Buy()) return true;
        if (Is_Interact_NoneSyncSelect()) return true;
        if (Is_Interact_NoneSyncBu()) return true;

        return false;
    }

    #endregion

    #region Interact (Picked)

    private bool Is_Interact_ModulePickSynergy()
    {
        if (CurrentBtn is AllySynergySlotEUIController eui)
        {
            if (!PickedModule.isEquipped)
            {
                // 사운드
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                eui.Set_SelectChange();
                Set_BuyBtn();           
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Toggle)

    private bool Is_Interact_ToggleBtn()
    {
        if (CurrentBtn == ToggleBtn)
        {
            Set_ToggleAllyPlayerSyncPanel();

            return true;
        }

        return false;
    }

    private bool Is_Interact_NoneSyneToggleBtn()
    {
        if (CurrentBtn == NoneSyneToggleBtn)
        {
            Set_ToggleNoneSyncPanel();

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
            SetOn_PlayerSynergyDesc(ssEui.id);
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

    #region Interact (None Sync)

    private bool Is_Interact_NoneSyncSelect()
    {
        if (CurrentBtn is AllyNoneSynergySlotEUIController eui)
        {
            eui.Set_SelectChange();

            if (eui.Get_IsOn())
                DevTool.Add_InList(SelectedNoneSyncIDList, eui.Get_ID());
            else
                DevTool.Remove_InList(SelectedNoneSyncIDList, eui.Get_ID());

            Set_NoneSyncAmountTxt(CurrentPickedAlly.Get_HadNoneSyncAmount(), Get_CurrentNeedNoneSync());
            Set_NoneSyncBuyBtn();

            return true;
        }

        return false;
    }

    private bool Is_Interact_NoneSyncBu()
    {
        if (CurrentBtn is OwnBtnEUIController eui && eui == NoneSyneBuyBtn)
        {
            if (Can_NoneSyncBuy())
            {
                Buy_FromNoneSync();
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

                AllyModuleUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);
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

        Set_NoneSynePanel();

        Set_BuyBtn();
    }

    #endregion

    #region None Sync (Buy)

    // 현재 선택한 Sync에서 필요한 NoneSync
    private int Get_CurrentNeedNoneSync()
    {
        return SelectedNoneSyncIDList.Count * AllyController.noneSyncNeedOneBuy;
    }

    // Sync를 살 수 있는가
    private bool Can_NoneSyncBuy()
    {
        if (CurrentPickedAlly == null) return false;

        return SelectedNoneSyncIDList.Count > 0 &&
            CurrentPickedAlly.Get_HadNoneSyncAmount() >= Get_CurrentNeedNoneSync() &&
            !AllyModuleUpgradeController.UsingShop.IsBroken;
    }

    // None Sync 패널의 버튼 활성화/비활성화 (살 수 있느냐에 따라)
    private void Set_NoneSyncBuyBtn()
    {
        if (Can_NoneSyncBuy())
        {
            NoneSyncCanBuyGO.SetActive(true);
            NoneSyncCannotBuyGO.SetActive(false);
        }
        else
        {
            NoneSyncCanBuyGO.SetActive(false);
            NoneSyncCannotBuyGO.SetActive(true);
        }
    }

    // None Sync 구매
    private void Buy_FromNoneSync()
    {
        // 추가
        CurrentPickedAlly.Add_Sync(SelectedNoneSyncIDList);

        // 재화 소모
        int needGoods = Get_CurrentNeedNoneSync();
        CurrentPickedAlly.Use_HadNoneSyncAmount(needGoods);
        AllyModuleUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);

        // 소비 효과
        Play_UseTxt(NoneSyncUseTxt, needGoods, 30f);

        // Extra 창에 State UI
        Set_AllyState(CurrentPickedAlly);

        // Extra 창에 Sync UI
        Set_AllySync(CurrentPickedAlly);

        // 패널 다시 세팅
        Set_NoneSynePanel();

    }

    #endregion

    #region None Sync (Toggle)

    // 현재 None Sync 패널에 진입할 수 있는지?
    private bool Can_EnterNoneSyncPanel()
    {
        if (CurrentPickedAlly.Get_HadNoneSyncAmount() >= AllyController.noneSyncNeedOneBuy)
        {
            return true;
        }

        return false;
    }

    // 진입 가능한지에 따라 Arrow, Toggle Btn 키고 끄기
    private void Set_NoneSyncEnterPanel()
    {
        if (Can_EnterNoneSyncPanel())
            SetOn_NoneSyncEnterPanel();
        else
            SetOff_NoneSyncEnterPanel();
    }

    // Arrow, Toggle Btn 키기
    private void SetOn_NoneSyncEnterPanel()
    {
        NoneSyncArrowPanelGO.gameObject.SetActive(true);
    }

    // Arrow, Toggle Btn 끄기
    private void SetOff_NoneSyncEnterPanel()
    {
        NoneSyncArrowPanelGO.gameObject.SetActive(false);

        if (InventoryPanel_NoneSyneGO.activeSelf)
            SetOn_ToggleNoneSyncPanel(false);
        
    }

    #endregion

    #region None Sync (Panel)

    // 현재 추가 획득할 수 있는 NoneSync 패널 세팅
    private void Set_NoneSynePanel()
    {
        if (CurrentPickedAlly == null)
        {
            Set_NoneSyncAmountTxt();
            SetOn_NoneSyneEmptyPanel(true);
            SetOff_NoneSyncEnterPanel();
        }
        else
        {
            Set_NoneSyncAmountTxt(CurrentPickedAlly.Get_HadNoneSyncAmount());
            SetOn_NoneSyneEmptyPanel(false);

            Dictionary<int, int> noFullSyncData = CurrentPickedAlly.Get_NoFullSyncData();
            Debug.Log(noFullSyncData.Count);
            Gen_NoneSyncItemEUI(noFullSyncData.Count);
            SetOff_AllNoneSyncEUI();
            SetOn_NoneSyncEUI(noFullSyncData); 
            Set_NoneSyncEnterPanel();
        }

        Set_NoneSyncBuyBtn();
    }

    // 현재 가지고 있는 NoneSync 개수
    private void Set_NoneSyncAmountTxt(int _Amount = -1, int _Need = 0)
    {
        string result = "";
        if (_Amount == -1)
        {
            result += "?";
        }
        else
        {
            result += CurrentPickedAlly.Get_HadNoneSyncAmount().ToString();
            
            if (_Need != 0)
            {
                result += $" <color=#933C8E>- {_Need}</color>";
            }
        }

        NoneSyncTxt.text = result;
    }

    // 필요한 만큼 생성하기
    private void Gen_NoneSyncItemEUI(int _NeedAmount)
    {
        if (NoneSyncItemEUIList.Count < _NeedAmount)
        {
            int genAmount = _NeedAmount - NoneSyncItemEUIList.Count;

            for (int i = 0; i < genAmount; i++)
            {
                AllyNoneSynergySlotEUIController genEUI = DevTool.Get_ComponentTType<AllyNoneSynergySlotEUIController>(
                    Instantiate(NoneSyncItemPrefab, NoneSyncItemParentTF));
                genEUI.ownerUIController = this;
                genEUI.Offset();

                NoneSyncItemEUIList.Add(genEUI);
            }
        }
    }

    // 모두 끄기
    private void SetOff_AllNoneSyncEUI()
    {
        for (int i = 0; i < NoneSyncItemEUIList.Count; i++)
        {
            NoneSyncItemEUIList[i].gameObject.SetActive(false);
            NoneSyncItemEUIList[i].Set_Select(false);
        }
    }

    // 키기
    private void SetOn_NoneSyncEUI(Dictionary<int, int> _ApplySyncData)
    {
        UsingNoneSyncItemEUIList.Clear();
        SelectedNoneSyncIDList.Clear();

        float lastY = 0;
        int index = 0;
        foreach(KeyValuePair<int, int> pair in _ApplySyncData)
        {
            int id = pair.Key;
            NoneSyncItemEUIList[index].gameObject.SetActive(true);
            NoneSyncItemEUIList[index].rt.anchoredPosition = 
                NoneSyncItemOffset + 
                new Vector2(
                    (index % NoneSyncRowAmount) * NoneSyncItemInterval, 
                    -((index / NoneSyncRowAmount) * NoneSyncItemInterval));
            NoneSyncItemEUIList[index].Set_SynergySlot(id);
            UsingNoneSyncItemEUIList.Add(NoneSyncItemEUIList[index]);
            lastY = NoneSyncItemEUIList[index].rt.anchoredPosition.y;
            index++;
        }

        NoneSyncScrollPanelEUI.Set_ScrollHeight(Mathf.Max(-lastY + 76, 716));
    }

    #endregion

    #region Buy

    private void Set_BuyBtn()
    {
        bool can = Can_Buy(out int goods);
        BuyBtnEUI.cg.alpha = can ? 1f : 0.5f;
        CanBuyArrowGO.gameObject.SetActive(can);

        DevTool.Set_KillTween(ModuleDetailExtraRT);
        ModuleDetailExtraRT.DOSizeDelta(can ? ModuleDetailExtraRTOpen : new Vector2(ModuleDetailExtraRTOpen.x, 0), 0.2f);

        NeedChargedBettery = can ? goods : 0;
        Set_ChargedBetteryUI(PlayerManager.instance.playerController.currentChargedBettery.Value, NeedChargedBettery);
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

        return !AllyModuleUpgradeController.UsingShop.IsBroken &&
            isExist && 
            PlayerManager.instance.playerController.currentChargedBettery.Value >= _Goods &&
            CurrentPickedProfileEUI != null &&
            PickedModulePanel_AllyGO.activeSelf;
    }

    private void Buy()
    {
        // 데이터
        PlayerManager.instance.playerController.currentChargedBettery.Value -= NeedChargedBettery;
        ModuleItemManager.instance.Remove_ModuleState(Get_CorrectMS(PickedItemEUI.slot).originalIndex);

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
        Set_NoneSynePanel();

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");
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
            ModuleItemManager.instance.Get_EquippedModuleState()
            .OrderByDescending(obj => obj.state.thisItemData.rank).ToList();

        // 존재하는 모듈 중, 장착 중이지 않은 모듈
        List<CopyModuleState> unEquippedMsList = 
            ModuleItemManager.instance.Get_ExistModuleState(equippedMsList)
            .OrderByDescending(obj => obj.state.thisItemData.rank).ToList();

        Set_CopyAllyShopMSInventory(unEquippedMsList, equippedMsList);
        Set_Picked(null);
    }

    private void Set_CopyAllyShopMSInventory(List<CopyModuleState> _UnEq, List<CopyModuleState> _Eq)
    {
        IndexData = new List<List<CoupleData<int>>>();
        CurrentData = new Dictionary<CoupleData<int>, CopyModuleState>();

        // 순서대로, Row Col 로 이중 리스트로 사용
        List<List<CopyModuleState>> combineData = DevTool.Get_RowColumeList(DevTool.Get_CombineList(_UnEq, _Eq), ModuleItemManager.rowAmount);
        
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
            // 사운드
            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            Set_PickedOnOffPanel(true);

            PickedItemEUI = _ItemEUI; // 아이템 EUI
            PickedModule = Get_CorrectMS(PickedItemEUI.slot); // MS
            PickedModuleMainChipID = ModuleItemManager.instance.Get_MainChipIDData(PickedModule.state); // MainChip

            Set_PickedInventoryUI(); // Inventory UI
            Set_PickedModuleUI(); // Module UI
            Set_PickedSynergyUI(); // Synergy UI
        }
    }

    private void Set_PickedInventoryUI()
    {
        InventoryEUI.SetOff_AllInventoryForgeSelectedUI();
        PickedItemEUI.slot.Set_ForgeSelectedTxt(true);
    }

    private void Set_PickedModuleUI()
    {
        string name = $"[ {PickedModule.state.thisItemData.name} ]";
        if (PickedModule.isEquipped) name += $" <size=75%><color=#7F7F7F>({ResourceManager.instance.Get_StaticWord(112)})</size></color>";
        PickedPanelItemNameTxt.text = name;
        PickedPanelItemRankTxt.text = $"<size=70%>(R: {PickedModule.state.thisItemData.rank})</size>";
        PickedPanelItemRankTxt.color = ResourceManager.instance.Get_AllyCardColor(PickedModule.state.thisItemData.rank - 1);
        PickedPanelSlotEUI.Set_EquipedTxt_NoneNum(PickedModule.isEquipped);
        
        PickedPanelSlotEUI.item.Set_Data(new ItemData_UIVisual(
            PickedModule.state.thisItemData.itemIcon,
            PickedModule.state.thisItemData.rank));

        PickedPanelItemLockImg.gameObject.SetActive(PickedModule.isEquipped);
    }

    private void Set_PickedSynergyUI()
    {
        for (int i = 0; i < PickedPanelSynergyEUIList.Count; i++)
        {
            MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(PickedModuleMainChipID[i]);
            PickedPanelSynergyEUIList[i].Set_SynergySlot(MDC.id, MDC.thisIcon, ResourceManager.instance.Get_MainChipBaseDesc(MDC.id));
            PickedPanelSynergyEUIList[i].Set_PlayerSynergyTxt(ModuleItemManager.instance.Get_MainChipAmount(MDC.id));
            PickedPanelSynergyEUIList[i].Set_Select(false);
            PickedPanelSynergyEUIList[i].Set_Lock(PickedModule.isEquipped);
        }
    }

    #endregion

    #region Set (Inventory Or NoneSync)

    private void Set_ToggleNoneSyncPanel()
    {
        if (InventoryPanel_NoneSyneGO.activeSelf)
            SetOn_ToggleNoneSyncPanel(false);
        else
            SetOn_ToggleNoneSyncPanel(true);
    }


    private void SetOn_ToggleNoneSyncPanel(bool _IsOn)
    {
        InventoryPanel_NoneSyneGO.SetActive(_IsOn);
        InventoryPanel_InventoryGO.SetActive(!_IsOn);
    }

    private void SetOn_NoneSyneEmptyPanel(bool _IsOn)
    {
        NoneSyncPanel_EmptyGO.SetActive(_IsOn);
        NoneSyncPanel_ExistGO.SetActive(!_IsOn);
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
        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Click_01");
    }

    private void Set_PlayerSyncState()
    {
        SetOff_PlayerSyncState();

        int orderIndex = 0;
        Dictionary<int, int> playerSync = ModuleItemManager.instance.Get_CurrentMainChipData();

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
                MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(sync.Key);
                PlayerSyncSlotEUIList[orderIndex].SetOn_SynergySlot(sync.Key, MDC.thisIcon, sync.Value);
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
        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        PlayerSynergyDescImg.gameObject.SetActive(true);

        PlayerSynergyDescImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(_ID).thisIcon;
        PlayerSynergyDescTxt.text = ResourceManager.instance.Get_MainChipBaseDesc(_ID);
    }

    #endregion

    #region Get (MS)

    private CopyModuleState Get_CorrectMS(InventorySlotEUIController _SlotBtn)
    {
        return CurrentData[IndexData[_SlotBtn.col][_SlotBtn.row]];
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

        // None Sync
        Set_NoneSynePanel();
        SetOn_ToggleNoneSyncPanel(false);

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
        LabelName = ResourceManager.instance.Get_StaticWord(95) + " " + ResourceManager.instance.Get_StaticWord(27) + " " + ResourceManager.instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tuner
        ModuleInventoryTxt.text = ResourceManager.instance.Get_StaticWord(110);
        ModuleDetailTxt.text = ResourceManager.instance.Get_StaticWord(111);

        // Buy
        BuyBtnEUI.txt.text = ResourceManager.instance.Get_StaticWord(47) + " & " + ResourceManager.instance.Get_StaticWord(105);

        // Player Sync
        PlayerSyncNameTxt.text = $"[ {ResourceManager.instance.Get_StaticWord(113)} {ResourceManager.instance.Get_StaticWord(50)} ]";
        base.Set_LanguageTxt();
    }

    #endregion
}

