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
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;
    [SerializeField] public TMP_Text MSTxt;

    [Space(10)]
    [SerializeField] private List<TMP_Text> SynergyLvTxtList;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescMUEUIController ThisDescPanel;

    [Space(10)]
    [Header("=== Drag")]
    [SerializeField] private InventoryItemEUIController DragItemEUI;


    #endregion

    #region - Module

    [Space(20)]
    [Header("=== Module")]

    #region - Module Equip

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] public InventoryEUIController Inventory_InEquip;
    [SerializeField] private OwnBtnEUIController ToggleBtn_InEquip;

    [Space(5)]
    [Header("* Equip")]
    [SerializeField] private GameObject EquippedPanelGO;
    [SerializeField] private Transform EquippedSlotsParentTF;
    [SerializeField] private Transform EquippedInnerParentTF;

    [Space(5)]
    [Header("* Synergy")]
    [SerializeField] private GameObject SynergyPanelGO;
    [SerializeField] private CoupleData<GameObject> SynergyPanelIsExistGO;
    [SerializeField] private Transform SynergyInnerParentTF;
    [SerializeField] private Transform SynergySlotsParentTF;
    [SerializeField] private Transform SynergyDescsParentTF;
    [SerializeField] public List<Sprite> SynergyTierFrames;

    [Space(5)]
    [Header("* Synergy Desc")]
    [SerializeField] private Image SelectViewImg;
    [SerializeField] private TMP_Text SelectViewName;
    [SerializeField] private TMP_Text SelectViewAmalgamation;
    [SerializeField] private Transform SynergyDescLinerParentTF;
    [SerializeField] private Transform SynergyDescTextParentTF;
    [SerializeField] private List<TMP_Text> AmalgamationTxtList;
    [SerializeField] public List<TMP_Text> AmalgamationDescTxtList;

    #endregion

    #region - Module Forge

    [Space(10)]
    [Header("-- In Forge")]
    [SerializeField] public InventoryEUIController Inventory_InForge;

    [SerializeField] private List<ForgeInteractPanel> ForgeInteractPanels;
    [SerializeField] private List<Image> ForgePanelInnerList;

    [Space(5)]
    [SerializeField] private TMP_Text NoticeTxt;
    [SerializeField] private TMP_Text WarningTxt;

    [Space(5)]
    [Header("* Decomposition")]
    [SerializeField] private InventorySlotEUIController DecompositionSlot;
    [SerializeField] private TMP_Text Preview_GainMS;
    [SerializeField] private TMP_Text Preview_GainBC;

    [Space(5)]
    [Header("* Fusion")]
    [SerializeField] private List<InventorySlotEUIController> FusionSlotList;
    [SerializeField] private TMP_Text Preview_NeedMS_ForFusion;

    [Space(5)]
    [Header("* Make")]
    [SerializeField] private TMP_Text Preview_NeedMS_ForMake;
    [SerializeField] private TMP_Text Preview_NeedCB_ForMake;

    #endregion

    #endregion

    #region - Hide

    // string
    [HideInInspector] public static string AmalgamationName;
    [HideInInspector] public static string Notice_Equiped;
    [HideInInspector] public static string Warning_NotSameRank;
    [HideInInspector] public static string Warning_NotEnoughItem;
    [HideInInspector] public static string Warning_AlreadyMaxLv;
    [HideInInspector] public static string Warning_InvenFull;

    // Current
    [HideInInspector] public InventoryItemEUIController CurrentDraggingItemBtn = null;

    // Inventory
    [HideInInspector] private List<InventoryEUIController> Inventories;

    // Panel
    [HideInInspector] private ForgeInteractPanel CurrentForgeInteractPanel;

    // Equiped
    [HideInInspector] private List<InventorySlotEUIController> EquipedSlots;
    [HideInInspector] private List<Image> EquipPanelInnerList;
    [HideInInspector] private List<TMP_Text> EquipDescStateTxtList;

    // Synergy
    [HideInInspector] private List<SynergySlotEUIController> SynergySlotList = new List<SynergySlotEUIController>();
    [HideInInspector] private SynergySlotEUIController SelectedSynergySlot;

    // Forge Element
    [HideInInspector] public static readonly float ForgeElementBtnOnAlpha = 1f;
    [HideInInspector] public static readonly float ForgeElementBtnOffAlpha = 0.5f;

    // Forge Anno
    [HideInInspector] private CanvasGroup NoticeCG;
    [HideInInspector] private CanvasGroup WarningCG;

    // Drag
    [HideInInspector] private RectTransform DragItemRT;
    [HideInInspector] private bool IsDragging;

    #endregion

    #endregion


    #region Offset

    public override void Offset()
    {
        base.Offset();

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
        ThisDescPanel.Offset();

        // Inventory
        Inventories = new List<InventoryEUIController>
        {
            Inventory_InEquip, Inventory_InForge
        };

        // Drag
        DragItemEUI.Offset();
        DragItemRT = DevTool.Get_ComponentTType(DragItemEUI.gameObject, out RectTransform rt) ? rt : null;

        DragItemEUI.gameObject.SetActive(false);

        // Sync Lv Txt
        for (int i = 0; i < SynergyLvTxtList.Count; i++)
            SynergyLvTxtList[i].text = (ModuleItemManager.synchoronyMaxLv * (i + 1)).ToString();
        
    }



    private void Offset_Equip()
    {
        Inventory_InEquip.Offset();
        Inventory_InEquip.Gen_AllSlotAndItem(this);

        ToggleBtn_InEquip.Offset();
        ToggleBtn_InEquip.OwnerUIController = this;

        Offset_Equip_InEquip();
        Offset_Synergy_InEquip();

        Reset_EquipPanel();
    }

    private void Offset_Equip_InEquip()
    {
        // 슬롯
        EquipedSlots = DevTool.Get_ChildList<InventorySlotEUIController>(EquippedSlotsParentTF);
        for (int i = 0; i < EquipedSlots.Count; i++)
        {
            EquipedSlots[i].OwnerUIController = this;
            EquipedSlots[i].Offset();
            EquipedSlots[i].ThisItem.Offset();
            EquipedSlots[i].ThisItem.OwnerUIController = this;

            EquipedSlots[i].Set_EquipedTxt(true, i);
        }

        // 이너 라인, 설명
        EquipPanelInnerList = DevTool.Get_ChildList<Image>(EquippedInnerParentTF);
        EquipDescStateTxtList = DevTool.Get_ChildList<TMP_Text>(EquippedInnerParentTF);
    }

    private void Offset_Synergy_InEquip()
    {
        // Synergy
        SynergySlotList = DevTool.Get_ChildList<SynergySlotEUIController>(SynergySlotsParentTF);
        for (int i = 0; i < SynergySlotList.Count; i++)
        {
            SynergySlotList[i].Offset();
            SynergySlotList[i].OwnerUIController = this;
        }

        SetOnOff_SynergySlot(false);
    }



    private void Offset_Forge()
    {
        Inventory_InForge.Offset();
        Inventory_InForge.Gen_AllSlotAndItem(this);

        ForgeInteractPanels[0].Offset(this, ResourceManager.instance.Get_StaticWord(51), ResourceManager.instance.Get_StaticDesc(24));
        ForgeInteractPanels[1].Offset(this, ResourceManager.instance.Get_StaticWord(52), ResourceManager.instance.Get_StaticDesc(25));
        ForgeInteractPanels[2].Offset(this, ResourceManager.instance.Get_StaticWord(53), ResourceManager.instance.Get_StaticDesc(26));

        Offset_Forge_Decomposition();
        Offset_Forge_Fusion();
        Offset_Forge_Make();

        NoticeCG =
            DevTool.Get_ComponentTType(NoticeTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup nCg) ? nCg : null;
        WarningCG =
            DevTool.Get_ComponentTType(WarningTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup wCg) ? wCg : null;

        NoticeCG.gameObject.SetActive(false);
        WarningCG.gameObject.SetActive(false);

        Reset_ForgePanel();
    }

    private void Offset_Forge_Decomposition()
    {
        DecompositionSlot.OwnerUIController = this;
        DecompositionSlot.Offset();
        DecompositionSlot.ThisItem.Offset();
        DecompositionSlot.ThisItem.OwnerUIController = this;

        DecompositionSlot.Set_ForgeSelectedTxt(true);
    }

    private void Offset_Forge_Fusion()
    {
        for (int i = 0; i < FusionSlotList.Count; i++)
        {
            FusionSlotList[i].OwnerUIController = this;
            FusionSlotList[i].Offset();
            FusionSlotList[i].ThisItem.Offset();
            FusionSlotList[i].ThisItem.OwnerUIController = this;

            FusionSlotList[i].Set_ForgeSelectedTxt(true, i);
        }
    }

    private void Offset_Forge_Make()
    {
        Preview_NeedMS_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        Preview_NeedCB_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();
    }


    public void Offset_ColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();

        // Equiped
        SubColorCompList.AddRange(EquipPanelInnerList);
        for (int i = 0; i < EquipDescStateTxtList.Count; i++)
        {
            MainColorCompList.Add(EquipDescStateTxtList[i]);
        }

        // Synergy
        MainColorCompList.AddRange(AmalgamationDescTxtList);
        SubColorCompList.Add(SelectViewAmalgamation);
        SubColorCompList.AddRange(DevTool.Get_ChildList<Image>(SynergyInnerParentTF));
        for (int i = 0; i < SynergySlotList.Count; i++)
        {
            MainColorCompList.Add(SynergySlotList[i].ThisTierImg);
            SubColorCompList.Add(SynergySlotList[i].ThisTxt);
        }
        MainColorCompList.AddRange(DevTool.Get_ChildList<Image>(SynergyDescLinerParentTF));
        for (int i = 0; i < SynergyDescTextParentTF.childCount; i++)
        {
            if (SynergyDescTextParentTF.GetChild(i).TryGetComponent(out TMP_Text Txt) &&
                !AmalgamationDescTxtList.Contains(Txt))
            {
                SubColorCompList.Add(Txt);
            }
        }

        // Equip Toggle Btn
        MainColorCompList.Add(ToggleBtn_InEquip.transform.GetChild(0).GetComponent<TMP_Text>());

        // Forge Interact Panel Inner
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            MainColorCompList.Add(ForgeInteractPanels[i].panelBtnTxt);

            MainColorCompList.Add(ForgeInteractPanels[i].roleBtnTxt);
            MainColorCompList.Add(ForgeInteractPanels[i].roleDescTxt);

            SubColorCompList.AddRange(ForgeInteractPanels[i].innerImgs);
        }
        SubColorCompList.AddRange(ForgePanelInnerList);

        // Desc
        MainColorCompList.AddRange(ThisDescPanel.Get_MainColorList());
        SubColorCompList.AddRange(ThisDescPanel.Get_SubColorList());

        // Item
        MainColorCompList.Add(Preview_GainMS);
        MainColorCompList.Add(Preview_GainBC);
        MainColorCompList.Add(Preview_NeedMS_ForFusion);
        MainColorCompList.Add(Preview_NeedMS_ForMake);
        MainColorCompList.Add(Preview_NeedCB_ForMake);

        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    private void Offset_Subscribe()
    {
        // BC // EC
        PlayerManager.instance.playerController.currentBettery
            .Subscribe(value =>
            {
                BCTxt.text = value.ToString();
            });
        PlayerManager.instance.playerController.currentChargedBettery
            .Subscribe(value =>
            {
                ECTxt.text = value.ToString();
            });
        PlayerManager.instance.playerController.currentModuleShard
            .Subscribe(value =>
            {
                MSTxt.text = value.ToString();
            });
    }

    #endregion

    #region Reset

    // 장착 패널 리셋
    private void Reset_EquipPanel()
    {
        EquippedPanelGO.gameObject.SetActive(true);
        SynergyPanelGO.gameObject.SetActive(false);
    }


    private void Reset_ForgePanel()
    {
        CurrentForgeInteractPanel = null;

        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            ForgeInteractPanels[i].panelRT.gameObject.SetActive(false);

            if (DevTool.Get_ComponentTType(ForgeInteractPanels[i].panelBtn.gameObject, out CanvasGroup btnCg))
            {
                btnCg.alpha = ForgeElementBtnOffAlpha;
            }
        }

        Reset_ForgeElementPanel();
    }

    // 강화 패널 리셋
    public void Reset_ForgeElementPanel()
    {
        ModuleItemManager.instance.Set_UnDecompositionSlot();
        ModuleItemManager.instance.Set_UnFusionSlotAll();

        Inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        DecompositionSlot.ThisItem.gameObject.SetActive(false);
        for (int i = 0; i < FusionSlotList.Count; i++)
            FusionSlotList[i].ThisItem.gameObject.SetActive(false);

        Preview_GainBC.text = "-";
        Preview_GainMS.text = "-";
        Preview_NeedMS_ForMake.text = "-";
        Preview_NeedMS_ForFusion.text = "-";
        Preview_NeedMS_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        Preview_NeedCB_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();

        SetOff_Anno();
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_EquipPanel();
        Reset_ForgePanel();
        ThisMsgEUI.Reset_Data();
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
        ThisDurEUI.Set_Dur(ModuleUpgradeController.UsingShop.CurrentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        Tween_Disable();
        SetOff_Desc();

        if (IsDragging)
        {
            DragItemRT.gameObject.SetActive(false);
            IsDragging = false;
            CurrentDraggingItemBtn = null;
        }

        ModuleUpgradeController.UsingShop = null;
    }

    public override void Change_ThisPanel(int _indexWindow)
    {
        // 인벤토리의 Scroll 벨류를 그대로 가져감
        float scrollValue = CurrentThisPanelTab.ThisTabScrollbar.value;

        base.Change_ThisPanel(_indexWindow);

        CurrentThisPanelTab.ThisTabScrollbar.value = scrollValue;
    }

    #endregion

    #region Tween

    private void Tween_Enable()
    {
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            if (ForgeInteractPanels[i].roleBtnTxtRT == null) break;

            ForgeInteractPanels[i].rtTween.Play();
        }
    }

    private void Tween_Disable()
    {
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            if (ForgeInteractPanels[i].roleBtnTxtRT == null) break;

            ForgeInteractPanels[i].rtTween.Pause();
        }
    }

    #endregion

    #region Synergy

    public void Set_SynergySlots(Dictionary<int, int> _Dict)
    {
        if (_Dict.Count <= 0)
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(false);
        }
        else
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(true);

            // 모두 끄기
            for (int i = 0; i < SynergySlotList.Count; i++)
                SynergySlotList[i].SetOff_SynergySlot();

            // 가지고 있는 시너지 부분을 추가
            int currentSynergies = 0;
            foreach (KeyValuePair<int, int> keyValuePair in _Dict)
            {
                MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(keyValuePair.Key);
                SynergySlotList[currentSynergies].SetOn_SynergySlot(keyValuePair.Key, MDC.thisIcon, keyValuePair.Value);

                currentSynergies++;
            }
        }
    }

    private void SetOnOff_SynergySlot(bool _Exist)
    {
        // 패널 키기/끄기
        SynergyPanelIsExistGO.typeBase.SetActive(!_Exist);
        SynergyPanelIsExistGO.typeSpecial.SetActive(_Exist);
    }

    #endregion

    #region Set

    #region Item UI

    // 인벤토리 UI 셋
    public void Set_InventoryUI(ModuleState[][] _AllModuleState)
    {
        for (int i = 0; i < Inventories.Count; i++)
            Inventories[i].Set_InventoryUI(_AllModuleState);
    }

    // 장착 슬롯 UI 셋
    public void Set_EquipedUI(ModuleState[][] _AllModuleState, CoupleData<int>[] _EquipedData)
    {
        for (int i = 0; i < _EquipedData.Length; i++)
        {
            int targetCol = _EquipedData[i].typeBase;
            int targetRow = _EquipedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                EquipedSlots[i].ThisItem.gameObject.SetActive(false);

                MainGameUIManager.instance.playerHUD_UIController.ModuleSlots[i].ThisItem.gameObject.SetActive(false);

                Set_EquipedDesc(i);
            }
            else
            {
                ItemData data = _AllModuleState[targetCol][targetRow].thisItemData;

                EquipedSlots[i].ThisItem.gameObject.SetActive(true);
                EquipedSlots[i].ThisItem.Set_Data(new ItemData_UIVisual(data));

                MainGameUIManager.instance.playerHUD_UIController.ModuleSlots[i].ThisItem.gameObject.SetActive(true);
                MainGameUIManager.instance.playerHUD_UIController.ModuleSlots[i].ThisItem.Set_Data(new ItemData_UIVisual(data));

                Set_EquipedDesc(i, data);
            }
        }

        for (int i = 0; i < Inventories.Count; i++)
            Inventories[i].Set_InventoryEquipedUI(_EquipedData);
    }

    // 장착된 모듈들의 설명 키기/끄기
    private void Set_EquipedDesc(int _Index, ItemData _ItemData = null)
    {
        EquipDescStateTxtList[_Index].text = _ItemData != null ? _ItemData.equipDesc : "-";
    }

    // 분해 슬롯 UI 셋
    public void Set_DecompositionUI(InventoryItemEUIController _ItemEUI, CoupleData<int> _ApplyIndex)
    {
        int col = _ItemEUI.ThisSlot.Col;
        int row = _ItemEUI.ThisSlot.Row;

        bool setActive;
        string gainBC;
        string gainMS;

        if (col != -1 && row != -1) // 만약 슬롯에 등록하는 것이라면
        {
            ModuleState moduleState = ModuleItemManager.instance.Get_ModuleState(_ItemEUI);

            setActive = true;
            gainBC = ModuleItemManager.Get_BC_ByDescomposition(moduleState).ToString();
            gainMS = ModuleItemManager.Get_MS_ByDecomposition(moduleState).ToString();

            DecompositionSlot.ThisItem.Set_Data(_ItemEUI);
        }
        else // 슬롯에서 빼는 것이라면
        {
            setActive = false;
            gainBC = "-";
            gainMS = "-";
        }

        Inventory_InForge.Set_InventoryForgeSelectedUI(_ApplyIndex, setActive);
        DecompositionSlot.ThisItem.gameObject.SetActive(setActive);
        Preview_GainBC.text = gainBC;
        Preview_GainMS.text = gainMS;

        Check_DecompositionAnno();
    }

    // 합성 슬롯 UI 셋
    public void Set_FusionUI(ModuleState[][] _AllModuleState, CoupleData<int>[] _SlottedData)
    {
        Inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        string needMS = "-";

        for (int i = 0; i < _SlottedData.Length; i++)
        {
            int targetCol = _SlottedData[i].typeBase;
            int targetRow = _SlottedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                FusionSlotList[i].ThisItem.gameObject.SetActive(false);
            }
            else
            {
                ItemData data = _AllModuleState[targetCol][targetRow].thisItemData;

                FusionSlotList[i].ThisItem.gameObject.SetActive(true);
                FusionSlotList[i].ThisItem.Set_Data(new ItemData_UIVisual(data));

                Inventory_InForge.Set_InventoryForgeSelectedUI(new CoupleData<int>(targetCol, targetRow), true, i);
            }
        }

        if (!ModuleItemManager.instance.Is_EmptyFusionSlot() &&
            ModuleItemManager.instance.Is_SameRankFusionSlots())
        {
            ModuleState ms = ModuleItemManager.instance.Get_ModuleState(_SlottedData[0].typeBase, _SlottedData[0].typeSpecial);
            if (ms.thisItemData.rank < PlayerController.maxRank)
                needMS = ModuleItemManager.Get_MS_ForFusion(ms).ToString();
        }

        Preview_NeedMS_ForFusion.text = needMS;

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
            Set_Notice(true, Notice_Equiped);
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
            Set_Warning(true, Warning_InvenFull);
            return;
        }
        else if (PlayerManager.instance.playerController.currentModuleShard.Value < ModuleItemManager.Get_MS_ForMake() ||
            !PlayerManager.instance.playerController.Is_EnoughChargedBettery(ModuleItemManager.Get_CB_ForMake()))
        {
            Set_Warning(true, Warning_NotEnoughItem);
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
            Set_Warning(true, Warning_NotSameRank);
            return;
        }

        for (int i = 0; i < index.Count; i++) // 이미 최대치인가?
        {
            if (ModuleItemManager.instance.Get_ModuleState(index[i]).thisItemData.rank >= PlayerController.maxRank)
            {
                Set_Warning(true, Warning_AlreadyMaxLv);
                return;
            }
        }

        if (ModuleItemManager.Get_MS_ForFusion(
            ModuleItemManager.instance.Get_ModuleState(index[0]))
                > PlayerManager.instance.playerController.currentModuleShard.Value) // MS가 부족한가?
        {
            Set_Warning(true, Warning_NotEnoughItem);
            return;
        }

        for (int i = 0; i < index.Count; i++)
        {
            if (ModuleItemManager.instance.Is_IncludeOnlyEquipped(index[i]))
            {
                Set_Notice(true, Notice_Equiped);
                return;
            }
        }

        SetOff_Anno();
    }

    // Notice
    private void Set_Notice(bool _IsOn, string _Anno = "")
    {
        if (NoticeCG == null) return;

        NoticeCG.gameObject.SetActive(_IsOn);
        if (_IsOn) NoticeTxt.text = _Anno;
    }

    // Warning
    private void Set_Warning(bool _IsOn, string _Anno = "")
    {
        if (WarningCG == null) return;

        WarningCG.gameObject.SetActive(_IsOn);
        if (_IsOn) WarningTxt.text = _Anno;
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
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
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
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
            btns.Add(ForgeInteractPanels[i].panelBtn);

        if (btns.Contains(CurrentBtn))
        {
            for (int i = 0; i < ForgeInteractPanels.Count; i++)
            {
                if (btns[i] == CurrentBtn)
                {
                    // 다른 탭일 경우 초기화
                    if (ForgeInteractPanels[i] != CurrentForgeInteractPanel)
                        Reset_ForgeElementPanel();

                    // 생성이라면
                    if (i == 2)
                    {
                        Check_MakeAnno();
                    }

                    ForgeInteractPanels[i].panelRT.gameObject.SetActive(true);
                    ForgeInteractPanels[i].PanelBtnCG.alpha = 1f;

                    CurrentForgeInteractPanel = ForgeInteractPanels[i];
                }
                else
                {
                    ForgeInteractPanels[i].panelRT.gameObject.SetActive(false);
                    ForgeInteractPanels[i].PanelBtnCG.alpha = 0.5f;
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
        if (CurrentBtn == ToggleBtn_InEquip)
        {
            if (EquippedPanelGO.activeSelf)
            {
                EquippedPanelGO.SetActive(false);
                SynergyPanelGO.SetActive(true);
                SynergyDescsParentTF.gameObject.SetActive(false);
            }
            else
            {
                EquippedPanelGO.SetActive(true);
                SynergyPanelGO.SetActive(false);
            }

            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            return true;
        }
        return false;
    }

    // 시너지 아이템
    private bool Is_Interact_SynergyItem()
    {
        if (CurrentBtn is SynergySlotEUIController synergySlot && SynergySlotList.Contains(synergySlot))
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            SynergyDescsParentTF.gameObject.SetActive(true);
            SelectedSynergySlot = synergySlot;

            MainChipData MCD = ModuleItemManager.instance.Get_CorrectMainChip(SelectedSynergySlot.ID);

            // 기본 정보
            SelectViewImg.sprite = SelectedSynergySlot.ThisImg.sprite;
            SelectViewAmalgamation.text = SelectedSynergySlot.ThisTxt.text;
            SelectViewName.text = MCD.name.ToString();

            // 적용 중인 시너지 싱크로니 레벨 Txt
            int synchoronyAmount = ModuleItemManager.instance.Get_SynchronyAmount(synergySlot.ID);
            int synchoronyLvLimit = 0;

            for (int i = ModuleItemManager.synchoronyMaxLv; i > 0; i--)
            {
                if ((ModuleItemManager.synchoronyOneTierRange * i) <= synchoronyAmount)
                {
                    synchoronyLvLimit = i;
                    break;
                }
            }

            for (int i = 0; i < AmalgamationDescTxtList.Count; i++)
            {
                AmalgamationDescTxtList[i].text = MCD.amalgamationDescArr[i];

                if (i < synchoronyLvLimit)
                    DevTool.Set_AlphaColor(AmalgamationDescTxtList[i], 1f);
                else
                    DevTool.Set_AlphaColor(AmalgamationDescTxtList[i], 0.3f);
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
        if (IsDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Msg()) return;

        if (CurrentBtn == null || ModuleUpgradeController.UsingShop == null) return;

        if (Is_Interact_TabPanel()) return;
        if (Is_Interact_ForgeTabPanel()) return;
        if (Is_Interact_SynergyTabPanel()) return;
        if (Is_Interact_SynergyItem()) return;
        if (Is_Interact_RoleBtn()) return;
        if (Is_Interact_CloseBtn()) return;
    }

    public void Try_InteractSub()
    {
        if (IsDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (ThisMsgEUI.gameObject.activeSelf || ModuleUpgradeController.UsingShop == null) return;

        // 아이템이면?
        if (CurrentItemBtn != null)
        {
            Interact_Item(CurrentItemBtn);
            return;
        }
    }

    public void Try_InteractDragOn()
    {
        if (Is_Interact_Msg()) return;

        if (ThisMsgEUI.gameObject.activeSelf || ModuleUpgradeController.UsingShop == null) return;

        if (CurrentItemBtn != null)
        {
            CurrentDraggingItemBtn = CurrentItemBtn;

            DragItemEUI.gameObject.SetActive(true);
            DragItemEUI.Set_Data(CurrentItemBtn);

            SoundManager.instance.Play_2D_SFX_UI("Click_01");

            IsDragging = true;
        }
    }

    public void Try_InteractDragOff()
    {
        if (!DragItemEUI.gameObject.activeSelf) return;

        DragItemEUI.gameObject.SetActive(false);

        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        IsDragging = false;

        Set_DragInSlot();

        CurrentDraggingItemBtn = null;
    }

    #endregion

    #region Item

    // 모듈 아이템
    private void Interact_Item(InventoryItemEUIController _ItemEUI)
    {
        int panelIndex = ThisPanelTabList.IndexOf(CurrentThisPanelTab);

        if (panelIndex == 0) // Equiped 창
        {
            if (_ItemEUI.ThisSlot.Col != -1 && _ItemEUI.ThisSlot.Row != -1) // 장착 시도
                Interact_Equipped(_ItemEUI);
            else // 장착 해제
                Interact_UnEquipped(_ItemEUI);
        }
        else if (panelIndex == 1) // Forge 창
        {
            int panelIndexOfForge = ForgeInteractPanels.IndexOf(CurrentForgeInteractPanel);

            if (panelIndexOfForge == 0) // 분해
            {
                if (_ItemEUI.ThisSlot.Col != -1 && _ItemEUI.ThisSlot.Row != -1) // 슬롯 장착
                    Interact_DecompositionInit(_ItemEUI);
                else // 슬롯 해제
                    Interact_UnDecompositionInit(_ItemEUI);
            }
            else if (panelIndexOfForge == 1)
            {
                if (_ItemEUI.ThisSlot.Col != -1 && _ItemEUI.ThisSlot.Row != -1) // 슬롯 장착
                    Interact_FusionInit(_ItemEUI);
                else // 슬롯 해제
                    Interact_UnFusionInit(_ItemEUI);
            }
        }

    }

    #region Equip

    // 장착
    private void Interact_Equipped(InventoryItemEUIController _ItemEUI, int _EquipSlotIndex = -1)
    {
        if (_EquipSlotIndex == -1) // 빈 공간을 찾아 장착
        {
            _EquipSlotIndex = ModuleItemManager.instance.Get_EmptyEquippedIndex(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
            if (_EquipSlotIndex == -1) return;
        }
        else if (ModuleItemManager.instance.Is_IncludeEquipped(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row), out int _ListIndex)) // 특정 위치에 이미 있다면, 제거
        {
            ModuleItemManager.instance.Set_UnEquip(_ListIndex);
        }
        // 삽입
        ModuleItemManager.instance.Set_Equip(_EquipSlotIndex, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Equip");
    }

    // 장착 해제
    private void Interact_UnEquipped(InventoryItemEUIController _ItemEUI)
    {
        int index = EquipedSlots.IndexOf(_ItemEUI.ThisSlot);

        ModuleItemManager.instance.Set_UnEquip(index);

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Unequip");

        CurrentItemBtn = null;
        CurrentBtn = null;

        SetOff_Desc();
    }

    // 장착 스위칭
    private void Interact_EquippedSwitch(InventoryItemEUIController _ItemEUI0, InventoryItemEUIController _ItemEUI1)
    {
        int index0 = EquipedSlots.IndexOf(_ItemEUI0.ThisSlot);
        int index1 = EquipedSlots.IndexOf(_ItemEUI1.ThisSlot);
        ModuleItemManager.instance.Set_SwitchEquipment(index0, index1);
    }

    #endregion

    #region Descomposition

    // 분해 장착
    private void Interact_DecompositionInit(InventoryItemEUIController _ItemEUI)
    {
        // 이미 존재한다면
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
            Interact_UnDecompositionInit(DecompositionSlot.ThisItem);

        ModuleItemManager.instance.Set_DecompositionSlot(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));

        Set_DecompositionUI(_ItemEUI, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
    }

    // 분해 해제
    private void Interact_UnDecompositionInit(InventoryItemEUIController _ItemEUI)
    {
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
        {
            // 미리 값을 저장해야함 => 삭제할 것임
            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_DecompositionIndex();

            ModuleItemManager.instance.Set_UnDecompositionSlot();

            Set_DecompositionUI(_ItemEUI, applyIndex);

            CurrentItemBtn = null;
        }
    }

    #endregion

    #region Fusion

    // 합성 장착
    private void Interact_FusionInit(InventoryItemEUIController _ItemEUI, int _SlotIndex = -1)
    {
        if (_SlotIndex == -1)
        {
            if (ModuleItemManager.instance.Is_EmptyFusionSlot(out int index) &&
                !ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row)))
            {
                ModuleItemManager.instance.Set_FusionSlot(index, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
            }
        }
        else
        {
            // 이미 슬롯에 있다면, 제거
            if (ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row), out int listIndex))
                Interact_UnFusionInit(FusionSlotList[listIndex].ThisItem);
            // 해당 인덱스 슬롯에 비어있지 않다면, 제거
            if (!ModuleItemManager.instance.Is_EmptyFusionSlot(_SlotIndex))
                Interact_UnFusionInit(FusionSlotList[_SlotIndex].ThisItem);

            ModuleItemManager.instance.Set_FusionSlot(_SlotIndex, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
        }
    }

    // 합성 해제
    private void Interact_UnFusionInit(InventoryItemEUIController _ItemEUI)
    {
        if (!ModuleItemManager.instance.Is_AllEmptyFusionSlot())
        {
            List<InventoryItemEUIController> itemEUIList = new List<InventoryItemEUIController>();
            for (int i = 0; i < FusionSlotList.Count; i++)
                itemEUIList.Add(FusionSlotList[i].ThisItem);

            int index = itemEUIList.IndexOf(_ItemEUI);

            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_FusionIndex()[index];

            ModuleItemManager.instance.Set_UnFusionSlot(index);

            CurrentItemBtn = null;
        }
    }

    // 합성 스위칭
    private void Interact_FusionSwitch(InventoryItemEUIController _ItemEUI0, InventoryItemEUIController _ItemEUI1)
    {
        int index0 = FusionSlotList.IndexOf(_ItemEUI0.ThisSlot);
        int index1 = FusionSlotList.IndexOf(_ItemEUI1.ThisSlot);
        ModuleItemManager.instance.Set_SwitchFusion(index0, index1);
    }

    #endregion

    #endregion

    #region Role

    private bool Is_Interact_RoleBtn()
    {
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            if (ForgeInteractPanels[i].roleBtn == CurrentBtn)
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
            WarningCG.gameObject.activeSelf ||
            ModuleUpgradeController.UsingShop.CurrentDur <= 0) return;

        CoupleData<int> index = ModuleItemManager.instance.Get_DecompositionIndex();

        // 보상 획득
        PlayerManager.instance.playerController.Add_CurrentBettery(
            ModuleItemManager.Get_BC_ByDescomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        PlayerManager.instance.playerController.Add_CurrentModuleShard(
            ModuleItemManager.Get_MS_ByDecomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        ModuleUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);


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
            WarningCG.gameObject.activeSelf ||
            ModuleUpgradeController.UsingShop.CurrentDur <= 0) return;

        List<CoupleData<int>> indexList = ModuleItemManager.instance.Get_FusionIndex();

        // 소모 재화
        PlayerManager.instance.playerController.Add_CurrentModuleShard(
            -ModuleItemManager.Get_MS_ForFusion(ModuleItemManager.instance.Get_ModuleState(indexList[0])));

        ModuleUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);

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
        if (WarningCG.gameObject.activeSelf ||
            ModuleUpgradeController.UsingShop.CurrentDur <= 0) return;

        // 소모 재화
        PlayerManager.instance.playerController.Add_CurrentModuleShard(-ModuleItemManager.Get_MS_ForMake());
        PlayerManager.instance.playerController.Use_ChargedBettery(ModuleItemManager.Get_CB_ForMake());
        ModuleUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);

        // 보상 획득
        ModuleItemManager.instance.Gain_ModuleState();

        // 사운드
        SoundManager.instance.Play_2D_SFX_UI("Make");

        Check_MakeAnno();
    }

    #endregion

    #endregion

    #region Desc

    public void SetOn_Desc(InventoryItemEUIController _ItemEUI)
    {
        int col = _ItemEUI.ThisSlot.Col;
        int row = _ItemEUI.ThisSlot.Row;
        ModuleState moduleState = null;
        if (col == -1 || row == -1) // 기타
        {
            if (EquipedSlots.Contains(_ItemEUI.ThisSlot)) // 장비 창
            {
                moduleState = ModuleItemManager.instance.Get_EquippedModuleState(
                    EquipedSlots.IndexOf(_ItemEUI.ThisSlot));
            }
        }
        else // 인벤토리
        {
            moduleState = ModuleItemManager.instance.Get_ModuleState(_ItemEUI);
        }

        ThisDescPanel.SetOn_Desc(moduleState);
    }

    public void SetOff_Desc()
    {
        ThisDescPanel.SetOff_Desc();
    }

    #endregion

    #region Drag

    private void Caculate_Drag()
    {
        if (!DragItemEUI.gameObject.activeSelf) return;

        DragItemRT.anchoredPosition = Get_PanelLocalPoint();
    }

    private void Set_DragInSlot()
    {
        if (CurrentSlotBtn != null)
        {
            CoupleData<int> originalIndex = new CoupleData<int>(CurrentDraggingItemBtn.ThisSlot.Col, CurrentDraggingItemBtn.ThisSlot.Row);
            CoupleData<int> targetIndex = new CoupleData<int>(CurrentSlotBtn.Col, CurrentSlotBtn.Row);

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
            int panelIndex = ThisPanelTabList.IndexOf(CurrentThisPanelTab);

            if (panelIndex == 0) // Equiped 창
            {
                Interact_Equipped(CurrentDraggingItemBtn, EquipedSlots.IndexOf(CurrentSlotBtn)); // 특정 위치에 장착
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = ForgeInteractPanels.IndexOf(CurrentForgeInteractPanel);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_DecompositionInit(CurrentDraggingItemBtn); // 분해 슬롯에 장착
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_FusionInit(CurrentDraggingItemBtn, FusionSlotList.IndexOf(CurrentSlotBtn)); // 퓨전 슬롯에 장착
                }
            }

        }
    }

    // 상호작용 슬롯에서 =>
    private void Set_DragFromInteractSlot(CoupleData<int> originalIndex, CoupleData<int> targetIndex)
    {
        int panelIndex = ThisPanelTabList.IndexOf(CurrentThisPanelTab);

        // => 인벤토리로
        if (targetIndex.typeBase != -1 && targetIndex.typeSpecial != -1)
        {
            if (panelIndex == 0) // Equiped 창
            {
                Interact_UnEquipped(CurrentDraggingItemBtn); // 아이템 해제
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = ForgeInteractPanels.IndexOf(CurrentForgeInteractPanel);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_UnDecompositionInit(CurrentDraggingItemBtn);
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_UnFusionInit(CurrentDraggingItemBtn);
                }
            }
            // => 상호작용으로
            else
            {
                if (panelIndex == 0) // Equiped 창
                {
                    Interact_EquippedSwitch(CurrentDraggingItemBtn, CurrentSlotBtn.ThisItem); // 아이템 장착 위치 바꾸기
                }
                else if (panelIndex == 1) // Forge 창
                {
                    if (ForgeInteractPanels.IndexOf(CurrentForgeInteractPanel) == 1) // 합성
                    {
                        Interact_FusionSwitch(CurrentDraggingItemBtn, CurrentSlotBtn.ThisItem);
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
        LabelName = ResourceManager.instance.Get_StaticWord(27) + " " + ResourceManager.instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tab
        TabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(32),
            ResourceManager.instance.Get_StaticWord(33),
        };

        AmalgamationName = ResourceManager.instance.Get_StaticWord(50);
        Notice_Equiped = ResourceManager.instance.Get_StaticDesc(20);
        Warning_NotSameRank = ResourceManager.instance.Get_StaticDesc(21);
        Warning_NotEnoughItem = ResourceManager.instance.Get_StaticDesc(22);
        Warning_AlreadyMaxLv = ResourceManager.instance.Get_StaticDesc(23);
        Warning_InvenFull = ResourceManager.instance.Get_StaticDesc(27);

        // Desc
        ThisDescPanel.Set_LanguageTxt();

        // Forge
        ForgeInteractPanels[0].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(51), ResourceManager.instance.Get_StaticDesc(24));
        ForgeInteractPanels[1].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(52), ResourceManager.instance.Get_StaticDesc(25));
        ForgeInteractPanels[2].Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(53), ResourceManager.instance.Get_StaticDesc(26));

        // Amalgamation
        DevTool.Set_TxtList(AmalgamationTxtList, AmalgamationName);

        base.Set_LanguageTxt();
    }

    #endregion
}