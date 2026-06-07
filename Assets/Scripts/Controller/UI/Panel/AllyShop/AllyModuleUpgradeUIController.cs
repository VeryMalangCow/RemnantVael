using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AllyModuleUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Inventory")]
    [SerializeField] private ScrollPanelEUIController inventoryScrollPanelEui;
    [SerializeField] private InventoryEUIController inventoryEui;
    [SerializeField] private GameObject noneSyncArrowPanelGo;
    [SerializeField] private OwnBtnEUIController noneSyneToggleBtn;

    [Space(10)]
    [Header("-- Panel (Inven Or NoneSyne)")]
    [SerializeField] private ScrollPanelEUIController noneSyncScrollPanelEui;
    [SerializeField] private GameObject inventoryPanel_InventoryGo;
    [SerializeField] private GameObject inventoryPanel_NoneSyneGo;

    [Space(10)]
    [Header("* Panel (NoneSyne)")]
    [SerializeField] private GameObject noneSyncPanel_EmptyGo;
    [SerializeField] private GameObject noneSyncPanel_ExistGo;
    [SerializeField] private Transform noneSyncItemParentTf;
    [SerializeField] private GameObject noneSyncItemPrefab;
    [SerializeField] private OwnBtnEUIController noneSyneBuyBtn;
    [SerializeField] private GameObject noneSyncCanBuyGo;
    [SerializeField] private TMP_Text noneSyncCanBuyTxt;
    [SerializeField] private GameObject noneSyncCannotBuyGo;


    [Space(10)]
    [Header("=== Picked Item")]
    [SerializeField] private OwnBtnEUIController toggleBtn;

    [Space(5)]
    [Header("-- Panel (Player)")]
    [SerializeField] private GameObject pickedModulePanel_PlayerGo;
    [SerializeField] private TMP_Text playerSyncNameTxt;

    [Space(2)]
    [Header("* Off")]
    [SerializeField] private GameObject playerSynergyEmptyGo;

    [Space(2)]
    [Header("* On")]
    [SerializeField] private GameObject playerSynergyExsitGo;
    [SerializeField] private Transform playerSynergySlotParentTf;
    [SerializeField] private Image playerSynergyDescImg;
    [SerializeField] private TMP_Text playerSynergyDescTxt;

    [Space(5)]
    [Header("-- Panel (Ally)")]
    [SerializeField] private GameObject pickedModulePanel_AllyGo;

    [Space(5)]
    [Header("-- Off")]
    [SerializeField] private GameObject pickedOffGo;

    [Space(5)]
    [Header("-- On")]
    [SerializeField] private GameObject pickedOnGo;

    [Space(2)]
    [Header("* Item & Slot")]
    [SerializeField] private InventorySlotEUIController pickedPanelSlotEui;
    [SerializeField] private TMP_Text pickedPanelItemNameTxt;
    [SerializeField] private TMP_Text pickedPanelItemRankTxt;
    [SerializeField] private Image pickedPanelItemLockImg;

    [Space(2)]
    [Header("* Synergy")]
    [SerializeField] private List<AllySynergySlotEUIController> pickedPanelSynergyEuiList;

    [Space(5)]
    [Header("-- Buy")]
    [SerializeField] private OwnCGBtnEUIController buyBtnEui;
    [SerializeField] private GameObject canBuyArrowGO;
    [SerializeField] private RectTransform moduleDetailExtraRt;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] private TMP_Text chargeBetteryTxt;
    [SerializeField] private TMP_Text chargeBetteryUseTxt;
    [SerializeField] private TMP_Text noneSyncTxt;
    [SerializeField] private TMP_Text noneSyncUseTxt;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text moduleInventoryTxt;
    [SerializeField] private TMP_Text moduleDetailTxt;

    #endregion

    #region - Hide

    // Inven Data
    [HideInInspector] private List<List<CoupleData<int>>> indexData;
    [HideInInspector] private Dictionary<CoupleData<int>, CopyModuleState> currentData;

    // Picked
    [HideInInspector] private InventoryItemEUIController pickedItemEui;
    [HideInInspector] private CopyModuleState pickedModule;
    [HideInInspector] private List<int> pickedModuleMainChipId;
    [HideInInspector] private int needChargedBettery = 0;

    // Tuner Detail
    [HideInInspector] private Vector2 ModuleDetailExtraRTOpen;

    // Player Sync
    [SerializeField] private List<SynergySlotEUIController> playerSyncSlotEuiList;

    // None Sync
    [HideInInspector] private List<AllyNoneSynergySlotEUIController> noneSyncItemEuiList;
    [HideInInspector] private List<AllyNoneSynergySlotEUIController> usingNoneSyncItemEuiList;
    [HideInInspector] private List<int> selectedNoneSyncIdList;

    [HideInInspector] private static readonly Vector2 noneSyncItemOffset = new Vector2(78, -80);
    [HideInInspector] private static readonly float noneSyncItemInterval = 134;
    [HideInInspector] private static readonly int noneSyncRowAmount = 4;

    #endregion

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        Offset_EUI();

        SetLanguageTxt();
    }

    private void Offset_EUI()
    {
        // Inven
        inventoryScrollPanelEui.Offset();
        inventoryEui.Offset();
        inventoryEui.Gen_AllSlotAndItem(this);

        noneSyncScrollPanelEui.Offset();

        // Picked Panel
        pickedPanelSlotEui.Offset();
        pickedPanelSlotEui.ownerUIController = this;

        pickedPanelSlotEui.item.Offset();
        pickedPanelSlotEui.item.ownerUIController = this;

        for (int i = 0; i < pickedPanelSynergyEuiList.Count; i++)
        {
            pickedPanelSynergyEuiList[i].ownerUIController = this;
            pickedPanelSynergyEuiList[i].Offset();
        }

        ModuleDetailExtraRTOpen = moduleDetailExtraRt.sizeDelta;

        // Toggle
        toggleBtn.ownerUIController = this;
        toggleBtn.Offset();

        noneSyneToggleBtn.ownerUIController = this;
        noneSyneToggleBtn.Offset();

        // Player Sync
        playerSyncSlotEuiList = new List<SynergySlotEUIController>();
        for (int i = 0; i < playerSynergySlotParentTf.childCount; i++)
        {
            if (playerSynergySlotParentTf.GetChild(i).TryGetComponent(out SynergySlotEUIController ssEui))
            {
                ssEui.ownerUIController = this;
                ssEui.Offset();
                playerSyncSlotEuiList.Add(ssEui);
            }
        }

        // Buy
        buyBtnEui.ownerUIController = this;
        buyBtnEui.Offset();

        noneSyncItemEuiList = new List<AllyNoneSynergySlotEUIController>();
        usingNoneSyncItemEuiList = new List<AllyNoneSynergySlotEUIController>();
        selectedNoneSyncIdList = new List<int>();

        // None Sync
        noneSyncCanBuyTxt.text = ResourceManager.instance.Get_StaticWord(115);
        noneSyneBuyBtn.ownerUIController = this;
        noneSyneBuyBtn.Offset();
    }

    public void SetChargedBetteryUI(int value)
    {
        Set_ChargedBetteryUI(value, needChargedBettery);
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (Is_Interact_Msg()) return true;

        if (currentBtn == null || AllyModuleUpgradeController.usingShop == null) return true;

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
        if (currentBtn is AllySynergySlotEUIController eui)
        {
            if (!pickedModule.isEquipped)
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
        if (currentBtn == toggleBtn)
        {
            Set_ToggleAllyPlayerSyncPanel();

            return true;
        }

        return false;
    }

    private bool Is_Interact_NoneSyneToggleBtn()
    {
        if (currentBtn == noneSyneToggleBtn)
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
        if (currentBtn is SynergySlotEUIController ssEui && playerSyncSlotEuiList.Contains(ssEui))
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
        if (currentBtn is InventoryItemEUIController eui && eui == currentItemBtn)
        {
            if (pickedItemEui != eui)
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
        if (currentBtn is AllyNoneSynergySlotEUIController eui)
        {
            eui.Set_SelectChange();

            if (eui.Get_IsOn())
                DevTool.Add_InList(selectedNoneSyncIdList, eui.Get_ID());
            else
                DevTool.Remove_InList(selectedNoneSyncIdList, eui.Get_ID());

            Set_NoneSyncAmountTxt(currentPickedAlly.Get_HadNoneSyncAmount(), Get_CurrentNeedNoneSync());
            Set_NoneSyncBuyBtn();

            return true;
        }

        return false;
    }

    private bool Is_Interact_NoneSyncBu()
    {
        if (currentBtn is OwnBtnEUIController eui && eui == noneSyneBuyBtn)
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
        if (currentBtn == buyBtnEui)
        {
            if (Can_Buy(out int goods))
            {
                Buy();
                Set_Picked(null);

                AllyModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Set (Profile)

    protected override void Pick_AllyProfile(AllyProfileEUIController eui)
    {
        if (isTweening) return;

        base.Pick_AllyProfile(eui);

        Set_NoneSynePanel();

        Set_BuyBtn();
    }

    #endregion

    #region None Sync (Buy)

    // 현재 선택한 Sync에서 필요한 NoneSync
    private int Get_CurrentNeedNoneSync()
    {
        return selectedNoneSyncIdList.Count * AllyController.noneSyncNeedOneBuy;
    }

    // Sync를 살 수 있는가
    private bool Can_NoneSyncBuy()
    {
        if (currentPickedAlly == null) return false;

        return selectedNoneSyncIdList.Count > 0 &&
            currentPickedAlly.Get_HadNoneSyncAmount() >= Get_CurrentNeedNoneSync() &&
            !AllyModuleUpgradeController.usingShop.isBroken;
    }

    // None Sync 패널의 버튼 활성화/비활성화 (살 수 있느냐에 따라)
    private void Set_NoneSyncBuyBtn()
    {
        if (Can_NoneSyncBuy())
        {
            noneSyncCanBuyGo.SetActive(true);
            noneSyncCannotBuyGo.SetActive(false);
        }
        else
        {
            noneSyncCanBuyGo.SetActive(false);
            noneSyncCannotBuyGo.SetActive(true);
        }
    }

    // None Sync 구매
    private void Buy_FromNoneSync()
    {
        // 추가
        currentPickedAlly.Add_Sync(selectedNoneSyncIdList);

        // 재화 소모
        int needGoods = Get_CurrentNeedNoneSync();
        currentPickedAlly.Use_HadNoneSyncAmount(needGoods);
        AllyModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

        // 소비 효과
        Play_UseTxt(noneSyncUseTxt, needGoods, 30f);

        // Extra 창에 State UI
        Set_AllyState(currentPickedAlly);

        // Extra 창에 Sync UI
        Set_AllySync(currentPickedAlly);

        // 패널 다시 세팅
        Set_NoneSynePanel();

    }

    #endregion

    #region None Sync (Toggle)

    // 현재 None Sync 패널에 진입할 수 있는지?
    private bool Can_EnterNoneSyncPanel()
    {
        if (currentPickedAlly.Get_HadNoneSyncAmount() >= AllyController.noneSyncNeedOneBuy)
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
        noneSyncArrowPanelGo.gameObject.SetActive(true);
    }

    // Arrow, Toggle Btn 끄기
    private void SetOff_NoneSyncEnterPanel()
    {
        noneSyncArrowPanelGo.gameObject.SetActive(false);

        if (inventoryPanel_NoneSyneGo.activeSelf)
            SetOn_ToggleNoneSyncPanel(false);
        
    }

    #endregion

    #region None Sync (Panel)

    // 현재 추가 획득할 수 있는 NoneSync 패널 세팅
    private void Set_NoneSynePanel()
    {
        if (currentPickedAlly == null)
        {
            Set_NoneSyncAmountTxt();
            SetOn_NoneSyneEmptyPanel(true);
            SetOff_NoneSyncEnterPanel();
        }
        else
        {
            Set_NoneSyncAmountTxt(currentPickedAlly.Get_HadNoneSyncAmount());
            SetOn_NoneSyneEmptyPanel(false);

            Dictionary<int, int> noFullSyncData = currentPickedAlly.Get_NoFullSyncData();
            Debug.Log(noFullSyncData.Count);
            Gen_NoneSyncItemEUI(noFullSyncData.Count);
            SetOff_AllNoneSyncEUI();
            SetOn_NoneSyncEUI(noFullSyncData); 
            Set_NoneSyncEnterPanel();
        }

        Set_NoneSyncBuyBtn();
    }

    // 현재 가지고 있는 NoneSync 개수
    private void Set_NoneSyncAmountTxt(int amount = -1, int need = 0)
    {
        string result = "";
        if (amount == -1)
        {
            result += "?";
        }
        else
        {
            result += currentPickedAlly.Get_HadNoneSyncAmount().ToString();
            
            if (need != 0)
            {
                result += $" <color=#933C8E>- {need}</color>";
            }
        }

        noneSyncTxt.text = result;
    }

    // 필요한 만큼 생성하기
    private void Gen_NoneSyncItemEUI(int needAmount)
    {
        if (noneSyncItemEuiList.Count < needAmount)
        {
            int genAmount = needAmount - noneSyncItemEuiList.Count;

            for (int i = 0; i < genAmount; i++)
            {
                AllyNoneSynergySlotEUIController genEUI = DevTool.Get_ComponentTType<AllyNoneSynergySlotEUIController>(
                    Instantiate(noneSyncItemPrefab, noneSyncItemParentTf));
                genEUI.ownerUIController = this;
                genEUI.Offset();

                noneSyncItemEuiList.Add(genEUI);
            }
        }
    }

    // 모두 끄기
    private void SetOff_AllNoneSyncEUI()
    {
        for (int i = 0; i < noneSyncItemEuiList.Count; i++)
        {
            noneSyncItemEuiList[i].gameObject.SetActive(false);
            noneSyncItemEuiList[i].Set_Select(false);
        }
    }

    // 키기
    private void SetOn_NoneSyncEUI(Dictionary<int, int> applySyncData)
    {
        usingNoneSyncItemEuiList.Clear();
        selectedNoneSyncIdList.Clear();

        float lastY = 0;
        int index = 0;
        foreach(KeyValuePair<int, int> pair in applySyncData)
        {
            int id = pair.Key;
            noneSyncItemEuiList[index].gameObject.SetActive(true);
            noneSyncItemEuiList[index].rt.anchoredPosition = 
                noneSyncItemOffset + 
                new Vector2(
                    (index % noneSyncRowAmount) * noneSyncItemInterval, 
                    -((index / noneSyncRowAmount) * noneSyncItemInterval));
            noneSyncItemEuiList[index].Set_SynergySlot(id);
            usingNoneSyncItemEuiList.Add(noneSyncItemEuiList[index]);
            lastY = noneSyncItemEuiList[index].rt.anchoredPosition.y;
            index++;
        }

        noneSyncScrollPanelEui.Set_ScrollHeight(Mathf.Max(-lastY + 76, 716));
    }

    #endregion

    #region Buy

    private void Set_BuyBtn()
    {
        bool can = Can_Buy(out int goods);
        buyBtnEui.cg.alpha = can ? 1f : 0.5f;
        canBuyArrowGO.gameObject.SetActive(can);

        DevTool.SetKillTween(moduleDetailExtraRt);
        moduleDetailExtraRt.DOSizeDelta(can ? ModuleDetailExtraRTOpen : new Vector2(ModuleDetailExtraRTOpen.x, 0), 0.2f);

        needChargedBettery = can ? goods : 0;
        Set_ChargedBetteryUI(PlayerManager.instance.playerController.chargedBettery, needChargedBettery);
    }

    private bool Can_Buy(out int goods)
    {
        goods = -2;

        if (pickedItemEui == null) return false;

        bool isExist = false;
        for (int i = 0; i < pickedPanelSynergyEuiList.Count; i++)
        {
            if (pickedPanelSynergyEuiList[i].Get_IsOn())
            {
                isExist = true;
                goods += 2;
            }
        }

        return !AllyModuleUpgradeController.usingShop.isBroken &&
            isExist && 
            PlayerManager.instance.playerController.chargedBettery >= goods &&
            currentPickedProfileEui != null &&
            pickedModulePanel_AllyGo.activeSelf;
    }

    private void Buy()
    {
        // 데이터
        PlayerManager.instance.playerController.UseChargedBettery(needChargedBettery);
        ModuleItemManager.instance.Remove_ModuleState(Get_CorrectMS(pickedItemEui.slot).originalIndex);

        currentPickedAlly.Add_Sync(Get_PickedSyncList());

        // 소비 효과
        Play_UseTxt(chargeBetteryUseTxt, needChargedBettery, 30f);

        // Extra 창에 State UI
        Set_AllyState(currentPickedAlly);

        // Extra 창에 Sync UI
        Set_AllySync(currentPickedAlly);

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

        for (int i = 0; i < pickedPanelSynergyEuiList.Count; i++)
        {
            if (pickedPanelSynergyEuiList[i].Get_IsOn())
            {
                result.Add(pickedPanelSynergyEuiList[i].Get_ID());
            }
        }

        return result;
    }

    #endregion

    #region Pay

    private void Set_ChargedBetteryUI(int amount, int needAmount = 0)
    {
        if (needAmount == 0)
        {
            chargeBetteryTxt.text = amount.ToString();
        }
        else
        {
            chargeBetteryTxt.text = $"{amount} <color=#933C8E>- {needAmount}</color>";
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

    private void Set_CopyAllyShopMSInventory(List<CopyModuleState> unEquip, List<CopyModuleState> equip)
    {
        indexData = new List<List<CoupleData<int>>>();
        currentData = new Dictionary<CoupleData<int>, CopyModuleState>();

        // 순서대로, Row Col 로 이중 리스트로 사용
        List<List<CopyModuleState>> combineData = DevTool.Get_RowColumeList(DevTool.Get_CombineList(unEquip, equip), ModuleItemManager.rowAmount);
        
        for (int i = 0; i < combineData.Count; i++)
        {
            indexData.Add(new List<CoupleData<int>>());
            for (int j = 0; j < combineData[i].Count; j++)
            {
                CoupleData<int> indexData = new CoupleData<int>(i, j);
                this.indexData[i].Add(indexData);
                currentData.Add(indexData, combineData[i][j]);
            }
        }

        // UI Set
        inventoryEui.SetOff_AllInventoryEquipedUI();

        inventoryEui.Set_InventoryUI(combineData);

        return;
    }

    #endregion

    #region Set (Picked)

    private void Set_PickedOnOffPanel(bool onOff)
    {
        pickedOnGo.SetActive(onOff);
        pickedOffGo.SetActive(!onOff);
    }

    private void Set_Picked(InventoryItemEUIController itemEui)
    {
        if (itemEui == null)
        {
            Set_PickedOnOffPanel(false);

            pickedItemEui = null;
            pickedModule = null;
            pickedModuleMainChipId = null;

            inventoryEui.SetOff_AllInventoryForgeSelectedUI();
        }
        else
        {
            // 사운드
            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            Set_PickedOnOffPanel(true);

            pickedItemEui = itemEui; // 아이템 EUI
            pickedModule = Get_CorrectMS(pickedItemEui.slot); // MS
            pickedModuleMainChipId = ModuleItemManager.instance.Get_MainChipIDData(pickedModule.state); // MainChip

            Set_PickedInventoryUI(); // Inventory UI
            Set_PickedModuleUI(); // Module UI
            Set_PickedSynergyUI(); // Synergy UI
        }
    }

    private void Set_PickedInventoryUI()
    {
        inventoryEui.SetOff_AllInventoryForgeSelectedUI();
        pickedItemEui.slot.Set_ForgeSelectedTxt(true);
    }

    private void Set_PickedModuleUI()
    {
        string name = $"[ {pickedModule.state.thisItemData.name} ]";
        if (pickedModule.isEquipped) name += $" <size=75%><color=#7F7F7F>({ResourceManager.instance.Get_StaticWord(112)})</size></color>";
        pickedPanelItemNameTxt.text = name;
        pickedPanelItemRankTxt.text = $"<size=70%>(R: {pickedModule.state.thisItemData.rank})</size>";
        pickedPanelItemRankTxt.color = ResourceManager.instance.Get_AllyCardColor(pickedModule.state.thisItemData.rank - 1);
        pickedPanelSlotEui.Set_EquipedTxt_NoneNum(pickedModule.isEquipped);
        
        pickedPanelSlotEui.item.Set_Data(new ItemData_UIVisual(
            pickedModule.state.thisItemData.itemIcon,
            pickedModule.state.thisItemData.rank));

        pickedPanelItemLockImg.gameObject.SetActive(pickedModule.isEquipped);
    }

    private void Set_PickedSynergyUI()
    {
        for (int i = 0; i < pickedPanelSynergyEuiList.Count; i++)
        {
            MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(pickedModuleMainChipId[i]);
            pickedPanelSynergyEuiList[i].Set_SynergySlot(MDC.id, MDC.thisIcon, ResourceManager.instance.Get_MainChipBaseDesc(MDC.id));
            pickedPanelSynergyEuiList[i].Set_PlayerSynergyTxt(ModuleItemManager.instance.Get_MainChipAmount(MDC.id));
            pickedPanelSynergyEuiList[i].Set_Select(false);
            pickedPanelSynergyEuiList[i].Set_Lock(pickedModule.isEquipped);
        }
    }

    #endregion

    #region Set (Inventory Or NoneSync)

    private void Set_ToggleNoneSyncPanel()
    {
        if (inventoryPanel_NoneSyneGo.activeSelf)
            SetOn_ToggleNoneSyncPanel(false);
        else
            SetOn_ToggleNoneSyncPanel(true);
    }


    private void SetOn_ToggleNoneSyncPanel(bool isOn)
    {
        inventoryPanel_NoneSyneGo.SetActive(isOn);
        inventoryPanel_InventoryGo.SetActive(!isOn);
    }

    private void SetOn_NoneSyneEmptyPanel(bool isOn)
    {
        noneSyncPanel_EmptyGo.SetActive(isOn);
        noneSyncPanel_ExistGo.SetActive(!isOn);
    }



    #endregion

    #region Set (Picked Ally Or Player)

    private void Set_ToggleAllyPlayerSyncPanel()
    {
        if (pickedModulePanel_AllyGo.activeSelf)
            SetOn_ToggleAllySyncPanel(false);
        else
            SetOn_ToggleAllySyncPanel(true);
    }

    
    private void SetOn_ToggleAllySyncPanel(bool isOn)
    {
        pickedModulePanel_AllyGo.SetActive(isOn);
        pickedModulePanel_PlayerGo.SetActive(!isOn);
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
            playerSynergyEmptyGo.gameObject.SetActive(true);
            playerSynergyExsitGo.gameObject.SetActive(false);
        }
        else
        {
            playerSynergyEmptyGo.gameObject.SetActive(false);
            playerSynergyExsitGo.gameObject.SetActive(true);

            foreach (KeyValuePair<int, int> sync in playerSync)
            {
                MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(sync.Key);
                playerSyncSlotEuiList[orderIndex].SetOn_SynergySlot(sync.Key, MDC.thisIcon, sync.Value);
                orderIndex++;
            }
        }   
    }

    private void SetOff_PlayerSyncState()
    {
        for (int i = 0; i < playerSyncSlotEuiList.Count; i++)
            playerSyncSlotEuiList[i].SetOff_SynergySlot();
    }

    #endregion

    #region Set (Player Sync Desc)

    private void SetOff_PlayerSynergyDesc()
    {
        playerSynergyDescImg.gameObject.SetActive(false);
    }

    private void SetOn_PlayerSynergyDesc(int id)
    {
        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        playerSynergyDescImg.gameObject.SetActive(true);

        playerSynergyDescImg.sprite = ModuleItemManager.instance.Get_CorrectMainChip(id).thisIcon;
        playerSynergyDescTxt.text = ResourceManager.instance.Get_MainChipBaseDesc(id);
    }

    #endregion

    #region Get (MS)

    private CopyModuleState Get_CorrectMS(InventorySlotEUIController slotBtn)
    {
        return currentData[indexData[slotBtn.col][slotBtn.row]];
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        durEui.Set_Dur(AllyModuleUpgradeController.usingShop.currentDur);

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

        AllyModuleUpgradeController.usingShop = null;
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        // Label
        labelName = ResourceManager.instance.Get_StaticWord(95) + " " + ResourceManager.instance.Get_StaticWord(27) + " " + ResourceManager.instance.Get_StaticWord(2);
        labelTxt.text = labelName;

        // Tuner
        moduleInventoryTxt.text = ResourceManager.instance.Get_StaticWord(110);
        moduleDetailTxt.text = ResourceManager.instance.Get_StaticWord(111);

        // Buy
        buyBtnEui.txt.text = ResourceManager.instance.Get_StaticWord(47) + " & " + ResourceManager.instance.Get_StaticWord(105);

        // Player Sync
        playerSyncNameTxt.text = $"[ {ResourceManager.instance.Get_StaticWord(113)} {ResourceManager.instance.Get_StaticWord(50)} ]";
        base.SetLanguageTxt();
    }

    #endregion
}

