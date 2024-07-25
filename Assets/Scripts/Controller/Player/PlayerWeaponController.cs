using UnityEngine;

public class PlayerWeaponController : WeaponController
{
    #region Value

    [Header("=== Input")]
    [SerializeField] public bool IsInputed = false;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        if (IsInputed && currentDelay >= 1)
        {
            Fire();
        }
    }

    #endregion
}
