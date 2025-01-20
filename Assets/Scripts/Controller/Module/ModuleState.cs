using System.Collections.Generic;

public class ModuleState : IWhen
{
    // Data
    public ItemData ThisItemData;

    // UI
    public List<ModifyEachInventoryItem> ThisMEII = new List<ModifyEachInventoryItem>();
    public List<ModifyEachInventoryItem> ThisExtraMEII = new List<ModifyEachInventoryItem>();
    
    // PC
    protected PlayerController PC;

    // Activity
    protected ModuleItemActivityManager.ActivityFuncDele ThisActivityFuncDele;

    // Const
    public ModuleState(int _ID)
    {
        ThisItemData = new ItemData();
        ThisItemData.ID = _ID;
        PC = PlayerManager.Instance.PlayerController;

        ThisActivityFuncDele = ModuleItemActivityManager.Instance.GetCollectActivity(ThisItemData.ID);
    }

    #region Get

    public static List<ModuleState> GetAllModuleState()
    {
        return new List<ModuleState>()
        {
            new ModuleItem000(0),
            new ModuleItem001(1),
            new ModuleItem002(2),
            new ModuleItem003(3)
        };
    }

    

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

    #region Interface

    public virtual void When()
    {
        ThisActivityFuncDele(GetRank(), GetBoostLv());
    }

    #endregion
}

#region Interface

// 상속을 위한 부모 인터페이스
public interface IWhen
{
    public abstract void When();
}

// (기본공격) 발사 시
public interface IWhen_Fire : IWhen { }

// (어떤 공격이든) 적을 타격 시
public interface IWhen_Hit : IWhen { } 


#endregion

#region Item Skill

public class ModuleItem000 : ModuleState, IWhen_Fire
{ public ModuleItem000(int _ID) : base(_ID) { } }

public class ModuleItem001 : ModuleState, IWhen_Fire
{ public ModuleItem001(int _ID) : base(_ID) { } }

public class ModuleItem002 : ModuleState, IWhen_Hit
{ public ModuleItem002(int _ID) : base(_ID) { } }

public class ModuleItem003 : ModuleState, IWhen_Hit
{ public ModuleItem003(int _ID) : base(_ID) { } }

public class ModuleItem004 : ModuleState, IWhen_Hit
{ public ModuleItem004(int _ID) : base(_ID) { } }

public class ModuleItem005 : ModuleState, IWhen_Hit
{ public ModuleItem005(int _ID) : base(_ID) { } }
#endregion