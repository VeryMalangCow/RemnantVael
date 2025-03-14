using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Rigidbody")]

    [Header("=== Component")]
    [SerializeField] public Rigidbody2D ThisRb;
    [SerializeField] private List<DirectionalAnimController> ThisAnimatorList;


    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Vector2 dir = ThisRb.velocity;

        Set_RotSmooth(dir.normalized, Time.deltaTime);
        DevTool.Set_AnimSpeed(ThisAnimatorList, dir.sqrMagnitude * 0.3f);

        base.LateUpdate();
    }

    #endregion
}
