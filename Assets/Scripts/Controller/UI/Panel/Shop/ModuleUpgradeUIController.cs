using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : PanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] public DurablityEUIController ThisDurEUI;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;
    [SerializeField] public TMP_Text MSTxt;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescMUEUIController ThisDescPanel;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] private OwnBtnEUIController CloseBtn;

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
    [SerializeField] private TMP_Text Preview_NeedMS;

    [Space(5)]
    [Header("* Upgrade")]
    [SerializeField] private InventorySlotEUIController UpgradeSlot;
    [SerializeField] private TMP_Text Preview_NeedEC;

    #endregion

    #endregion

    #region - Hide

    // string
    [HideInInspector] public static string LabelName = "MODULE UPGRADE SHOP";
    [HideInInspector] public static string AmalgamationName = "Synchrony";
    [HideInInspector] public static string Notice_Equiped = "This Module is Equipped";
    [HideInInspector] public static string Warning_NotSameRank = "Not the same Rank";
    [HideInInspector] public static string Warning_NotEnoughItem = "Not Enough Materials";
    [HideInInspector] public static string Warning_AlreadyMaxLv = "It's already at the Maximum";

    // Current
    [HideInInspector] public InventoryItemEUIController CurrentItemBtn = null;

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
    }


    private void Offset_Basic()
    {
        // Tab
        foreach (TabEUIController MET in ThisPanelTabList)
        {
            MET.Offset();
            MET.ThisTabBtn.OwnerUIController = this;
        }

        // Label
        LabelTxt.text = LabelName;

        // Dur
        ThisDurEUI.Offset();

        // Desc
        ThisDescPanel.Offset();

        // Close
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        // Inventory
        Inventories = new List<InventoryEUIController>
        {
            Inventory_InEquip, Inventory_InForge
        };
    }



    private void Offset_Equip()
    {
        Inventory_InEquip.Offset();

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

        DevTool.Set_TxtList(AmalgamationTxtList, AmalgamationName);
    }



    private void Offset_Forge()
    {
        Inventory_InForge.Offset();

        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            ForgeInteractPanels[i].Offset(this);
        }

        Offset_Forge_Decomposition();
        Offset_Forge_Fusion();
        Offset_Forge_Upgrade();

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
        DecompositionSlot.Offset();
        DecompositionSlot.ThisItem.Offset();
        DecompositionSlot.ThisItem.OwnerUIController = this;

        DecompositionSlot.Set_ForgeSelectedTxt(true);
    }

    private void Offset_Forge_Fusion()
    {
        for (int i = 0; i < FusionSlotList.Count; i++)
        {
            FusionSlotList[i].Offset();
            FusionSlotList[i].ThisItem.Offset();
            FusionSlotList[i].ThisItem.OwnerUIController = this;

            FusionSlotList[i].Set_ForgeSelectedTxt(true, i);
        }
    }

    private void Offset_Forge_Upgrade()
    {
        UpgradeSlot.Offset();
        UpgradeSlot.ThisItem.Offset();
        UpgradeSlot.ThisItem.OwnerUIController = this;

        UpgradeSlot.Set_ForgeSelectedTxt(true);
    }


    private void Offset_ColorComp()
    {
        // Label
        MainColorCompList.Add(LabelTxt);

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
            MainColorCompList.Add(ForgeInteractPanels[i].PanelBtnTxt);

            MainColorCompList.Add(ForgeInteractPanels[i].RoleBtnTxt);
            MainColorCompList.Add(ForgeInteractPanels[i].RoleDescTxt);

            SubColorCompList.AddRange(ForgeInteractPanels[i].InnerImgs);
        }
        SubColorCompList.AddRange(ForgePanelInnerList);

        // Desc
        MainColorCompList.AddRange(ThisDescPanel.Get_MainColorList());
        SubColorCompList.AddRange(ThisDescPanel.Get_SubColorList());

        // Item
        MainColorCompList.Add(Preview_GainMS);
        MainColorCompList.Add(Preview_GainBC);
        MainColorCompList.Add(Preview_NeedMS);
        MainColorCompList.Add(Preview_NeedEC);

        // Tab Btn
        MainColorCompList.AddRange(Get_AllTabBtn_Txt());
        SubColorCompList.AddRange(Get_AllTabBtn_Img());

        // Close Btn
        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    private void Offset_Subscribe()
    {
        // BC // EC
        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(value =>
            {
                BCTxt.text = value.ToString();
            });
        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(value =>
            {
                ECTxt.text = value.ToString();
            });
        PlayerManager.Instance.PlayerController.CurrentMS
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
        ModuleItemManager.Instance.Set_MainChipData();

        EquippedPanelGO.gameObject.SetActive(true);
        SynergyPanelGO.gameObject.SetActive(false);
    }


    private void Reset_ForgePanel()
    {
        CurrentForgeInteractPanel = null;

        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            ForgeInteractPanels[i].PanelRT.gameObject.SetActive(false);

            if (DevTool.Get_ComponentTType(ForgeInteractPanels[i].PanelBtn.gameObject, out CanvasGroup btnCg))
            {
                btnCg.alpha = ForgeElementBtnOffAlpha;
            }
        }

        Reset_ForgeElementPanel();
    }

    // 강화 패널 리셋
    public void Reset_ForgeElementPanel()
    {
        ModuleItemManager.Instance.Set_UnDecompositionSlot();
        ModuleItemManager.Instance.Set_UnFusionSlotAll();
        ModuleItemManager.Instance.Set_UnUpgradeSlot();

        Inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        DecompositionSlot.ThisItem.gameObject.SetActive(false);
        for (int i = 0; i < FusionSlotList.Count; i++)
            FusionSlotList[i].ThisItem.gameObject.SetActive(false);
        UpgradeSlot.ThisItem.gameObject.SetActive(false);

        Preview_GainBC.text = "-";
        Preview_GainMS.text = "-";
        Preview_NeedEC.text = "-";
        Preview_NeedMS.text = "-";

        SetOff_Anno();
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach (TabEUIController MET in ThisPanelTabList)
        {
            MET.Reset_ScrollBar();
        }

        Reset_EquipPanel();
        Reset_ForgePanel();
    }

    #endregion

    #region Set Panel

    public override void SetOn_ThisPanel()
    {
        Tween_Enable();

        base.SetOn_ThisPanel();

        ThisDurEUI.Set_Dur(ModuleUpgradeController.UsingShop.CurrentDur);
    }

    public override void SetOff_ThisPanel()
    {
        Tween_Disable();

        base.SetOff_ThisPanel();

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
            if (ForgeInteractPanels[i].RoleBtnTxtRT == null) break;

            ForgeInteractPanels[i].rtTween.Play();
        }
    }

    private void Tween_Disable()
    {
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            if (ForgeInteractPanels[i].RoleBtnTxtRT == null) break;

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
                MainChipData MDC = ModuleItemManager.Instance.Get_CorrectMainChip(keyValuePair.Key);
                SynergySlotList[currentSynergies].SetOn_SynergySlot(keyValuePair.Key, MDC.ThisIcon, keyValuePair.Value);

                currentSynergies++;
            }
        }
    }

    private void SetOnOff_SynergySlot(bool _Exist)
    {
        // 패널 키기/끄기
        SynergyPanelIsExistGO.TypeBase.SetActive(!_Exist);
        SynergyPanelIsExistGO.TypeSpecial.SetActive(_Exist);
    }

    #endregion

    #region Set

    #region Item UI

    // 인벤토리 UI 셋
    public void Set_InventoryUI(List<List<ModuleState>> _AllModuleState)
    {
        for (int i = 0; i < Inventories.Count; i++)
            Inventories[i].Set_InventoryUI(_AllModuleState);
    }

    // 장착 슬롯 UI 셋
    public void Set_EquipedUI(List<List<ModuleState>> _AllModuleState, List<CoupleData<int>> _EquipedData)
    {
        for (int i = 0; i < _EquipedData.Count; i++)
        {
            int targetCol = _EquipedData[i].TypeBase;
            int targetRow = _EquipedData[i].TypeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                EquipedSlots[i].ThisItem.gameObject.SetActive(false);

                Set_EquipedDesc(i);
            }
            else
            {
                EquipedSlots[i].ThisItem.gameObject.SetActive(true);

                ItemData data = _AllModuleState[targetCol][targetRow].ThisItemData;
                EquipedSlots[i].ThisItem.Set_Data(new ItemData_UIVisual(data));

                Set_EquipedDesc(i, data);
            }
        }
        for (int i = 0; i < Inventories.Count; i++)
            Inventories[i].Set_InventoryEquipedUI(_EquipedData);
    }

    // 장착된 모듈들의 설명 키기/끄기
    private void Set_EquipedDesc(int _Index, ItemData _ItemData = null)
    {
        EquipDescStateTxtList[_Index].text = _ItemData != null ? _ItemData.EquipDescription : "-";
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
            ModuleState moduleState = ModuleItemManager.Instance.Get_ModuleState(_ItemEUI);

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
    public void Set_FusionUI(InventoryItemEUIController _ItemEUI, CoupleData<int> _ApplyIndex, int _Index)
    {
        int col = _ItemEUI.ThisSlot.Col;
        int row = _ItemEUI.ThisSlot.Row;

        bool setActive;
        string needMS;

        if (col != -1 && row != -1) // 만약 슬롯에 등록하는 것이라면
        {
            setActive = true;
            if (ModuleItemManager.Instance.Is_EmptyFusionSlot() ||
                !ModuleItemManager.Instance.Is_SameRankFusionSlots() ||
                ModuleItemManager.Instance.Get_ModuleState(_ItemEUI).ThisItemData.Rank >= PlayerController.MaxRank)
            {
                needMS = "-";
            }
            else
            {
                ModuleState moduleState = ModuleItemManager.Instance.Get_ModuleState(_ItemEUI);
                needMS = ModuleItemManager.Get_MS_ForFusion(moduleState).ToString();
            }

            FusionSlotList[_Index].ThisItem.Set_Data(_ItemEUI);
        }
        else // 슬롯에서 빼는 것이라면
        {
            setActive = false;
            needMS = "-";
        }

        Inventory_InForge.Set_InventoryForgeSelectedUI(_ApplyIndex, setActive, _Index);
        FusionSlotList[_Index].ThisItem.gameObject.SetActive(setActive);
        Preview_NeedMS.text = needMS;

        Check_FusionAnno();
    }

    // 분해 슬롯 UI 셋
    public void Set_UpgradeUI(InventoryItemEUIController _ItemEUI, CoupleData<int> _ApplyIndex)
    {
        int col = _ItemEUI.ThisSlot.Col;
        int row = _ItemEUI.ThisSlot.Row;

        bool setActive;
        string needEC;

        if (col != -1 && row != -1) // 만약 슬롯에 등록하는 것이라면
        {
            ModuleState moduleState = ModuleItemManager.Instance.Get_ModuleState(_ItemEUI);

            setActive = true;
            if (moduleState.ThisItemData.BoostLv < PlayerController.MaxBoostLv)
                needEC = ModuleItemManager.Get_EC_ForUpgrade(moduleState).ToString();
            else
                needEC = "-";

            UpgradeSlot.ThisItem.Set_Data(_ItemEUI);
        }
        else // 슬롯에서 빼는 것이라면
        {
            setActive = false;
            needEC = "-";
        }

        Inventory_InForge.Set_InventoryForgeSelectedUI(_ApplyIndex, setActive);
        UpgradeSlot.ThisItem.gameObject.SetActive(setActive);
        Preview_NeedEC.text = needEC;

        Check_UpgradeAnno();
    }

    #endregion

    #region Forge Anno

    // 분해 경고
    private void Check_DecompositionAnno()
    {
        CoupleData<int> index = ModuleItemManager.Instance.Get_DecompositionIndex();
        if (index.TypeBase == -1 || index.TypeSpecial == -1)
        {
            SetOff_Anno();
            return;
        }
        else if (ModuleItemManager.Instance.Is_IncludeOnlyEquipped(index)) // 장비하고 있나?
        {
            Set_Notice(true, Notice_Equiped); 
            return;
        }

        SetOff_Anno();
    }

    // 합성 경고
    private void Check_FusionAnno()
    {
        List<CoupleData<int>> index = ModuleItemManager.Instance.Get_FusionIndex(); 

        for (int i = 0; i < index.Count; i++)
        {
            if (index[i].TypeBase == -1 || index[i].TypeSpecial == -1)
            {
                SetOff_Anno();
                return;
            }
        }
        if (!ModuleItemManager.Instance.Is_SameRankFusionSlots()) // 랭크가 다른가?
        {
            Set_Warning(true, Warning_NotSameRank);
            return;
        }
        else if (ModuleItemManager.Get_MS_ForFusion(
            ModuleItemManager.Instance.Get_ModuleState(index[0])) 
                > PlayerManager.Instance.PlayerController.CurrentMS.Value) // MS가 부족한가?
        {
            Set_Warning(true, Warning_NotEnoughItem);
            return;
        }
        for (int i = 0; i < index.Count; i++) // 이미 최대치인가?
        {
            if (ModuleItemManager.Instance.Get_ModuleState(index[i]).ThisItemData.Rank >= PlayerController.MaxRank)
            {
                Set_Warning(true, Warning_AlreadyMaxLv); 
            }
        }
        for (int i = 0; i < index.Count; i++)
        {
            if (ModuleItemManager.Instance.Is_IncludeOnlyEquipped(index[i]))
            {
                Set_Notice(true, Notice_Equiped);
                return;
            }
        }

        SetOff_Anno();
    }

    // 업글 경고
    private void Check_UpgradeAnno()
    {
        CoupleData<int> index = ModuleItemManager.Instance.Get_UpgradeIndex();
        if (index.TypeBase == -1 || index.TypeSpecial == -1)
        {
            SetOff_Anno();
            return;
        }
        if (ModuleItemManager.Instance.Get_ModuleState(index).ThisItemData.BoostLv >= 
            PlayerController.MaxBoostLv) // 최대치인가
        {
            Set_Warning(true, Warning_AlreadyMaxLv);
            return;
        }
        else if (ModuleItemManager.Get_EC_ForUpgrade(
            ModuleItemManager.Instance.Get_ModuleState(index))
                > PlayerManager.Instance.PlayerController.CurrentEC.Value) // 재료 부족
        {
            Set_Warning(true, Warning_NotEnoughItem);
            return;
        }
        else if (ModuleItemManager.Instance.Is_IncludeOnlyEquipped(index)) // 장비 중
        {
            Set_Notice(true, Notice_Equiped);
            return;
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
            btns.Add(ForgeInteractPanels[i].PanelBtn);

        if (btns.Contains(CurrentBtn))
        {
            for (int i = 0; i < ForgeInteractPanels.Count; i++)
            {
                if (btns[i] == CurrentBtn)
                {
                    // 다른 탭일 경우 초기화
                    if (ForgeInteractPanels[i] != CurrentForgeInteractPanel)
                        Reset_ForgeElementPanel();

                    ForgeInteractPanels[i].PanelRT.gameObject.SetActive(true);
                    ForgeInteractPanels[i].PanelBtnCG.alpha = 1f;

                    CurrentForgeInteractPanel = ForgeInteractPanels[i];
                }
                else
                {
                    ForgeInteractPanels[i].PanelRT.gameObject.SetActive(false);
                    ForgeInteractPanels[i].PanelBtnCG.alpha = 0.5f;
                }
            }
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

                ModuleItemManager.Instance.Set_MainChipData();
            }
            else
            {
                EquippedPanelGO.SetActive(true);
                SynergyPanelGO.SetActive(false);
            }
            return true;
        }
        return false;
    }

    // 시너지 아이템
    private bool Is_Interact_SynergyItem()
    {
        if (CurrentBtn is SynergySlotEUIController synergySlot && SynergySlotList.Contains(synergySlot))
        {
            SynergyDescsParentTF.gameObject.SetActive(true);
            SelectedSynergySlot = synergySlot;

            MainChipData MCD = ModuleItemManager.Instance.Get_CorrectMainChip(SelectedSynergySlot.ID);

            // 기본 정보
            SelectViewImg.sprite = SelectedSynergySlot.ThisImg.sprite;
            SelectViewAmalgamation.text = SelectedSynergySlot.ThisTxt.text;
            SelectViewName.text = MCD.Name.ToString();

            // 적용 중인 시너지 싱크로니 레벨 Txt
            int synchoronyAmount = ModuleItemManager.Instance.Get_SynchronyAmount(synergySlot.ID);
            int synchoronyLvLimit = 0;

            for (int i = ModuleItemManager.SynchoronyMaxLv; i > 0; i--)
            {
                if ((ModuleItemManager.SynchoronyOneTierRange * i) <= synchoronyAmount)
                {
                    synchoronyLvLimit = i;
                    break;
                }
            }

            for (int i = 0; i < AmalgamationDescTxtList.Count; i++)
            {
                AmalgamationDescTxtList[i].text = MCD.AmalgamationDescList[i];

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

    public void Try_Interact()
    {
        if (CurrentBtn == null) return;

        if (Is_Interact_TabPanel()) return;
        if (Is_Interact_ForgeTabPanel()) return;
        if (Is_Interact_SynergyTabPanel()) return;
        if (Is_Interact_SynergyItem()) return;
        if (Is_Interact_RoleBtn()) return;

        /*
                if (ModuleUpgradeController.UsingShop == null)
                { return; }

                // 아이템
                if (CurrentSlot != null)
                { 
                    Try_InteractItem(CurrentSlot); 
                    return; 
                }
                else if (CurrentBtn != null)
                {
                    // 닫기
                    if (CurrentBtn == CloseBtn)
                    {
                        MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOff_ThisPanel();
                        return;
                    }

                    // 탭
                    for (int i = 0; i < ThisPanelTabList.Count; i++)
                    {
                        if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
                        {
                            Change_ThisPanel(i);
                            Reset_ForgeElementPanel();
                            Set_EquipDesc();
                            return;
                        }
                    }

                    // 장착 부분의 토글 버튼
                    if (CurrentBtn == ToggleBtn_InEquip)
                    {
                        if (EquipedPanelGO.activeSelf)
                        {
                            EquipedPanelGO.SetActive(false);
                            SynergyPanelGO.SetActive(true);
                            SynergyDescsParentTF.gameObject.SetActive(false);
                            return;
                        }
                        else
                        {
                            EquipedPanelGO.SetActive(true);
                            SynergyPanelGO.SetActive(false);
                            return;
                        }
                    }

                    // 특별 상호작용
                    if (CurrentBtn == ForgeInteractPanels[0].RoleBtn)
                    {
                        Try_Desomposition(); return;
                    }
                    else if (CurrentBtn == ForgeInteractPanels[1].RoleBtn)
                    {
                        Try_Fusion(); return;
                    }
                    else if (CurrentBtn == ForgeInteractPanels[2].RoleBtn)
                    {
                        Try_Upgrade(); return;
                    }

                    // 특별 상호작용 탭
                    for (int i = 0; i < ForgeInteractPanels.Count; i++)
                    {
                        if (ForgeInteractPanels[i].PanelBtn == CurrentBtn)
                        {
                            // 다른 탭일 경우 초기화
                            if (ForgeInteractPanels[i] != CurrentForgeInteractPanel)
                            {
                                Reset_ForgeElementPanel();
                            }

                            ForgeInteractPanels[i].PanelRT.gameObject.SetActive(true);
                            if (ForgeInteractPanels[i].PanelBtn.gameObject.TryGetComponent(out CanvasGroup cg))
                            {
                                cg.alpha = 1f;
                            }
                            CurrentForgeInteractPanel = ForgeInteractPanels[i];
                        }
                        else
                        {
                            ForgeInteractPanels[i].PanelRT.gameObject.SetActive(false);
                            if (ForgeInteractPanels[i].PanelBtn.gameObject.TryGetComponent(out CanvasGroup cg))
                            {
                                cg.alpha = 0.5f;
                            }
                        }
                    }

                    // 시너지 탭의 정보
                    if (CurrentBtn is SynergySlotEUIController mss && SynergySlotList.Contains(mss))
                    {
                        SynergyDescsParentTF.gameObject.SetActive(true);
                        SelectedMSS = mss;

                        MainChipData MCD = ModuleItemManager.Instance.Get_CorrectMainChip(SelectedMSS.ID);

                        SelectViewImg.sprite = SelectedMSS.ThisImg.sprite;
                        SelectViewAmalgamation.text = SelectedMSS.ThisTxt.text;
                        SelectViewName.text = MCD.Name.ToString();

                        for (int i = 0; i < AmalgamationDescTxtList.Count; i++)
                        {
                            AmalgamationDescTxtList[i].text = MCD.AmalgamationDescList[i]; 

                        }
                    }
                }
        */
    }

    public void Try_InteractSub()
    {
        if (ModuleUpgradeController.UsingShop == null) return;

        // 아이템이면?
        if (CurrentItemBtn != null)
        {
            Interact_Item(CurrentItemBtn);
            return;
        }
    }

    #region Item

    // 모듈 아이템
    private void Interact_Item(InventoryItemEUIController _ItemEUI)
    {
        int panelIndex = ThisPanelTabList.IndexOf(CurrentThisPanelTab);

        if (panelIndex == 0) // Equiped 창
        {
            if (_ItemEUI.ThisSlot.Col != -1 && _ItemEUI.ThisSlot.Row != -1) // 장착 시도
                Interact_Equiped(_ItemEUI);
            else // 장착 해제
                Interact_UnEquiped(_ItemEUI);
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
            else if (panelIndexOfForge == 2)
            {
                if (_ItemEUI.ThisSlot.Col != -1 && _ItemEUI.ThisSlot.Row != -1) // 슬롯 장착
                    Interact_UpgradeInit(_ItemEUI);
                else // 슬롯 해제
                    Interact_UnUpgradeInit(_ItemEUI);
            }
        }

    }

    #region Equip

    // 장착
    private void Interact_Equiped(InventoryItemEUIController _ItemEUI)
    {
        int emptyIndex = ModuleItemManager.Instance.Get_EmptyEquippedIndex(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
        if (emptyIndex != -1)
        {
            ModuleItemManager.Instance.Set_Equip(emptyIndex, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
        }
    }

    // 장착 해제
    private void Interact_UnEquiped(InventoryItemEUIController _ItemEUI)
    {
        int index = EquipedSlots.IndexOf(_ItemEUI.ThisSlot);

        ModuleItemManager.Instance.Set_UnEquip(index);

        CurrentItemBtn = null;
        CurrentBtn = null;

        SetOff_Desc();
    }

    #endregion

    #region Descomposition

    // 분해 장착
    private void Interact_DecompositionInit(InventoryItemEUIController _ItemEUI)
    {
        if (ModuleItemManager.Instance.Is_EmptyDecompositionSlot())
        {
            ModuleItemManager.Instance.Set_DecompositionSlot(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));

            Set_DecompositionUI(_ItemEUI, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
        }
    }
    
    // 분해 해제
    private void Interact_UnDecompositionInit(InventoryItemEUIController _ItemEUI)
    {
        if (!ModuleItemManager.Instance.Is_EmptyDecompositionSlot())
        {
            // 미리 값을 저장해야함 => 삭제할 것임
            CoupleData<int> applyIndex = ModuleItemManager.Instance.Get_DecompositionIndex();

            ModuleItemManager.Instance.Set_UnDecompositionSlot();

            Set_DecompositionUI(_ItemEUI, applyIndex);
        }
    }

    #endregion

    #region Fusion

    // 합성 장착
    private void Interact_FusionInit(InventoryItemEUIController _ItemEUI)
    {
        if (ModuleItemManager.Instance.Is_EmptyFusionSlot(out int index) && 
            !ModuleItemManager.Instance.Is_IncludeFusionSlots(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row)))
        {
            ModuleItemManager.Instance.Set_FusionSlot(index, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));

            Set_FusionUI(_ItemEUI, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row), index);
        }
    }

    // 합성 해제
    private void Interact_UnFusionInit(InventoryItemEUIController _ItemEUI)
    {
        if (!ModuleItemManager.Instance.Is_AllEmptyFusionSlot())
        {
            List<InventoryItemEUIController> itemEUIList = new List<InventoryItemEUIController>();
            for (int i = 0; i < FusionSlotList.Count; i++)
                itemEUIList.Add(FusionSlotList[i].ThisItem);

            int index = itemEUIList.IndexOf(_ItemEUI);

            CoupleData<int> applyIndex = ModuleItemManager.Instance.Get_FusionIndex()[index];

            ModuleItemManager.Instance.Set_UnFusionSlot(index);

            Set_FusionUI(_ItemEUI, applyIndex, index);
        }
    }

    #endregion

    #region Upgrade

    // 업글 장착
    private void Interact_UpgradeInit(InventoryItemEUIController _ItemEUI)
    {
        if (ModuleItemManager.Instance.Is_EmptyUpgradeSlot())
        {
            ModuleItemManager.Instance.Set_UpgradeSlot(new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));

            Set_UpgradeUI(_ItemEUI, new CoupleData<int>(_ItemEUI.ThisSlot.Col, _ItemEUI.ThisSlot.Row));
        }
    }

    // 업글 해제
    private void Interact_UnUpgradeInit(InventoryItemEUIController _ItemEUI)
    {
        if (!ModuleItemManager.Instance.Is_EmptyUpgradeSlot())
        {
            // 미리 값을 저장해야함 => 삭제할 것임
            CoupleData<int> applyIndex = ModuleItemManager.Instance.Get_UpgradeIndex();

            ModuleItemManager.Instance.Set_UnUpgradeSlot();

            Set_UpgradeUI(_ItemEUI, applyIndex);
        }
    }

    #endregion

    #endregion

    #region Role

    private bool Is_Interact_RoleBtn()
    {
        for (int i = 0; i < ForgeInteractPanels.Count; i++)
        {
            if (ForgeInteractPanels[i].RoleBtn == CurrentBtn)
            {
                if (i == 0) // 분해
                    Role_Decomposition();
                else if (i == 1) // 합성
                    Role_Fusion();
                else if (i == 2) // 업글
                    Role_Upgrade();
            }
        }
        return false;
    }

    private void Role_Decomposition()
    {
        if (ModuleItemManager.Instance.Is_EmptyDecompositionSlot() ||
            WarningCG.gameObject.activeSelf) return;

        CoupleData<int> index = ModuleItemManager.Instance.Get_DecompositionIndex();

        // 보상 획득
        PlayerManager.Instance.PlayerController.Add_CurrentBC(
            ModuleItemManager.Get_BC_ByDescomposition(ModuleItemManager.Instance.Get_ModuleState(index)));

        PlayerManager.Instance.PlayerController.Add_CurrentMS(
            ModuleItemManager.Get_MS_ByDecomposition(ModuleItemManager.Instance.Get_ModuleState(index)));
        

        // 모듈 아이템 제거
        ModuleItemManager.Instance.Remove_ModuleState(index);

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Fusion()
    {
        if (ModuleItemManager.Instance.Is_EmptyFusionSlot() ||
            WarningCG.gameObject.activeSelf) return;

        List<CoupleData<int>> indexList = ModuleItemManager.Instance.Get_FusionIndex();

        // 소모 재화
        PlayerManager.Instance.PlayerController.Add_CurrentMS(
            -ModuleItemManager.Get_MS_ForFusion(ModuleItemManager.Instance.Get_ModuleState(indexList[0])));

        // 보상 획득
        ModuleItemManager.Instance.Set_UpRank(indexList[0]);

        // 모듈 아이템 제거
        indexList.RemoveAt(0);
        ModuleItemManager.Instance.Remove_ModuleState(indexList);

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Upgrade()
    {
        if (ModuleItemManager.Instance.Is_EmptyUpgradeSlot() ||
            WarningCG.gameObject.activeSelf) return;

        CoupleData<int> index = ModuleItemManager.Instance.Get_UpgradeIndex();

        // 소모 재화
        PlayerManager.Instance.PlayerController.Use_EC(
            ModuleItemManager.Get_EC_ForUpgrade(ModuleItemManager.Instance.Get_ModuleState(index)));

        // 보상 획득
        ModuleItemManager.Instance.Set_UpBoostLv(index);

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
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
                moduleState = ModuleItemManager.Instance.Get_EquippedModuleState(
                    EquipedSlots.IndexOf(_ItemEUI.ThisSlot));
            }
        }
        else // 인벤토리
        {
            moduleState = ModuleItemManager.Instance.Get_ModuleState(_ItemEUI);
        }

        ThisDescPanel.SetOn_Desc(moduleState);
    }

    public void SetOff_Desc()
    {
        ThisDescPanel.SetOff_Desc();
    }

    #endregion
}
