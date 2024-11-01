using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [HideInInspector] public PlayerController PlayerController;

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;

    [Header("=== Aim")]
    [SerializeField] private GameObject AimPrefab;

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
        this.PlayerController = UnitManager.GenerateUnit<PlayerController>(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        
        LayerOrderManager.Instance.NeedLayerObjects.Add(PlayerController);
        CameraController.TargetTF = PlayerController.gameObject.transform;
        BaseUpgradeManager.Instance.Offset(PlayerController);

        GameObject spawnedAimGO = Instantiate(AimPrefab);
        if (spawnedAimGO != null && spawnedAimGO.TryGetComponent(out HaveShadowThingStatic aim)) 
        {
            InputManager.Instance.Aim = aim;
        }
    }


    #endregion

}