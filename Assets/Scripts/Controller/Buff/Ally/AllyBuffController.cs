using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AllyBuffController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff")]
    [FormerlySerializedAs("Sync005_Buff")][SerializeField] private AllyBuff sync005_Buff;
    [FormerlySerializedAs("Sync006_Buff")][SerializeField] private AllyBuff sync006_Buff;
    [FormerlySerializedAs("Sync007_Buff")][SerializeField] private AllyBuff sync007_Buff;
    [FormerlySerializedAs("Sync008_Buff")][SerializeField] private AllyBuff sync008_Buff;
    [FormerlySerializedAs("TotisToteme_Buff")][SerializeField] private AllyBuff totisToteme_Buff;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] private AllyController ally;
    [FormerlySerializedAs("BuffingState")][SerializeField] public AllyBuffState buffingState;

    // Dict
    [HideInInspector] private Dictionary<string, AllyBuff> buffDict;

    #endregion

    #endregion

    #region Offset

    public void Offset(AllyController ally)
    {
        this.ally = ally;

        buffingState = new AllyBuffState();

        Offset_Buff();
        Offset_BuffDict();
    }

    private void Offset_Buff()
    {
        sync005_Buff = new AllyBuff(ally, AllyBuffManager.instance.sync005_OriginalBuff);
        sync006_Buff = new AllyBuff(ally, AllyBuffManager.instance.sync006_OriginalBuff);
        sync007_Buff = new AllyBuff(ally, AllyBuffManager.instance.sync007_OriginalBuff);
        sync008_Buff = new AllyBuff(ally, AllyBuffManager.instance.sync008_OriginalBuff);
        totisToteme_Buff = new AllyBuff(ally, AllyBuffManager.instance.totisToteme_OriginalBuff);
    }
    
    private void Offset_BuffDict()
    {
        buffDict = new Dictionary<string, AllyBuff>
        {
            { "Sync005", sync005_Buff },
            { "Sync006", sync006_Buff },
            { "Sync007", sync007_Buff },
            { "Sync008", sync008_Buff },
            { "TotisToteme", totisToteme_Buff }
        };
    }

    #endregion

    #region Framework

    private void Update()
    {
        buffingState.UpdateData(Time.deltaTime);
    }

    #endregion

    #region Set

    public void Reset_SyncState()
    {
        sync005_Buff.SetOff_State();
        sync006_Buff.SetOff_State();
        sync007_Buff.SetOff_State();
        sync008_Buff.SetOff_State();
    }

    #endregion

    #region Get

    // Get Ally Buff
    public AllyBuff Get_AllyBuff(string key)
    {
        return buffDict[key];
    }

    // Buffed State Return 
    public AllyState Get_BuffedState()
    {
        return buffingState.Get_BuffedAllyState();
    }

    #endregion
}
