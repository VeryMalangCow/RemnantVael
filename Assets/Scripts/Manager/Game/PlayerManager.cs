using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

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