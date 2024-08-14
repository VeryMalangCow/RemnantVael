using System.Collections.Generic;

public class PassiveSkill
{
    public int ThisItemID = 0;
    public PassiveSkill(int _ID)
    {
        ThisItemID = _ID;
    }

    public static List<PassiveSkill> AllPassiveItemSkill()
    {
        return new List<PassiveSkill>
        {
            new Item0(0), 
            new Item1(1)
        };
    }
}

#region Interface

public interface IWhen_Always
{
    public void When_Always(int _Rank);
}

public interface IWhen_Fire
{
    public void When_Fire(int _Rank);
}

#endregion

#region Item Skill

public class Item0 : PassiveSkill, IWhen_Always
{
    public Item0(int _ID) : base(_ID) { }

    public void When_Always(int _Rank)
    {
        
    }
}

public class Item1 : PassiveSkill, IWhen_Fire
{
    public Item1(int _ID) : base(_ID) { }

    public void When_Fire(int _Rank)
    {

    }
}

#endregion