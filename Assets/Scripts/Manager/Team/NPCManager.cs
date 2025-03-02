using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    #region Value

    [SerializeField] private List<NPCController> AllNPC;

    #endregion

    #region Get

    public NPCController Get_NPC(int _ID)
    {
        for (int i = 0; i < AllNPC.Count; i++)
        {
            if (AllNPC[i].ID == _ID)
            { return AllNPC[i]; }
        }
        return null;
    }

    #endregion

    #region Set

    public void Add_NPCList(NPCController _NPC)
    {
        if (!AllNPC.Contains(_NPC))
        { AllNPC.Add(_NPC); }
    }

    public void Remove_NPCList(NPCController _NPC)
    {
        if (AllNPC.Contains(_NPC))
        { AllNPC.Remove(_NPC); }
    }

    #endregion
}
