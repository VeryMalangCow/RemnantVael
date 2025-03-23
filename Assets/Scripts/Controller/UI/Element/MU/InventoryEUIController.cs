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
        for (int column = 0; column < ModuleItemManager.ColumnAmount; column++) 
        {
            List<InventorySlotEUIController> colSlot = new List<InventorySlotEUIController>();
            List<InventoryItemEUIController> colItem = new List<InventoryItemEUIController>();

            for (int row = 0; row < ModuleItemManager.RowAmount; row++)
            {
                // Generate GO
                GameObject slotGO = Instantiate(ModuleItemManager.Instance.InventorySlotPrefab, this.transform);
                GameObject itemGO = Instantiate(ModuleItemManager.Instance.InventoryItemPrefab, slotGO.transform);

                slotGO.name = $"Slot_Col:{column}_Row:{row}";
                itemGO.name = $"Item_Col:{column}_Row:{row}";

                // Offset
                if (DevTool.Get_ComponentTType(slotGO.gameObject, out RectTransform slotRt) &&
                    DevTool.Get_ComponentTType(slotGO.gameObject, out InventorySlotEUIController slot) &&
                    DevTool.Get_ComponentTType(itemGO.gameObject, out InventoryItemEUIController item))
                {
                    slotRt.anchoredPosition = new Vector2((row * 110 + 10), -(column * 110 + 10));

                    slot.Offset();
                    slot.ThisImg.sprite = SlotSprite;

                    item.Offset();
                    itemGO.gameObject.SetActive(false);

                    colSlot.Add(slot);
                    colItem.Add(item);

                    slot.ThisItem = item;
                    item.ThisSlot = slot;
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

    #endregion
}
