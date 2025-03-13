using System.Collections.Generic;
using UnityEngine;

public class ModuleItemManager : Singleton<ModuleItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== MainChip")]
    [SerializeField] private List<MainChipData> MainChipDataList;
    [HideInInspector] private Dictionary<int, int> MainChopAmalgamationDict = new Dictionary<int, int>();

    [Header("=== Gotten Item")]
    [HideInInspector] private List<ModuleState> Gotten_MSList = new List<ModuleState>();
    [HideInInspector] public List<ModuleState> Equiped_MSList = new List<ModuleState>();


    [Header("=== Icon Data")]
    [SerializeField] private List<Sprite> RankIconList;
    [SerializeField] private List<Sprite> MUUIDescRankIconList;

    // Interface
    private List<IWhen_Hit> iWhen_HitList = new List<IWhen_Hit>();
    private List<IWhen_CriticalHit> iWhen_CriticalHitList = new List<IWhen_CriticalHit>();
    private List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #region Field

    public ItemData Get_RandomInteractItem()
    {
        //return ItemDataList[4];
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void Gain_ModuleState(ItemData _ItemData)
    {
        foreach (ModuleState MS in ModuleState.Get_AllModuleState())
        {
            if (MS.ThisItemData.ID == _ItemData.ID) 
            {
                MS.ThisItemData = new ItemData(_ItemData);

                MS.InventoryUI_Equip = MainGameUIManager.Instance.ModuleUpgrade_UIController.Get_MEIIList(MS.ThisItemData.ItemIcon, Get_CorrectRankIcon(MS), MS.ThisItemData.BoostLv);
                
                foreach(InventoryItemEUIController MEII in MS.InventoryUI_Equip)
                {
                    MEII.gameObject.name = $"{MS.ThisItemData.ID}_{MS.ThisItemData.Rank}_{MS.ThisItemData.BoostLv}";
                }

                Gotten_MSList.Add(MS);
            }
        }
    }


    #endregion

    #region Interface

    public void Reset_Interface()
    {
        iWhen_HitList.Clear();
        iWhen_FireList.Clear();
        iWhen_CriticalHitList.Clear();

        foreach (ModuleState MS in Equiped_MSList)
        {
            if (MS is IWhen_Hit iHit && !iWhen_HitList.Contains(iHit))
            { iWhen_HitList.Add(iHit); }
            if (MS is IWhen_Fire iFire && !iWhen_FireList.Contains(iFire))
            { iWhen_FireList.Add(iFire); }
            if (MS is IWhen_CriticalHit iCriticalHit && !iWhen_CriticalHitList.Contains(iCriticalHit))
            { iWhen_CriticalHitList.Add(iCriticalHit); }
        }
    }

    public void Active_Hit(EnemyController _EC)
    {
        if (iWhen_HitList.Count > 0)
        {
            for (int i = 0; i < iWhen_HitList.Count; i++)
            {
                iWhen_HitList[i].Play_When(_EC);
            }
        }
    }

    public void Active_CriticalHit(EnemyController _EC)
    {
        if (iWhen_CriticalHitList.Count > 0)
        {
            for (int i = 0; i < iWhen_CriticalHitList.Count; i++)
            {
                iWhen_CriticalHitList[i].Play_When(_EC);
            }
        }
    }

    public void Active_Fire()
    {
        if (iWhen_FireList.Count > 0)
        {
            for (int i = 0; i < iWhen_FireList.Count; i++)
            {
                iWhen_FireList[i].Play_When();
            }
        }
    }

    #endregion

    #region Find

    public ModuleState Get_EquipedModuleState(InventoryItemEUIController _MEII)
    {
        foreach (ModuleState MS in Gotten_MSList)
        {
            if (MS.InventoryUI_Forge.Contains(_MEII))
            {
                return MS;
            }
        }

        return null;
    }

    public ModuleState Get_InventoryModuleState(InventoryItemEUIController _MEII)
    {
        foreach(ModuleState MS in Gotten_MSList)
        {
            if (MS.InventoryUI_Equip.Contains(_MEII))
            {
                return MS;
            }
        }

        return null;
    }


    public Sprite Get_CorrectRankIcon(ModuleState _MS)
    {
        return RankIconList[_MS.ThisItemData.Rank - 1];
    }

    public Sprite Get_CorrectMUUIDescRankIcon(ModuleState _MS)
    {
        return MUUIDescRankIconList[_MS.ThisItemData.Rank - 1];
    }

    #endregion

    #region Decomposition

    public int Get_EC_ForUpgrade(InventoryItemEUIController _MEII)
    {
        if (_MEII == null)
        { return 0; }

        ModuleState ms = Get_InventoryModuleState(_MEII);
        if (ms != null)
        {
            return (ms.ThisItemData.BoostLv + 1);
        }
        return 0;
    }

    public int Get_MC_ForFusion(InventoryItemEUIController _MEII)
    {
        if (_MEII == null)
        { return 0; }

        ModuleState ms = Get_InventoryModuleState(_MEII);
        if (ms != null)
        {
            return (ms.ThisItemData.Rank + 1);
        }
        return 0;
    }


    #endregion

    #region MainChip

    public MainChipData Get_CorrectMainChip(int _ID)
    {
        for (int i = 0; i < MainChipDataList.Count; i++)
        {
            if (MainChipDataList[i].ID == _ID)
            {
                return MainChipDataList[i];
            }
        }
        return null;
    }

    public void Set_MainChipData()
    {
        MainChopAmalgamationDict = new Dictionary<int, int>();
        if (Equiped_MSList.Count > 0)
        {
            for (int i = 0; i < Equiped_MSList.Count; i++)
            {
                int synergyID_1 = Equiped_MSList[i].ThisItemData.R1_MainChipID;
                int synergyID_3 = Equiped_MSList[i].ThisItemData.R3_MainChipID;
                int synergyID_5 = Equiped_MSList[i].ThisItemData.R5_MainChipID;

                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 0.5f;

                if (Equiped_MSList[i].ThisItemData.Rank >= 5)
                {
                    Add_MainChipData(synergyID_1, 3);
                    Add_MainChipData(synergyID_3, 2);
                    Add_MainChipData(synergyID_5, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 1;
                }
                else if (Equiped_MSList[i].ThisItemData.Rank >= 3)
                {
                    Add_MainChipData(synergyID_1, 2);
                    Add_MainChipData(synergyID_3, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;

                }
                else
                {
                    Add_MainChipData(synergyID_1, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                }
            }
        }
        if (MainGameUIManager.Instance.ModuleUpgrade_UIController != null)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_SynergySlots(MainChopAmalgamationDict);
        }

    }

    public void Add_MainChipData(int _SynergyID, int _Amount)
    {
        if (MainChopAmalgamationDict.ContainsKey(_SynergyID))
        { MainChopAmalgamationDict[_SynergyID] += _Amount; }
        else
        { MainChopAmalgamationDict.Add(_SynergyID, _Amount); }
    }

    #endregion

    #region Delete

    public void Remove_ModuleState(InventoryItemEUIController _MEII)
    {
        ModuleState foundMs = Get_CorrectModuleState(Gotten_MSList, _MEII);

        if (Gotten_MSList.Contains(foundMs))
        { Gotten_MSList.Remove(foundMs); }

        if (Equiped_MSList.Contains(foundMs))
        { Equiped_MSList.Remove(foundMs); }

        Remove_MEIIList(foundMs);
        foundMs = null;
    }

    private ModuleState Get_CorrectModuleState(List<ModuleState> TargetMsList, InventoryItemEUIController _MEII)
    {
        foreach(ModuleState MS in TargetMsList)
        {
            if (MS.InventoryUI_Equip.Contains(_MEII))
            {
                return MS;
            }
        }
        return null;
    }

    private void Remove_MEIIList(ModuleState _MS)
    {
        for (int i = _MS.InventoryUI_Equip.Count - 1; i >= 0; i--)
        { Destroy(_MS.InventoryUI_Equip[i].gameObject); }

        for (int i = _MS.InventoryUI_Forge.Count - 1; i >= 0; i--)
        { Destroy(_MS.InventoryUI_Forge[i].gameObject); }
    }

    #endregion
}