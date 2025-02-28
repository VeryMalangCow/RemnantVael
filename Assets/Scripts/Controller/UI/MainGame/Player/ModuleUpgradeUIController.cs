using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : PanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;
    [SerializeField] private string LabelName;
    [SerializeField] private string AmalgamationName;

    [Space(10)]
    [Header("=== BC, EC")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;
    [SerializeField] public TMP_Text MSTxt;

    [Space(10)]
    [Header("=== Module")]

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] private ModifyInventory MI_InEquipTab;
    [SerializeField] private ModifyOwnEachBtn InEquipToggleBtn;

    [Space(5)]
    [Header("* Module")]
    [SerializeField] private GameObject ModulePanelGO;
    [SerializeField] private List<ModifyEachInventorySlot> EquipedMEIS_List;
    [SerializeField] private List<Image> EquipPanelInnerList;
    [SerializeField] private List<TMP_Text> EquipDescStateTxtList;

    [Space(5)]
    [Header("* Synergy")]
    [SerializeField] private GameObject SynergyPanelGO;
    [SerializeField] private GameObject SynergyPanelExistGO;
    [SerializeField] private GameObject SynergyPanelEmptyGO;
    [SerializeField] private Transform SynergyPanelInnerParentTF;
    [SerializeField] private Transform SynergySlotParentTF;
    [HideInInspector] private List<ModifySynergySlot> SynergySlotList = new List<ModifySynergySlot>();
    [SerializeField] public Sprite SynergyTier0;
    [SerializeField] public Sprite SynergyTier1;
    [SerializeField] public Sprite SynergyTier2;

    [Space(5)]
    [Header("* Synergy Desc")]
    [SerializeField] private GameObject SynergyDescTF;
    [HideInInspector] private ModifySynergySlot SelectedMSS;
    [SerializeField] private Image SelectViewImg;
    [SerializeField] private TMP_Text SelectViewName;
    [SerializeField] private TMP_Text SelectViewAmalgamation;
    [SerializeField] private Transform SynergyDescLinerParentTF;
    [SerializeField] private Transform SynergyDescTextParentTF;
    [SerializeField] private List<TMP_Text> AmalgamationTxtList;
    [SerializeField] public List<TMP_Text> AmalgamationDescTxtList;

    [Space(10)]
    [Header("-- In Reinforce")]
    [SerializeField] private ModifyInventory MI_InReinforceTab;
    [SerializeField] private List<SimplePanelAndBtn> ReinforceInteractPanels;
    [HideInInspector] private SimplePanelAndBtn CurrentReinforceInteractPanel;
    [SerializeField] private List<Image> ReinforcePanelInnerList;

    // Other
    [HideInInspector] private List<ModifyInventory> MI_List;



    [Space(5)]
    [Header("* Decomposition")]
    [SerializeField] private ModifyEachInventorySlot DecompositionSlot;
    [SerializeField] private ModifyOwnEachBtn DecompositionBtn;
    [SerializeField] private TMP_Text Preview_MS;
    [SerializeField] private TMP_Text Preview_BC;

    [Space(5)]
    [Header("* Fusion")]
    [SerializeField] private List<ModifyEachInventorySlot> FusionSlotList;
    [SerializeField] private ModifyOwnEachBtn FusionBtn;
    [SerializeField] private TMP_Text Preview_NeedMS;

    [Space(5)]
    [Header("* Upgrade")]
    [SerializeField] private ModifyEachInventorySlot UpgradeSlot;
    [SerializeField] private ModifyOwnEachBtn UpgradeBtn;
    [SerializeField] private TMP_Text Preview_NeedEC;

    [Space(5)]
    [Header("* Current Details")]
    [SerializeField] private ModifyEachInventoryItem CurrentDecompositionItem;
    [SerializeField] private List<ModifyEachInventoryItem> CurrentFusionItemList;
    [SerializeField] private ModifyEachInventoryItem CurrentUpgradeItem;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public ModifyEachInventorySlot CurrentSelectedMEIS;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private ModifyOwnEachBtn CloseBtn;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private ModifyDescPanel_ForModuleUpgrade ThisDescPanel;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private TMP_Text DurablityTxt;
    [SerializeField] private TMP_Text DurablityStateTxt;
    [SerializeField] private string DurablityStringTxt;
    [SerializeField] private Transform FillImgListParentTF;
    [HideInInspector] private List<Image> FillImgList;

    [Space(10)]
    [Header("=== Color")]
    [Header("-- MainColor")]
    [SerializeField] public List<TMP_Text> TabTxtList;
    [HideInInspector] public List<Component> MainColorCompList;
    [Header("-- SubColor")]
    [SerializeField] public List<CanvasGroup> LightTabCGList;
    [HideInInspector] public List<Component> SubColorCompList;

    Sequence ForgeSeq;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        MI_InEquipTab.Offset();
        MI_InReinforceTab.Offset();

        ThisDescPanel.Offset();

        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.Offset();
            MET.ThisTabBtn.OwnerUIController = this;
        }

        foreach (ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            MEIS.Offset();
        }

        foreach (SimplePanelAndBtn SPAB in ReinforceInteractPanels)
        {
            SPAB.Offset(this);
        }
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;



        DecompositionSlot.Offset();
        DecompositionSlot.ThisSlotItem.Offset();
        DecompositionSlot.ThisSlotItem.OwnerUIController = this;

        foreach (ModifyEachInventorySlot meii in FusionSlotList)
        {
            meii.Offset();
            meii.ThisSlotItem.Offset();
            CurrentFusionItemList.Add(null);
            meii.ThisSlotItem.OwnerUIController = this;
        }

        UpgradeSlot.Offset();
        UpgradeSlot.ThisSlotItem.Offset();
        UpgradeSlot.ThisSlotItem.OwnerUIController = this;

        DecompositionBtn.Offset();
        DecompositionBtn.OwnerUIController = this;
        FusionBtn.Offset();
        FusionBtn.OwnerUIController = this;
        UpgradeBtn.Offset();
        UpgradeBtn.OwnerUIController = this;

        InEquipToggleBtn.Offset();
        InEquipToggleBtn.OwnerUIController = this;
    }

    protected override void Offset_UI()
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

        // Synergy
        for (int i = 0; i < SynergySlotParentTF.childCount; i++)
        {
            if (SynergySlotParentTF.GetChild(i).TryGetComponent(out ModifySynergySlot MSS))
            {
                SynergySlotList.Add(MSS);
                MSS.Offset();
                MSS.OwnerUIController = this;


                MainColorCompList.Add(MSS.ThisTierImg);
                SubColorCompList.Add(MSS.ThisTxt);
            }
        }
        for (int i = 0; i < AmalgamationTxtList.Count; i++)
        { 
            AmalgamationTxtList[i].text = AmalgamationName;
        }

        for (int i = 0; i < SynergyDescLinerParentTF.childCount; i++)
        {
            if (SynergyDescLinerParentTF.GetChild(i).TryGetComponent(out Image Img))
            {
                MainColorCompList.Add(Img);
            }
        }
        for (int i = 0; i < SynergyDescTextParentTF.childCount; i++)
        {
            if (SynergyDescTextParentTF.GetChild(i).TryGetComponent(out TMP_Text Txt) && 
                !AmalgamationDescTxtList.Contains(Txt))
            {
                SubColorCompList.Add(Txt);
            }
        }

        for (int i = 0; i < AmalgamationDescTxtList.Count; i++)
        {
            MainColorCompList.Add(AmalgamationDescTxtList[i]);
        }
        SubColorCompList.Add(SelectViewAmalgamation);


        // Dur
        DurablityTxt.text = DurablityStringTxt + " :";

        FillImgList = new List<Image>();
        for (int i = 0; i < FillImgListParentTF.childCount; i++)
        {
            FillImgListParentTF.GetChild(i).gameObject.transform.GetChild(0).gameObject.TryGetComponent(out Image EmptyImg);
            FillImgList.Add(EmptyImg);
        }

        MI_List = new List<ModifyInventory>
        {
            MI_InEquipTab,
            MI_InReinforceTab
        };
        LabelTxt.text = LabelName;

        MainColorCompList.Add(LabelTxt);

        MainColorCompList.Add(ThisDescPanel.ItemNameTxt);
        SubColorCompList.Add(ThisDescPanel.ITemIntroTxt);

        MainColorCompList.Add(ThisDescPanel.CurrentActualRankTxt);
        SubColorCompList.Add(ThisDescPanel.CurrentRankTxt);

        MainColorCompList.Add(ThisDescPanel.CurrentActualBoostLvTxt);
        SubColorCompList.Add(ThisDescPanel.CurrentBoostLvTxt);

        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
        SubColorCompList.AddRange(EquipPanelInnerList);

        for (int i = 0; i < SynergyPanelInnerParentTF.childCount; i++)
        {
            if (SynergyPanelInnerParentTF.GetChild(i).TryGetComponent(out Image img))
            {
                SubColorCompList.Add(img);
            }
        }

        MainColorCompList.Add(InEquipToggleBtn.transform.GetChild(0).GetComponent<TMP_Text>());

        for (int i = 0; i < EquipDescStateTxtList.Count; i++)
        {
            MainColorCompList.Add(EquipDescStateTxtList[i]);
            if (EquipDescStateTxtList[i].gameObject.transform.childCount > 0 &&
                EquipDescStateTxtList[i].gameObject.transform.GetChild(0).TryGetComponent(out Image img))
            {
                SubColorCompList.Add(img);
            }
        }

        for (int i = 0; i < ReinforceInteractPanels.Count; i++)
        {
            if (ReinforceInteractPanels[i].PanelBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text txt))
            {
                MainColorCompList.Add(txt);
                MainColorCompList.Add(ReinforceInteractPanels[i].RoleDescTxt);
            }
        }

        if (DecompositionBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out TMP_Text decomTxt))
        {
            MainColorCompList.Add(decomTxt);
            decomTxt.text = ">>  " + ReinforceInteractPanels[0].BtnString + "  <<";
        }
        if (FusionBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out TMP_Text fusTxt))
        {
            MainColorCompList.Add(fusTxt);
            fusTxt.text = ">>  " + ReinforceInteractPanels[1].BtnString + "  <<";
        }
        if (UpgradeBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out TMP_Text upgTxt))
        {
            MainColorCompList.Add(upgTxt);
            upgTxt.text = ">>  " + ReinforceInteractPanels[2].BtnString + "  <<";
        }

        for (int i = 0; i < ReinforcePanelInnerList.Count; i++)
        {
            SubColorCompList.Add(ReinforcePanelInnerList[i]);
        }

        MainColorCompList.Add(Preview_MS);
        MainColorCompList.Add(Preview_BC);
        MainColorCompList.Add(Preview_NeedMS);
        MainColorCompList.Add(Preview_NeedEC);

        SetTabTxt(TabTxtList, MainColorCompList);
        TabTxtList.Clear(); TabTxtList = null;

        SetTabLightAlpha(0.1f, LightTabCGList, SubColorCompList);
        LightTabCGList.Clear(); LightTabCGList = null;

        Color mainClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, false);
        SetColor(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, true);
        SetColor(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;


        ModulePanelGO.gameObject.SetActive(true);
        SynergyPanelGO.gameObject.SetActive(false);
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.OnReset();
        }

        ResetReinforcePanel();
        ResetEquipPanel();
        OnReset_SPAB();
        SetOnEnable();
    }

    private void OnDisable()
    {
        SetOnDisable();
    }

    #endregion

    #region Set Panel

    public override void OpenThisPanel()
    {
        base.OpenThisPanel();

        MainGameUIManager.Instance.ModuleUpgrade_UIController.SetDur(
            ModuleUpgradeController.UsingShop.ThisDurablity);
    }

    public override void CloseThisPanel()
    {
        base.CloseThisPanel();
        ModuleUpgradeController.UsingShop = null;

    }
    public override void ChangeThisPanel(int _indexWindow)
    {
        float scrollValue = CurrentThisPanelTab.ThisTabScrollbar.value;
        base.ChangeThisPanel(_indexWindow);
        CurrentThisPanelTab.ThisTabScrollbar.value = scrollValue;

    }

    #endregion

    #region Set Dotween

    private void DotweenInEquip(float _A, string _TweenID, List<Image> _TweenImgList)
    {

        if (DOTween.IsTweening(_TweenID))
        { DOTween.Complete(_TweenID); }

        Sequence seq1 = DOTween.Sequence();
        Sequence seq2 = DOTween.Sequence();
        for (int i = 0; i < _TweenImgList.Count; i++)
        { seq1.Join(_TweenImgList[i].DOFade(_A, 0.1f)); }

        for (int i = 0; i < _TweenImgList.Count; i++)
        { seq2.Join(_TweenImgList[i].DOFade(0.5f, 0.1f)); }

        Sequence seq = DOTween.Sequence();
        seq.Append(seq1);
        seq.Append(seq2);
        seq.SetId(_TweenID);
    }

    private void SetOnEnable()
    {
        DOTween.Kill(ForgeSeq);
        ForgeSeq = DOTween.Sequence();

        List<RectTransform> RTList = new List<RectTransform>();

        if (DecompositionBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out RectTransform decomRT))
        { RTList.Add(decomRT); }
        if (FusionBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out RectTransform fusRT))
        { RTList.Add(fusRT); }
        if (UpgradeBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out RectTransform upgRT))
        { RTList.Add(upgRT); }

        for (int i = 0; i < RTList.Count; i++)
        { ForgeSeq.Join(RTList[i].DOScale(1.2f, 1.0f)); }

        ForgeSeq.SetLoops(-1, LoopType.Yoyo);
    }

    private void SetOnDisable()
    {
        DOTween.Kill(ForgeSeq);
        ForgeSeq = DOTween.Sequence();
        ForgeSeq = null;
    }

    #endregion

    #region Input

    public void TryInteractClick()
    {
        if (ModuleUpgradeController.UsingShop == null)
        { return; }

        // 아이템
        if (CurrentSelectedMEIS != null)
        { 
            TryInteractItem(); 
            return; 
        }
        else if (CurrentBtn != null)
        {
            // 닫기
            if (CurrentBtn == CloseBtn)
            {
                MainGameUIManager.Instance.ModuleUpgrade_UIController.CloseThisPanel();
                return;
            }

            // 탭
            for (int i = 0; i < ThisPanelTabList.Count; i++)
            {
                if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
                {
                    ChangeThisPanel(i);
                    ResetReinforcePanel();
                    SetEquipDesc();
                    return;
                }
            }

            // 장착 부분의 토글 버튼
            if (CurrentBtn == InEquipToggleBtn)
            {
                if (ModulePanelGO.activeSelf)
                {
                    ModulePanelGO.SetActive(false);
                    SynergyPanelGO.SetActive(true);
                    SynergyDescTF.SetActive(false);
                    return;
                }
                else
                {
                    ModulePanelGO.SetActive(true);
                    SynergyPanelGO.SetActive(false);
                    return;
                }
            }

            // 특별 상호작용
            if (CurrentBtn == DecompositionBtn)
            {
                TryDesomposition(); return;
            }
            else if (CurrentBtn == FusionBtn)
            {
                TryFusion(); return;
            }
            else if (CurrentBtn == UpgradeBtn)
            {
                TryUpgrade(); return;
            }

            // 특별 상호작용 탭
            for (int i = 0; i < ReinforceInteractPanels.Count; i++)
            {
                if (ReinforceInteractPanels[i].PanelBtn == CurrentBtn)
                {
                    // 다른 탭일 경우 초기화
                    if (ReinforceInteractPanels[i] != CurrentReinforceInteractPanel)
                    {
                        ResetReinforcePanel();
                    }

                    ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(true);
                    if (ReinforceInteractPanels[i].PanelBtn.gameObject.TryGetComponent(out CanvasGroup cg))
                    {
                        cg.alpha = 1f;
                    }
                    CurrentReinforceInteractPanel = ReinforceInteractPanels[i];
                }
                else
                {
                    ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(false);
                    if (ReinforceInteractPanels[i].PanelBtn.gameObject.TryGetComponent(out CanvasGroup cg))
                    {
                        cg.alpha = 0.5f;
                    }
                }
            }

            // 시너지 탭의 정보
            if (CurrentBtn is ModifySynergySlot mss && SynergySlotList.Contains(mss))
            {
                SynergyDescTF.SetActive(true);
                SelectedMSS = mss;

                MainChipData MCD = ModuleItemManager.Instance.GetCorrectMainChip(SelectedMSS.ID);

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

    #region Item -> Spawn

    // 비어있는 슬롯 가져오기
    private ModifyEachInventorySlot GetEquipedEmptySlot()
    {
        foreach(ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            if(MEIS.ThisSlotItem == null)
            {
                return MEIS;
            }
        }
        return null;
    }

    // 인벤토리 아이템 생성
    public List<ModifyEachInventoryItem> SpawnMEIIList(Sprite _ItemSprite, Sprite _RankImg, int _BoostLv)
    {
        List<ModifyEachInventoryItem> mi_List = new List<ModifyEachInventoryItem>();
        foreach (ModifyInventory MI in MI_List)
        {
            ModifyEachInventoryItem meii = MI.SpawnMEII_ThisInventory(_ItemSprite, _RankImg, _BoostLv);
            meii.OwnerUIController = this;
            mi_List.Add(meii);
        }
        return mi_List;
    }

    #endregion

    #region Item -> Interact

    // 아이템 상호작용
    public void TryInteractItem()
    {
        int indexOfAboutPanel = ThisPanelTabList.IndexOf(CurrentThisPanelTab);
        if (indexOfAboutPanel == 0) // Equiped Window
        {
            if (CurrentSelectedMEIS.IsInventory) // Equip
            {
                TryEquip();
            }
            else // Unequip
            {
                TryUnequip();
            }

            ModuleItemManager.Instance.ResetInterface();
        }
        else if (indexOfAboutPanel == 1) // Reinforce Window
        {
            if (CurrentSelectedMEIS.IsInventory)
            {
                int indexOfAboutReinforce = ReinforceInteractPanels.IndexOf(CurrentReinforceInteractPanel);
                if (indexOfAboutReinforce == 0) // Decompostion
                {
                    TryInteract_DesompositionSlot();
                }
                else if (indexOfAboutReinforce == 1) // Fusion
                {
                    TryInteract_FusionSlot();
                }
                else // Upgrade
                {
                    TryInteract_UpgradeSlot();
                }
            }
        }

    }

    
    #endregion

    #region Item -> About Equip

    // 장착 시도
    private void TryEquip()
    {
        if (ModuleItemManager.Instance.Equiped_MSList.Count >= EquipedMEIS_List.Count)
        { return; }

        if (!ModulePanelGO.gameObject.activeSelf)
        { return; }

        ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem);

        foreach (ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            if (ms.ThisExtraMEII.Contains(MEIS.ThisSlotItem))
            {
#if UNITY_EDITOR
                Debug.Log("이미 존재합니다.");
#endif
                return;
            }
        }

        // Module UI
        ModuleItemManager.Instance.Equiped_MSList.Add(ms);
        ModifyEachInventorySlot meis = GetEquipedEmptySlot();
        ModifyEachInventoryItem meii 
            = MI_InEquipTab.SpawnMEII_Module(
                meis, 
                ms.ThisItemData.ItemIcon, 
                ModuleItemManager.Instance.GetCorrectRankIcon(ms), 
                ms.ThisItemData.BoostLv);
        ms.ThisExtraMEII.Add(meii);
        meii.OwnerUIController = this;

        // Player HUD
        int slotIndex = EquipedMEIS_List.IndexOf(meis);

        ModifyEachInventoryItem meii_PlayerHUD
            = MI_InEquipTab.SpawnMEII_Module(
                MainGameUIManager.Instance.PlayerHUD_UIController.MEISList[slotIndex],
                ms.ThisItemData.ItemIcon,
                ModuleItemManager.Instance.GetCorrectRankIcon(ms),
                ms.ThisItemData.BoostLv);
        meii_PlayerHUD.IsCanSelect = false;
        ms.ThisExtraMEII.Add(meii_PlayerHUD);

        DotweenInEquip(1f, "EquipInner", EquipPanelInnerList);
        SetEquipDesc();

        ModuleItemManager.Instance.SetMainChipData();
    }

    // 장착 해제 시도
    private void TryUnequip()
    {
        ModuleState ms = ModuleItemManager.Instance.GetModuleState_Equiped(CurrentSelectedMEIS.ThisSlotItem);

        ModuleItemManager.Instance.Equiped_MSList.Remove(ms);
        ms.ThisExtraMEII.Remove(CurrentSelectedMEIS.ThisSlotItem);
        Destroy(CurrentSelectedMEIS.ThisSlotItem.gameObject);

        CurrentSelectedMEIS.ThisSlotItem = null;
        CurrentSelectedMEIS.OutIt_SelectedItem();


        DotweenInEquip(0f, "EquipInner", EquipPanelInnerList);
        SetEquipDesc();

        ModuleItemManager.Instance.SetMainChipData();
    }

    // 장착된 모듈들의 설명
    private void SetEquipDesc()
    {
        for (int i = 0; i < EquipedMEIS_List.Count; i++)
        {
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Equiped(EquipedMEIS_List[i].ThisSlotItem);
            if (ms != null)
            {
                EquipDescStateTxtList[i].text = ms.ThisItemData.EquipDescription;
            }
            else
            {
                EquipDescStateTxtList[i].text = "-";
            }
        }
    }

    // 장착 패널 리셋
    private void ResetEquipPanel()
    {
        ModuleItemManager.Instance.SetMainChipData();

        ModulePanelGO.gameObject.SetActive(true);
        SynergyPanelGO.gameObject.SetActive(false);
    }

    #endregion

    #region Item -> About Reinforce

    #region Descomposition

    // 장착 또는 해제
    private void TryInteract_DesompositionSlot()
    {
        if (CurrentSelectedMEIS == DecompositionSlot) // 해제
        {
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
            DecompositionSlot.OutIt_SelectedItem();
            CurrentDecompositionItem = null;

            Preview_MS.text = "-";
            Preview_BC.text = "-";
        }
        else if (CurrentDecompositionItem == null) // 선택
        {
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(true);

            DecompositionSlot.ThisSlotItem.SetData(
                CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);
            
            CurrentDecompositionItem = CurrentSelectedMEIS.ThisSlotItem;

            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentDecompositionItem);
            Preview_MS.text = (ms.ThisItemData.Rank * 2).ToString();
            Preview_BC.text = ms.ThisItemData.BoostLv.ToString();
            return;
        }
    }

    // 인터렉트 -> 분해
    private void TryDesomposition()
    {
        if (CurrentDecompositionItem != null)
        {
            // Take Info
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentDecompositionItem);
            int itemRank = ms.ThisItemData.Rank;
            int boostLv = ms.ThisItemData.BoostLv;

            // Be Empty
            RemoveDataInInventory(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentDecompositionItem));

            // Give
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
            DecompositionSlot.OutIt_SelectedItem();

            ModuleItemManager.Instance.DeleteModuleState(CurrentDecompositionItem);


            // Take
            PlayerManager.Instance.PlayerController.CurrentMS.Value += itemRank * 2;
            PlayerManager.Instance.PlayerController.CurrentBC.Value += boostLv;

            ModuleUpgradeController.UsingShop.TakeDamage(false); 
            DotweenInEquip(1f, "ReinforceInner", ReinforcePanelInnerList);
            Preview_MS.text = "-";
            Preview_BC.text = "-";

            ModuleItemManager.Instance.SetMainChipData();
        }
    }

    #endregion

    #region Fusion

    // 합성 슬롯에 장착
    private void TryInteract_FusionSlot()
    {
        if (FusionSlotList.Contains(CurrentSelectedMEIS)) // 해제
        {
            int index = FusionSlotList.IndexOf(CurrentSelectedMEIS);

            FusionSlotList[index].ThisSlotItem.gameObject.SetActive(false);
            FusionSlotList[index].OutIt_SelectedItem();
            CurrentFusionItemList[index] = null;
            Preview_NeedMS.text = "-";
        }
        else
        {
            if (CurrentFusionItemList.Contains(CurrentSelectedMEIS.ThisSlotItem))
            { return; }

            if (ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.Rank >= 5)
            { return; }

            for (int i = 0; i < FusionSlotList.Count; i++)
            {
                if (CurrentFusionItemList[i] == null)
                {
                    FusionSlotList[i].ThisSlotItem.gameObject.SetActive(true);

                    FusionSlotList[i].ThisSlotItem.SetData(
                        CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                        CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                        ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);

                    CurrentFusionItemList[i] = CurrentSelectedMEIS.ThisSlotItem;
                    bool CanSeePreview = true;
                    for (int j = 0; j < CurrentFusionItemList.Count; j++)
                    {
                        if (CurrentFusionItemList[j] == null)
                        {
                            CanSeePreview = false;
                        }
                    }
                    
                    if (CanSeePreview)
                    {
                        if (ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[0]).ThisItemData.Rank
                        == ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[1]).ThisItemData.Rank)
                        {
                            Preview_NeedMS.text = ModuleItemManager.Instance.NeedMC_AbleFusion(CurrentFusionItemList[0]).ToString();
                        }
                        else
                        {
                            Preview_NeedMS.text = "≠";
                        }
                    }

                    return;
                }
            }
        }
    }

    // 인터렉트 -> 합성
    private void TryFusion()
    {
        if (CurrentFusionItemList[0] == null ||
            CurrentFusionItemList[1] == null ||
            ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[0]).ThisItemData.Rank !=
            ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[1]).ThisItemData.Rank)
        { return; }

        int needMS = ModuleItemManager.Instance.NeedMC_AbleFusion(CurrentFusionItemList[0]);
        if (needMS == 0 ||
            needMS > PlayerManager.Instance.PlayerController.CurrentMS.Value)
        { return; }

        // Take Info

        ItemData itemData;

        if (UnityEngine.Random.Range(0, 2) == 0)
        { itemData = new ItemData(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[0]).ThisItemData); }
        else
        { itemData = new ItemData(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[1]).ThisItemData); }

        itemData.Rank++;
        itemData.BoostLv = 1;

        for (int i = FusionSlotList.Count - 1; i >= 0; i--)
        {
            // Be Empty
            RemoveDataInInventory(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[i]));

            // Give

            FusionSlotList[i].ThisSlotItem.gameObject.SetActive(false);
            FusionSlotList[i].OutIt_SelectedItem();

            ModuleItemManager.Instance.DeleteModuleState(CurrentFusionItemList[i]);
        }

        PlayerManager.Instance.PlayerController.CurrentMS.Value -= needMS;

        // Take
        ModuleItemManager.Instance.GetModuleState(itemData);

        ModuleUpgradeController.UsingShop.TakeDamage(false);
        DotweenInEquip(1f, "ReinforceInner", ReinforcePanelInnerList);
        Preview_NeedMS.text = "-";

        ModuleItemManager.Instance.SetMainChipData();
    }

    #endregion

    #region Upgrade

    // 장착 또는 해제
    private void TryInteract_UpgradeSlot()
    {
        if (CurrentSelectedMEIS == UpgradeSlot) // 해제
        {
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);
            UpgradeSlot.OutIt_SelectedItem();
            CurrentUpgradeItem = null;

            Preview_NeedEC.text = "-";
        }
        else if (CurrentUpgradeItem == null &&
            ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv < PlayerManager.Instance.PlayerController.MaxBoostLv) // 선택
        {
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(true);

            UpgradeSlot.ThisSlotItem.SetData(
                CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                ModuleItemManager.Instance.GetModuleState_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);

            CurrentUpgradeItem = CurrentSelectedMEIS.ThisSlotItem;
            Preview_NeedEC.text = ModuleItemManager.Instance.NeedEC_AbleUpgrade(CurrentUpgradeItem).ToString();
            return;
        }
    }

    // 인터렉트 -> 강화
    private void TryUpgrade()
    {
        if (CurrentUpgradeItem != null)
        {
            // Cost
            int needEC = ModuleItemManager.Instance.NeedEC_AbleUpgrade(CurrentUpgradeItem);

            if (needEC == 0 ||
                needEC > PlayerManager.Instance.PlayerController.CurrentEC.Value)
            { return; }

            // Take Info
            ItemData itemData = new ItemData(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentUpgradeItem).ThisItemData);
            if (itemData.BoostLv >= PlayerManager.Instance.PlayerController.MaxBoostLv)
            { return; }

            itemData.BoostLv++;

            // Be Empty
            RemoveDataInInventory(ModuleItemManager.Instance.GetModuleState_Inventory(CurrentUpgradeItem));

            // Give
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);
            UpgradeSlot.OutIt_SelectedItem();

            ModuleItemManager.Instance.DeleteModuleState(CurrentUpgradeItem);

            PlayerManager.Instance.PlayerController.CurrentEC.Value -= needEC;

            // Take
            ModuleItemManager.Instance.GetModuleState(itemData);

            ModuleUpgradeController.UsingShop.TakeDamage(false);
            DotweenInEquip(1f, "ReinforceInner", ReinforcePanelInnerList);
            Preview_NeedEC.text = "-";

            ModuleItemManager.Instance.SetMainChipData();
        }
    }

    #endregion


    // 강화 패널 리셋
    public void ResetReinforcePanel()
    {
        DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
        foreach (ModifyEachInventorySlot meis in FusionSlotList)
        { meis.ThisSlotItem.gameObject.SetActive(false); }
        UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);

        CurrentDecompositionItem = null;
        for (int i = 0; i < CurrentFusionItemList.Count; i++)
        { CurrentFusionItemList[i] = null; }
        CurrentUpgradeItem = null;

        Preview_BC.text = "-";
        Preview_MS.text = "-";
        Preview_NeedEC.text = "-";
        Preview_NeedMS.text = "-";
    }

    // 인벤토리에 슬롯에 연결된 파일 Null로 바꾸기 (Missing이면 파일에 자리를 차지하게 됨)
    private void RemoveDataInInventory(ModuleState _MS)
    {
        foreach (ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            foreach (ModifyEachInventoryItem MEII in _MS.ThisExtraMEII)
            {
                if (MEIS.ThisSlotItem == MEII)
                {
                    MEIS.ThisSlotItem = null;
                }
            }
        }
        foreach (ModifyEachInventoryItem MEII in _MS.ThisMEII)
        {
            MI_InEquipTab.RemoveItemInSlotData(MEII);
            MI_InReinforceTab.RemoveItemInSlotData(MEII);
        }
        
    }

    #endregion

    #region Kind of Reinforce Panel & Btn

    [System.Serializable]
    class SimplePanelAndBtn
    {
        public RectTransform PanelRT;
        public ModifyOwnEachBtn PanelBtn;
        public TMP_Text RoleDescTxt;
        public string BtnString;
        public string RoleString;

        public void Offset(ModuleUpgradeUIController _MUUC)
        {
            PanelBtn.Offset();
            if (PanelBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out TMP_Text txt))
            { txt.text = BtnString; }
            RoleDescTxt.text = RoleString;
            PanelBtn.OwnerUIController = _MUUC;
        }
    }

    private void OnReset_SPAB()
    {
        for (int i = 0; i < ReinforceInteractPanels.Count; i++)
        {
            /*if (0 == i)
            {
                ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(true);
                CurrentReinforceInteractPanel = ReinforceInteractPanels[i];
            }
            else
            {
                ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(false);
            }*/
            CurrentReinforceInteractPanel = null;
            ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(false);
            if (ReinforceInteractPanels[i].PanelBtn.gameObject.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 0.5f;
            }
        }
        Preview_BC.text = "-";
        Preview_MS.text = "-";
        Preview_NeedEC.text = "-";
        Preview_NeedMS.text = "-";
    }

    #endregion

    #region Synergy

    public void SetSynergySlots(Dictionary<int, int> _Dict)
    {
        if (_Dict.Count <= 0)
        {
            // 패널 키기/끄기
            SynergyPanelExistGO.SetActive(false);
            SynergyPanelEmptyGO.SetActive(true);
        }
        else
        {
            // 패널 키기/끄기
            SynergyPanelExistGO.SetActive(true);
            SynergyPanelEmptyGO.SetActive(false);

            // 모두 끄기
            for (int i = 0; i < SynergySlotList.Count; i++)
            {
                SynergySlotList[i].SetOffSynergySlot();
            }

            // 가지고 있는 시너지 부분을 추가
            int currentSeq = 0;
            foreach (KeyValuePair<int, int> keyValuePair in _Dict)
            {
                MainChipData MDC = ModuleItemManager.Instance.GetCorrectMainChip(keyValuePair.Key);
                SynergySlotList[currentSeq].SetOnSynergySlot(keyValuePair.Key, MDC.ThisIcon, keyValuePair.Value);

                currentSeq++;
            }
        }
        
    }

    #endregion

    #region Desc

    public void SetDesc(ModifyEachInventoryItem _MEII)
    {
        ModuleState ms_InInventory = ModuleItemManager.Instance.GetModuleState_Inventory(_MEII);
        if (ms_InInventory != null)
        {
            ThisDescPanel.SetDesc(ms_InInventory);
            return;
        }

        ModuleState ms_InEquiped = ModuleItemManager.Instance.GetModuleState_Equiped(_MEII);
        if (ms_InEquiped != null)
        {
            ThisDescPanel.SetDesc(ms_InEquiped);
            return;
        }

        if (_MEII == DecompositionSlot.ThisSlotItem &&
            CurrentDecompositionItem != null)
        {
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentDecompositionItem);
            ThisDescPanel.SetDesc(ms);
        }
        else if (_MEII == FusionSlotList[0].ThisSlotItem &&
            CurrentFusionItemList[0] != null)
        {
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[0]);
            ThisDescPanel.SetDesc(ms);
        }
        else if (_MEII == FusionSlotList[1].ThisSlotItem &&
            CurrentFusionItemList[1] != null)
        {
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentFusionItemList[1]);
            ThisDescPanel.SetDesc(ms);
        }
        else if (_MEII == UpgradeSlot.ThisSlotItem &&
            CurrentUpgradeItem != null)
        {
            ModuleState ms = ModuleItemManager.Instance.GetModuleState_Inventory(CurrentUpgradeItem);
            ThisDescPanel.SetDesc(ms);
        }
    }

    public void SetOffDesc()
    {
        ThisDescPanel.SetDescOff();
    }

    #endregion

    #region Dur

    public void SetDur(int _DurState)
    {
        base.SetDur(_DurState, FillImgList, DurablityStateTxt);
    }

    #endregion
}
