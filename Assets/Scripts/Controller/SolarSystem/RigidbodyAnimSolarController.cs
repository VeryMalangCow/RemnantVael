using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RigidbodyAnimSolarController : RigidbodySolarController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Anim")]

    [Header("=== Component")]
    [FormerlySerializedAs("ThisAnimatorList")][SerializeField] private List<DirectionalAnimController> atList;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        DevTool.Set_AnimSpeed(atList, rb.velocity.sqrMagnitude * 0.3f);

        base.LateUpdate();
    }

    #endregion
}
