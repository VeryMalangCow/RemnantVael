using UnityEngine;

public class GateController : HaveShadowThingStatic, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Gate Controller")]

    [SerializeField] public GameObject ExtraTargetObject;

    [Space(10)]
    [Header("=== Room State")]
    [SerializeField] public Vector2Int RoomPosGate;
    [SerializeField] public Vector2Int GateDir;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public bool IsOpen = false;
    [HideInInspector] public RoomController ThisRoom;
    [HideInInspector] public bool HadParter = false;
    [HideInInspector] public bool SettedPos = false;
    [HideInInspector] public GateController ParterGate = null;


    [Space(10)]
    [Header("=== Other")]
    [SerializeField] public GameObject OnThingsGO;
    [SerializeField] public GameObject OffThingsGO;


    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        ExtraTargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion

    #region On Off

    public void IsExistDoor(bool _IsExist)
    {
        if (_IsExist)
        {
            OnThingsGO.gameObject.SetActive(true);
            OffThingsGO.gameObject.SetActive(false);
        }
        else
        {
            OnThingsGO.gameObject.SetActive(false);
            OffThingsGO.gameObject.SetActive(true);
        }
    }

    public void SetOnOff(bool _IsOn)
    {
        IsOpen = _IsOn;

        if (IsOpen)
        {

        }
        else
        {

        }
    }

    #endregion

    #region Interact

    public void Interact()
    {
        if (IsOpen && ParterGate != null)
        {
            PlayerManager.Instance.PlayerController.gameObject.transform.position = ParterGate.gameObject.transform.position
            + new Vector3(GateDir.x * 0.5f, GateDir.y * 0.5f, 0);
            StageManager.Instance.StartCurrentRoom(ParterGate.ThisRoom);
        }
    }

    #endregion
}
