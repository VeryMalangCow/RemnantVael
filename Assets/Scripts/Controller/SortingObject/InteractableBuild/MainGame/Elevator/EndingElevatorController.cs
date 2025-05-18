using UnityEngine;

public class EndingElevatorController : ElevatorController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Entrance")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int NextStageIndex;

    #endregion

    #region Set

    public void Set_Data(int _NextStageIndex)
    {
        NextStageIndex = _NextStageIndex;
    }

    #endregion

    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();
        
        // Stage
        PlayerManager.Instance.PlayerController.Set_EndStage();

        // Input
        EventManager.Instance.Set_Input(false);

        // Screen
        MainGameUIManager.Instance.Play_FadeIn(3f);
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
