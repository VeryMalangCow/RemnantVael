using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject InventorySlotPrefab;
    [SerializeField] private Sprite SlotSprite;

    [Header("-- Item")]
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
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log("Row Or Column Is Zero!");
        }
#endif

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
                GameObject slot = Instantiate(InventorySlotPrefab, this.transform);
                slot.name = "InventorySlot_" + column + "_" + row;

                // RT
                if (slot.TryGetComponent(out RectTransform rt))
                {
                    rt.anchoredPosition = new Vector2((row * 110 + 10), -(column * 110 + 10));
                }

                // Class
                if (slot.TryGetComponent(out ModifyEachInventorySlot MEIS))
                {
                    MEIS.Offset();
                    MEIS.SetData(SlotSprite);

                    rowMEISList.Add(MEIS);
                }

            }

            MEISList.Add(rowMEISList);
        }
    }

    #endregion

    #region Item

    public ModifyEachInventoryItem SpawnMEII_Module(ModifyEachInventorySlot _ParentSlot, Sprite _ItemSprite, Sprite _RankImg, int _BoostLv)
    {
        // Generate GO
        GameObject item = Instantiate(InventoryItemPrefab, _ParentSlot.transform);

        // RT
        if (item.TryGetComponent(out RectTransform rt))
        {
            rt.anchoredPosition = Vector2.zero;
        }

        // Class
        if (item.TryGetComponent(out ModifyEachInventoryItem MEII))
        {
            MEII.Offset();
            MEII.SetData(_ItemSprite, _RankImg, _BoostLv);

            _ParentSlot.ThisSlotItem = MEII;
            return MEII;
        }

        return null;
    }

    public ModifyEachInventoryItem SpawnMEII_ThisInventory(Sprite _ItemSprite, Sprite _RankImg, int _BoostLv)
    {
        // Generate GO
        ModifyEachInventorySlot emptySlot = GetEmptyMEIS();
        GameObject item = Instantiate(InventoryItemPrefab, emptySlot.transform);
        
        // RT
        if(item.TryGetComponent(out RectTransform rt))
        {
            rt.anchoredPosition = Vector2.zero;
        }

        // Class
        if (item.TryGetComponent(out ModifyEachInventoryItem MEII))
        {
            MEII.Offset();
            MEII.SetData(_ItemSprite, _RankImg, _BoostLv);

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

    public ModifyEachInventorySlot GetTargetSlot(ModifyEachInventoryItem _MEII)
    {
        foreach (List<ModifyEachInventorySlot> MEIS_List in MEISList)
        {
            foreach (ModifyEachInventorySlot MEIS in MEIS_List)
            {
                if (MEIS.ThisSlotItem == _MEII)
                {
                    return MEIS;
                }
            }
        }
        return null;
    }

    public void RemoveItemInSlotData(ModifyEachInventoryItem _MEII)
    {
        ModifyEachInventorySlot MEIS = GetTargetSlot(_MEII);
        if (MEIS != null)
        {
            MEIS.ThisSlotItem = null;
        }
    }

    #endregion

}
