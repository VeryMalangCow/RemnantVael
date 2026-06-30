using UnityEngine;

public class PassageRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Passage")]

    [Space(10)]
    [Header("=== Elevator")]
    [SerializeField] public StartingElevatorController startingElevator;
    [SerializeField] private EndingElevatorController endingElevator;

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
        if (endingElevator != null &&
            !endingElevator.isOn)
        {
            endingElevator.isOn = true;
        }
    }

    public void Set_ElevatorData(int nextStageIndex)
    {
        endingElevator.Set_Data(nextStageIndex, false);
    }

    #endregion
}
