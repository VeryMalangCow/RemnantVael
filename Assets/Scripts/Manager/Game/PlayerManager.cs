using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;
    [SerializeField] private Transform PlayerPingFrameSpawnTF;

    [Header("=== Class")]
    [SerializeField] public CameraController CameraController;

    [Header("=== Target Enemy")]
    [SerializeField] private GameObject PlayerPingFramePrefab;

    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public static int KindOfPlayerAmount = 1;

    [HideInInspector] private PingController PlayerPing;
    [HideInInspector] private EnemyController PingedEnemy;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Gen_Player(out AimController aim, out AimRoundController aimRound);

        PlayerPing = DevTool.Get_ComponentTType<PingController>(Gen_PlayerTargetEnemyGO());
        SetOff_PingEnemy();

        InputManager.Instance.AimController = aim;
        InputManager.Instance.AimRoundController = aimRound;

        CameraController.TargetTF = PlayerController.gameObject.transform;
        BaseUpgradeManager.Instance.Offset(PlayerController);
    }

    #endregion

    #region Gen

    private PlayerController Gen_Player(out AimController _Aim, out AimRoundController _AimRound)
    {
        PlayerController pc = this.PlayerController =
            DevTool.Get_ComponentTType<PlayerController>(
                Instantiate(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF));
        _Aim = DevTool.Get_ComponentTType<AimController>(
            Instantiate(PlayerController.AimPrefab, PlayerSpawnParentTF));
        _AimRound = DevTool.Get_ComponentTType<AimRoundController>(
            Instantiate(PlayerController.AimRoundPrefab, PlayerController.transform));

        return pc;
    }

    private GameObject Gen_PlayerTargetEnemyGO()
    {
        return Instantiate(PlayerPingFramePrefab, PlayerPingFrameSpawnTF);
    }

    #endregion

    #region Is

    public bool Is_PingedEnemy(EnemyController _Enemy)
    {
        return PingedEnemy == _Enemy;
    }

    #endregion

    #region Set (Ping)

    public void SetOff_PingEnemy()
    {
        Set_PingedEnemy(null);
        PlayerPing.SetOff_Ping(PlayerPingFrameSpawnTF);
    }

    public void SetOn_PingEnemy(EnemyController _Enemy)
    {
        if (PingedEnemy == _Enemy) return;

        Set_PingedEnemy(_Enemy);
        PlayerPing.SetOn_Ping(_Enemy);
    }

    private void Set_PingedEnemy(EnemyController _Enemy)
    {
        PingedEnemy = _Enemy;
        AllyManager.Instance.Set_AllAllyTargetEnemy(PingedEnemy);
    }

    #endregion

    #region Set (Ping Sort)

    public void Set_SortingOrderPing(EnemyController _Enemy, int _Order)
    {
        if (Is_PingedEnemy(_Enemy))
            PlayerPing.Set_SortingOrder(_Order);
    }

    #endregion

    #region Get

    public EnemyController Get_PingedEnemy()
    {
        return PingedEnemy;
    }

    #endregion
}