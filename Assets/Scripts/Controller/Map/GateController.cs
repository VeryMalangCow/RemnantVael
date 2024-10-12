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
    [HideInInspector] public GateController ParterGate = null;

    #endregion

    #region Interact

    public void Interact()
    {
        PlayerManager.Instance.PlayerController.gameObject.transform.position = ParterGate.gameObject.transform.position;
        StageManager.Instance.StartCurrentRoom(ParterGate.ThisRoom);
    }

    #endregion
}
