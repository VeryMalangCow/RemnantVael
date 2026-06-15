using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InventoryEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Item Inventory")]
    [Header("-- RT")]
    [SerializeField] private RectTransform rt;

    [Header("-- Slot")]
    [SerializeField] private Sprite slotSprite;

    // Class
    [HideInInspector] public List<List<InventorySlotEUIController>> allSlot = new List<List<InventorySlotEUIController>>();
    [HideInInspector] public List<List<InventoryItemEUIController>> allItem = new List<List<InventoryItemEUIController>>();

    #endregion

    #region Offset

    public override void Offset()
    {
        //Chech This RT
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;

        this.rt.sizeDelta = new Vector2(
            ((ModuleItemManager.rowAmount * 110) + 10), 
            ((ModuleItemManager.columnAmount * 110) + 10));
    }

    #endregion

    #region Gen

    public IEnumerator Gen_AllSlotAndItemAsync(
    SinglePanelUIController ownerUI,
    float maxMsPerFrame = 1f)
    {
#if UNITY_EDITOR
        List<int> createAmount = new List<int>();
        int allFrame = 0;
        int createdThisFrame = 0;
#endif

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < ModuleItemManager.columnAmount; i++)
        {
            List<InventorySlotEUIController> colSlot = new List<InventorySlotEUIController>();
            List<InventoryItemEUIController> colItem = new List<InventoryItemEUIController>();

            for (int j = 0; j < ModuleItemManager.rowAmount; j++)
            {
                InventorySlotEUIController slot =
                    Instantiate(MainGameUIManager.instance.GetModuleSlotEUIPrefab(), this.transform);

                InventoryItemEUIController item =
                    Instantiate(MainGameUIManager.instance.GetModuleItemEUIPrefab(), slot.transform);

                slot.name = $"Slot_Col:{i}_Row:{j}";
                item.name = $"Item_Col:{i}_Row:{j}";

                if (DevTool.Get_ComponentTType(slot.gameObject, out RectTransform slotRt))
                {
                    slotRt.anchoredPosition = new Vector2(
                        j * 110 + 10,
                        -(i * 110 + 10)
                    );

                    slot.Offset();
                    slot.thisImg.sprite = slotSprite;
                    slot.col = i;
                    slot.row = j;

                    item.Offset();
                    item.gameObject.SetActive(false);

                    colSlot.Add(slot);
                    colItem.Add(item);

                    slot.item = item;
                    item.slot = slot;

                    slot.ownerUIController = ownerUI;
                    item.ownerUIController = ownerUI;
                }

#if UNITY_EDITOR
                createdThisFrame++;
#endif

                if (sw.Elapsed.TotalMilliseconds >= maxMsPerFrame)
                {
#if UNITY_EDITOR
                    createAmount.Add(createdThisFrame);
                    createdThisFrame = 0;
                    allFrame++;
#endif

                    yield return null;
                    sw.Restart();
                }
            }

            allSlot.Add(colSlot);
            allItem.Add(colItem);
        }

        sw.Stop();

#if UNITY_EDITOR
        if (createdThisFrame > 0)
        {
            createAmount.Add(createdThisFrame);
            allFrame++;
        }

        string s =
            $"<color=yellow>Inventory UI</color> : Create Slot/Item" +
            $"\nColumn -> <color=orange>{ModuleItemManager.columnAmount}</color>" +
            $"\nRow -> <color=orange>{ModuleItemManager.rowAmount}</color>" +
            $"\nTotal -> <color=orange>{ModuleItemManager.columnAmount * ModuleItemManager.rowAmount}</color>" +
            $"\nFrame -> <color=red>{allFrame}</color>" +
            $"\nLimitMs -> <color=red>{maxMsPerFrame}</color>\n";

        for (int i = 0; i < createAmount.Count; i++)
        {
            s += $"<color=yellow>{createAmount[i]}</color> / ";
        }

        UnityEngine.Debug.Log(s);
