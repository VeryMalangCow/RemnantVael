using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : Singleton<ItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== Gotten Item")]
    [SerializeField] private List<int> ItemIDList;
    private List<PassiveSkill> PISList = new List<PassiveSkill>();
    private List<IWhen_Get> iWhen_GetList = new List<IWhen_Get>();
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

                if (PIS is IWhen_Get iGet)
                { iWhen_GetList.Add(iGet); /* Get */ iGet.When_Get(); }
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

}

[System.Serializable]
public class ItemData
{
    public int ID;
    public string Name;
    public string Description;
    public Sprite Sprite;
}
