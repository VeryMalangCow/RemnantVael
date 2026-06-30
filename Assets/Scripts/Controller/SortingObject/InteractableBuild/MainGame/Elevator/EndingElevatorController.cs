using System;
using UnityEngine;

public class EndingElevatorController : ElevatorController, IInteract
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Entrance")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int nextStageIndex;

    [SerializeField] private bool isLobbyElevator = false;
    [SerializeField] private bool forPassageElevator = true;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Data

    public void Set_Data(int nextStageIndex, bool forPassage)
    {
        this.nextStageIndex = nextStageIndex;

        if (StageManager.instance.targetStageId == 99)
            isLobbyElevator = true;

        forPassageElevator = forPassage;
    }

    public int Get_Data()
    {
        return nextStageIndex;
    }

    #endregion

    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();
        
        // Stage
        PlayerManager.instance.playerController.Set_EndStage();

        // Input
        EventManager.instance.Set_Input(false);

        // Ally
        AllyManager.instance.Stop_AllAllies_Combat();
        AllyManager.instance.Set_AllAlliesActive(false);

        // Screen
        MainGameUIManager.instance.Play_FadeIn(3f);
        MainGameUIManager.instance.Play_OnLoadingIcon(3f);
        EventManager.instance.Set_BlackUpDownCover(true);

        thisSr.sortingOrder = 3000;

        // Intetactable Anno Panel
        MainGameUIManager.instance.interactAnnoUi.Set_VisualCG(false);

        // Sound
        SoundManager.instance.Set_MasterVolume(1f, 0f, 2.5f);
    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        // 로비 엘레베이터면 바로 맵 생성 OR
        // 통로 맵으로 가는 엘레베이터가 아니면 맵 생성
        if (isLobbyElevator == true || !forPassageElevator) 
        {
            StageManager.instance.GenerateStage(nextStageIndex);
        }
        else // 둘 모두 아니면 통로 맵 생성
        {
            StageManager.instance.GeneratePassageStage(nextStageIndex);
        }

        SaveDataManager.instance.Save_JsonData();
        MainGameUIManager.instance.Play_SaveData();
    }

    #endregion

    #region Play


    #endregion

    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = isOn;
        return ResourceManager.instance.Get_StaticWord(3);
    }

    public void PlayInteract()
    {
        if (isOn)
        {
            PlayerManager.instance.playerController.SetInteractable(null);
            Play_MoveToTarget();
        }
    }

    #endregion
}