#endif

        yield return null;
    }

    public void Gen_AllSlotAndItem(SinglePanelUIController ownerUI)
    {
        for (int i = 0; i < ModuleItemManager.columnAmount; i++) 
        {
            List<InventorySlotEUIController> colSlot = new List<InventorySlotEUIController>();
            List<InventoryItemEUIController> colItem = new List<InventoryItemEUIController>();

            for (int j = 0; j < ModuleItemManager.rowAmount; j++)
            {
                // Generate GO
                InventorySlotEUIController slot = Instantiate(MainGameUIManager.instance.GetModuleSlotEUIPrefab(), this.transform);
                InventoryItemEUIController item = Instantiate(MainGameUIManager.instance.GetModuleItemEUIPrefab(), slot.transform);

                slot.name = $"Slot_Col:{i}_Row:{j}";
                item.name = $"Item_Col:{i}_Row:{j}";

                // Offset
                if (DevTool.Get_ComponentTType(slot.gameObject, out RectTransform slotRt))
                {
                    slotRt.anchoredPosition = new Vector2((j * 110 + 10), -(i * 110 + 10));

                    slot.Offset();
                    slot.thisImg.sprite = slotSprite;
                    slot.col = i;
                    slot.row = j;

                    item.Offset();
                    item.gameObject.SetActive(false);

                    colSlot.Add(slot);
                    colItem.Add(item);

                    slot.item = item;
                    item.slot = slot;

                    slot.ownerUIController = ownerUI;
                    item.ownerUIController = ownerUI;
                }
            }

            allSlot.Add(colSlot);
            allItem.Add(colItem);
        }
    }

    #endregion

    #region Set

    // Player Module Shop
    public void Set_InventoryUI(ModuleState[][] allModuleData)
    {
        SetOff_AllInventoryUI();

        for (int i = 0; i < allModuleData.Length; i++)
        {
            for (int j = 0; j < allModuleData[i].Length; j++)
            {
                ModuleState ms = allModuleData[i][j];

                Set_InventoryItem(ms, allItem[i][j]);
            }
        }
    }

    // Ally Module Shop
    public void Set_InventoryUI(List<List<CopyModuleState>> allModuleData)
    {
        SetOff_AllInventoryUI();

        for (int i = 0; i < allModuleData.Count; i++)
        {
            for (int j = 0; j < allModuleData[i].Count; j++)
            {
                ModuleState ms = allModuleData[i][j].state;

                Set_InventoryItem(ms, allItem[i][j]);

                if (allModuleData[i][j].isEquipped)
                {
                    allItem[i][j].slot.Set_EquipedTxt(true, "#"); 
                    allItem[i][j].Set_EquipedImg(true);
                }
            }
        }
    }
    public void SetOff_AllInventoryUI()
    {
        for (int i = 0; i < allItem.Count; i++)
        {
            for (int j = 0; j < allItem[i].Count; j++)
            {
                allSlot[i][j].Set_SelectedOff();
                allItem[i][j].gameObject.SetActive(false);
            }
        }
    }

    // Each
    private void Set_InventoryItem(ModuleState moduleState, InventoryItemEUIController itemEui)
    {
        if (moduleState != null)
        {
            itemEui.gameObject.SetActive(true);

            itemEui.Set_Data(new ItemData_UIVisual(
                moduleState.thisItemData.itemIcon,
                moduleState.thisItemData.rank));
        }
        else
        {
            itemEui.gameObject.SetActive(false);
        }
    }


    #region Equiped

    public void Set_InventoryEquipedUI(CoupleData<int>[] equipedIndex)
    {
        SetOff_AllInventoryEquipedUI();

        for (int i = 0; i < equipedIndex.Length; i++)
        {
            int targetCol = equipedIndex[i].typeBase;
            int targetRow = equipedIndex[i].typeSpecial;

            if (targetCol != -1 && targetRow != -1)
            {
                allItem[targetCol][targetRow].slot.Set_EquipedTxt(true, i);
                allItem[targetCol][targetRow].Set_EquipedImg(true);
            }

        }
    }

    public void SetOff_AllInventoryEquipedUI()
    {
        for (int i = 0; i < allSlot.Count; i++)
        {
            for (int j = 0; j < allSlot[i].Count; j++)
            {
                allItem[i][j].slot.Set_EquipedTxt(false);
                allItem[i][j].Set_EquipedImg(false);
            }
        }
    }

    #endregion

    #region Forge

    public void Set_InventoryForgeSelectedUI(CoupleData<int> selectedIndex, bool isOn, int index = -1)
    {
        allItem[selectedIndex.typeBase][selectedIndex.typeSpecial]
            .slot.Set_ForgeSelectedTxt(isOn, index);
    }

    public void SetOff_AllInventoryForgeSelectedUI()
    {
        for (int i = 0; i < allSlot.Count; i++)
        {
            for (int j = 0; j < allSlot[i].Count; j++)
            {
                allItem[i][j].slot.Set_ForgeSelectedTxt(false);
            }
        }
    }

    #endregion

    #endregion
}
