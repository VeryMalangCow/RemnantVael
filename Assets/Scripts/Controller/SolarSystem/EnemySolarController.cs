using UnityEngine;

public class EnemySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Controller")]
    [SerializeField] protected EnemyController EnemyController;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        Set_RotSmooth(EnemyController.LookAtDir, Time.deltaTime);
    }

    #endregion
}
