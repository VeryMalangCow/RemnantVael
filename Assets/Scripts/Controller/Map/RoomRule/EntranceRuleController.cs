using UnityEngine;

public class EntranceRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Entrance")]

    [Space(10)]
    [Header("=== Elevator")]
    [SerializeField] private EndingElevatorController InRoom_Elevator;

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Elevator();
    }

    private void SetOn_Elevator()
    {
        if (InRoom_Elevator != null &&
            !InRoom_Elevator.IsOn)
        {
            InRoom_Elevator.IsOn = true;
        }
    }

    #endregion
}
