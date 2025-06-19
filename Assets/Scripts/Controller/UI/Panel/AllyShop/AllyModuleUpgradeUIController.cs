using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllyModuleUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Inventory")]
    [SerializeField] private ScrollPanelEUIController InventoryScrollPanelEUI;
    [SerializeField] private InventoryEUIController InventoryEUI;

    #endregion

    #region - Hide

    [HideInInspector] private Dictionary<CoupleData<int>, CopyModuleState> CurrentData;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_EUI();

        Set_LanguageTxt();
    }

    private void Offset_EUI()
    {
        InventoryScrollPanelEUI.Offset();
        InventoryEUI.Offset();
        InventoryEUI.Gen_AllSlotAndItem(this);
    }


    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;
        if (Is_Interact_ModuleInInventory()) return true;

        return false;
    }

    #endregion

    #region Interact (Module)

    private bool Is_Interact_ModuleInInventory()
    {
        if (CurrentBtn == null || CurrentBtn is not InventoryItemEUIController eui) return false;


        return true;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(95) + " " + ResourceManager.Instance.Get_StaticWord(27) + " " + ResourceManager.Instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        base.Set_LanguageTxt();
    }

    #endregion

    #region Set (Inventory)

    private void Set_Inventory()
    {
        // 존재하는 모듈 중, 장착 중인 모듈
        List<CopyModuleState> equippedMsList = 
            ModuleItemManager.Instance.Get_EquippedModuleState()
            .OrderByDescending(obj => obj.MS.ThisItemData.Rank).ToList();

        // 존재하는 모듈 중, 장착 중이지 않은 모듈
        List<CopyModuleState> unEquippedMsList = 
            ModuleItemManager.Instance.Get_ExistModuleState(equippedMsList)
            .OrderByDescending(obj => obj.MS.ThisItemData.Rank).ToList();

        Set_CopyAllyShopMSInventory(unEquippedMsList, equippedMsList);
    }

    private void Set_CopyAllyShopMSInventory(List<CopyModuleState> _UnEq, List<CopyModuleState> _Eq)
    {
        CurrentData = new Dictionary<CoupleData<int>, CopyModuleState>();

        // 순서대로, Row Col 로 이중 리스트로 사용
        List<List<CopyModuleState>> combineData = DevTool.Get_RowColumeList(DevTool.Get_CombineList(_UnEq, _Eq), ModuleItemManager.RowAmount);
        
        for (int i = 0; i < combineData.Count; i++)
        {
            for (int j = 0; j < combineData[i].Count; j++)
            {
                CurrentData.Add(new CoupleData<int>(i, j), combineData[i][j]);
            }
        }

        // UI Set
        InventoryEUI.SetOff_AllInventoryEquipedUI();

        InventoryEUI.Set_InventoryUI(combineData);

        return;
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        ThisDurEUI.Set_Dur(AllyModuleUpgradeController.UsingShop.CurrentDur);

        // Inven
        Set_Inventory();
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        AllyModuleUpgradeController.UsingShop = null;
    }


    #endregion
}

