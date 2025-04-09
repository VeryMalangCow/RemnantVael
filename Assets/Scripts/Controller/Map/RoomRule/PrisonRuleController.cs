using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform InRoom_PrisonParentTF;
    [SerializeField] public Transform InRoom_PrisonOperactorParentTF;

    [HideInInspector] public PrisonController Prison;

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Prison();
    }

    private void SetOn_Prison()
    {
        Prison.gameObject.SetActive(true);
    }

    #endregion
}
