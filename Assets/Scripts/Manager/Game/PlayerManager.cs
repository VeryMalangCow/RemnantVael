
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== State")]
    [SerializeField] public PlayerMovementState MovementState;
    [SerializeField] public PlayerUtilityState UtilityState;
    #endregion

}

[System.Serializable]
public class PlayerMovementState
{
    [SerializeField] public float walkSpeed = 3f;

    [SerializeField] public float dashCooltime = 3f;
    [SerializeField] public int maxDashCharge = 2;
    [SerializeField] public float dashSpeed = 7f;
    [SerializeField] public float dashDur = 0.2f;
}

[System.Serializable]
public class PlayerUtilityState
{
    [SerializeField] public float mechanicalDebrisSpawnProbability = 0.1f;
    [SerializeField] public float EnergySpawnProbability = 0.1f;
}

