using System.Collections.Generic;
using UnityEngine;

public class AllyBuffController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Dmg")]
    [SerializeField] private AllyBuff Sync005_Buff;
    [SerializeField] private AllyBuff Sync006_Buff;
    [SerializeField] private AllyBuff Sync007_Buff;
    [SerializeField] private AllyBuff Sync008_Buff;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] private AllyController Ally;
    [SerializeField] public AllyBuffState BuffingState;

    // Dict
    [HideInInspector] private Dictionary<string, AllyBuff> BuffDict;

    #endregion

    #endregion

    #region Offset

    public void Offset(AllyController _Ally)
    {
        Ally = _Ally;

        BuffingState = new AllyBuffState();

        Offset_Buff();
        Offset_BuffDict();
    }

    private void Offset_Buff()
    {
        Sync005_Buff = new AllyBuff(Ally, AllyBuffManager.Instance.Sync005_OriginalBuff);
        Sync006_Buff = new AllyBuff(Ally, AllyBuffManager.Instance.Sync006_OriginalBuff);
        Sync007_Buff = new AllyBuff(Ally, AllyBuffManager.Instance.Sync007_OriginalBuff);
        Sync008_Buff = new AllyBuff(Ally, AllyBuffManager.Instance.Sync008_OriginalBuff);
    }
    
    private void Offset_BuffDict()
    {
        BuffDict = new Dictionary<string, AllyBuff>
        {
            { "Sync005", Sync005_Buff },
            { "Sync006", Sync006_Buff },
            { "Sync007", Sync007_Buff },
            { "Sync008", Sync008_Buff }
        };
    }

    #endregion

    #region Framework

    private void Update()
    {
        BuffingState.UpdateData(Time.deltaTime);
    }

    #endregion

    #region Set

    public void Reset_SyncState()
    {
        Sync005_Buff.SetOff_State();
        Sync006_Buff.SetOff_State();
        Sync007_Buff.SetOff_State();
        Sync008_Buff.SetOff_State();
    }

    #endregion

    #region Get

    // Get Ally Buff
    public AllyBuff Get_AllyBuff(string _Key)
    {
        return BuffDict[_Key];
    }

    // Buffed State Return 
    public AllyState Get_BuffedState()
    {
        return BuffingState.Get_BuffedAllyState();
    }

    #endregion
}
