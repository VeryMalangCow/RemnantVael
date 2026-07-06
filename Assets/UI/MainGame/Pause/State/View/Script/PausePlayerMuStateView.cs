using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PausePlayerMuStateView : MonoBehaviour
{
    [SerializeField] private ScrollPanelEUIController playerMuScrollEui;
    [Space(5)]
    [SerializeField] private StandbyPlayerMUEUIController playerMuEuiPrefab;
    [SerializeField] private Transform moduleEuiParentTf;
    [SerializeField] private Vector2Int moduleAmount;
    [SerializeField] private Vector2 moduleInterval;

    [Space(5)]
    [SerializeField] private StandbyPlayerSynergyEUIController playerSyncEuiPrefab;
    [SerializeField] private Transform syncEuiParentTf;
    [SerializeField] private Vector2Int syncAmount;
    [SerializeField] private Vector2 syncInterval;

    private StandbyPlayerMUEUIController[] muEuiArr;
    private StandbyPlayerSynergyEUIController[] synergyEuiArr;

    public IEnumerator InitAsync(Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif
        playerMuScrollEui.Offset();

        muEuiArr = new StandbyPlayerMUEUIController[moduleAmount.x * moduleAmount.y];
        int i, j, k = 0;
        for (i = 0; i < moduleAmount.y; i++)
        {
            for (j = 0; j < moduleAmount.x; j++)
            {
                StandbyPlayerMUEUIController eui = Instantiate(playerMuEuiPrefab, moduleEuiParentTf);
                eui.Offset();
                eui.rt.anchoredPosition = new Vector2(moduleInterval.x * j, moduleInterval.y * i);
                eui.SetColor(subClr);
                muEuiArr[k] = eui;
                k++;
            }
        }
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=#FFFF80>Mu State (Module Element) View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        synergyEuiArr = new StandbyPlayerSynergyEUIController[syncAmount.x * syncAmount.y];
        k = 0;
        for (i = 0; i < syncAmount.y; i++)
        {
            for (j = 0; j < syncAmount.x; j++)
            {
                StandbyPlayerSynergyEUIController eui = Instantiate(playerSyncEuiPrefab, syncEuiParentTf);
                eui.Offset();
                eui.rt.anchoredPosition = new Vector2(syncInterval.x * j, syncInterval.y * i);
                eui.SetColor(mainClr, subClr);
                synergyEuiArr[k] = eui;
                k++;
            }
        }

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=#FFFF80>Mu State (Sync Element) View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    public void SetColor(Color imgClr, Color txtClr)
    {
        for (int i = 0; i < muEuiArr.Length; i++)
            muEuiArr[i].SetColor(txtClr);

        for (int i = 0; i < synergyEuiArr.Length; i++)
            synergyEuiArr[i].SetColor(imgClr, txtClr);
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
