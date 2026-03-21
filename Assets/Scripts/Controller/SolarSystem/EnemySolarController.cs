using UnityEngine;
using UnityEngine.Serialization;

public class EnemySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Controller")]
    [FormerlySerializedAs("EnemyController")][SerializeField] protected EnemyController enemy;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        Set_RotSmooth(enemy.lookAtDir, Time.deltaTime);
    }

    #endregion
}
