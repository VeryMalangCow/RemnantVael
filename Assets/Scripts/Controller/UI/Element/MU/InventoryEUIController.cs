using System.Collections.Generic;
using UnityEngine;

public class InventoryEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Item Inventory")]
    [Header("-- RT")]
    [SerializeField] private RectTransform ThisRT;
    [SerializeField] private int RowAmount;
    [SerializeField] private int ColumnAmount;

    [Header("-- Slot")]
    [SerializeField] private Sprite SlotSprite;

    // Class
    [HideInInspector] public List<List<InventorySlotEUIController>> MEISList;

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

        Gen_SlotList();
    }

    private void Gen_SlotList()
    {
        MEISList = new List<List<InventorySlotEUIController>>();

        for (int column = 0; column < ColumnAmount; column++) 
        {
            List<InventorySlotEUIController> rowMEISList = new List<InventorySlotEUIController>();

            for (int row = 0; row < RowAmount; row++)
            {
                // Generate GO
                GameObject slot = Instantiate(ModuleItemManager.Instance.InventorySlotPrefab, this.transform);
                slot.name = "InventorySlot_" + column + "_" + row;

                // RT
                if (slot.TryGetComponent(out RectTransform rt))
                {
                    rt.anchoredPosition = new Vector2((row * 110 + 10), -(column * 110 + 10));
                }

                // Class
                if (slot.TryGetComponent(out InventorySlotEUIController MEIS))
                {
                    MEIS.Offset();
                    MEIS.Set_Data(SlotSprite);

                    rowMEISList.Add(MEIS);
                }

            }

            MEISList.Add(rowMEISList);
        }
    }

    #endregion

    #region Item

    public static InventoryItemEUIController Gen_ItemUI(InventorySlotEUIController _ParentSlot, 
        Sprite _ItemSprite, 
        Sprite _RankImg, 
        int _BoostLv)
    {
        // Generate GO
        GameObject item = Instantiate(ModuleItemManager.Instance.InventoryItemPrefab, _ParentSlot.transform);

        // RT
        if (item.TryGetComponent(out RectTransform rt))
        {
            rt.anchoredPosition = Vector2.zero;
        }

        // Class
        if (item.TryGetComponent(out InventoryItemEUIController MEII))
        {
            MEII.Offset();
            MEII.Set_Data(_ItemSprite, _RankImg, _BoostLv);

            _ParentSlot.ThisSlotItem = MEII;
            return MEII;
        }

        return null;
    }

    public InventoryItemEUIController Gen_Item_ThisInventory(
        Sprite _ItemSprite, 
        Sprite _RankImg, 
        int _BoostLv, 
        ModuleUpgradeUIController _Owner)
    {
        // Generate GO
        InventorySlotEUIController emptySlot = Get_EmptyMEIS();
        GameObject item = Instantiate(ModuleItemManager.Instance.InventoryItemPrefab, emptySlot.transform);
        
        // RT
        if(item.TryGetComponent(out RectTransform rt))
        {
            rt.anchoredPosition = Vector2.zero;
        }

        // Class
        if (item.TryGetComponent(out InventoryItemEUIController MEII))
        {
            MEII.Offset();
            MEII.Set_Data(_ItemSprite, _RankImg, _BoostLv);
            MEII.OwnerUIController = _Owner;
            emptySlot.ThisSlotItem = MEII;
            return MEII;
        }

        return null;
    }

    private InventorySlotEUIController Get_EmptyMEIS()
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

    public InventorySlotEUIController Get_TargetSlot(InventoryItemEUIController _MEII)
    {
        foreach (List<InventorySlotEUIController> MEIS_List in MEISList)
        {
            foreach (InventorySlotEUIController MEIS in MEIS_List)
            {
                if (MEIS.ThisSlotItem == _MEII)
                {
                    return MEIS;
                }
            }
        }
        return null;
    }

    public void Remove_ItemInSlotData(InventoryItemEUIController _MEII)
    {
        InventorySlotEUIController MEIS = Get_TargetSlot(_MEII);
        if (MEIS != null)
        {
            MEIS.ThisSlotItem = null;
        }
    }

    #endregion

}
