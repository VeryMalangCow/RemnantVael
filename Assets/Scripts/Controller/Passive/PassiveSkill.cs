using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill
{
    public ItemData ThisItemData;
    public List<ModifyEachInventoryItem> ThisMEII = new List<ModifyEachInventoryItem>();
    public List<ModifyEachInventoryItem> ThisExtraMEII = new List<ModifyEachInventoryItem>();
    protected PlayerController PC;

    public PassiveSkill(int _ID)
    {
        ThisItemData = new ItemData();
        ThisItemData.ID = _ID;
        PC = PlayerManager.Instance.PlayerController;
    }


    public static List<PassiveSkill> AllPassiveItemSkill()
    {
        return new List<PassiveSkill>
        {
            new Item0(0), 
            new Item1(1)
        };
    }

    #region Get

    protected int GetRank()
    {
        return ThisItemData.Rank;
    }

    protected int GetBoostLv()
    {
        int targetBoostLv = PC.CurrentBoostLv.Value;
        if (targetBoostLv > ThisItemData.BoostLv)
        {
            targetBoostLv = ThisItemData.BoostLv;
        }
        return targetBoostLv;
    }

    #endregion

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

public class Item0 : PassiveSkill, IWhen_Fire
{
    public Item0(int _ID) : base(_ID) { }

    public void When_Fire()
    {
        BoostItemManager.Instance.Spawn_MI_000(GetRank(), GetBoostLv());
    }
}

public class Item1 : PassiveSkill, IWhen_Fire
{
    public Item1(int _ID) : base(_ID) { }

    public void When_Fire()
    {
        
    }
}

#endregion