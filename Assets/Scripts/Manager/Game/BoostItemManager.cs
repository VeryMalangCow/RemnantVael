using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using System.Collections;
using Unity.VisualScripting;

public class BoostItemManager : Singleton<BoostItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== Gotten Item")]
    [HideInInspector] private List<PassiveSkill> Gotten_PSList = new List<PassiveSkill>();
    [HideInInspector] public List<PassiveSkill> Equiped_PSList = new List<PassiveSkill>();

    [Header("=== Goods")]
    [SerializeField] public ReactiveProperty<int> CurrentMC = new();


    [Header("=== Icon Data")]
    [SerializeField] private List<Sprite> RankIconList;

    // Interface
    private List<IWhen_Always> iWhen_AlwaysList = new List<IWhen_Always>();
    public List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #region Field

    public ItemData GetRandomInteractItem()
    {
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void GetItemSkill(ItemData _ItemData)
    {
        foreach (PassiveSkill PIS in PassiveSkill.AllPassiveItemSkill())
        {
            if (PIS.ThisItemData.ID == _ItemData.ID) 
            {
                PIS.ThisItemData = new ItemData(_ItemData);

                PIS.ThisMEII = MainGameUIManager.Instance.ModuleUpgrade_UIController.SpawnMEIIList(PIS.ThisItemData.ItemIcon, GetRankIcon(PIS.ThisItemData.Rank), PIS.ThisItemData.BoostLv);
                
                foreach(ModifyEachInventoryItem MEII in PIS.ThisMEII)
                {
                    MEII.gameObject.name = $"{PIS.ThisItemData.ID}_{PIS.ThisItemData.Rank}_{PIS.ThisItemData.BoostLv}";
                }

                Gotten_PSList.Add(PIS);
            }
        }
    }

    public void ResetInterface()
    {
        foreach(PassiveSkill PS in Equiped_PSList)
        {
            if (PS is IWhen_Always iGet)
            { iWhen_AlwaysList.Add(iGet); }
            if (PS is IWhen_Fire iFire)
            { iWhen_FireList.Add(iFire); }
        }
    }

    #endregion

    #region Interface

    public void ActiveSkill_Always()
    {
        foreach (IWhen_Always fire in iWhen_AlwaysList)
        {
            fire.When_Always();
        }
    }

    public void ActiveSkill_Fire()
    {
        foreach (IWhen_Fire fire in iWhen_FireList)
        {
            fire.When_Fire();
        }
    }

    #endregion

    #region Find

    public PassiveSkill GetPassiveSkill_Equiped(ModifyEachInventoryItem _MEII)
    {
        foreach (PassiveSkill PS in Gotten_PSList)
        {
            if (PS.ThisExtraMEII.Contains(_MEII))
            {
                return PS;
            }
        }

        return null;
    }

    public PassiveSkill GetPassiveSkill_Inventory(ModifyEachInventoryItem _MEII)
    {
        foreach(PassiveSkill PS in Gotten_PSList)
        {
            if (PS.ThisMEII.Contains(_MEII))
            {
                return PS;
            }
        }

        return null;
    }

    public Sprite GetRankIcon(int _Rank)
    {
        return RankIconList[_Rank - 1];
    }

    #endregion

    #region Decomposition

    public int NeedEC_AbleUpgrade(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        PassiveSkill ps = GetPassiveSkill_Inventory(_MEII);
        if (ps != null)
        {
            return (ps.ThisItemData.BoostLv + 1);
        }
        return 0;
    }

    public int NeedMC_AbleFusion(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        PassiveSkill ps = GetPassiveSkill_Inventory(_MEII);
        if (ps != null)
        {
            return (ps.ThisItemData.Rank + 1);
        }
        return 0;
    }


    #endregion

    #region Delete

    public void DeletePassiveSkill(ModifyEachInventoryItem _MEII)
    {
        PassiveSkill foundPS = FindPassiveSkill(Gotten_PSList, _MEII);

        if (Gotten_PSList.Contains(foundPS))
        { Gotten_PSList.Remove(foundPS); }

        if (Equiped_PSList.Contains(foundPS))
        { Equiped_PSList.Remove(foundPS); }

        DestroyMEIIList(foundPS);
        foundPS = null;
    }

    private PassiveSkill FindPassiveSkill(List<PassiveSkill> TargetPsList, ModifyEachInventoryItem _MEII)
    {
        foreach(PassiveSkill ps in TargetPsList)
        {
            if (ps.ThisMEII.Contains(_MEII))
            {
                return ps;
            }
        }
        return null;
    }

    private void DestroyMEIIList(PassiveSkill _PS)
    {
        for (int i = _PS.ThisMEII.Count - 1; i >= 0; i--)
        { Destroy(_PS.ThisMEII[i].gameObject); }

        for (int i = _PS.ThisExtraMEII.Count - 1; i >= 0; i--)
        { Destroy(_PS.ThisExtraMEII[i].gameObject); }
    }

    #endregion
}

[System.Serializable]
public class ItemData
{
    public int ID;
    public string Name;
    public string Description;
    public Sprite Sprite;
    public Sprite ItemIcon;

    [Space(10)]

    public int BoostLv = 1;
    public int Rank = 1;

    public ItemData() { }
    public ItemData(ItemData _ItemData)
    {
        ID = _ItemData.ID;
        Name = _ItemData.Name;
        Description = _ItemData.Description;
        Sprite = _ItemData.Sprite;
        ItemIcon = _ItemData.ItemIcon;
        BoostLv = _ItemData.BoostLv;
        Rank = _ItemData.Rank;
    }
}
