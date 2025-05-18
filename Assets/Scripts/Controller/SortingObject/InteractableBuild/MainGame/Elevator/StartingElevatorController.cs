
public class StartingElevatorController : ElevatorController
{
    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();

        // Stage
        EventManager.Instance.Set_BlackUpDownCover(true);
        PlayerManager.Instance.PlayerController.Set_PastStartStage();

        // Screen
        MainGameUIManager.Instance.Play_FadeOut(3f);
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

        // Intetactable Anno Panel
        MainGameUIManager.Instance.InteractAnno_UIController.Set_VisualCG(true);
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Play_MoveToTarget();
    }

    #endregion
}
