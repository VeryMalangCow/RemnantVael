using System.Collections.Generic;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    private AlwaysCooltimeData totemeTimer = new AlwaysCooltimeData(1f);
    private List<TotemeController> allTotemeList = new List<TotemeController>();

    #region Framework

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        Caculate_TotemeTimer(deltaTime);
    }

    #endregion

    #region Timer

    private void Caculate_TotemeTimer(float deltaTime)
    {
        if (totemeTimer.Is_Full(deltaTime))
        {
            for (int i = 0; i < allTotemeList.Count; i++)
            {
                allTotemeList[i].Active_Buff();
            }
        }
    }

    #endregion

    #region List

    public void Add_Toteme(TotemeController toteme)
    {
        DevTool.Add_InList(allTotemeList, toteme);    
    }

    public void Remove_Toteme(TotemeController toteme)
    {
        DevTool.Remove_InList(allTotemeList, toteme);
    }

    #endregion
}
