using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;

    [Header("=== Class")]
    [SerializeField] public CameraController CameraController;

    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public static int KindOfPlayerAmount = 1;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Gen_Player(out AimController aim, out AimRoundController aimRound);

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

    #endregion
}