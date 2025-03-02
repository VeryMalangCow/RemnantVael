using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    #region Value

    [SerializeField] private List<NPCController> AllNPCs;

    #endregion

    #region Get

    public NPCController Get_CorrectNPC(int _ID)
    {
        return IDController.Get_CorrectIDObject<NPCController>(_ID, new List<IDController>(AllNPCs));
    }

    

    #endregion

    #region Set

    public void Add_NPCList(NPCController _NPC)
    {
        if (!AllNPCs.Contains(_NPC))
        { AllNPCs.Add(_NPC); }
    }

    public void Remove_NPCList(NPCController _NPC)
    {
        if (AllNPCs.Contains(_NPC))
        { AllNPCs.Remove(_NPC); }
    }

    #endregion
}
