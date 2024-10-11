using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [HideInInspector] public PlayerController PlayerController;

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;
    

    [Header("=== Class")]
    [SerializeField] public CameraController CameraController;
    [SerializeField] public InputManager InputManager;

    #endregion

    #region Framework
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        this.PlayerController = UnitManager.GenerateUnit<PlayerController>(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        LayerOrderManager.Instance.MovableObjects.Add(PlayerController);
        CameraController.TargetTF = PlayerController.gameObject.transform;
        BaseUpgradeManager.Instance.Offset(PlayerController);
        InputManager.OnEnableInput();
    }


    #endregion

}