using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Module")]

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] private ModifyInventory MI_InEquipTab;
    [SerializeField] private List<ModifyEachInventorySlot> EquipedMEIS_List;

    [Space(10)]
    [Header("-- In Reinforce")]
    [SerializeField] private ModifyInventory MI_InReinforceTab;
    [SerializeField] private List<SimplePanelAndBtn> ReinforceInteractPanels;
    [HideInInspector] private SimplePanelAndBtn CurrentReinforceInteractPanel;

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

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        MI_InEquipTab.Offset();
        MI_InReinforceTab.Offset();

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

        foreach (ModifyEachInventorySlot meii in FusionSlotList)
        {
            meii.Offset();
            meii.ThisSlotItem.Offset();
            CurrentFusionItemList.Add(null);
        }

        UpgradeSlot.Offset();
        UpgradeSlot.ThisSlotItem.Offset();


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

        if (TryGetComponent(out CanvasGroup CG))
        {
            CG.alpha = 0.0f;
        }
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
    }

    #endregion

    #region Set Panel

    public override void OpenThisPanel(float _DurTime)
    {
        if (IsTweening)
        { return; }

        base.OpenThisPanel(_DurTime);

        if (TryGetComponent(out CanvasGroup CG))
        {
            CG.DOFade(1f, _DurTime);
        }
    }

    public override void CloseThisPanel(float _DurTime)
    {
        if (IsTweening)
        { return; }

        base.CloseThisPanel(_DurTime);
        ModuleUpgradeController.UsingShop = null;

        if (TryGetComponent(out CanvasGroup CG))
        {
            CG.DOFade(0f, _DurTime);
        }
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
                MainGameUIManager.Instance.ModuleUpgrade_UIController.CloseThisPanel(TabDurTime);
                return;
            }

            // 탭
            for (int i = 0; i < ThisPanelTabList.Count; i++)
            {
                if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
                {
                    ChangeThisPanel(TabDurTime, i);
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
                    CurrentReinforceInteractPanel = ReinforceInteractPanels[i];
                }
                else
                {
                    ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(false);
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
        BoostItemManager.Instance.Equiped_PSList.Add(ps);
        ModifyEachInventoryItem meii = MI_InEquipTab.SpawnMEII_Module(GetEquipedEmptySlot(), ps.ThisItemData.ItemIcon, BoostItemManager.Instance.GetRankIcon(ps.ThisItemData.Rank), ps.ThisItemData.BoostLv);
        ps.ThisExtraMEII.Add(meii);

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

        public void Offset(ModuleUpgradeUIController _MUUC)
        {
            PanelBtn.Offset();
            PanelBtn.OwnerUIController = _MUUC;
        }
    }

    private void OnReset_SPAB()
    {
        for (int i = 0; i < ReinforceInteractPanels.Count; i++)
        {
            if (0 == i)
            {
                ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(true);
                CurrentReinforceInteractPanel = ReinforceInteractPanels[i];
            }
            else
            {
                ReinforceInteractPanels[i].PanelRT.gameObject.SetActive(false);
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
        ThisDescPanel.SetOffDesc();
    }

    #endregion
}
