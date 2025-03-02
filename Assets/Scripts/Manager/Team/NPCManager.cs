using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    #region Value

    [SerializeField] private List<NPCController> AllNPC;

    #endregion

    #region Get

    public NPCController Get_CorrectNPC(int _ID)
    {
        return IDController.Get_CorrectIDObject<NPCController>(_ID, new List<IDController>(AllNPC));
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
