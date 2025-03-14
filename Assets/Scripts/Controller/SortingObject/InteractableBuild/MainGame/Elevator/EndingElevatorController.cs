
public class EndingElevatorController : ElevatorController, IInteract
{
    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();
        
        // Stage
        PlayerManager.Instance.PlayerController.Set_EndStage();

        // Input
        EventManager.Instance.Set_Input(false);

        // Screen
        MainGameUIManager.Instance.Play_Dark(3f);
        ThisSR.sortingOrder = 3000;

    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

    }

    #endregion

    #region Interact

    public void Play_Interact()
    {
        if (IsOn)
        {
            PlayerManager.Instance.PlayerController.CurrentInteractable.Value = null;
            Play_MoveToTarget();
        }
    }

    #endregion
}
