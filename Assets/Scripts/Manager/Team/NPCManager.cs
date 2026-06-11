using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>, IMainGameInitializer
{
    #region Value

    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;


    [SerializeField] public List<NPCController> allNpcs;

    #endregion

    #region Init

    public IEnumerator Initialize()
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"NpcManager: <color=orange>NONE</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield break;
    }

    #endregion

    #region Get

    public NPCController Get_CorrectNPC(int id)
    {
        return IDController.Get_CorrectIDObject<NPCController>(id, new List<IDController>(allNpcs));
    }

    #endregion
}
