using System.Collections.Generic;
using System.Linq;
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

    public ItemData GetRandomInteractItem()
    {
        //return ItemDataList[4];
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void GetModuleState(ItemData _ItemData)
    {
        foreach (ModuleState MS in ModuleState.GetAllModuleState())
        {
            if (MS.ThisItemData.ID == _ItemData.ID) 
            {
                MS.ThisItemData = new ItemData(_ItemData);

                MS.ThisMEII = MainGameUIManager.Instance.ModuleUpgrade_UIController.SpawnMEIIList(MS.ThisItemData.ItemIcon, GetCorrectRankIcon(MS), MS.ThisItemData.BoostLv);
                
                foreach(ModifyEachInventoryItem MEII in MS.ThisMEII)
                {
                    MEII.gameObject.name = $"{MS.ThisItemData.ID}_{MS.ThisItemData.Rank}_{MS.ThisItemData.BoostLv}";
                }

                Gotten_MSList.Add(MS);
            }
        }
    }


    #endregion

    #region Interface

    public void ResetInterface()
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
                iWhen_HitList[i].When(_EC);
            }
        }
    }

    public void Active_CriticalHit(EnemyController _EC)
    {
        Debug.Log(iWhen_CriticalHitList.Count);
        if (iWhen_CriticalHitList.Count > 0)
        {
            for (int i = 0; i < iWhen_CriticalHitList.Count; i++)
            {
                iWhen_CriticalHitList[i].When(_EC);
            }
        }
    }

    public void Active_Fire()
    {
        if (iWhen_FireList.Count > 0)
        {
            for (int i = 0; i < iWhen_FireList.Count; i++)
            {
                iWhen_FireList[i].When();
            }
        }
    }

    #endregion

    #region Find

    public ModuleState GetModuleState_Equiped(ModifyEachInventoryItem _MEII)
    {
        foreach (ModuleState MS in Gotten_MSList)
        {
            if (MS.ThisExtraMEII.Contains(_MEII))
            {
                return MS;
            }
        }

        return null;
    }

    public ModuleState GetModuleState_Inventory(ModifyEachInventoryItem _MEII)
    {
        foreach(ModuleState MS in Gotten_MSList)
        {
            if (MS.ThisMEII.Contains(_MEII))
            {
                return MS;
            }
        }

        return null;
    }


    public Sprite GetCorrectRankIcon(ModuleState _MS)
    {
        return RankIconList[_MS.ThisItemData.Rank - 1];
    }
    public Sprite GetCorrectMUUIDescRankIcon(ModuleState _MS)
    {
        return MUUIDescRankIconList[_MS.ThisItemData.Rank - 1];
    }

    #endregion

    #region Decomposition

    public int NeedEC_AbleUpgrade(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        ModuleState ms = GetModuleState_Inventory(_MEII);
        if (ms != null)
        {
            return (ms.ThisItemData.BoostLv + 1);
        }
        return 0;
    }

    public int NeedMC_AbleFusion(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        ModuleState ms = GetModuleState_Inventory(_MEII);
        if (ms != null)
        {
            return (ms.ThisItemData.Rank + 1);
        }
        return 0;
    }


    #endregion

    #region MainChip

    public MainChipData GetCorrectMainChip(int _ID)
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

    public void SetMainChipData()
    {
        MainChopAmalgamationDict = new Dictionary<int, int>();
        if (Equiped_MSList.Count > 0)
        {
            for (int i = 0; i < Equiped_MSList.Count; i++)
            {
                int synergyID_1 = Equiped_MSList[i].ThisItemData.Rank1_ItemMainChipID;
                int synergyID_3 = Equiped_MSList[i].ThisItemData.Rank3_ItemMainChipID;
                int synergyID_5 = Equiped_MSList[i].ThisItemData.Rank5_ItemMainChipID;

                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 0.5f;

                if (Equiped_MSList[i].ThisItemData.Rank >= 5)
                {
                    AddMainChipData(synergyID_1, 3);
                    AddMainChipData(synergyID_3, 2);
                    AddMainChipData(synergyID_5, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 1;
                }
                else if (Equiped_MSList[i].ThisItemData.Rank >= 3)
                {
                    AddMainChipData(synergyID_1, 2);
                    AddMainChipData(synergyID_3, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;

                }
                else
                {
                    AddMainChipData(synergyID_1, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                }
            }
        }
        if (MainGameUIManager.Instance.ModuleUpgrade_UIController != null)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetSynergySlots(MainChopAmalgamationDict);
        }

    }

    public void AddMainChipData(int _SynergyID, int _Amount)
    {
        if (MainChopAmalgamationDict.ContainsKey(_SynergyID))
        { MainChopAmalgamationDict[_SynergyID] += _Amount; }
        else
        { MainChopAmalgamationDict.Add(_SynergyID, _Amount); }
    }

    #endregion

    #region Delete

    public void DeleteModuleState(ModifyEachInventoryItem _MEII)
    {
        ModuleState foundMs = FindModuleState(Gotten_MSList, _MEII);

        if (Gotten_MSList.Contains(foundMs))
        { Gotten_MSList.Remove(foundMs); }

        if (Equiped_MSList.Contains(foundMs))
        { Equiped_MSList.Remove(foundMs); }

        DestroyMEIIList(foundMs);
        foundMs = null;
    }

    private ModuleState FindModuleState(List<ModuleState> TargetMsList, ModifyEachInventoryItem _MEII)
    {
        foreach(ModuleState MS in TargetMsList)
        {
            if (MS.ThisMEII.Contains(_MEII))
            {
                return MS;
            }
        }
        return null;
    }

    private void DestroyMEIIList(ModuleState _MS)
    {
        for (int i = _MS.ThisMEII.Count - 1; i >= 0; i--)
        { Destroy(_MS.ThisMEII[i].gameObject); }

        for (int i = _MS.ThisExtraMEII.Count - 1; i >= 0; i--)
        { Destroy(_MS.ThisExtraMEII[i].gameObject); }
    }

    #endregion
}

[System.Serializable]
public class ItemData
{
    [Space(20)]

    public int ID;

    [Space(10)]
    public string Name;
    public string Description;
    public string EquipDescription;
    public Sprite ItemIcon;

    [Space(10)]
    public int Rank1_ItemMainChipID;
    public int Rank3_ItemMainChipID;
    public int Rank5_ItemMainChipID;

    [Space(10)]
    public int BoostLv = 1;
    public int Rank = 1;

    public ItemData() { }
    public ItemData(ItemData _ItemData)
    {
        ID = _ItemData.ID;

        Name = _ItemData.Name;
        Description = _ItemData.Description;
        EquipDescription = _ItemData.EquipDescription;
        ItemIcon = _ItemData.ItemIcon;

        Rank1_ItemMainChipID = _ItemData.Rank1_ItemMainChipID;
        Rank3_ItemMainChipID = _ItemData.Rank3_ItemMainChipID;
        Rank5_ItemMainChipID = _ItemData.Rank5_ItemMainChipID;

        BoostLv = _ItemData.BoostLv;
        Rank = _ItemData.Rank;
    }
}

[System.Serializable]
public class MainChipData
{
    public Sprite ThisIcon;
    public int ID;
    public string Name;
    public List<string> AmalgamationDescList;
}
