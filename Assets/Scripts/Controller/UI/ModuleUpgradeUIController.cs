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

    [Header("-- In Equip")]
    [SerializeField] private ModifyInventory MI_InEquipTab;
    [SerializeField] private List<ModifyEachInventorySlot> EquipedMEIS_List;

    [Header("-- In Reinforce")]
    [SerializeField] private ModifyInventory MI_InReinforceTab;

    [HideInInspector] private List<ModifyInventory> MI_List;


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
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.OnReset();
        }
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

    public List<ModifyEachInventoryItem> SpawnMEIIList(Sprite _ItemSprite, Sprite _RankImg, int _BoostLv)
    {
        List<ModifyEachInventoryItem> mi_List = new List<ModifyEachInventoryItem>();
        foreach (ModifyInventory MI in MI_List)
        {
            mi_List.Add(MI.SpawnMEII_ThisInventory(_ItemSprite, _RankImg, _BoostLv));
        }
        return mi_List;
    }


    private void TryInteractItem()
    {
        if(CurrentSelectedMEIS == null)
        { return; }

        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            if (i == 0) // Equiped Window
            {
                if(CurrentSelectedMEIS.IsInventory)// Equip
                {
                    TryEquip();
                }
                else // Unequip
                {
                    TryUnequip();
                }

                BoostItemManager.Instance.ResetInterface();
            }
            else if (i == 1) // Reinforce Window
            {

            }
        }
    }

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
}
