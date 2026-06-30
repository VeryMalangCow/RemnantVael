

public class StartingElevatorController : ElevatorController
{
    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();

        // Stage
        EventManager.instance.Set_BlackUpDownCover(true);
        PlayerManager.instance.playerController.Set_PastStartStage();

        // Screen
        MainGameUIManager.instance.Play_FadeOut(3f);
        MainGameUIManager.instance.Play_OffLoadingIcon(3f);

        // Sound
        SoundManager.instance.Set_MasterVolume(0f, 1f, 2.5f);

    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        // Stage
        PlayerManager.instance.playerController.Set_StartStage();

        // Input
        EventManager.instance.Set_Input(true);

        // Ally Pos
        AllyManager.instance.Set_AllAllyPlayerNearPos();
        AllyManager.instance.Set_AllAlliesActive(true);
        AllyManager.instance.Start_AllAllies_Combat();

        // Screen
        EventManager.instance.Set_BlackUpDownCover(false);

        // Intetactable Anno Panel
        MainGameUIManager.instance.interactAnnoUi.Set_VisualCG(true);

/*      
        if (StageManager.instance.targetStageId == 1)
        {
            MainGameUIManager.instance.Play_EndGameProd();
        }
*/
    }

    #endregion

}
