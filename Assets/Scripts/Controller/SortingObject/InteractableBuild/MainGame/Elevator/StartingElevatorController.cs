
public class StartingElevatorController : ElevatorController
{
    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();

        // Stage
        PlayerManager.Instance.PlayerController.Set_PastStartStage();
    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        // Stage
        PlayerManager.Instance.PlayerController.Set_StartStage();

        // Input
        EventManager.Instance.Set_Input(true);

        // Screen
        EventManager.Instance.Set_BlackUpDownCover(false);
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        EventManager.Instance.Set_BlackUpDownCover(true);
        Play_MoveToTarget();
    }

    #endregion
}
