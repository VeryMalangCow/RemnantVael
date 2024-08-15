using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== State")]
    [SerializeField] public PlayerLifeState LifeState;
    [SerializeField] public PlayerMovementState MovementState;

    [Header("=== Const State")]
    [SerializeField] public const float fireMinDisLimit = 4;

    [Header("=== Prefab")]
    [HideInInspector] public PlayerController PlayerController;
    [SerializeField] private Transform PlayerSpawnParentTF;


    #endregion

    #region Framework

    private void Start()
    {
        GameObject SpawnedPlayerGO = Instantiate(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        if (SpawnedPlayerGO.TryGetComponent(out PlayerController PC))
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
    [SerializeField] public float MaxEP = 100f;
    [SerializeField] public float NeedToMakeBC = 20f;
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