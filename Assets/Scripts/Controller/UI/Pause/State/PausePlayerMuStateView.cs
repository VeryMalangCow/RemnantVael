using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PausePlayerMuStateView : MonoBehaviour
{
    [SerializeField] private ScrollPanelEUIController playerMuScrollEui;
    [SerializeField] private StandbyPlayerMUEUIController[] muEuiArr;
    [SerializeField] private StandbyPlayerSynergyEUIController[] synergyEuiArr;

    public void Init()
    {
        for (int i = 0; i < muEuiArr.Length; i++)
            muEuiArr[i].Offset();

        for (int i = 0; i < synergyEuiArr.Length; i++)
            synergyEuiArr[i].Offset();

    }
    public void SetColor(Color imgClr, Color txtClr)
    {
        for (int i = 0; i < muEuiArr.Length; i++)
            muEuiArr[i].Set_Color(txtClr);

        for (int i = 0; i < synergyEuiArr.Length; i++)
            synergyEuiArr[i].Set_Color(imgClr, txtClr);
    }

    public void SetState()
    {
        // Module
        List<CopyModuleState> states = ModuleItemManager.instance.Get_EquippedModuleState();
        for (int i = 0; i < muEuiArr.Length; i++)
        {

            if (i < states.Count)
                muEuiArr[i].SetOn(states[i].state);
            else
                muEuiArr[i].SetOff();
        }

        // Sync
        for (int i = 0; i < synergyEuiArr.Length; i++)
            synergyEuiArr[i].SetOff();

        int index = 0;
        Dictionary<int, int> syncData = ModuleItemManager.instance.Get_CurrentMainChipData();
        foreach (KeyValuePair<int, int> data in syncData)
        {
            synergyEuiArr[index].SetOn(data.Key, data.Value);
            index++;
        }
    }
}
