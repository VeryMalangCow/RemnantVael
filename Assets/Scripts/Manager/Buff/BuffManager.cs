using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Manager")]

    // Current Data
    [HideInInspector] private List<BuffController> AllBuffs = new List<BuffController>();
    [HideInInspector] public List<IWhen_GetElectricity> IWhen_GetElectricityList = new List<IWhen_GetElectricity>();

    #endregion

    #region Framework

    private void Offset()
    {
        // Set All Buff Controller
        BuffController[] bcArray = this.gameObject.GetComponents<BuffController>();
        for (int i = 0; i < bcArray.Length; i++)
        {
            AllBuffs.Add(bcArray[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        Offset();
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

    #endregion

    #region Get

    public BuffController Get_CorrectBuff(int _ID)
    {
        return IDController.Get_CorrectIDObject<BuffController>(_ID, new List<IDController>(AllBuffs));
    }

    #endregion

    #region Active

    // 전기 속성 디버프를 얻을 시
    public void Active_GetElectricity()
    {
        DevTool.Play_AllIWhen(IWhen_GetElectricityList);
    }

    #endregion
}
