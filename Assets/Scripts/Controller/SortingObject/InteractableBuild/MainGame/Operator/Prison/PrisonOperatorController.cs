using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected PrisonController TargetPrison;

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        IconStateAnim.Set_Anim(new State_Anim(UnitManager.Instance.Operator_AllyAC, 1f), 1f);
    }

    public virtual void Set_TargetBuild(PrisonController _TargetPrison)
    {
        TargetPrison = _TargetPrison;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !TargetPrison.IsOn;
    }

    #endregion

    #region Interact

    public override void Play_Interact()
    {
        
    }

    #endregion
}
