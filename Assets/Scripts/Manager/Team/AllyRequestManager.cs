using System.Collections.Generic;
using UnityEngine;

public class AllyRequestManager : Singleton<AllyRequestManager>
{
    #region Value

    // Ally Request
    [HideInInspector] private AllyCompleteList<IWhen_Complete_KillNormalEnemy> Complete_KillNormalEnemy = new AllyCompleteList<IWhen_Complete_KillNormalEnemy>();
    [HideInInspector] private AllyCompleteList<IWhen_Complete_KillEliteEnemy> Complete_KillEliteEnemy = new AllyCompleteList<IWhen_Complete_KillEliteEnemy>();

    [HideInInspector] private AllyFailList<IWhen_Fail_TakingDamage> Fail_TakingDamage = new AllyFailList<IWhen_Fail_TakingDamage>();
    [HideInInspector] private AllyFailList<IWhen_Fail_UsingSkill> Fail_UsingSkill = new AllyFailList<IWhen_Fail_UsingSkill>();

    [HideInInspector] private Dictionary<string, object> CompleteDict;
    [HideInInspector] private Dictionary<string, object> FailDict;

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        Offset();
    }

    private void Offset()
    {
        CompleteDict = new Dictionary<string, object>
        {
            { "KillNormalEnemy", Complete_KillNormalEnemy },
            { "KillEliteEnemy", Complete_KillEliteEnemy }
        };

        FailDict = new Dictionary<string, object>
        {
            { "TakingDamage", Fail_TakingDamage },
            { "UsingSkill", Fail_UsingSkill }
        };
    }

    #endregion

    #region List

    #region Add

    public void Add_RequestComplete<T>(string _Name, T _Data) where T : IWhen_Request
    {
        DevTool.Add_InList(Get_CorrectCompleteList<T>(_Name).List, _Data);
    }

    public void Add_RequestFail<T>(string _Name, T _Data) where T : IWhen_Fail
    {
        DevTool.Add_InList(Get_CorrectFailList<T>(_Name).List, _Data);
    }


    #endregion

    #region Remove

    public void Remove_RequestComplete<T>(string _Name, T _Data) where T : IWhen_Request
    {
        AllyCompleteList<T> listData = Get_CorrectCompleteList<T>(_Name);
        DevTool.Remove_InList(listData.List, _Data);
    }

    public void Remove_RequestFail<T>(string _Name, T _Data) where T : IWhen_Fail
    {
        AllyFailList<T> listData = Get_CorrectFailList<T>(_Name);
        DevTool.Remove_InList(listData.List, _Data);
    }

    #endregion

    #region Get

    private AllyCompleteList<T> Get_CorrectCompleteList<T>(string _Name) where T : IWhen_Request
    {
        return DevTool.Can_CastingTType(CompleteDict[_Name], out AllyCompleteList<T> allyRequest) ? allyRequest : null;
    }
    private AllyFailList<T> Get_CorrectFailList<T>(string _Name) where T : IWhen_Fail
    {
        return DevTool.Can_CastingTType(FailDict[_Name], out AllyFailList<T> allyRequest) ? allyRequest : null;
    }

    #endregion

    #endregion

    #region Play (Complete)

    public void Play_KillNormalEnemy() => Complete_KillNormalEnemy.Play_Request();
    public void Play_KillEliteEnemy() => Complete_KillEliteEnemy.Play_Request();

    #endregion

    #region Play (Fail)

    public void Play_TakingDamage() => Fail_TakingDamage.Play_Request();
    public void Play_UsingSkill() => Fail_UsingSkill.Play_Request();

    #endregion
}
