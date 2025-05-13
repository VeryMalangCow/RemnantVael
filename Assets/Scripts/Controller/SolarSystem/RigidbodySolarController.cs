using UnityEngine;

public class RigidbodySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Rigidbody")]

    [Header("=== Component")]
    [SerializeField] public Rigidbody2D ThisRb;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Set_RotSmooth(ThisRb.velocity, Time.deltaTime);

        base.LateUpdate();
    }

    #endregion
}
