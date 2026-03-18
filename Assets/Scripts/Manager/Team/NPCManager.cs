using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    #region Value

    [SerializeField] public List<NPCController> allNpcs;

    #endregion

    #region Get

    public NPCController Get_CorrectNPC(int _ID)
    {
        return IDController.Get_CorrectIDObject<NPCController>(_ID, new List<IDController>(allNpcs));
    }

    #endregion
}
