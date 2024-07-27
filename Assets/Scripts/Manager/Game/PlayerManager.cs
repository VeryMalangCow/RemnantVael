using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== State")]
    [SerializeField] public PlayerLifeState LifeState;
    [SerializeField] public PlayerMovementState MovementState;
    [SerializeField] public PlayerUtilityState UtilityState;

    [Header("=== Const State")]
    [SerializeField] public const float fireMinDisLimit = 4;

    [Header("=== Prefab")]
    [SerializeField] private GameObject PlayerPrefab;
    [HideInInspector] public PlayerController PlayerController;
    [SerializeField] private Transform PlayerSpawnParentTF;


    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        GameObject SpawnedPlayerGO = Instantiate(PlayerPrefab, PlayerSpawnParentTF);
        if(SpawnedPlayerGO.TryGetComponent(out PlayerController PC))
        { 
            this.PlayerController = PC;
            LayerOrderManager.Instance.MovableObjects.Add(PlayerController);
        }
    }

    #endregion

}

[System.Serializable]
public class PlayerLifeState
{
    [SerializeField] public int HealthPoint = 3;
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
