using System.Collections.Generic;
using UnityEngine;

public class AllyRequestManager : Singleton<AllyRequestManager>
{
    #region Value

    // Ally Request
    [HideInInspector] private AllyCompleteList<IWhen_Complete_KillNormalEnemy> complete_KillNormalEnemy = new AllyCompleteList<IWhen_Complete_KillNormalEnemy>();
    [HideInInspector] private AllyCompleteList<IWhen_Complete_KillEliteEnemy> complete_KillEliteEnemy = new AllyCompleteList<IWhen_Complete_KillEliteEnemy>();

    [HideInInspector] private AllyFailList<IWhen_Fail_TakingDamage> fail_TakingDamage = new AllyFailList<IWhen_Fail_TakingDamage>();
    [HideInInspector] private AllyFailList<IWhen_Fail_UsingSkill> fail_UsingSkill = new AllyFailList<IWhen_Fail_UsingSkill>();

    [HideInInspector] private Dictionary<string, object> completeDict;
    [HideInInspector] private Dictionary<string, object> failDict;

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        Offset();
    }

    private void Offset()
    {
        completeDict = new Dictionary<string, object>
        {
            { "KillNormalEnemy", complete_KillNormalEnemy },
            { "KillEliteEnemy", complete_KillEliteEnemy }
        };

        failDict = new Dictionary<string, object>
        {
            { "TakingDamage", fail_TakingDamage },
            { "UsingSkill", fail_UsingSkill }
        };
    }

    #endregion

    #region List

    #region Add

    public void Add_RequestComplete<T>(string name, T data) where T : IWhen_Request
    {
        DevTool.Add_InList(Get_CorrectCompleteList<T>(name).list, data);
    }

    public void Add_RequestFail<T>(string name, T data) where T : IWhen_Fail
    {
        DevTool.Add_InList(Get_CorrectFailList<T>(name).list, data);
    }


    #endregion

    #region Remove

    public void Remove_RequestComplete<T>(string name, T data) where T : IWhen_Request
    {
        AllyCompleteList<T> listData = Get_CorrectCompleteList<T>(name);
        DevTool.Remove_InList(listData.list, data);
    }

    public void Remove_RequestFail<T>(string name, T data) where T : IWhen_Fail
    {
        AllyFailList<T> listData = Get_CorrectFailList<T>(name);
        DevTool.Remove_InList(listData.list, data);
    }

    #endregion

    #region Get

    private AllyCompleteList<T> Get_CorrectCompleteList<T>(string name) where T : IWhen_Request
    {
        return DevTool.Can_CastingTType(completeDict[name], out AllyCompleteList<T> allyRequest) ? allyRequest : null;
    }
    private AllyFailList<T> Get_CorrectFailList<T>(string name) where T : IWhen_Fail
    {
        return DevTool.Can_CastingTType(failDict[name], out AllyFailList<T> allyRequest) ? allyRequest : null;
    }

    #endregion

    #endregion

    #region Play (Complete)

    public void Play_KillNormalEnemy() => complete_KillNormalEnemy.Play_Request();
    public void Play_KillEliteEnemy() => complete_KillEliteEnemy.Play_Request();

    #endregion

    #region Play (Fail)

    public void Play_TakingDamage() => fail_TakingDamage.Play_Request();
    public void Play_UsingSkill() => fail_UsingSkill.Play_Request();

    #endregion
}
