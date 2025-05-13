using System.Collections.Generic;
using UnityEngine;

public class RigidbodyAnimSolarController : RigidbodySolarController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Anim")]

    [Header("=== Component")]
    [SerializeField] private List<DirectionalAnimController> ThisAnimatorList;


    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        DevTool.Set_AnimSpeed(ThisAnimatorList, ThisRb.velocity.sqrMagnitude * 0.3f);

        base.LateUpdate();
    }

    #endregion
}
