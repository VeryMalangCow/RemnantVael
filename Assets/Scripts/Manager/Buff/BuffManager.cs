using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>, IMainGameInitializer
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Manager")]

    // Current Data
    [HideInInspector] private Dictionary<int, BuffController> allBuffDict = new Dictionary<int, BuffController>();
    [HideInInspector] public List<IWhen_GetElectricity> iWhen_GetElectricityList = new List<IWhen_GetElectricity>();

    // Init
    [HideInInspector] private Dictionary<int, BuffController> allWhenSyncSetDict = new Dictionary<int, BuffController>();

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    #endregion

    #region Init

    // Init
    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Offset();
        sw.Stop();
        UnityEngine.Debug.Log($"BuffManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        yield return null;
    }
    
    #endregion


    #region Offset

    private void Offset()
    {
        // Set All Buff Controller
        allBuffDict = new Dictionary<int, BuffController>();
        allWhenSyncSetDict = new Dictionary<int, BuffController>();

        BuffController[] bcArray = this.gameObject.GetComponents<BuffController>();
        for (int i = 0; i < bcArray.Length; i++)
        {
            allBuffDict.Add(bcArray[i].Get_ID(), bcArray[i]);
            if (bcArray[i].condition_PlayerSyncSet)
            {
                allWhenSyncSetDict.Add(bcArray[i].Get_ID(), bcArray[i]);
            }
        }
    }

    #endregion

    #region Buff

    // 버프 획득 (증가)
    public void Gain_Buff(int id)
    {
        BuffController correctBuff = Get_CorrectBuff(id);

        if (correctBuff != null) correctBuff.Gain_Buff(); 
        
    }

    // 버프 사용 (감소)
    public void Reduce_Buff(int id)
    {
        BuffController correctBuff = Get_CorrectBuff(id);

        if (correctBuff != null) correctBuff.Reduct_Buff();
    }

    // 버프 종료
    public void End_Buff(int id)
    {
        BuffController correctBuff = Get_CorrectBuff(id);

        if (correctBuff != null) correctBuff.End_Buff();
    }

    #endregion

    #region Get

    public BuffController Get_CorrectBuff(int id)
    {
        return IDController.Get_CorrectIDObject(id, new Dictionary<int, BuffController>(allBuffDict));
    }

    #endregion

    #region Active

    // 전기 속성 디버프를 얻을 시
    public void Active_GetElectricity()
    {
        DevTool.Play_AllIWhen(iWhen_GetElectricityList);
    }

    #endregion

    #region another Condition

    public void Init_SyncSetBuff()
    {
        foreach(KeyValuePair<int, BuffController> syncBuff in allWhenSyncSetDict)
        {
            End_Buff(syncBuff.Value.Get_ID());
        }
    }

    #endregion
}
