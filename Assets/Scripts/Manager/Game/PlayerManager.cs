using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [HideInInspector] public PlayerController PlayerController;

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;

    [Header("=== Class")]
    [SerializeField] public CameraController CameraController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        this.PlayerController = UnitManager.Gen_Unit<PlayerController>(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        
        LayerOrderManager.Instance.NeedLayerObjects.Add(PlayerController);
        CameraController.TargetTF = PlayerController.gameObject.transform;
        BaseUpgradeManager.Instance.Offset(PlayerController);

        GameObject spawnedAimGO = Instantiate(PlayerController.AimPrefab, PlayerSpawnParentTF);
        if (spawnedAimGO != null && spawnedAimGO.TryGetComponent(out AimController aim)) 
        {
            InputManager.Instance.AimController = aim;
        }
    }


    #endregion

}