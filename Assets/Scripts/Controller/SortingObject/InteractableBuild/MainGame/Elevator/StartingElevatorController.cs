using System.Collections;
using UnityEngine;

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
        MainGameUIManager.Instance.Play_OffLoadingIcon(3f);

        // Sound
        SoundManager.Instance.Set_MasterVolume(0f, 1f, 2.5f);
    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        // Stage
        PlayerManager.Instance.PlayerController.Set_StartStage();

        // Input
        EventManager.Instance.Set_Input(true);

        // Ally Pos
        AllyManager.Instance.Set_AllAllyPlayerNearPos();
        AllyManager.Instance.Set_AllAlliesActive(true);
        AllyManager.Instance.Start_AllAllies_Combat();

        // Screen
        EventManager.Instance.Set_BlackUpDownCover(false);

        // Intetactable Anno Panel
        MainGameUIManager.Instance.InteractAnno_UIController.Set_VisualCG(true);

        if (StageManager.Instance.TargetStageID == 1)
        {
            MainGameUIManager.Instance.Play_EndGameProd();
        }
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
