using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> NPC")]
    [SerializeField] public int ID;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        NPCManager.Instance.InitNPCList(this);
    }

    #endregion
}
