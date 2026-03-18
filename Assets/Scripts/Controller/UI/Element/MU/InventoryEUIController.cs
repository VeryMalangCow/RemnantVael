using System.Collections.Generic;
using UnityEngine;

public class InventoryEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Item Inventory")]
    [Header("-- RT")]
    [SerializeField] private RectTransform ThisRT;

    [Header("-- Slot")]
    [SerializeField] private Sprite SlotSprite;

    // Class
    [HideInInspector] public List<List<InventorySlotEUIController>> AllSlot = new List<List<InventorySlotEUIController>>();
    [HideInInspector] public List<List<InventoryItemEUIController>> AllItem = new List<List<InventoryItemEUIController>>();

    #endregion

    #region Offset

    public override void Offset()
    {
        //Chech This RT
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;

        ThisRT.sizeDelta = new Vector2(
            ((ModuleItemManager.rowAmount * 110) + 10), 
            ((ModuleItemManager.columnAmount * 110) + 10));
    }

    #endregion

    #region Gen

    public void Gen_AllSlotAndItem(SinglePanelUIController _OwnerUI)
    {
        for (int i = 0; i < ModuleItemManager.columnAmount; i++) 
        {
            List<InventorySlotEUIController> colSlot = new List<InventorySlotEUIController>();
            List<InventoryItemEUIController> colItem = new List<InventoryItemEUIController>();

            for (int j = 0; j < ModuleItemManager.rowAmount; j++)
            {
                // Generate GO
                GameObject slotGO = Instantiate(ResourceManager.instance.Get_ModuleSlotUI_Prefab(), this.transform);
                GameObject itemGO = Instantiate(ResourceManager.instance.Get_ModuleItemUI_Prefab(), slotGO.transform);

                slotGO.name = $"Slot_Col:{i}_Row:{j}";
                itemGO.name = $"Item_Col:{i}_Row:{j}";

                // Offset
                if (DevTool.Get_ComponentTType(slotGO.gameObject, out RectTransform slotRt) &&
                    DevTool.Get_ComponentTType(slotGO.gameObject, out InventorySlotEUIController slot) &&
                    DevTool.Get_ComponentTType(itemGO.gameObject, out InventoryItemEUIController item))
                {
                    slotRt.anchoredPosition = new Vector2((j * 110 + 10), -(i * 110 + 10));

                    slot.Offset();
                    slot.ThisImg.sprite = SlotSprite;
                    slot.Col = i;
                    slot.Row = j;

                    item.Offset();
                    itemGO.gameObject.SetActive(false);

                    colSlot.Add(slot);
                    colItem.Add(item);

                    slot.ThisItem = item;
                    item.ThisSlot = slot;

                    slot.OwnerUIController = _OwnerUI;
                    item.OwnerUIController = _OwnerUI;
                }
            }

            AllSlot.Add(colSlot);
            AllItem.Add(colItem);
        }
    }

    #endregion

    #region Set

    // Player Module Shop
    public void Set_InventoryUI(ModuleState[][] _AllModuleData)
    {
        SetOff_AllInventoryUI();

        for (int i = 0; i < _AllModuleData.Length; i++)
        {
            for (int j = 0; j < _AllModuleData[i].Length; j++)
            {
                ModuleState ms = _AllModuleData[i][j];

                Set_InventoryItem(ms, AllItem[i][j]);
            }
        }
    }

    // Ally Module Shop
    public void Set_InventoryUI(List<List<CopyModuleState>> _AllModuleData)
    {
        SetOff_AllInventoryUI();

        for (int i = 0; i < _AllModuleData.Count; i++)
        {
            for (int j = 0; j < _AllModuleData[i].Count; j++)
            {
                ModuleState ms = _AllModuleData[i][j].MS;

                Set_InventoryItem(ms, AllItem[i][j]);

                if (_AllModuleData[i][j].IsEquipped)
                {
                    AllItem[i][j].ThisSlot.Set_EquipedTxt(true, "#"); 
                    AllItem[i][j].Set_EquipedImg(true);
                }
            }
        }
    }
    public void SetOff_AllInventoryUI()
    {
        for (int i = 0; i < AllItem.Count; i++)
        {
            for (int j = 0; j < AllItem[i].Count; j++)
            {
                AllSlot[i][j].Set_SelectedOff();
                AllItem[i][j].gameObject.SetActive(false);
            }
        }
    }

    // Each
    private void Set_InventoryItem(ModuleState _MS, InventoryItemEUIController _ItemEUI)
    {
        if (_MS != null)
        {
            _ItemEUI.gameObject.SetActive(true);

            _ItemEUI.Set_Data(new ItemData_UIVisual(
                _MS.ThisItemData.ItemIcon,
                _MS.ThisItemData.Rank));
        }
        else
        {
            _ItemEUI.gameObject.SetActive(false);
        }
    }


    #region Equiped

    public void Set_InventoryEquipedUI(CoupleData<int>[] _EquipedIndex)
    {
        SetOff_AllInventoryEquipedUI();

        for (int i = 0; i < _EquipedIndex.Length; i++)
        {
            int targetCol = _EquipedIndex[i].TypeBase;
            int targetRow = _EquipedIndex[i].TypeSpecial;

            if (targetCol != -1 && targetRow != -1)
            {
                AllItem[targetCol][targetRow].ThisSlot.Set_EquipedTxt(true, i);
                AllItem[targetCol][targetRow].Set_EquipedImg(true);
            }

        }
    }

    public void SetOff_AllInventoryEquipedUI()
    {
        for (int i = 0; i < AllSlot.Count; i++)
        {
            for (int j = 0; j < AllSlot[i].Count; j++)
            {
                AllItem[i][j].ThisSlot.Set_EquipedTxt(false);
                AllItem[i][j].Set_EquipedImg(false);
            }
        }
    }

    #endregion

    #region Forge

    public void Set_InventoryForgeSelectedUI(CoupleData<int> _SelectedIndex, bool _IsOn, int _Index = -1)
    {
        AllItem[_SelectedIndex.TypeBase][_SelectedIndex.TypeSpecial]
            .ThisSlot.Set_ForgeSelectedTxt(_IsOn, _Index);
    }

    public void SetOff_AllInventoryForgeSelectedUI()
    {
        for (int i = 0; i < AllSlot.Count; i++)
        {
            for (int j = 0; j < AllSlot[i].Count; j++)
            {
                AllItem[i][j].ThisSlot.Set_ForgeSelectedTxt(false);
            }
        }
    }

    #endregion

    #endregion
}
