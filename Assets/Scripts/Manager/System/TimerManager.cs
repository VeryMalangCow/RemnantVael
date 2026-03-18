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

    private void Caculate_TotemeTimer(float _DeltaTime)
    {
        if (totemeTimer.Is_Full(_DeltaTime))
        {
            for (int i = 0; i < allTotemeList.Count; i++)
            {
                allTotemeList[i].Active_Buff();
            }
        }
    }

    #endregion

    #region List

    public void Add_Toteme(TotemeController _Toteme)
    {
        DevTool.Add_InList(allTotemeList, _Toteme);    
    }

    public void Remove_Toteme(TotemeController _Toteme)
    {
        DevTool.Remove_InList(allTotemeList, _Toteme);
    }

    #endregion
}
