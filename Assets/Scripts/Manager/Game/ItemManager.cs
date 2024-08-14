using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== Gotten Item")]
    [SerializeField] private List<int> ItemIDList;
    private List<PassiveSkill> PISList = new List<PassiveSkill>();
    private List<IWhen_Always> iWhen_AlwaysList = new List<IWhen_Always>();
    public List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #region Field

    public ItemData GetRandomInteractItem()
    {
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void GetItemSkill(int _ItemID)
    {
        if(!CheckAlreadyHaveItem(_ItemID))
        { return; }
        
        foreach (PassiveSkill PIS in PassiveSkill.AllPassiveItemSkill())
        {
            if (PIS.ThisItemID == _ItemID) 
            {
                ItemIDList.Add(PIS.ThisItemID);
                PISList.Add(PIS);

                if (PIS is IWhen_Always iGet)
                { iWhen_AlwaysList.Add(iGet); }

                if (PIS is IWhen_Fire iFire)
                { iWhen_FireList.Add(iFire); }

                
            }
        }
    }

    public bool CheckAlreadyHaveItem(int _ID)
    {
        if (ItemIDList.Contains(_ID))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Interface

    public void ActiveSkill_Always(int _BoostRank)
    {
        foreach (IWhen_Always fire in iWhen_AlwaysList)
        {
            fire.When_Always(_BoostRank);
        }
    }

    public void ActiveSkill_Fire(int _BoostRank)
    {
        foreach (IWhen_Fire fire in iWhen_FireList)
        {
            fire.When_Fire(_BoostRank);
        }
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
}
