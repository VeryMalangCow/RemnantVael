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
    [SerializeField] public float MaxEP = 100;
    [SerializeField] public float NeedToMakeBC = 20;
}

[System.Serializable]
public class PlayerMovementState
{
    [SerializeField] public float WalkSpeed = 3f;

    [SerializeField] public float DashCooltime = 3f;
    [SerializeField] public int MaxDashCharge = 2;
    [SerializeField] public float DashSpeed = 7f;
    [SerializeField] public float DashDur = 0.2f;
}

[System.Serializable]
public class PlayerUtilityState
{
    [SerializeField] public float MaxCastingTime = 0.85f;
}
