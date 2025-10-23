using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Manager")]

    // Current Data
    [HideInInspector] private Dictionary<int, BuffController> AllBuffDict = new Dictionary<int, BuffController>();
    [HideInInspector] public List<IWhen_GetElectricity> IWhen_GetElectricityList = new List<IWhen_GetElectricity>();

    // Init
    [HideInInspector] private Dictionary<int, BuffController> AllWhenSyncSetDict = new Dictionary<int, BuffController>();

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Offset();
    }

    #endregion

    #region Offset

    private void Offset()
    {
        // Set All Buff Controller
        AllBuffDict = new Dictionary<int, BuffController>();
        AllWhenSyncSetDict = new Dictionary<int, BuffController>();

        BuffController[] bcArray = this.gameObject.GetComponents<BuffController>();
        for (int i = 0; i < bcArray.Length; i++)
        {
            AllBuffDict.Add(bcArray[i].Get_ID(), bcArray[i]);
            if (bcArray[i].Condition_PlayerSyncSet)
            {
                AllWhenSyncSetDict.Add(bcArray[i].Get_ID(), bcArray[i]);
            }
        }
    }

    #endregion

    #region Buff

    // 버프 획득 (증가)
    public void Gain_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null) correctBuff.Gain_Buff(); 
        
    }

    // 버프 사용 (감소)
    public void Reduce_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null) correctBuff.Reduct_Buff();
    }

    // 버프 종료
    public void End_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null) correctBuff.End_Buff();
    }

    #endregion

    #region Get

    public BuffController Get_CorrectBuff(int _ID)
    {
        return IDController.Get_CorrectIDObject(_ID, new Dictionary<int, BuffController>(AllBuffDict));
    }

    #endregion

    #region Active

    // 전기 속성 디버프를 얻을 시
    public void Active_GetElectricity()
    {
        DevTool.Play_AllIWhen(IWhen_GetElectricityList);
    }

    #endregion

    #region another Condition

    public void Init_SyncSetBuff()
    {
        foreach(KeyValuePair<int, BuffController> syncBuff in AllWhenSyncSetDict)
        {
            End_Buff(syncBuff.Value.Get_ID());
        }
    }

    #endregion
}
