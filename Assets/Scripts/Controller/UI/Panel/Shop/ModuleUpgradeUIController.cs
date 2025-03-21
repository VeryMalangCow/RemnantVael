using DG.Tweening;
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
    [SerializeField] public DescMUEUIController ThisDescPanel;

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
    [SerializeField] private InventoryEUIController Inventory_InEquip;
    [SerializeField] private OwnBtnEUIController ToggleBtn_InEquip;

    [Space(5)]
    [Header("* Equip")]
    [SerializeField] private GameObject EquipedPanelGO;
    [SerializeField] private Transform EquipedSlotsParentTF;
    [SerializeField] private Transform EquipedInnerParentTF;

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
    [SerializeField] private InventoryEUIController Inventory_InForge;

    [SerializeField] private List<ForgeInteractPanel> ForgeInteractPanels;
    [SerializeField] private List<Image> ForgePanelInnerList;



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
    [HideInInspector] public static string AmalgamationName = "AMALGAMATION";

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
    [HideInInspector] private SynergySlotEUIController SelectedMSS;

    // Current
    [HideInInspector] public InventorySlotEUIController CurrentSlot;

    [HideInInspector] private InventoryItemEUIController CurrentDecompositionItem = null;
    [HideInInspector] private List<InventoryItemEUIController> CurrentFusionItemList = new List<InventoryItemEUIController>();
    [HideInInspector] private InventoryItemEUIController CurrentUpgradeItem = null;

    // Forge Element
    [HideInInspector] public static readonly float ForgeElementBtnOnAlpha = 1f;
    [HideInInspector] public static readonly float ForgeElementBtnOffAlpha = 0.5f;

    // Seq
    [HideInInspector] private Sequence InnerEquipSeq = null;

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
            Inventory_InEquip,
            Inventory_InForge
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

        EquipedSlots = DevTool.Get_ChildList<InventorySlotEUIController>(EquipedSlotsParentTF);
        for (int i = 0; i < EquipedSlots.Count; i++) 
        { 
            EquipedSlots[i].Offset();
            EquipedSlots[i].ThisSlotItem.Offset();
            EquipedSlots[i].ThisSlotItem.OwnerUIController = this;
        }

        // 이너 라인, 설명
        EquipPanelInnerList = DevTool.Get_ChildList<Image>(EquipedInnerParentTF);
        EquipDescStateTxtList = DevTool.Get_ChildList<TMP_Text>(EquipedInnerParentTF);
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

        for (int i = 0; i < AmalgamationTxtList.Count; i++)
        {
            AmalgamationTxtList[i].text = AmalgamationName;
        }
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

        Reset_ForgePanel();
    }

    private void Offset_Forge_Decomposition()
    {
        DecompositionSlot.Offset();
        DecompositionSlot.ThisSlotItem.Offset();
        DecompositionSlot.ThisSlotItem.OwnerUIController = this;
    }

    private void Offset_Forge_Fusion()
    {
        for (int i = 0; i < FusionSlotList.Count; i++)
        {
            FusionSlotList[i].Offset();
            FusionSlotList[i].ThisSlotItem.Offset();
            FusionSlotList[i].ThisSlotItem.OwnerUIController = this;

            CurrentFusionItemList.Add(null);
        }
    }

    private void Offset_Forge_Upgrade()
    {
        UpgradeSlot.Offset();
        UpgradeSlot.ThisSlotItem.Offset();
        UpgradeSlot.ThisSlotItem.OwnerUIController = this;
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

        EquipedPanelGO.gameObject.SetActive(true);
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
        DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
        FusionSlotList[0].ThisSlotItem.gameObject.SetActive(false);
        FusionSlotList[1].ThisSlotItem.gameObject.SetActive(false);
        UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);

        CurrentDecompositionItem = null;
        DevTool.Set_Null(CurrentFusionItemList);
        CurrentUpgradeItem = null;

        Preview_GainBC.text = "-";
        Preview_GainMS.text = "-";
        Preview_NeedEC.text = "-";
        Preview_NeedMS.text = "-";
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

    private void Set_DotweenInEquip(float _Alpha, List<Image> _TweenImgList)
    {
        DevTool.Set_CompleteTween(InnerEquipSeq);
        InnerEquipSeq = DOTween.Sequence();

        InnerEquipSeq.Append(Seq_Fade(_TweenImgList, _Alpha));
        InnerEquipSeq.Append(Seq_Fade(_TweenImgList, 0.5f));
    }

    private Sequence Seq_Fade(List<Image> _TweenImgList, float _Alpha)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < _TweenImgList.Count; i++)
        { seq.Join(_TweenImgList[i].DOFade(_Alpha, 0.1f)); }

        return seq;
    }

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

    #region Desc

    public void Set_Desc(InventoryItemEUIController _Item)
    {
        ModuleState moduleState = ModuleItemManager.Instance.Get_InventoryModuleState(_Item);
        if (moduleState != null)
        {
            ThisDescPanel.SetOn_Desc(moduleState);
            return;
        }

        moduleState = ModuleItemManager.Instance.Get_EquipedModuleState(_Item);
        if (moduleState != null)
        {
            ThisDescPanel.SetOn_Desc(moduleState);
            return;
        }

        // 슬롯 매핑을 위한 Dictionary 사용
        Dictionary<InventoryItemEUIController, InventoryItemEUIController> slotMapping 
            = new Dictionary<InventoryItemEUIController, InventoryItemEUIController>
                {
                    { DecompositionSlot.ThisSlotItem, CurrentDecompositionItem },
                    { FusionSlotList[0].ThisSlotItem, CurrentFusionItemList[0] },
                    { FusionSlotList[1].ThisSlotItem, CurrentFusionItemList[1] },
                    { UpgradeSlot.ThisSlotItem, CurrentUpgradeItem }
                };

        if (slotMapping.TryGetValue(_Item, out InventoryItemEUIController mappedItem) && mappedItem != null)
        {
            moduleState = ModuleItemManager.Instance.Get_InventoryModuleState(mappedItem);
            ThisDescPanel.SetOn_Desc(moduleState);
        }
    }


    #endregion

    #region Get

    // 비어있는 슬롯 가져오기
    private InventorySlotEUIController Get_EquipedEmptySlot(List<InventorySlotEUIController> _TargetList)
    { 
        for (int i = 0; i < _TargetList.Count; i++)
        {
            if (!_TargetList[i].ThisSlotItem.gameObject.activeSelf) return _TargetList[i];
        }
        return null;
    }

    #endregion

    #region Gen

    // 인벤토리 아이템 생성
    public List<InventoryItemEUIController> Gen_NewItemList(State_ItemData _State)
    {
        List<InventoryItemEUIController> result = new List<InventoryItemEUIController>();
        for (int i = 0; i < Inventories.Count; i++)
        {
            InventoryItemEUIController spawnItem =
                Inventories[i].Gen_Item_ThisInventory(_State, this);
            result.Add(spawnItem);

        }
        return result;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
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
    }

    #endregion

    #region Item & Slot

    #region  Interact -> Slot

    // 아이템 상호작용
    public void Try_InteractItem(InventorySlotEUIController _CurrentSlot)
    {
        Dele dele = null;

        int panelIndex = ThisPanelTabList.IndexOf(CurrentThisPanelTab);

        if (panelIndex == 0) // Equiped Window
        {
            dele = _CurrentSlot.IsInventory ? 
                Try_Interact_PutIn_Equip : Try_Interact_PutOut_Equip;
            
            ModuleItemManager.Instance.Reset_Interface();
        }
        else if (panelIndex == 1) // Forge Window
        {
            int panelIndexOfForge = ForgeInteractPanels.IndexOf(CurrentForgeInteractPanel);

            if (panelIndexOfForge == 0)
                dele = _CurrentSlot.IsInventory ?
                    Try_Interact_PutInDescompositionSlot : Try_Interact_PutOutDescompositionSlot;

            else if (panelIndexOfForge == 1)
                dele = _CurrentSlot.IsInventory ?
                    Try_Interact_PutInFusionSlot : Try_Interact_PutOutFusionSlot;

            else
                dele = _CurrentSlot.IsInventory ?
                    Try_Interact_PutInUpgradeSlot : Try_Interact_PutOutUpgradeSlot;
        }

        if (dele != null) dele();
    }

    #endregion

    #region Equip -> Slot

    // 장착 가능한가?
    private bool Can_PutIn_Equip(out ModuleState _ModuleState)
    {
        // 슬롯이 남아있는가
        bool enoughSlot = ModuleItemManager.Instance.Equiped_MSList.Count < EquipedSlots.Count;
        
        // 현재 패널이 올바른가
        bool openedEquipPanel = EquipedPanelGO.gameObject.activeSelf;

        // 이미 장착슬롯에 있진 않은가
        _ModuleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentSlot.ThisSlotItem);
        bool isContain = !ModuleItemManager.Instance.Equiped_MSList.Contains(_ModuleState);

        Debug.Log(enoughSlot + " / " + openedEquipPanel + " / " + isContain);
        return enoughSlot && openedEquipPanel && isContain;
    }

    // 장착 시도
    private void Try_Interact_PutIn_Equip()
    {
        if (!Can_PutIn_Equip(out ModuleState moduleState)) return;

        InventorySlotEUIController emptySlot = Get_EquipedEmptySlot(EquipedSlots);
        int index = EquipedSlots.IndexOf(emptySlot);
        ItemData data = moduleState.ThisItemData;
        State_ItemData state = new State_ItemData(data.ID, data.Rank, data.BoostLv);
        // Module UI
        emptySlot.ThisSlotItem.gameObject.SetActive(true);
        emptySlot.ThisSlotItem.Set_Data(state);

        moduleState.ItemUI_Extra.Add(emptySlot.ThisSlotItem);

        // Player HUD
        MainGameUIManager.Instance.PlayerHUD_UIController.MEISList[index].ThisSlotItem.Set_Data(state);

        moduleState.ItemUI_Extra.Add(MainGameUIManager.Instance.PlayerHUD_UIController.MEISList[index].ThisSlotItem);

        ModuleItemManager.Instance.Equiped_MSList.Add(moduleState);


        // Tween
        Set_DotweenInEquip(1f, EquipPanelInnerList);
        Set_EquipDesc();

        ModuleItemManager.Instance.Set_MainChipData();
    }

    // 장착 해제 시도
    private void Try_Interact_PutOut_Equip()
    {
        ModuleState moduleState = ModuleItemManager.Instance.Get_EquipedModuleState(CurrentSlot.ThisSlotItem);

        moduleState.ItemUI_Extra[0].gameObject.SetActive(false);
        moduleState.ItemUI_Extra[1].gameObject.SetActive(false);
        moduleState.ItemUI_Extra.Clear();

        ModuleItemManager.Instance.Equiped_MSList.Remove(moduleState);

        CurrentSlot = null;
        //CurrentSlot.SetOff_SelectedItem();

        // Tween
        Set_DotweenInEquip(0f, EquipPanelInnerList);
        Set_EquipDesc();

        ModuleItemManager.Instance.Set_MainChipData();
    }

    // 장착된 모듈들의 설명
    private void Set_EquipDesc()
    {
        for (int i = 0; i < EquipedSlots.Count; i++)
        {
            ModuleState moduleState = ModuleItemManager.Instance.Get_EquipedModuleState(EquipedSlots[i].ThisSlotItem);
            
            EquipDescStateTxtList[i].text = moduleState != null ?
                moduleState.ThisItemData.EquipDescription : "-";
        }
    }

    #endregion

    #region Descomposition -> Slot

    // 장착
    private void Try_Interact_PutInDescompositionSlot()
    {
        ModuleState moduleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentSlot.ThisSlotItem);

        CurrentDecompositionItem = CurrentSlot.ThisSlotItem;

        ItemData data = moduleState.ThisItemData;
        State_ItemData state = new State_ItemData(data.ID, data.Rank, data.BoostLv);

        DecompositionSlot.ThisSlotItem.gameObject.SetActive(true);
        DecompositionSlot.ThisSlotItem.Set_Data(state);

        Preview_GainMS.text = ModuleItemManager.Get_MS_ByDescomposition(moduleState).ToString();
        Preview_GainBC.text = ModuleItemManager.Get_BC_ByDescomposition(moduleState).ToString();
    }

    // 해제
    private void Try_Interact_PutOutDescompositionSlot()
    {
        CurrentDecompositionItem = null;

        DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);

        CurrentSlot = null;
        //DecompositionSlot.SetOff_SelectedItem();

        Preview_GainMS.text = "-";
        Preview_GainBC.text = "-";
    }


    #endregion

    #region Fusion -> Slot

    private bool Can_PutIn_Fusion(out ModuleState _ModuleState)
    {
        _ModuleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentSlot.ThisSlotItem);
        
        return !CurrentFusionItemList.Contains(CurrentSlot.ThisSlotItem) &&
            CurrentFusionItemList.Contains(null) &&
            _ModuleState.ThisItemData.Rank < 5;
    }

    // 장착
    private void Try_Interact_PutInFusionSlot()
    {
        if (!Can_PutIn_Fusion(out ModuleState moduleState)) return;

        for (int i = 0; i < FusionSlotList.Count; i++)
        {
            if (CurrentFusionItemList[i] == null)
            {
                CurrentFusionItemList[i] = CurrentSlot.ThisSlotItem;

                ItemData data = moduleState.ThisItemData;
                State_ItemData state = new State_ItemData(data.ID, data.Rank, data.BoostLv);

                FusionSlotList[i].ThisSlotItem.gameObject.SetActive(true);
                FusionSlotList[i].ThisSlotItem.Set_Data(state);

                if (!CurrentFusionItemList.Contains(null))
                {
                    ModuleState moduleState1 = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[0]);
                    ModuleState moduleState2 = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[1]);

                    Preview_NeedMS.text = moduleState1.ThisItemData.Rank == moduleState2.ThisItemData.Rank ?
                         ModuleItemManager.Get_MC_ForFusion(moduleState).ToString() : "≠";
                }
                return;
            }
        }
    }

    // 해제
    private void Try_Interact_PutOutFusionSlot()
    {
        int index = FusionSlotList.IndexOf(CurrentSlot);

        CurrentFusionItemList[index] = null;

        FusionSlotList[index].ThisSlotItem.gameObject.SetActive(false);

        CurrentSlot = null;
        //FusionSlotList[index].SetOff_SelectedItem();
        Preview_NeedMS.text = "-";
    }

    #endregion

    #region Upgrade -> Slot

    private bool Can_PutIn_Upgrade(out ModuleState _ModuleState)
    {
        _ModuleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentSlot.ThisSlotItem);
        return _ModuleState.ThisItemData.BoostLv < PlayerManager.Instance.PlayerController.MaxBoostLv;
    }
    
    // 장착
    private void Try_Interact_PutInUpgradeSlot()
    {
        if (!Can_PutIn_Upgrade(out ModuleState moduleState)) return;

        CurrentUpgradeItem = CurrentSlot.ThisSlotItem;

        UpgradeSlot.ThisSlotItem.gameObject.SetActive(true);

        ItemData data = moduleState.ThisItemData;
        State_ItemData state = new State_ItemData(data.ID, data.Rank, data.BoostLv);


        UpgradeSlot.ThisSlotItem.Set_Data(state);


        Preview_NeedEC.text = ModuleItemManager.Get_EC_ForUpgrade(moduleState).ToString();

    }

    // 해제
    private void Try_Interact_PutOutUpgradeSlot()
    {
        UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);

        CurrentSlot = null;
        //UpgradeSlot.SetOff_SelectedItem();
        CurrentUpgradeItem = null;

        Preview_NeedEC.text = "-";
    }

    #endregion

    #endregion

    #region Role

    #region Descomposition


    // 인터렉트 -> 분해
    private void Try_Desomposition()
    {
        if (CurrentDecompositionItem != null)
        {
            // Take Info
            ModuleState moduleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentDecompositionItem);

            // Take
            PlayerManager.Instance.PlayerController.CurrentMS.Value += 
                ModuleItemManager.Get_MS_ByDescomposition(moduleState);
            PlayerManager.Instance.PlayerController.CurrentBC.Value +=
                ModuleItemManager.Get_BC_ByDescomposition(moduleState);

            ModuleItemManager.Instance.Remove_ModuleState(CurrentDecompositionItem);

            // Be Empty
            //Remove_DataInInventory(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentDecompositionItem));
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);



            CurrentSlot = null;

            ModuleUpgradeController.UsingShop.Take_Damage(false);
            List<Image> tweenImgList = new List<Image>();
            tweenImgList.AddRange(ForgePanelInnerList);
            tweenImgList.AddRange(ForgeInteractPanels[0].InnerImgs);

            Set_DotweenInEquip(1f, tweenImgList);
            ModuleItemManager.Instance.Set_MainChipData();
            Reset_ForgeElementPanel();
        }
    }

    #endregion

    #region Fusion

    // 인터렉트 -> 합성
    private void Try_Fusion()
    {
        ModuleState moduleState1 = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[0]);
        if (CurrentFusionItemList[0] == null ||
            CurrentFusionItemList[1] == null ||
            moduleState1.ThisItemData.Rank !=
            ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[1]).ThisItemData.Rank)
        { return; }

        int needMS = ModuleItemManager.Get_MC_ForFusion(moduleState1);
        if (needMS == 0 ||
            needMS > PlayerManager.Instance.PlayerController.CurrentMS.Value)
        { return; }

        // Take Info

        ItemData itemData;

        if (Random.Range(0, 2) == 0)
        { itemData = new ItemData(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[0]).ThisItemData); }
        else
        { itemData = new ItemData(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[1]).ThisItemData); }

        itemData.Rank++;
        itemData.BoostLv = 1;

        for (int i = FusionSlotList.Count - 1; i >= 0; i--)
        {
            // Be Empty
            //Remove_DataInInventory(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentFusionItemList[i]));

            // Give
            FusionSlotList[i].ThisSlotItem.gameObject.SetActive(false);

            ModuleItemManager.Instance.Remove_ModuleState(CurrentFusionItemList[i]);
        }

        PlayerManager.Instance.PlayerController.CurrentMS.Value -= needMS;

        // Take
        ModuleItemManager.Instance.Gain_ModuleState(itemData);

        CurrentSlot = null;

        ModuleUpgradeController.UsingShop.Take_Damage(false);

        List<Image> tweenImgList = new List<Image>();
        tweenImgList.AddRange(ForgePanelInnerList);
        tweenImgList.AddRange(ForgeInteractPanels[1].InnerImgs);

        Set_DotweenInEquip(1f, tweenImgList);
        ModuleItemManager.Instance.Set_MainChipData();
        Reset_ForgeElementPanel();
    }

    #endregion

    #region Upgrade

    // 인터렉트 -> 강화
    private void Try_Upgrade()
    {
        if (CurrentUpgradeItem != null)
        {
            ModuleState moduleState = ModuleItemManager.Instance.Get_InventoryModuleState(CurrentUpgradeItem);

            // Cost
            int needEC = ModuleItemManager.Get_EC_ForUpgrade(moduleState);

            if (needEC == 0 ||
                needEC > PlayerManager.Instance.PlayerController.CurrentEC.Value)
            { Debug.Log("?");  return; }

            // Take Info
            ItemData itemData = new ItemData(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentUpgradeItem).ThisItemData);
            if (itemData.BoostLv >= PlayerManager.Instance.PlayerController.MaxBoostLv)
            { Debug.Log("?"); return; }

            itemData.BoostLv++;

            // Be Empty
            //Remove_DataInInventory(ModuleItemManager.Instance.Get_InventoryModuleState(CurrentUpgradeItem));

            // Give
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);

            ModuleItemManager.Instance.Remove_ModuleState(CurrentUpgradeItem);

            PlayerManager.Instance.PlayerController.CurrentEC.Value -= needEC;

            // Take
            ModuleItemManager.Instance.Gain_ModuleState(itemData);

            CurrentSlot = null;

            ModuleUpgradeController.UsingShop.Take_Damage(false);

            List<Image> tweenImgList = new List<Image>();
            tweenImgList.AddRange(ForgePanelInnerList);
            tweenImgList.AddRange(ForgeInteractPanels[2].InnerImgs);

            Set_DotweenInEquip(1f, tweenImgList);
            ModuleItemManager.Instance.Set_MainChipData(); 
            Reset_ForgeElementPanel();
        }
    }

    #endregion

    #endregion
}
