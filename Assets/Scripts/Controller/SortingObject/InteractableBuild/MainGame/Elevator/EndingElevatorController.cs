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
    [SerializeField] private int NextStageIndex;

    [SerializeField] private bool Is_LobbyElevator = false;
    [SerializeField] private bool ForPassageElevator = true;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Data

    public void Set_Data(int _NextStageIndex, bool _ForPassage)
    {
        NextStageIndex = _NextStageIndex;

        if (StageManager.Instance.targetStageID == 99)
            Is_LobbyElevator = true;

        ForPassageElevator = _ForPassage;
    }

    public int Get_Data()
    {
        return NextStageIndex;
    }

    #endregion

    #region Tween

    protected override void Tween_Start()
    {
        base.Tween_Start();
        
        // Stage
        PlayerManager.Instance.playerController.Set_EndStage();

        // Input
        EventManager.Instance.Set_Input(false);

        // Ally
        AllyManager.Instance.Stop_AllAllies_Combat();
        AllyManager.Instance.Set_AllAlliesActive(false);

        // Screen
        MainGameUIManager.Instance.Play_FadeIn(3f);
        MainGameUIManager.Instance.Play_OnLoadingIcon(3f);
        EventManager.Instance.Set_BlackUpDownCover(true);

        ThisSR.sortingOrder = 3000;

        // Intetactable Anno Panel
        MainGameUIManager.Instance.interactAnno_UIController.Set_VisualCG(false);

        // Sound
        SoundManager.Instance.Set_MasterVolume(1f, 0f, 2.5f);
    }

    protected override void Tween_Complete()
    {
        base.Tween_Complete();

        // 로비 엘레베이터면 바로 맵 생성 OR
        // 통로 맵으로 가는 엘레베이터가 아니면 맵 생성
        if (Is_LobbyElevator == true || !ForPassageElevator) 
        {
            StageManager.Instance.Play_GenStage(NextStageIndex);
        }
        else // 둘 모두 아니면 통로 맵 생성
        {
            StageManager.Instance.Play_GenPassageStage(NextStageIndex);
        }

        SaveDataManager.Instance.Save_JsonData();
        MainGameUIManager.Instance.Play_SaveData();

        Resources.UnloadUnusedAssets();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    #endregion

    #region Play


    #endregion

    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = IsOn;
        return ResourceManager.Instance.Get_StaticWord(3);
    }

    public void Play_Interact()
    {
        if (IsOn)
        {
            PlayerManager.Instance.playerController.CurrentInteractable.Value = null;
            Play_MoveToTarget();
        }
    }

    #endregion
}
