using UnityEngine;

public class GateController : BuildingController_OnlyPlayerLayer, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Gate Controller")]

    [Space(10)]
    [Header("=== Room State")]
    [SerializeField] public Vector2Int RoomPosGate;
    [SerializeField] public Vector2Int GateDir;

    // Data
    [HideInInspector] public RoomController ThisRoom;
    [HideInInspector] public bool HadParter = false;
    [HideInInspector] public bool SettedPos = false;
    [SerializeField] public GateController ParterGate = null;

    #endregion

    #region Interact

    public void Interact()
    {
        PlayerManager.Instance.PlayerController.gameObject.transform.position = ParterGate.gameObject.transform.position 
            + new Vector3(GateDir.x * 0.5f, GateDir.y * 0.5f, 0);
        StageManager.Instance.StartCurrentRoom(ParterGate.ThisRoom);
    }

    #endregion
}
