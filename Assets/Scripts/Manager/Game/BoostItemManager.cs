using System.Collections.Generic;
using UnityEngine;

public class BoostItemManager : Singleton<BoostItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== Gotten Item")]
    [HideInInspector] private List<PassiveSkill> PSList = new List<PassiveSkill>();
   
    // Interface
    private List<IWhen_Always> iWhen_AlwaysList = new List<IWhen_Always>();
    public List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #region Field

    public ItemData GetRandomInteractItem()
    {
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void GetItemSkill(int _ItemID, int _BoostLv, int _Rank)
    {
        foreach (PassiveSkill PIS in PassiveSkill.AllPassiveItemSkill())
        {
            if (PIS.ThisItemID == _ItemID) 
            {
                PIS.ThisBoostLv = _BoostLv;
                PIS.ThisRank = _Rank;
                PIS.ThisMEII = UIManager.Instance.ModuleUpgrade_UIController.SpawnMEIIList();
                foreach(ModifyEachInventoryItem MEII in PIS.ThisMEII)
                {
                    MEII.gameObject.name = $"{PIS.ThisItemID}_{PIS.ThisRank}_{PIS.ThisBoostLv}";
                }
                PSList.Add(PIS);

                /*
                if (PIS is IWhen_Always iGet)
                { iWhen_AlwaysList.Add(iGet); }
                if (PIS is IWhen_Fire iFire)
                { iWhen_FireList.Add(iFire); }
                */


            }
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


    private void Update()
    {
        // debug

        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            for (int i = 0; i < PSList.Count; i++)
            {
                Debug.Log($"{i}¹øÂ°: Id.{PSList[i].ThisItemID} / Bl.{PSList[i].ThisBoostLv} / R.{PSList[i].ThisRank} / U0.{PSList[i].ThisMEII[0].name} / U1.{PSList[i].ThisMEII[1].name}");
            }
        }
    }
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
}
