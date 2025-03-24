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
            ((ModuleItemManager.RowAmount * 110) + 10), 
            ((ModuleItemManager.ColumnAmount * 110) + 10));
        
        Gen_AllSlotAndItem();
    }

    #endregion

    #region Gen

    private void Gen_AllSlotAndItem()
    {
        for (int i = 0; i < ModuleItemManager.ColumnAmount; i++) 
        {
            List<InventorySlotEUIController> colSlot = new List<InventorySlotEUIController>();
            List<InventoryItemEUIController> colItem = new List<InventoryItemEUIController>();

            for (int j = 0; j < ModuleItemManager.RowAmount; j++)
            {
                // Generate GO
                GameObject slotGO = Instantiate(ModuleItemManager.Instance.InventorySlotPrefab, this.transform);
                GameObject itemGO = Instantiate(ModuleItemManager.Instance.InventoryItemPrefab, slotGO.transform);

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

                    item.OwnerUIController = MainGameUIManager.Instance.ModuleUpgrade_UIController;
                }
            }

            AllSlot.Add(colSlot);
            AllItem.Add(colItem);
        }
    }

    #endregion

    #region Set

    public void Set_InventoryUI(List<List<ModuleState>> _AllModuleData)
    {
        for (int i = 0; i < _AllModuleData.Count; i++)
        {
            for (int j = 0; j < _AllModuleData[i].Count; j++)
            {
                ModuleState ms = _AllModuleData[i][j];

                if (ms != null)
                {
                    AllItem[i][j].gameObject.SetActive(true);

                    AllItem[i][j].Set_Data(new ItemData_UIVisual(
                        ms.ThisItemData.ItemIcon,
                        ms.ThisItemData.Rank,
                        ms.ThisItemData.BoostLv));
                }
                else
                {
                    AllItem[i][j].gameObject.SetActive(false);
                }
            }
        }
    }

    #region Equiped

    public void Set_InventoryEquipedUI(List<CoupleData<int>> _EquipedIndex)
    {
        SetOff_AllInventoryEquipedUI();

        for (int i = 0; i < _EquipedIndex.Count; i++)
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

    private void SetOff_AllInventoryEquipedUI()
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
