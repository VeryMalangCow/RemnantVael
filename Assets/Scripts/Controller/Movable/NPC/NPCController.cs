using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MovableObjectController
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
        NPCManager.Instance.Add_NPCList(this);
    }

    #endregion
}
