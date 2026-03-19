using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    #region Value

    [SerializeField] public List<NPCController> allNpcs;

    #endregion

    #region Get

    public NPCController Get_CorrectNPC(int id)
    {
        return IDController.Get_CorrectIDObject<NPCController>(id, new List<IDController>(allNpcs));
    }

    #endregion
}
