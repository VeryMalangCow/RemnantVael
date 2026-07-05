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

    public override void Complete()
    {
        base.Complete();

        SetOnElevator();
    }

    #endregion

    #region Boss



    #endregion

    #region Elevator

    private void SetOnElevator()
    {
        if (inRoom_Elevator != null &&
            !inRoom_Elevator.isOn)
        {
            inRoom_Elevator.isOn = true;
        }
    }

    public void SetElevatorData(int nextStageIndex)
    {
        inRoom_Elevator.Set_Data(nextStageIndex, true);
    }

    public int GetElevatorData()
    {
        return inRoom_Elevator.Get_Data();
    }

    public bool IsOnElevator()
    {
        return inRoom_Elevator.isOn;
    }

    #endregion

    #region Lobby

    public void SetEntranceRuleInLobby()
    {
        needKeyCardId = -1;
    }

    #endregion
}
