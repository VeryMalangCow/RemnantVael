using UnityEngine;

public class PassageRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Passage")]

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
        inRoom_Elevator.Set_Data(nextStageIndex, false);
    }

    #endregion
}
