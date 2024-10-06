using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Button DecompositionBtn;

    [Space(5)]
    [Header("* Fusion")]
    [SerializeField] private List<ModifyEachInventorySlot> FusionSlotList;
    [SerializeField] private Button FusionBtn;

    [Space(5)]
    [Header("* Upgrade")]
    [SerializeField] private ModifyEachInventorySlot UpgradeSlot;
    [SerializeField] private Button UpgradeBtn;


    [Header("-------------------- Test")]
    [SerializeField] private ModifyEachInventoryItem CurrentDecompositionItem;
    [SerializeField] private List<ModifyEachInventoryItem> CurrentFusionItemList;
    [SerializeField] private ModifyEachInventoryItem CurrentUpgradeItem;


    [Header("=== Item")]
    [SerializeField] public ModifyEachInventorySlot CurrentSelectedMEIS;


    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Button CloseBtn;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        MI_InEquipTab.Offset();
        MI_InReinforceTab.Offset();

        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.Offset();
        }

        foreach(ModifyEachInventorySlot MEIS in EquipedMEIS_List)
        {
            MEIS.Offset();
        }

        foreach (SimplePanelAndBtn SPAB in ReinforceInteractPanels)
        {
            SPAB.Offset(this, ReinforceInteractPanels);
        }




        DecompositionSlot.Offset();
        DecompositionSlot.ThisSlotItem.Offset();

        foreach(ModifyEachInventorySlot meii in FusionSlotList)
        {
            meii.Offset();
            meii.ThisSlotItem.Offset();
            CurrentFusionItemList.Add(null);
        }

        UpgradeSlot.Offset();
        UpgradeSlot.ThisSlotItem.Offset();
    }

    protected override void Offset_UI()
    {
        MI_List = new List<ModifyInventory>
        {
            MI_InEquipTab,
            MI_InReinforceTab
        };

        // Close Btn
        CloseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                UIManager.Instance.ModuleUpgrade_UIController.CloseThisPanel(TabDurTime);
            });

        // Tab Btn List
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            int index = i;
            ThisPanelTabList[index].ThisTabBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    ChangeThisPanel(TabDurTime, index);
                });
        }

        // BG Offset
        if (TryGetComponent(out Image img))
        {
            Color BGColor = img.color;
            BGColor.a = 0f;
            img.color = BGColor;
        }

        // interact btn

        DecompositionBtn.OnClickAsObservable()
            .Subscribe(btn => 
            {
                TryDesomposition();
            });
        FusionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                TryFusion();
            });
        UpgradeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                TryUpgrade();
            });
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.OnReset();
        }

        OnReset_SPAB();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            TryInteractItem();
        }
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

        if (TryGetComponent(out CanvasGroup CG))
        {
            CG.DOFade(0f, _DurTime);
        }
    }

    #endregion

    #region Item

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
            mi_List.Add(MI.SpawnMEII_ThisInventory(_ItemSprite, _RankImg, _BoostLv));
        }
        return mi_List;
    }

    // 아이템 상호작용
    private void TryInteractItem()
    {
        if(CurrentSelectedMEIS == null)
        { return; }

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


    #region About Equip

    // 장착 시도
    private void TryEquip()
    {
        if(BoostItemManager.Instance.Equiped_PSList.Count >= EquipedMEIS_List.Count)
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
        ModifyEachInventoryItem meii = MI_InEquipTab.SpawnMEII_Module(GetEquipedEmptySlot(), ps.ThisIcon, BoostItemManager.Instance.GetRankIcon(ps.ThisRank), ps.ThisBoostLv);
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

    #region Reinforce

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
                CurrentSelectedMEIS.ThisSlotItem.ThisRankLv);

            CurrentDecompositionItem = CurrentSelectedMEIS.ThisSlotItem;

            return;
        }
    }

    // 인터렉트 -> 분해
    private void TryDesomposition()
    {
        if (CurrentDecompositionItem != null)
        {
            DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
            DecompositionSlot.OutIt_SelectedItem();

            BoostItemManager.Instance.DeletePassiveSkill(CurrentDecompositionItem);
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
                        CurrentSelectedMEIS.ThisSlotItem.ThisRankLv);

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
            CurrentFusionItemList[0].ThisRankLv != CurrentFusionItemList[1].ThisRankLv)
        { return; }

        for (int i = FusionSlotList.Count - 1; i >= 0; i--)
        {
            if (CurrentFusionItemList[i] != null)
            {
                FusionSlotList[i].ThisSlotItem.gameObject.SetActive(false);
                FusionSlotList[i].OutIt_SelectedItem();

                BoostItemManager.Instance.DeletePassiveSkill(CurrentFusionItemList[i]);
            }
        }
    }

    #endregion

    #region Descomposition

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
                CurrentSelectedMEIS.ThisSlotItem.ThisRankLv);

            CurrentUpgradeItem = CurrentSelectedMEIS.ThisSlotItem;

            return;
        }
    }

    // 인터렉트 -> 강화
    private void TryUpgrade()
    {
        if (CurrentUpgradeItem != null)
        {
            UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);
            UpgradeSlot.OutIt_SelectedItem();

            BoostItemManager.Instance.DeletePassiveSkill(CurrentUpgradeItem);
        }
    }

    #endregion


    // 강화 패널 리셋
    public void ResetReinforcePanel()
    {
        DecompositionSlot.ThisSlotItem.gameObject.SetActive(false);
        foreach(ModifyEachInventorySlot meis in FusionSlotList)
        { meis.ThisSlotItem.gameObject.SetActive(false); }
        UpgradeSlot.ThisSlotItem.gameObject.SetActive(false);

        CurrentDecompositionItem = null;
        for (int i = 0; i < CurrentFusionItemList.Count; i++)
        { CurrentFusionItemList[i] = null; }
        CurrentUpgradeItem = null;
    }

    #endregion

    #endregion

    #region Kind of Reinforce Panel & Btn

    [System.Serializable]
    class SimplePanelAndBtn
    {
        public RectTransform PanelRT;
        public Button PanelBtn;

        public void Offset(ModuleUpgradeUIController _OwnerController, List<SimplePanelAndBtn> _ContainList)
        {
            PanelBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    foreach(SimplePanelAndBtn SPAB in _ContainList)
                    {
                        if(SPAB.PanelBtn == PanelBtn)
                        {
                            if(!(SPAB == _OwnerController.CurrentReinforceInteractPanel))
                            {
                                _OwnerController.ResetReinforcePanel();
                            }
                            SPAB.PanelRT.gameObject.SetActive(true);
                            _OwnerController.CurrentReinforceInteractPanel = this;
                        }
                        else
                        {
                            SPAB.PanelRT.gameObject.SetActive(false);
                        }
                    }
                });
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
}
