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

    #region Offset

    public override void Offset()
    {
        base.Offset();

        NeedKeyCardID = 0;
    }

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Elevator();
    }

    #endregion

    #region Elevator

    private void SetOn_Elevator()
    {
        if (InRoom_Elevator != null &&
            !InRoom_Elevator.IsOn)
        {
            InRoom_Elevator.IsOn = true;
        }
    }

    public void Set_ElevatorData(int _NextStageIndex)
    {
        InRoom_Elevator.Set_Data(_NextStageIndex, true);
    }

    public int Get_ElevatorData()
    {
        return InRoom_Elevator.Get_Data();
    }

    public bool IsOn_Elevator()
    {
        return InRoom_Elevator.IsOn;
    }

    #endregion

    #region Lobby

    public void Set_EntranceRuleInLobby()
    {
        NeedKeyCardID = -1;
    }

    #endregion
}
