using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Manager")]

    [Space(10)]
    [Header("=== Buff Controller")]
    [SerializeField] private List<BuffController> AllBuffs = new List<BuffController>();


    [HideInInspector] public List<IWhen_Hitted> iWhen_HittedList = new List<IWhen_Hitted>();

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

    private void Start()
    {
        Offset();
    }

    #endregion

    #region Buff

    // 버프 키기
    public void SetOn_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null)
        { 
            correctBuff.enabled = true; 
        }
    }

    // 버프 끄기
    public void SetOff_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null)
        { 
            correctBuff.enabled = false; 
        }
    }

    // 버프 획득 (증가)
    public void Gain_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null)
        {
            correctBuff.Gain_Buff(); 
        }
    }

    // 버프 사용 (감소)
    public void Reduce_Buff(int _ID)
    {
        BuffController correctBuff = Get_CorrectBuff(_ID);

        if (correctBuff != null)
        { 
            correctBuff.Reduct_Buff();
        }
    }

    // 맞는 버프컨트롤러 찾기

    public BuffController Get_CorrectBuff(int _ID)
    {
        return IDController.Get_CorrectIDObject<BuffController>(_ID, new List<IDController>(AllBuffs));
    }

    #endregion

    #region Active

    // 맞을 시, 실행
    public void Active_Hitted()
    {
        if (iWhen_HittedList.Count > 0)
        {
            for (int i = 0; i < iWhen_HittedList.Count; i++)
            {
                iWhen_HittedList[i].Play_When();
            }
        }
    }

    #endregion
}
