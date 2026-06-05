using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : PlayerShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text bcTxt;
    [SerializeField] public TMP_Text ecTxt;
    [SerializeField] public TMP_Text msTxt;

    [Space(10)]
    [SerializeField] private List<TMP_Text> synergyLvTxtList;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescMUEUIController descPanel;

    [Space(10)]
    [Header("=== Drag")]
    [SerializeField] private InventoryItemEUIController dragItemEui;


    #endregion

    #region - Module

    [Space(20)]
    [Header("=== Module")]

    #region - Module Equip

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] public InventoryEUIController inventory_InEquip;
    [SerializeField] private OwnBtnEUIController toggleBtn_InEquip;

    [Space(5)]
    [Header("* Equip")]
    [SerializeField] private GameObject equippedPanelGo;
    [SerializeField] private Transform equippedSlotsParentTf;
    [SerializeField] private Transform equippedInnerParentTf;

    [Space(5)]
    [Header("* Synergy")]
    [SerializeField] private GameObject synergyPanelGo;
    [SerializeField] private CoupleData<GameObject> synergyPanelIsExistGo;
    [SerializeField] private Transform synergyInnerParentTf;
    [SerializeField] private Transform synergySlotsParentTf;
    [SerializeField] private Transform synergyDescsParentTf;
    [SerializeField] public List<Sprite> synergyTierFrames;

    [Space(5)]
    [Header("* Synergy Desc")]
    [SerializeField] private Image selectViewImg;
    [SerializeField] private TMP_Text selectViewName;
    [SerializeField] private TMP_Text selectViewAmalgamation;
    [SerializeField] private Transform synergyDescLinerParentTf;
    [SerializeField] private Transform synergyDescTextParentTf;
    [SerializeField] private List<TMP_Text> amalgamationTxtList;
    [SerializeField] public List<TMP_Text> amalgamationDescTxtList;

    #endregion

    #region - Module Forge

    [Space(10)]
    [Header("-- In Forge")]
    [SerializeField] public InventoryEUIController inventory_InForge;

    [SerializeField] private List<ForgeInteractPanel> forgeInteractPanels;
    [SerializeField] private List<Image> forgePanelInnerList;

    [Space(5)]
    [SerializeField] private TMP_Text noticeTxt;
    [SerializeField] private TMP_Text warningTxt;

    [Space(5)]
    [Header("* Decomposition")]
    [SerializeField] private InventorySlotEUIController decompositionSlot;
    [SerializeField] private TMP_Text preview_GainMs;
    [SerializeField] private TMP_Text preview_GainBc;

    [Space(5)]
    [Header("* Fusion")]
    [SerializeField] private List<InventorySlotEUIController> fusionSlotList;
    [SerializeField] private TMP_Text preview_NeedMs_ForFusion;

    [Space(5)]
    [Header("* Make")]
    [SerializeField] private TMP_Text preview_NeedMs_ForMake;
    [SerializeField] private TMP_Text preview_NeedCb_ForMake;

    #endregion

    #endregion

    #region - Hide

    // string
    [HideInInspector] public static string amalgamationName;
    [HideInInspector] public static string notice_Equiped;
    [HideInInspector] public static string warning_NotSameRank;
    [HideInInspector] public static string warning_NotEnoughItem;
    [HideInInspector] public static string warning_AlreadyMaxLv;
    [HideInInspector] public static string warning_InvenFull;

    // Current
    [HideInInspector] public InventoryItemEUIController currentDraggingItemBtn = null;

    // Inventory
    [HideInInspector] private List<InventoryEUIController> inventories;

    // Panel
    [HideInInspector] private ForgeInteractPanel currentForgeInteractPanel;

    // Equiped
    [HideInInspector] private List<InventorySlotEUIController> equipedSlots;
    [HideInInspector] private List<Image> equipPanelInnerList;
    [HideInInspector] private List<TMP_Text> equipDescStateTxtList;

    // Synergy
    [HideInInspector] private List<SynergySlotEUIController> synergySlotList = new List<SynergySlotEUIController>();
    [HideInInspector] private SynergySlotEUIController selectedSynergySlot;

    // Forge Element
    [HideInInspector] public static readonly float forgeElementBtnOnAlpha = 1f;
    [HideInInspector] public static readonly float forgeElementBtnOffAlpha = 0.5f;

    // Forge Anno
    [HideInInspector] private CanvasGroup noticeCg;
    [HideInInspector] private CanvasGroup warningCg;

    // Drag
    [HideInInspector] private RectTransform dragItemRt;
    [HideInInspector] private bool isDragging;

    #endregion

    #endregion


    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        Offset_Basic();
        Offset_Equip();
        Offset_Forge();
        Offset_ColorComp();
        Offset_Subscribe();

        Set_LanguageTxt();
    }

    private void Offset_Basic()
    {
        // Desc
        descPanel.Offset();

        // Inventory
        inventories = new List<InventoryEUIController>
        {
            inventory_InEquip, inventory_InForge
        };

        // Drag
        dragItemEui.Offset();
        dragItemRt = DevTool.Get_ComponentTType(dragItemEui.gameObject, out RectTransform rt) ? rt : null;

        dragItemEui.gameObject.SetActive(false);

        // Sync Lv Txt
        for (int i = 0; i < synergyLvTxtList.Count; i++)
            synergyLvTxtList[i].text = (ModuleItemManager.synchoronyMaxLv * (i + 1)).ToString();
        
    }



    private void Offset_Equip()
    {
        inventory_InEquip.Offset();
        inventory_InEquip.Gen_AllSlotAndItem(this);

        toggleBtn_InEquip.Offset();
        toggleBtn_InEquip.ownerUIController = this;

        Offset_Equip_InEquip();
        Offset_Synergy_InEquip();

        Reset_EquipPanel();
    }

    private void Offset_Equip_InEquip()
    {
        // 슬롯
        equipedSlots = DevTool.Get_ChildList<InventorySlotEUIController>(equippedSlotsParentTf);
        for (int i = 0; i < equipedSlots.Count; i++)
        {
            equipedSlots[i].ownerUIController = this;
            equipedSlots[i].Offset();
            equipedSlots[i].item.Offset();
            equipedSlots[i].item.ownerUIController = this;

            equipedSlots[i].Set_EquipedTxt(true, i);
        }

        // 이너 라인, 설명
        equipPanelInnerList = DevTool.Get_ChildList<Image>(equippedInnerParentTf);
        equipDescStateTxtList = DevTool.Get_ChildList<TMP_Text>(equippedInnerParentTf);
    }

    private void Offset_Synergy_InEquip()
    {
        // Synergy
        synergySlotList = DevTool.Get_ChildList<SynergySlotEUIController>(synergySlotsParentTf);
        for (int i = 0; i < synergySlotList.Count; i++)
        {
            synergySlotList[i].Offset();
            synergySlotList[i].ownerUIController = this;
        }

        SetOnOff_SynergySlot(false);
    }



    private void Offset_Forge()
    {
        inventory_InForge.Offset();
        inventory_InForge.Gen_AllSlotAndItem(this);

        forgeInteractPanels[0].Offset(this, ResourceManager.instance.Get_StaticWord(51), ResourceManager.instance.Get_StaticDesc(24));
        forgeInteractPanels[1].Offset(this, ResourceManager.instance.Get_StaticWord(52), ResourceManager.instance.Get_StaticDesc(25));
        forgeInteractPanels[2].Offset(this, ResourceManager.instance.Get_StaticWord(53), ResourceManager.instance.Get_StaticDesc(26));

        Offset_Forge_Decomposition();
        Offset_Forge_Fusion();
        Offset_Forge_Make();

        noticeCg =
            DevTool.Get_ComponentTType(noticeTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup nCg) ? nCg : null;
        warningCg =
            DevTool.Get_ComponentTType(warningTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup wCg) ? wCg : null;

        noticeCg.gameObject.SetActive(false);
        warningCg.gameObject.SetActive(false);

        Reset_ForgePanel();
    }

    private void Offset_Forge_Decomposition()
    {
        decompositionSlot.ownerUIController = this;
        decompositionSlot.Offset();
        decompositionSlot.item.Offset();
        decompositionSlot.item.ownerUIController = this;

        decompositionSlot.Set_ForgeSelectedTxt(true);
    }

    private void Offset_Forge_Fusion()
    {
        for (int i = 0; i < fusionSlotList.Count; i++)
        {
            fusionSlotList[i].ownerUIController = this;
            fusionSlotList[i].Offset();
            fusionSlotList[i].item.Offset();
            fusionSlotList[i].item.ownerUIController = this;

            fusionSlotList[i].Set_ForgeSelectedTxt(true, i);
        }
    }

    private void Offset_Forge_Make()
    {
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();
    }


    public void Offset_ColorComp()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        // Equiped
        subColorCompList.AddRange(equipPanelInnerList);
        for (int i = 0; i < equipDescStateTxtList.Count; i++)
        {
            mainColorCompList.Add(equipDescStateTxtList[i]);
        }

        // Synergy
        mainColorCompList.AddRange(amalgamationDescTxtList);
        subColorCompList.Add(selectViewAmalgamation);
        subColorCompList.AddRange(DevTool.Get_ChildList<Image>(synergyInnerParentTf));
        for (int i = 0; i < synergySlotList.Count; i++)
        {
            mainColorCompList.Add(synergySlotList[i].tierImg);
            subColorCompList.Add(synergySlotList[i].txt);
        }
        mainColorCompList.AddRange(DevTool.Get_ChildList<Image>(synergyDescLinerParentTf));
        for (int i = 0; i < synergyDescTextParentTf.childCount; i++)
        {
            if (synergyDescTextParentTf.GetChild(i).TryGetComponent(out TMP_Text Txt) &&
                !amalgamationDescTxtList.Contains(Txt))
            {
                subColorCompList.Add(Txt);
            }
        }

        // Equip Toggle Btn
        mainColorCompList.Add(toggleBtn_InEquip.transform.GetChild(0).GetComponent<TMP_Text>());

        // Forge Interact Panel Inner
        for (int i = 0; i < forgeInteractPanels.Count; i++)
        {
            mainColorCompList.Add(forgeInteractPanels[i].panelBtnTxt);

            mainColorCompList.Add(forgeInteractPanels[i].roleBtnTxt);
            mainColorCompList.Add(forgeInteractPanels[i].roleDescTxt);

            subColorCompList.AddRange(forgeInteractPanels[i].innerImgs);
        }
        subColorCompList.AddRange(forgePanelInnerList);

        // Desc
        mainColorCompList.AddRange(descPanel.Get_MainColorList());
        subColorCompList.AddRange(descPanel.Get_SubColorList());

        // Item
        mainColorCompList.Add(preview_GainMs);
        mainColorCompList.Add(preview_GainBc);
        mainColorCompList.Add(preview_NeedMs_ForFusion);
        mainColorCompList.Add(preview_NeedMs_ForMake);
        mainColorCompList.Add(preview_NeedCb_ForMake);

        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }

    private void Offset_Subscribe()
    {
        // BC // EC
        PlayerManager.instance.playerController.currentBettery
            .Subscribe(value =>
            {
                bcTxt.text = value.ToString();
            });
        PlayerManager.instance.playerController.currentChargedBettery
            .Subscribe(value =>
            {
                ecTxt.text = value.ToString();
            });
        PlayerManager.instance.playerController.currentModuleShard
            .Subscribe(value =>
            {
                msTxt.text = value.ToString();
            });
    }

    #endregion

    #region Reset

    // 장착 패널 리셋
    private void Reset_EquipPanel()
    {
        equippedPanelGo.gameObject.SetActive(true);
        synergyPanelGo.gameObject.SetActive(false);
    }


    private void Reset_ForgePanel()
    {
        currentForgeInteractPanel = null;

        for (int i = 0; i < forgeInteractPanels.Count; i++)
        {
            forgeInteractPanels[i].panelRT.gameObject.SetActive(false);

            if (DevTool.Get_ComponentTType(forgeInteractPanels[i].panelBtn.gameObject, out CanvasGroup btnCg))
            {
                btnCg.alpha = forgeElementBtnOffAlpha;
            }
        }

        Reset_ForgeElementPanel();
    }

    // 강화 패널 리셋
    public void Reset_ForgeElementPanel()
    {
        ModuleItemManager.instance.Set_UnDecompositionSlot();
        ModuleItemManager.instance.Set_UnFusionSlotAll();

        inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        decompositionSlot.item.gameObject.SetActive(false);
        for (int i = 0; i < fusionSlotList.Count; i++)
            fusionSlotList[i].item.gameObject.SetActive(false);

        preview_GainBc.text = "-";
        preview_GainMs.text = "-";
        preview_NeedMs_ForMake.text = "-";
        preview_NeedMs_ForFusion.text = "-";
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();

        SetOff_Anno();
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_EquipPanel();
        Reset_ForgePanel();
        msgEui.Reset_Data();
    }

    private void LateUpdate()
    {
        Caculate_Drag();
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        Tween_Enable();

        base.SetOn_ThisPanel();

        // Dur
        durEui.Set_Dur(ModuleUpgradeController.usingShop.currentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        Tween_Disable();
        SetOff_Desc();

        if (isDragging)
        {
            dragItemRt.gameObject.SetActive(false);
            isDragging = false;
            currentDraggingItemBtn = null;
        }

        ModuleUpgradeController.usingShop = null;
    }

    public override void Change_ThisPanel(int indexWindow)
    {
        // 인벤토리의 Scroll 벨류를 그대로 가져감
        float scrollValue = currentThisPanelTab.tabScrollbar.value;

        base.Change_ThisPanel(indexWindow);

        currentThisPanelTab.tabScrollbar.value = scrollValue;
    }

    #endregion

    #region Tween

    private void Tween_Enable()
    {
        for (int i = 0; i < forgeInteractPanels.Count; i++)
        {
            if (forgeInteractPanels[i].roleBtnTxtRT == null) break;

            forgeInteractPanels[i].rtTween.Play();
        }
    }

    private void Tween_Disable()
    {
        for (int i = 0; i < forgeInteractPanels.Count; i++)
        {
            if (forgeInteractPanels[i].roleBtnTxtRT == null) break;

            forgeInteractPanels[i].rtTween.Pause();
        }
    }

    #endregion

    #region Synergy

    public void Set_SynergySlots(Dictionary<int, int> dict)
    {
        if (dict.Count <= 0)
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(false);
        }
        else
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(true);

            // 모두 끄기
            for (int i = 0; i < synergySlotList.Count; i++)
                synergySlotList[i].SetOff_SynergySlot();

            // 가지고 있는 시너지 부분을 추가
            int currentSynergies = 0;
            foreach (KeyValuePair<int, int> keyValuePair in dict)
            {
                MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(keyValuePair.Key);
                synergySlotList[currentSynergies].SetOn_SynergySlot(keyValuePair.Key, MDC.thisIcon, keyValuePair.Value);

                currentSynergies++;
            }
        }
    }

    private void SetOnOff_SynergySlot(bool exist)
    {
        // 패널 키기/끄기
        synergyPanelIsExistGo.typeBase.SetActive(!exist);
        synergyPanelIsExistGo.typeSpecial.SetActive(exist);
    }

    #endregion

    #region Set

    #region Item UI

    // 인벤토리 UI 셋
    public void Set_InventoryUI(ModuleState[][] allModuleState)
    {
        for (int i = 0; i < inventories.Count; i++)
            inventories[i].Set_InventoryUI(allModuleState);
    }

    // 장착 슬롯 UI 셋
    public void Set_EquipedUI(ModuleState[][] allModuleState, CoupleData<int>[] equipedData)
    {
        for (int i = 0; i < equipedData.Length; i++)
        {
            int targetCol = equipedData[i].typeBase;
            int targetRow = equipedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                equipedSlots[i].item.gameObject.SetActive(false);

                MainGameUIManager.instance.playerHud.TabModuleView.moduleSlots[i].item.gameObject.SetActive(false);

                Set_EquipedDesc(i);
            }
            else
            {
                ItemData data = allModuleState[targetCol][targetRow].thisItemData;

                equipedSlots[i].item.gameObject.SetActive(true);
                equipedSlots[i].item.Set_Data(new ItemData_UIVisual(data));

                MainGameUIManager.instance.playerHud.TabModuleView.moduleSlots[i].item.gameObject.SetActive(true);
                MainGameUIManager.instance.playerHud.TabModuleView.moduleSlots[i].item.Set_Data(new ItemData_UIVisual(data));

                Set_EquipedDesc(i, data);
            }
        }

        for (int i = 0; i < inventories.Count; i++)
            inventories[i].Set_InventoryEquipedUI(equipedData);
    }

    // 장착된 모듈들의 설명 키기/끄기
    private void Set_EquipedDesc(int index, ItemData itemData = null)
    {
        equipDescStateTxtList[index].text = itemData != null ? itemData.equipDesc : "-";
    }

    // 분해 슬롯 UI 셋
    public void Set_DecompositionUI(InventoryItemEUIController itemEui, CoupleData<int> applyIndex)
    {
        int col = itemEui.slot.col;
        int row = itemEui.slot.row;

        bool setActive;
        string gainBC;
        string gainMS;

        if (col != -1 && row != -1) // 만약 슬롯에 등록하는 것이라면
        {
            ModuleState moduleState = ModuleItemManager.instance.Get_ModuleState(itemEui);

            setActive = true;
            gainBC = ModuleItemManager.Get_BC_ByDescomposition(moduleState).ToString();
            gainMS = ModuleItemManager.Get_MS_ByDecomposition(moduleState).ToString();

            decompositionSlot.item.Set_Data(itemEui);
        }
        else // 슬롯에서 빼는 것이라면
        {
            setActive = false;
            gainBC = "-";
            gainMS = "-";
        }

        inventory_InForge.Set_InventoryForgeSelectedUI(applyIndex, setActive);
        decompositionSlot.item.gameObject.SetActive(setActive);
        preview_GainBc.text = gainBC;
        preview_GainMs.text = gainMS;

        Check_DecompositionAnno();
    }

    // 합성 슬롯 UI 셋
    public void Set_FusionUI(ModuleState[][] allModuleState, CoupleData<int>[] slottedData)
    {
        inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        string needMS = "-";

        for (int i = 0; i < slottedData.Length; i++)
        {
            int targetCol = slottedData[i].typeBase;
            int targetRow = slottedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                fusionSlotList[i].item.gameObject.SetActive(false);
            }
            else
            {
                ItemData data = allModuleState[targetCol][targetRow].thisItemData;

                fusionSlotList[i].item.gameObject.SetActive(true);
                fusionSlotList[i].item.Set_Data(new ItemData_UIVisual(data));

                inventory_InForge.Set_InventoryForgeSelectedUI(new CoupleData<int>(targetCol, targetRow), true, i);
            }
        }

        if (!ModuleItemManager.instance.Is_EmptyFusionSlot() &&
            ModuleItemManager.instance.Is_SameRankFusionSlots())
        {
            ModuleState ms = ModuleItemManager.instance.Get_ModuleState(slottedData[0].typeBase, slottedData[0].typeSpecial);
            if (ms.thisItemData.rank < PlayerController.maxRank)
                needMS = ModuleItemManager.Get_MS_ForFusion(ms).ToString();
        }

        preview_NeedMs_ForFusion.text = needMS;

        Check_FusionAnno();
    }

    #endregion

    #region Forge Anno

    // 분해 경고
    private void Check_DecompositionAnno()
    {
        CoupleData<int> index = ModuleItemManager.instance.Get_DecompositionIndex();
        if (index.typeBase == -1 || index.typeSpecial == -1)
        {
            SetOff_Anno();
            return;
        }
        else if (ModuleItemManager.instance.Is_IncludeOnlyEquipped(index)) // 장비하고 있나?
        {
            Set_Notice(true, notice_Equiped);
            return;
        }

        SetOff_Anno();
    }

    // 조립 경고
    private void Check_MakeAnno()
    {
        CoupleData<int> index = ModuleItemManager.instance.Get_EmptyModuleState();
        if (index.typeBase == -1 || index.typeSpecial == -1)
        {
            Set_Warning(true, warning_InvenFull);
            return;
        }
        else if (PlayerManager.instance.playerController.currentModuleShard.Value < ModuleItemManager.Get_MS_ForMake() ||
            !PlayerManager.instance.playerController.Is_EnoughChargedBettery(ModuleItemManager.Get_CB_ForMake()))
        {
            Set_Warning(true, warning_NotEnoughItem);
            return;
        }

        SetOff_Anno();
        return;
    }

    // 합성 경고
    private void Check_FusionAnno()
    {
        List<CoupleData<int>> index = ModuleItemManager.instance.Get_FusionIndex();

        for (int i = 0; i < index.Count; i++)
        {
            if (index[i].typeBase == -1 || index[i].typeSpecial == -1)
            {
                SetOff_Anno();
                return;
            }
        }
        if (!ModuleItemManager.instance.Is_SameRankFusionSlots()) // 랭크가 다른가?
        {
            Set_Warning(true, warning_NotSameRank);
            return;
        }

        for (int i = 0; i < index.Count; i++) // 이미 최대치인가?
        {
            if (ModuleItemManager.instance.Get_ModuleState(index[i]).thisItemData.rank >= PlayerController.maxRank)
            {
                Set_Warning(true, warning_AlreadyMaxLv);
                return;
            }
        }

        if (ModuleItemManager.Get_MS_ForFusion(
            ModuleItemManager.instance.Get_ModuleState(index[0]))
                > PlayerManager.instance.playerController.currentModuleShard.Value) // MS가 부족한가?
        {
            Set_Warning(true, warning_NotEnoughItem);
            return;
        }

        for (int i = 0; i < index.Count; i++)
        {
            if (ModuleItemManager.instance.Is_IncludeOnlyEquipped(index[i]))
            {
                Set_Notice(true, notice_Equiped);
                return;
            }
        }

        SetOff_Anno();
    }

    // Notice
    private void Set_Notice(bool isOn, string anno = "")
    {
        if (noticeCg == null) return;

        noticeCg.gameObject.SetActive(isOn);
        if (isOn) noticeTxt.text = anno;
    }

    // Warning
    private void Set_Warning(bool isOn, string anno = "")
    {
        if (warningCg == null) return;

        warningCg.gameObject.SetActive(isOn);
        if (isOn) warningTxt.text = anno;
    }

    private void SetOff_Anno()
    {
        Set_Notice(false);
        Set_Warning(false);
    }

    #endregion

    #endregion

    #region Tab

    // 탭
    private bool Is_Interact_TabPanel()
    {
        for (int i = 0; i < panelTabList.Count; i++)
        {
            if (panelTabList[i].tabBtn == currentBtn)
            {
                Change_ThisPanel(i);
                return true;
            }
        }
        return false;
    }

    // 강화 탭
    private bool Is_Interact_ForgeTabPanel()
    {
        List<OwnBtnEUIController> btns = new List<OwnBtnEUIController>();
        for (int i = 0; i < forgeInteractPanels.Count; i++)
            btns.Add(forgeInteractPanels[i].panelBtn);

        if (btns.Contains(currentBtn))
        {
            for (int i = 0; i < forgeInteractPanels.Count; i++)
            {
                if (btns[i] == currentBtn)
                {
                    // 다른 탭일 경우 초기화
                    if (forgeInteractPanels[i] != currentForgeInteractPanel)
                        Reset_ForgeElementPanel();

                    // 생성이라면
                    if (i == 2)
                    {
                        Check_MakeAnno();
                    }

                    forgeInteractPanels[i].panelRT.gameObject.SetActive(true);
                    forgeInteractPanels[i].PanelBtnCG.alpha = 1f;

                    currentForgeInteractPanel = forgeInteractPanels[i];
                }
                else
                {
                    forgeInteractPanels[i].panelRT.gameObject.SetActive(false);
                    forgeInteractPanels[i].PanelBtnCG.alpha = 0.5f;
                }
            }

            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            return true;
        }
        return false;
    }

    // 시너지 변경 탭
    private bool Is_Interact_SynergyTabPanel()
    {
        if (currentBtn == toggleBtn_InEquip)
        {
            if (equippedPanelGo.activeSelf)
            {
                equippedPanelGo.SetActive(false);
                synergyPanelGo.SetActive(true);
                synergyDescsParentTf.gameObject.SetActive(false);
            }
            else
            {
                equippedPanelGo.SetActive(true);
                synergyPanelGo.SetActive(false);
            }

            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            return true;
        }
        return false;
    }

    // 시너지 아이템
    private bool Is_Interact_SynergyItem()
    {
        if (currentBtn is SynergySlotEUIController synergySlot && synergySlotList.Contains(synergySlot))
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            synergyDescsParentTf.gameObject.SetActive(true);
            selectedSynergySlot = synergySlot;

            MainChipData MCD = ModuleItemManager.instance.Get_CorrectMainChip(selectedSynergySlot.id);

            // 기본 정보
            selectViewImg.sprite = selectedSynergySlot.img.sprite;
            selectViewAmalgamation.text = selectedSynergySlot.txt.text;
            selectViewName.text = MCD.name.ToString();

            // 적용 중인 시너지 싱크로니 레벨 Txt
            int synchoronyAmount = ModuleItemManager.instance.Get_SynchronyAmount(synergySlot.id);
            int synchoronyLvLimit = 0;

            for (int i = ModuleItemManager.synchoronyMaxLv; i > 0; i--)
            {
                if ((ModuleItemManager.synchoronyOneTierRange * i) <= synchoronyAmount)
                {
                    synchoronyLvLimit = i;
                    break;
                }
            }

            for (int i = 0; i < amalgamationDescTxtList.Count; i++)
            {
                amalgamationDescTxtList[i].text = MCD.amalgamationDescArr[i];

                if (i < synchoronyLvLimit)
                    DevTool.Set_AlphaColor(amalgamationDescTxtList[i], 1f);
                else
                    DevTool.Set_AlphaColor(amalgamationDescTxtList[i], 0.3f);
            }
            return true;
        }
        return false;
    }

    #endregion

    #region Input

    #region Try Interact

    public void Try_Interact()
    {
        if (isDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Msg()) return;

        if (currentBtn == null || ModuleUpgradeController.usingShop == null) return;

        if (Is_Interact_TabPanel()) return;
        if (Is_Interact_ForgeTabPanel()) return;
        if (Is_Interact_SynergyTabPanel()) return;
        if (Is_Interact_SynergyItem()) return;
        if (Is_Interact_RoleBtn()) return;
        if (Is_Interact_CloseBtn()) return;
    }

    public void Try_InteractSub()
    {
        if (isDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (msgEui.gameObject.activeSelf || ModuleUpgradeController.usingShop == null) return;

        // 아이템이면?
        if (currentItemBtn != null)
        {
            Interact_Item(currentItemBtn);
            return;
        }
    }

    public void Try_InteractDragOn()
    {
        if (Is_Interact_Msg()) return;

        if (msgEui.gameObject.activeSelf || ModuleUpgradeController.usingShop == null) return;

        if (currentItemBtn != null)
        {
            currentDraggingItemBtn = currentItemBtn;

            dragItemEui.gameObject.SetActive(true);
            dragItemEui.Set_Data(currentItemBtn);

            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            isDragging = true;
        }
    }

    public void Try_InteractDragOff()
    {
        if (!dragItemEui.gameObject.activeSelf) return;

        dragItemEui.gameObject.SetActive(false);

        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        isDragging = false;

        Set_DragInSlot();

        currentDraggingItemBtn = null;
    }

    #endregion

    #region Item

    // 모듈 아이템
    private void Interact_Item(InventoryItemEUIController itemEui)
    {
        int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

        if (panelIndex == 0) // Equiped 창
        {
            if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 장착 시도
                Interact_Equipped(itemEui);
            else // 장착 해제
                Interact_UnEquipped(itemEui);
        }
        else if (panelIndex == 1) // Forge 창
        {
            int panelIndexOfForge = forgeInteractPanels.IndexOf(currentForgeInteractPanel);

            if (panelIndexOfForge == 0) // 분해
            {
                if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 슬롯 장착
                    Interact_DecompositionInit(itemEui);
                else // 슬롯 해제
                    Interact_UnDecompositionInit(itemEui);
            }
            else if (panelIndexOfForge == 1)
            {
                if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 슬롯 장착
                    Interact_FusionInit(itemEui);
                else // 슬롯 해제
                    Interact_UnFusionInit(itemEui);
            }
        }

    }

    #region Equip

    // 장착
    private void Interact_Equipped(InventoryItemEUIController itemEui, int equipSlotIndex = -1)
    {
        if (equipSlotIndex == -1) // 빈 공간을 찾아 장착
        {
            equipSlotIndex = ModuleItemManager.instance.Get_EmptyEquippedIndex(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
            if (equipSlotIndex == -1) return;
        }
        else if (ModuleItemManager.instance.Is_IncludeEquipped(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row), out int _ListIndex)) // 특정 위치에 이미 있다면, 제거
        {
            ModuleItemManager.instance.Set_UnEquip(_ListIndex);
        }
        // 삽입
        ModuleItemManager.instance.Set_Equip(equipSlotIndex, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Equip");
    }

    // 장착 해제
    private void Interact_UnEquipped(InventoryItemEUIController itemEui)
    {
        int index = equipedSlots.IndexOf(itemEui.slot);

        ModuleItemManager.instance.Set_UnEquip(index);

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Unequip");

        currentItemBtn = null;
        currentBtn = null;

        SetOff_Desc();
    }

    // 장착 스위칭
    private void Interact_EquippedSwitch(InventoryItemEUIController itemEui0, InventoryItemEUIController itemEui1)
    {
        int index0 = equipedSlots.IndexOf(itemEui0.slot);
        int index1 = equipedSlots.IndexOf(itemEui1.slot);
        ModuleItemManager.instance.Set_SwitchEquipment(index0, index1);
    }

    #endregion

    #region Descomposition

    // 분해 장착
    private void Interact_DecompositionInit(InventoryItemEUIController itemEui)
    {
        // 이미 존재한다면
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
            Interact_UnDecompositionInit(decompositionSlot.item);

        ModuleItemManager.instance.Set_DecompositionSlot(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));

        Set_DecompositionUI(itemEui, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
    }

    // 분해 해제
    private void Interact_UnDecompositionInit(InventoryItemEUIController itemEui)
    {
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
        {
            // 미리 값을 저장해야함 => 삭제할 것임
            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_DecompositionIndex();

            ModuleItemManager.instance.Set_UnDecompositionSlot();

            Set_DecompositionUI(itemEui, applyIndex);

            currentItemBtn = null;
        }
    }

    #endregion

    #region Fusion

    // 합성 장착
    private void Interact_FusionInit(InventoryItemEUIController itemEui, int slotIndex = -1)
    {
        if (slotIndex == -1)
        {
            if (ModuleItemManager.instance.Is_EmptyFusionSlot(out int index) &&
                !ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row)))
            {
                ModuleItemManager.instance.Set_FusionSlot(index, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
            }
        }
        else
        {
            // 이미 슬롯에 있다면, 제거
            if (ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row), out int listIndex))
                Interact_UnFusionInit(fusionSlotList[listIndex].item);
            // 해당 인덱스 슬롯에 비어있지 않다면, 제거
            if (!ModuleItemManager.instance.Is_EmptyFusionSlot(slotIndex))
                Interact_UnFusionInit(fusionSlotList[slotIndex].item);

            ModuleItemManager.instance.Set_FusionSlot(slotIndex, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
        }
    }

    // 합성 해제
    private void Interact_UnFusionInit(InventoryItemEUIController itemEui)
    {
        if (!ModuleItemManager.instance.Is_AllEmptyFusionSlot())
        {
            List<InventoryItemEUIController> itemEUIList = new List<InventoryItemEUIController>();
            for (int i = 0; i < fusionSlotList.Count; i++)
                itemEUIList.Add(fusionSlotList[i].item);

            int index = itemEUIList.IndexOf(itemEui);

            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_FusionIndex()[index];

            ModuleItemManager.instance.Set_UnFusionSlot(index);

            currentItemBtn = null;
        }
    }

    // 합성 스위칭
    private void Interact_FusionSwitch(InventoryItemEUIController itemEui0, InventoryItemEUIController itemEui1)
    {
        int index0 = fusionSlotList.IndexOf(itemEui0.slot);
        int index1 = fusionSlotList.IndexOf(itemEui1.slot);
        ModuleItemManager.instance.Set_SwitchFusion(index0, index1);
    }

    #endregion

    #endregion

    #region Role

    private bool Is_Interact_RoleBtn()
    {
        for (int i = 0; i < forgeInteractPanels.Count; i++)
        {
            if (forgeInteractPanels[i].roleBtn == currentBtn)
            {
                if (i == 0) // 분해
                    Role_Decomposition();
                else if (i == 1) // 합성
                    Role_Fusion();
                else if (i == 2) // 업글
                    Role_Make();
            }
        }
        return false;
    }

    private void Role_Decomposition()
    {
        if (ModuleItemManager.instance.Is_EmptyDecompositionSlot() ||
            warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        CoupleData<int> index = ModuleItemManager.instance.Get_DecompositionIndex();

        // 보상 획득
        PlayerManager.instance.playerController.Add_CurrentBettery(
            ModuleItemManager.Get_BC_ByDescomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        PlayerManager.instance.playerController.Add_CurrentModuleShard(
            ModuleItemManager.Get_MS_ByDecomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);


        // 모듈 아이템 제거
        ModuleItemManager.instance.Remove_ModuleState(index);

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Decomposition");

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Fusion()
    {
        if (ModuleItemManager.instance.Is_EmptyFusionSlot() ||
            warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        List<CoupleData<int>> indexList = ModuleItemManager.instance.Get_FusionIndex();

        // 소모 재화
        PlayerManager.instance.playerController.Add_CurrentModuleShard(
            -ModuleItemManager.Get_MS_ForFusion(ModuleItemManager.instance.Get_ModuleState(indexList[0])));

        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

        // 보상 획득
        ModuleItemManager.instance.Set_UpRank(indexList[0]);

        // 모듈 아이템 제거
        indexList.RemoveAt(0);
        ModuleItemManager.instance.Remove_ModuleState(indexList);

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Fusion");

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Make()
    {
        if (warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        // 소모 재화
        PlayerManager.instance.playerController.Add_CurrentModuleShard(-ModuleItemManager.Get_MS_ForMake());
        PlayerManager.instance.playerController.Use_ChargedBettery(ModuleItemManager.Get_CB_ForMake());
        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

        // 보상 획득
        ModuleItemManager.instance.Gain_ModuleState();

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Make");

        Check_MakeAnno();
    }

    #endregion

    #endregion

    #region Desc

    public void SetOn_Desc(InventoryItemEUIController itemEui)
    {
        int col = itemEui.slot.col;
        int row = itemEui.slot.row;
        ModuleState moduleState = null;
        if (col == -1 || row == -1) // 기타
        {
            if (equipedSlots.Contains(itemEui.slot)) // 장비 창
            {
                moduleState = ModuleItemManager.instance.Get_EquippedModuleState(
                    equipedSlots.IndexOf(itemEui.slot));
            }
        }
        else // 인벤토리
        {
            moduleState = ModuleItemManager.instance.Get_ModuleState(itemEui);
        }

        descPanel.SetOn_Desc(moduleState);
    }

    public void SetOff_Desc()
    {
        descPanel.SetOff_Desc();
    }

    #endregion

    #region Drag

    private void Caculate_Drag()
    {
        if (!dragItemEui.gameObject.activeSelf) return;

        dragItemRt.anchoredPosition = Get_PanelLocalPoint();
    }

    private void Set_DragInSlot()
    {
        if (currentSlotBtn != null)
        {
            CoupleData<int> originalIndex = new CoupleData<int>(currentDraggingItemBtn.slot.col, currentDraggingItemBtn.slot.row);
            CoupleData<int> targetIndex = new CoupleData<int>(currentSlotBtn.col, currentSlotBtn.row);

            // 인벤토리에서 이동 시키기
            if (originalIndex.typeBase != -1 && originalIndex.typeSpecial != -1)
                Set_DragFromInventorySlot(originalIndex, targetIndex);
            else
                Set_DragFromInteractSlot(originalIndex, targetIndex);
        }
    }

    // 인벤토리에서 =>
    private void Set_DragFromInventorySlot(CoupleData<int> originalIndex, CoupleData<int> targetIndex)
    {
        // => 인벤토리로
        if (targetIndex.typeBase != -1 && targetIndex.typeSpecial != -1)
        {
            ModuleItemManager.instance.Set_ChangeInventorySlot(originalIndex, targetIndex); // 인벤토리 내 아이템 위치 변경
        }
        else // => 상호작용 슬롯으로
        {
            // 장착 패널
            int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

            if (panelIndex == 0) // Equiped 창
            {
                Interact_Equipped(currentDraggingItemBtn, equipedSlots.IndexOf(currentSlotBtn)); // 특정 위치에 장착
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = forgeInteractPanels.IndexOf(currentForgeInteractPanel);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_DecompositionInit(currentDraggingItemBtn); // 분해 슬롯에 장착
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_FusionInit(currentDraggingItemBtn, fusionSlotList.IndexOf(currentSlotBtn)); // 퓨전 슬롯에 장착
                }
            }

        }
    }

    // 상호작용 슬롯에서 =>
    private void Set_DragFromInteractSlot(CoupleData<int> originalIndex, CoupleData<int> targetIndex)
    {
        int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

        // => 인벤토리로
        if (targetIndex.typeBase != -1 && targetIndex.typeSpecial != -1)
        {
            if (panelIndex == 0) // Equiped 창
            {
                Interact_UnEquipped(currentDraggingItemBtn); // 아이템 해제
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = forgeInteractPanels.IndexOf(currentForgeInteractPanel);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_UnDecompositionInit(currentDraggingItemBtn);
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_UnFusionInit(currentDraggingItemBtn);
                }
            }
            // => 상호작용으로
            else
            {
                if (panelIndex == 0) // Equiped 창
                {
                    Interact_EquippedSwitch(currentDraggingItemBtn, currentSlotBtn.item); // 아이템 장착 위치 바꾸기
                }
                else if (panelIndex == 1) // Forge 창
                {
                    if (forgeInteractPanels.IndexOf(currentForgeInteractPanel) == 1) // 합성
                    {
                        Interact_FusionSwitch(currentDraggingItemBtn, currentSlotBtn.item);
                    }
                }
            }

        }
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        labelName = ResourceManager.instance.Get_StaticWord(27) + " " + ResourceManager.instance.Get_StaticWord(2);
        labelTxt.text = labelName;

        // Tab
        tabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(32),
            ResourceManager.instance.Get_StaticWord(33),
        };

        amalgamationName = ResourceManager.instance.Get_StaticWord(50);
        notice_Equiped = ResourceManager.instance.Get_StaticDesc(20);
        warning_NotSameRank = ResourceManager.instance.Get_StaticDesc(21);
        warning_NotEnoughItem = ResourceManager.instance.Get_StaticDesc(22);
        warning_AlreadyMaxLv = ResourceManager.instance.Get_StaticDesc(23);
        warning_InvenFull = ResourceManager.instance.Get_StaticDesc(27);

        // Desc
        descPanel.Set_LanguageTxt();

        // Forge
        forgeInteractPanels[0].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(51), ResourceManager.instance.Get_StaticDesc(24));
        forgeInteractPanels[1].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(52), ResourceManager.instance.Get_StaticDesc(25));
        forgeInteractPanels[2].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(53), ResourceManager.instance.Get_StaticDesc(26));

        // Amalgamation
        DevTool.Set_TxtList(amalgamationTxtList, amalgamationName);

        base.Set_LanguageTxt();
    }

    #endregion
}