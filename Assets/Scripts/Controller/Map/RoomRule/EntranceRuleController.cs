using UnityEngine;

public class EntranceRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Entrance")]

    [Space(10)]
    [Header("=== Elevator")]
    [SerializeField] private EndingElevatorController inRoom_Elevator;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        needKeyCardId = 0;
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
        if (inRoom_Elevator != null &&
            !inRoom_Elevator.isOn)
        {
            inRoom_Elevator.isOn = true;
        }
    }

    public void Set_ElevatorData(int nextStageIndex)
    {
        inRoom_Elevator.Set_Data(nextStageIndex, true);
    }

    public int Get_ElevatorData()
    {
        return inRoom_Elevator.Get_Data();
    }

    public bool IsOn_Elevator()
    {
        return inRoom_Elevator.isOn;
    }

    #endregion

    #region Lobby

    public void Set_EntranceRuleInLobby()
    {
        needKeyCardId = -1;
    }

    #endregion
}
