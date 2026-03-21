using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Controller")]
    [FormerlySerializedAs("PlayerController")][SerializeField] protected PlayerController player;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        Set_RotSmooth(InputManager.instance.dirFromPlayerPos, Time.deltaTime);
    }

    #endregion
}
