using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    #region - Inspector

    [Header("=== TF")]
    [SerializeField] private Transform playerSpawnParentTF;
    [SerializeField] private Transform playerPingFrameSpawnTF;

    [Header("=== Class")]
    [SerializeField] public CameraController cameraController;

    [Header("=== Target Enemy")]
    [SerializeField] private GameObject playerPingFramePrefab;

    #endregion

    #region - Hide

    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public static int kindOfPlayerAmount = 1;

    [HideInInspector] private PingController playerPing;
    [HideInInspector] private EnemyController pingedEnemy;

    [HideInInspector] private Dictionary<int, int> havingKeycardDict = new Dictionary<int, int>();

    #endregion

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Gen_Player(out AimController aim, out AimRoundController aimRound);

        playerPing = DevTool.Get_ComponentTType<PingController>(Gen_PlayerTargetEnemyGO());
        SetOff_PingEnemy();

        InputManager.instance.aimController = aim;
        InputManager.instance.aimRoundController = aimRound;

        cameraController.TargetTF = playerController.gameObject.transform;
        BaseUpgradeManager.instance.Offset(playerController);

        Offset_KeyCard();
    }

    #endregion

    #region Offset

    private void Offset_KeyCard()
    {
        havingKeycardDict = new Dictionary<int, int>();

        for (int i = 0; i < ResourceManager.instance.Get_KeycardAmount(); i++)
            havingKeycardDict.Add(i, 0);
    }

    #endregion

    #region Gen

    private PlayerController Gen_Player(out AimController _Aim, out AimRoundController _AimRound)
    {
        PlayerController pc = this.playerController =
            DevTool.Get_ComponentTType<PlayerController>(
                Instantiate(GameManager.instance.designatedPlayerPrefab, playerSpawnParentTF));
        _Aim = DevTool.Get_ComponentTType<AimController>(
            Instantiate(playerController.AimPrefab, playerSpawnParentTF));
        _AimRound = DevTool.Get_ComponentTType<AimRoundController>(
            Instantiate(playerController.AimRoundPrefab, playerController.transform));

        return pc;
    }

    private GameObject Gen_PlayerTargetEnemyGO()
    {
        return Instantiate(playerPingFramePrefab, playerPingFrameSpawnTF);
    }

    #endregion

    #region Is

    public bool Is_PingedEnemy(EnemyController _Enemy)
    {
        return pingedEnemy == _Enemy;
    }

    #endregion

    #region Set (Ping)

    public void SetOff_PingEnemy()
    {
        Set_PingedEnemy(null);
        playerPing.SetOff_Ping(playerPingFrameSpawnTF);
    }

    public void SetOn_PingEnemy(EnemyController _Enemy)
    {
        if (pingedEnemy == _Enemy) return;

        Set_PingedEnemy(_Enemy);
        playerPing.SetOn_Ping(_Enemy);
    }

    private void Set_PingedEnemy(EnemyController _Enemy)
    {
        pingedEnemy = _Enemy;
        AllyManager.instance.Set_AllAllyTargetEnemy(pingedEnemy);
    }

    #endregion

    #region Set (Ping Sort)

    public void Set_SortingOrderPing(EnemyController _Enemy, int _Order)
    {
        if (Is_PingedEnemy(_Enemy))
            playerPing.Set_SortingOrder(_Order);
    }

    #endregion

    #region Get

    public EnemyController Get_PingedEnemy()
    {
        return pingedEnemy;
    }

    #endregion

    #region KeyCard

    public void Gain_KeyCard(int _KeyCardID, int _Amount = 1)
    {
        if (havingKeycardDict.ContainsKey(_KeyCardID))
        {
            havingKeycardDict[_KeyCardID] += _Amount;
            MainGameUIManager.instance.playerHUD_UIController.Set_KeyItem(havingKeycardDict);
            MainGameUIManager.instance.playerHUD_UIController.Effect_KeyIcon(_KeyCardID);
        }
    }

    public void Use_KeyCard(int _KeyCardID, int _Amount = 1)
    {
        if (havingKeycardDict.ContainsKey(_KeyCardID))
        {
            havingKeycardDict[_KeyCardID] -= _Amount;
            MainGameUIManager.instance.playerHUD_UIController.Set_KeyItem(havingKeycardDict);
            SoundManager.instance.Play_2D_SFX_Build("UseKeycard");
        }
    }

    public bool Can_UseKeyCard(int _KeyCardID)
    {
        return havingKeycardDict.ContainsKey(_KeyCardID) && havingKeycardDict[_KeyCardID] > 0;
    }

    #endregion
}