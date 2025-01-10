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

    public void GetBuff(int _ID)
    {
        BuffController correctBuff = GetCorrectBuff(_ID);

        if (correctBuff == null)
        { return; }

        correctBuff.GainBuff();
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
