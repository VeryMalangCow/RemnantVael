using UnityEngine;

public class RigidbodySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Rigidbody")]

    [Header("=== Component")]
    [SerializeField] public Rigidbody2D rb;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Set_RotSmooth(rb.velocity, Time.deltaTime);

        base.LateUpdate();
    }

    #endregion
}
