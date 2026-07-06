using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, IMainGameInitializer
{
    #region Value

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;


    [Header("=== TF")]
    [SerializeField] private Transform playerSpawnParentTF;
    [SerializeField] private Transform playerPingFrameSpawnTF;

    [Header("=== Class")]
    [SerializeField] public CameraController cameraController;

    [Header("=== Ping")]
    [SerializeField] private PingController playerPingFramePrefab;
    [HideInInspector] private PingController playerPing;
    [HideInInspector] private EnemyController pingedEnemy;

    [Header("=== Theme (Temp)")]
    [SerializeField] private PlayerThemeSO[] playerThemes;
    public PlayerThemeSO targetPlayerTheme { get; private set; }
    public PlayerController playerController { get; private set; }
    public static int kindOfPlayerAmount = 1;


    #endregion

    #region Init

    public IEnumerator Initialize()
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif
        SetPlayerTheme(0);
        GenPlayer(out AimController aim, out AimRoundController aimRound);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"PlayerManager: <color=orange>Generate</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        playerPing = GenPlayerPing();
        SetOff_PingEnemy();

        InputManager.instance.aimController = aim;
        InputManager.instance.aimRoundController = aimRound;

        cameraController.Offset(playerController.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"PlayerManager: <color=orange>DataOffset</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

#endregion

    #region Offset



    #endregion

    private void SetPlayerTheme(int targetId)
    {
        targetPlayerTheme = playerThemes[targetId]; 
        SoundManager.instance.SetPlayerThemeDict(targetPlayerTheme);
    }

    #region Gen

    private PlayerController GenPlayer(out AimController aim, out AimRoundController aimRound)
    {
        playerController = Instantiate(targetPlayerTheme.player, playerSpawnParentTF);
        aim = Instantiate(playerController.aimPrefab, playerSpawnParentTF);
        aimRound = Instantiate(playerController.aimRoundPrefab, playerController.transform);
        
        return playerController;
    }

    private PingController GenPlayerPing()
    {
        return Instantiate(playerPingFramePrefab, playerPingFrameSpawnTF);
    }

    #endregion

    #region Is

    public bool Is_PingedEnemy(EnemyController enemy)
    {
        return pingedEnemy == enemy;
    }

    #endregion

    #region Set (Ping)

    public void SetOff_PingEnemy()
    {
        Set_PingedEnemy(null);
        playerPing.SetOff_Ping(playerPingFrameSpawnTF);
    }

    public void SetOn_PingEnemy(EnemyController enemy)
    {
        if (pingedEnemy == enemy) return;

        Set_PingedEnemy(enemy);
        playerPing.SetOn_Ping(enemy);
    }

    private void Set_PingedEnemy(EnemyController enemy)
    {
        pingedEnemy = enemy;
        AllyManager.instance.Set_AllAllyTargetEnemy(pingedEnemy);
    }

    #endregion

    #region Set (Ping Sort)

    public void Set_SortingOrderPing(EnemyController enemy, int order)
    {
        if (Is_PingedEnemy(enemy))
            playerPing.Set_SortingOrder(order);
    }

    #endregion

    #region Get

    public EnemyController Get_PingedEnemy()
    {
        return pingedEnemy;
    }

    #endregion
}