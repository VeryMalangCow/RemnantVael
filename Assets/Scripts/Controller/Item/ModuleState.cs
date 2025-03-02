using System.Collections.Generic;

public class ModuleState : IWhen
{
    // Data
    public ItemData ThisItemData;

    // UI
    public List<InventoryItemEUIController> ThisMEII = new List<InventoryItemEUIController>();
    public List<InventoryItemEUIController> ThisExtraMEII = new List<InventoryItemEUIController>();
    
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

        ThisActivityFuncDele = ModuleItemActivityManager.Instance.Get_CollectActivity(ThisItemData.ID);
    }

    #region Get

    public static List<ModuleState> Get_AllModuleState()
    {
        return new List<ModuleState>()
        {
            new ModuleItem000(0),
            new ModuleItem001(1),
            new ModuleItem002(2),
            new ModuleItem003(3),
            new ModuleItem004(4),
            new ModuleItem005(5),
        };
    }

    

    protected int Get_Rank()
    {
        return ThisItemData.Rank;
    }

    protected int Get_BoostLv()
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

    public virtual void Play_When(EnemyController _EC = null)
    {
        ThisActivityFuncDele(Get_Rank(), Get_BoostLv(), _EC);
    }

    #endregion
}

#region Interface

// 상속을 위한 부모 인터페이스
public interface IWhen
{
    public abstract void Play_When(EnemyController _EC = null);
}

// (기본공격) 발사 시
public interface IWhen_Fire : IWhen { }

// (어떤 공격이든) 적을 타격 시
public interface IWhen_Hit : IWhen { }

// (어떤 공격이든) 적을 크리티컬로 타격 시
public interface IWhen_CriticalHit : IWhen { }

// (어떤 공격이든) 적에게 타격 입을 시
public interface IWhen_Hitted : IWhen { }


#endregion

#region Item Skill

public class ModuleItem000 : ModuleState, IWhen_Fire
{ public ModuleItem000(int _ID) : base(_ID) { } }

public class ModuleItem001 : ModuleState, IWhen_Fire
{ public ModuleItem001(int _ID) : base(_ID) { } }

public class ModuleItem002 : ModuleState, IWhen_CriticalHit
{ public ModuleItem002(int _ID) : base(_ID) { } }

public class ModuleItem003 : ModuleState, IWhen_CriticalHit
{ public ModuleItem003(int _ID) : base(_ID) { } }

public class ModuleItem004 : ModuleState, IWhen_CriticalHit
{ public ModuleItem004(int _ID) : base(_ID) { } }

public class ModuleItem005 : ModuleState, IWhen_CriticalHit
{ public ModuleItem005(int _ID) : base(_ID) { } }

#endregion