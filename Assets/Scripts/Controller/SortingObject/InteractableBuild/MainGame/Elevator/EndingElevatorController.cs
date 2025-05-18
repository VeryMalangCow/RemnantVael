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
        EventManager.Instance.Set_BlackUpDownCover(true);

        ThisSR.sortingOrder = 3000;

        // Intetactable Anno Panel
        MainGameUIManager.Instance.InteractAnno_UIController.Set_VisualCG(false);
    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        StageManager.Instance.Gen_Stage(NextStageIndex);
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
