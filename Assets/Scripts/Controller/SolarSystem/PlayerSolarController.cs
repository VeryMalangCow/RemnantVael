using UnityEngine;

public class PlayerSolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Controller")]
    [SerializeField] protected PlayerController PlayerController;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        Set_RotSmooth(InputManager.Instance.DirFromPlayerPos.normalized, Time.deltaTime);
    }

    #endregion
}
