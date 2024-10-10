using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill
{
    public ItemData ThisItemData;
    public List<ModifyEachInventoryItem> ThisMEII = new List<ModifyEachInventoryItem>();
    public List<ModifyEachInventoryItem> ThisExtraMEII = new List<ModifyEachInventoryItem>();

    public PassiveSkill(int _ID)
    {
        ThisItemData = new ItemData();
        ThisItemData.ID = _ID;
    }


    public static List<PassiveSkill> AllPassiveItemSkill()
    {
        return new List<PassiveSkill>
        {
            new Item0(0), 
            new Item1(1)
        };
    }

    protected int GetTargetRank()
    {
        int targetRank = PlayerManager.Instance.PlayerController.CurrentBoostLv.Value;
        if (targetRank > ThisItemData.Rank)
        {
            targetRank = ThisItemData.Rank;
        }
        return targetRank;
    }
}

#region Interface

public interface IWhen_Always
{
    public void When_Always();
}

public interface IWhen_Fire
{
    public void When_Fire();
}

#endregion

#region Item Skill

public class Item0 : PassiveSkill, IWhen_Always
{
    public Item0(int _ID) : base(_ID) { }

    public void When_Always()
    {
        int targetRank = GetTargetRank();

        switch(targetRank)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

            default:
                break;
        }
    }
}

public class Item1 : PassiveSkill, IWhen_Fire
{
    public Item1(int _ID) : base(_ID) { }

    public void When_Fire()
    {
        int targetRank = GetTargetRank();

        switch (targetRank)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

            default:
                break;
        }
    }
}

#endregion