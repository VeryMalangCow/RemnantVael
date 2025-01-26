using System.Collections;
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

    public void OnBuff(int _ID)
    {
        BuffController correctBuff = GetCorrectBuff(_ID);

        if (correctBuff == null)
        { return; }

        correctBuff.enabled = true;
    }

    public void OffBuff(int _ID)
    {
        BuffController correctBuff = GetCorrectBuff(_ID);

        if (correctBuff == null)
        { return; }

        correctBuff.enabled = false;
    }

    public void GetBuff(int _ID)
    {
        BuffController correctBuff = GetCorrectBuff(_ID);

        if (correctBuff == null)
        { return; }

        correctBuff.GainBuff();
    }

    public void UseBuff(int _ID)
    {
        BuffController correctBuff = GetCorrectBuff(_ID);

        if (correctBuff == null)
        {  return; }

        correctBuff.ReductBuff();
    }

    private BuffController GetCorrectBuff(int _ID)
    {
        for (int i = 0; i < AllBuffs.Count; i++)
        {
            if (AllBuffs[i].BuffID == _ID)
            {
                return AllBuffs[i];
            }
        }
        return null;
    }

    #endregion
}
