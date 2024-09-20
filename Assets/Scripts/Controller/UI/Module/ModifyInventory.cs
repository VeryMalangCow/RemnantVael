using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class ModifyInventory : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Item Inventory")]
    [Header("-- RT")]
    [SerializeField] private RectTransform ThisRT;
    [SerializeField] private int RowAmount;
    [SerializeField] private int ColumnAmount;

    [Header("-- Slot")]
    [SerializeField] private Sprite InventorySlot;

    [Header("-- Prefab")]
    [SerializeField] private GameObject InventoryItemPrefab;

    // Class
    [HideInInspector] public List<List<ModifyEachInventorySlot>> MEISList;

    #endregion

    #region Offset

    public override void Offset()
    {
        //Chech This RT
        if (ThisRT == null && TryGetComponent(out RectTransform thisRT))
        {
            ThisRT = thisRT;
        }
        if(RowAmount != 0 && ColumnAmount != 0)
        {
            float CaculateWidth = ((RowAmount * 110) + 10);
            float CaculateHeight = ((ColumnAmount * 110) + 10);
            if (CaculateWidth != ThisRT.rect.width || CaculateHeight != ThisRT.rect.height)
            {
                ThisRT.sizeDelta = new Vector2(CaculateWidth, CaculateHeight);
                Debug.LogWarning("Incorrect Size Inventory: " + this.gameObject.name + 
                    " -> Correct Value: " + ThisRT.sizeDelta);
            }
        }
        else
        {
            Debug.Log("Row Or Column Is Zero!");
        }

        GenSlotList();
    }

    private void GenSlotList()
    {
        MEISList = new List<List<ModifyEachInventorySlot>>();

        for (int column = 0; column < ColumnAmount; column++) 
        {
            List<ModifyEachInventorySlot> rowMEISList = new List<ModifyEachInventorySlot>();

            for (int row = 0; row < RowAmount; row++)
            {
                // Generate GO
                GameObject slot = new GameObject();
                slot.name = "InventorySlot_" + column + "_" + row;
                slot.transform.SetParent(this.gameObject.transform);

                // RT
                RectTransform rt = slot.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(100, 100);
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = Vector2.up;
                rt.anchoredPosition = new Vector2((row * 110 + 10), -(column * 110 + 10));

                // Img
                Image img = slot.AddComponent<Image>();

                // Class
                ModifyEachInventorySlot MEIS = slot.AddComponent<ModifyEachInventorySlot>();
                rowMEISList.Add(MEIS);
            }

            MEISList.Add(rowMEISList);
        }
    }

    #endregion

    #region Item

    public ModifyEachInventoryItem SpawnMEII()
    {
        ModifyEachInventorySlot emptySlot = GetEmptyMEIS();
        GameObject item = Instantiate(InventoryItemPrefab, emptySlot.transform);

        if(item.TryGetComponent(out RectTransform rt))
        {
            rt.anchoredPosition = Vector2.zero;
        }

        if (item.TryGetComponent(out ModifyEachInventoryItem MEII))
        {
            emptySlot.ThisSlotItem = MEII;
            return MEII;
        }

        return null;
    }

    private ModifyEachInventorySlot GetEmptyMEIS()
    {
        for (int i = 0; i < MEISList.Count; i++)
        {
            for (int ii = 0; ii < MEISList[i].Count; ii++)
            {
                if (MEISList[i][ii].ThisSlotItem == null)
                {
                    return MEISList[i][ii];
                }
            }
        }
        return null;
    }

    #endregion

}
