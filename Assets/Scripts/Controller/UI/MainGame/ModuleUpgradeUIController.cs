using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;
    [SerializeField] private string LabelName;

    [Space(10)]
    [Header("=== Module")]

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] private ModifyInventory MI_InEquipTab;
    [SerializeField] private List<ModifyEachInventorySlot> EquipedMEIS_List;
    [SerializeField] private List<Image> EquipPanelInnerList;
    [SerializeField] private List<TMP_Text> EquipDescStateTxtList;

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

    [Space(5)]
    [Header("* Fusion")]
    [SerializeField] private List<ModifyEachInventorySlot> FusionSlotList;
    [SerializeField] private ModifyOwnEachBtn FusionBtn;

    [Space(5)]
    [Header("* Upgrade")]
    [SerializeField] private ModifyEachInventorySlot UpgradeSlot;
    [SerializeField] private ModifyOwnEachBtn UpgradeBtn;

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
    }

    protected override void Offset_UI()
    {
        
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
            Debug.Log("Equiped Window");
            if (CurrentSelectedMEIS.IsInventory) // Equip
            {
                TryEquip();
            }
            else // Unequip
            {
                TryUnequip();
            }

            BoostItemManager.Instance.ResetInterface();
        }
        else if (indexOfAboutPanel == 1) // Reinforce Window
        {
            Debug.Log("Reinforce Window");
            if (CurrentSelectedMEIS.IsInventory)
            {
                int indexOfAboutReinforce = ReinforceInteractPanels.IndexOf(CurrentReinforceInteractPanel);
                if (indexOfAboutReinforce == 0) // Decompostion
                {
                    Debug.Log("Deco");
                    TryInteract_DesompositionSlot();
                }
                else if (indexOfAboutReinforce == 1) // Fusion
                {
                    Debug.Log("Fusion");
                    TryInteract_FusionSlot();
                }
                else // Upgrade
                {
                    Debug.Log("Upgrade");
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
        if (BoostItemManager.Instance.Equiped_PSList.Count >= EquipedMEIS_List.Count)
        { return; }

        PassiveSkill ps = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentSelectedMEIS.ThisSlotItem);

        foreach (ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            if (ps.ThisExtraMEII.Contains(MEIS.ThisSlotItem))
            {
                Debug.Log("Exist Already!");
                return;
            }
        }

        Debug.Log("Equip!");

        // Module UI
        BoostItemManager.Instance.Equiped_PSList.Add(ps);
        ModifyEachInventorySlot meis = GetEquipedEmptySlot();
        ModifyEachInventoryItem meii 
            = MI_InEquipTab.SpawnMEII_Module(
                meis, 
                ps.ThisItemData.ItemIcon, 
                BoostItemManager.Instance.GetCorrectRankIcon(ps), 
                ps.ThisItemData.BoostLv);
        ps.ThisExtraMEII.Add(meii);
        meii.OwnerUIController = this;

        // Player HUD
        int slotIndex = EquipedMEIS_List.IndexOf(meis);
        Debug.Log(slotIndex);
        ModifyEachInventoryItem meii_PlayerHUD
            = MI_InEquipTab.SpawnMEII_Module(
                MainGameUIManager.Instance.PlayerHUD_UIController.MEISList[slotIndex],
                ps.ThisItemData.ItemIcon,
                BoostItemManager.Instance.GetCorrectRankIcon(ps),
                ps.ThisItemData.BoostLv);
        meii_PlayerHUD.IsCanSelect = false;
        ps.ThisExtraMEII.Add(meii_PlayerHUD);

        DotweenInEquip(1f);
        SetEquipDesc();
    }

    // 장착 해제 시도
    private void TryUnequip()
    {
        PassiveSkill ps = BoostItemManager.Instance.GetPassiveSkill_Equiped(CurrentSelectedMEIS.ThisSlotItem);

        BoostItemManager.Instance.Equiped_PSList.Remove(ps);
        ps.ThisExtraMEII.Remove(CurrentSelectedMEIS.ThisSlotItem);
        Destroy(CurrentSelectedMEIS.ThisSlotItem.gameObject);

        CurrentSelectedMEIS.ThisSlotItem = null;
        CurrentSelectedMEIS.OutIt_SelectedItem();

        DotweenInEquip(0f);
        SetEquipDesc();
    }

    private void DotweenInEquip(float _A)
    {

        if (DOTween.IsTweening("EquipInner"))
        { DOTween.Complete("EquipInner"); }

        Sequence seq1 = DOTween.Sequence();
        Sequence seq2 = DOTween.Sequence();
        for (int i = 0; i < EquipPanelInnerList.Count; i++)
        { seq1.Join(EquipPanelInnerList[i].DOFade(_A, 0.1f)); }

        for (int i = 0; i < EquipPanelInnerList.Count; i++)
        { seq2.Join(EquipPanelInnerList[i].DOFade(0.5f, 0.1f)); }

        Sequence seq = DOTween.Sequence();
        seq.Append(seq1);
        seq.Append(seq2);
        seq.SetId("EquipInner");
    }

    private void SetEquipDesc()
    {
        for (int i = 0; i < EquipedMEIS_List.Count; i++)
        {
            PassiveSkill ps = BoostItemManager.Instance.GetPassiveSkill_Equiped(EquipedMEIS_List[i].ThisSlotItem);
            if (ps != null)
            {
                EquipDescStateTxtList[i].text = ps.ThisItemData.EquipDescription;
            }
            else
            {
                EquipDescStateTxtList[i].text = "-";
            }
        }
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
        }
        else if (CurrentDecompositionItem == null) // 선택
        {
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(true);

            DecompositionSlot.ThisSlotItem.SetData(
                CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);
            
            CurrentDecompositionItem = CurrentSelectedMEIS.ThisSlotItem;

            return;
        }
    }

    // 인터렉트 -> 분해
    private void TryDesomposition()
    {
        if (CurrentDecompositionItem != null)
        {
            // Take Info
            PassiveSkill ps = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentDecompositionItem);
            int itemRank = ps.ThisItemData.Rank;
            int boostLv = ps.ThisItemData.BoostLv;

            // Be Empty
            RemoveDataInInventory(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentDecompositionItem));

            // Give
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
            DecompositionSlot.OutIt_SelectedItem();

            BoostItemManager.Instance.DeletePassiveSkill(CurrentDecompositionItem);


            // Take
            PlayerManager.Instance.PlayerController.CurrentMS.Value += itemRank * 2;
            PlayerManager.Instance.PlayerController.CurrentBC.Value += boostLv;

            ModuleUpgradeController.UsingShop.TakeDamage(false);
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
        }
        else
        {
            if (CurrentFusionItemList.Contains(CurrentSelectedMEIS.ThisSlotItem))
            {
                return;
            }

            for (int i = 0; i < FusionSlotList.Count; i++)
            {
                if (CurrentFusionItemList[i] == null)
                {
                    FusionSlotList[i].ThisSlotItem.gameObject.SetActive(true);

                    FusionSlotList[i].ThisSlotItem.SetData(
                        CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                        CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                        BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);

                    CurrentFusionItemList[i] = CurrentSelectedMEIS.ThisSlotItem;

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
            BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[0]).ThisItemData.Rank !=
            BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[1]).ThisItemData.Rank)
        { return; }

        int needMC = BoostItemManager.Instance.NeedMC_AbleFusion(CurrentFusionItemList[0]);
        if (needMC == 0 ||
            needMC > PlayerManager.Instance.PlayerController.CurrentMS.Value)
        { return; }

        // Take Info

        ItemData itemData;

        if (UnityEngine.Random.Range(0, 2) == 0)
        { itemData = new ItemData(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[0]).ThisItemData); }
        else
        { itemData = new ItemData(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[1]).ThisItemData); }

        itemData.Rank++;
        itemData.BoostLv = 1;

        for (int i = FusionSlotList.Count - 1; i >= 0; i--)
        {
            // Be Empty
            RemoveDataInInventory(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[i]));

            // Give

            FusionSlotList[i].ThisSlotItem.gameObject.SetActive(false);
            FusionSlotList[i].OutIt_SelectedItem();

            BoostItemManager.Instance.DeletePassiveSkill(CurrentFusionItemList[i]);
        }

        PlayerManager.Instance.PlayerController.CurrentMS.Value -= needMC;

        // Take
        BoostItemManager.Instance.GetItemSkill(itemData);

        ModuleUpgradeController.UsingShop.TakeDamage(false);
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
        }
        else if (CurrentUpgradeItem == null) // 선택
        {
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(true);

            UpgradeSlot.ThisSlotItem.SetData(
                CurrentSelectedMEIS.ThisSlotItem.ThisImg.sprite,
                CurrentSelectedMEIS.ThisSlotItem.RankImg.sprite,
                BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentSelectedMEIS.ThisSlotItem).ThisItemData.BoostLv);

            CurrentUpgradeItem = CurrentSelectedMEIS.ThisSlotItem;

            return;
        }
    }

    // 인터렉트 -> 강화
    private void TryUpgrade()
    {
        if (CurrentUpgradeItem != null)
        {
            // Cost
            int needEC = BoostItemManager.Instance.NeedEC_AbleUpgrade(CurrentUpgradeItem);
            Debug.Log(needEC);

            if (needEC == 0 ||
                needEC > PlayerManager.Instance.PlayerController.CurrentEC.Value)
            { return; }

            // Take Info
            ItemData itemData = new ItemData(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentUpgradeItem).ThisItemData);
            if (itemData.BoostLv >= PlayerManager.Instance.PlayerController.MaxBoostLv)
            { return; }

            itemData.BoostLv++;

            // Be Empty
            RemoveDataInInventory(BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentUpgradeItem));

            // Give
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);
            UpgradeSlot.OutIt_SelectedItem();

            BoostItemManager.Instance.DeletePassiveSkill(CurrentUpgradeItem);

            PlayerManager.Instance.PlayerController.CurrentEC.Value -= needEC;

            // Take
            BoostItemManager.Instance.GetItemSkill(itemData);

            ModuleUpgradeController.UsingShop.TakeDamage(false);
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
    }

    // 인벤토리에 슬롯에 연결된 파일 Null로 바꾸기 (Missing이면 파일에 자리를 차지하게 됨)
    private void RemoveDataInInventory(PassiveSkill _PS)
    {
        foreach (ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            foreach (ModifyEachInventoryItem MEII in _PS.ThisExtraMEII)
            {
                if (MEIS.ThisSlotItem == MEII)
                {
                    MEIS.ThisSlotItem = null;
                }
            }
        }
        foreach (ModifyEachInventoryItem MEII in _PS.ThisMEII)
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
        public string BtnString;

        public void Offset(ModuleUpgradeUIController _MUUC)
        {
            PanelBtn.Offset();
            if (PanelBtn.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out TMP_Text txt))
            { txt.text = BtnString; }
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
    }

    #endregion

    #region Desc

    public void SetDesc(ModifyEachInventoryItem _MEII)
    {
        PassiveSkill PS_InInventory = BoostItemManager.Instance.GetPassiveSkill_Inventory(_MEII);
        if (PS_InInventory != null)
        {
            Debug.Log("Inven");
            ThisDescPanel.SetDesc(PS_InInventory);
            return;
        }

        PassiveSkill PS_InEquiped = BoostItemManager.Instance.GetPassiveSkill_Equiped(_MEII);
        if (PS_InEquiped != null)
        {
            Debug.Log("Equiped");
            ThisDescPanel.SetDesc(PS_InEquiped);
            return;
        }

        if (_MEII == DecompositionSlot.ThisSlotItem &&
            CurrentDecompositionItem != null)
        {
            PassiveSkill PS = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentDecompositionItem);
            ThisDescPanel.SetDesc(PS);
        }
        else if (_MEII == FusionSlotList[0].ThisSlotItem &&
            CurrentFusionItemList[0] != null)
        {
            PassiveSkill PS = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[0]);
            ThisDescPanel.SetDesc(PS);
        }
        else if (_MEII == FusionSlotList[1].ThisSlotItem &&
            CurrentFusionItemList[1] != null)
        {
            PassiveSkill PS = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentFusionItemList[1]);
            ThisDescPanel.SetDesc(PS);
        }
        else if (_MEII == UpgradeSlot.ThisSlotItem &&
            CurrentUpgradeItem != null)
        {
            PassiveSkill PS = BoostItemManager.Instance.GetPassiveSkill_Inventory(CurrentUpgradeItem);
            ThisDescPanel.SetDesc(PS);
        }
    }

    public void SetOffDesc()
    {
        ThisDescPanel.SetDescOff();
    }

    #endregion
}
