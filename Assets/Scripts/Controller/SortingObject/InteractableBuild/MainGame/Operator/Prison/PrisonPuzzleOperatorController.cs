

public class PrisonPuzzleOperatorController : PrisonOperatorController
{
    #region Offset

    protected override void Offset()
    {
        base.Offset();
        Set_Language();
    }

    #endregion

    #region Set

    public override void Set_TargetBuild(PrisonController targetPrison)
    {
        base.Set_TargetBuild(targetPrison);

        targetPrison.puzzleOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        //base.Get_InteractName();
        canInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(59);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        if (targetPrison == null || targetPrison.isOn) return;

        string debugString = "";
        switch (targetPrison)
        {
            case StrikeTeamPrisonController:
                MainGameUIManager.instance.boxLineConnector_UIController.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.boxLineConnector_UIController.SetOn_ThisPanel();
                break;
            case UplinkTeamPrisonController:
                MainGameUIManager.instance.numShapeColorPassword_UIController.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.numShapeColorPassword_UIController.SetOn_ThisPanel();
                break;
            case NeoTeamPrisonController:
                MainGameUIManager.instance.inOrderLocker_UIController.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.inOrderLocker_UIController.SetOn_ThisPanel();
                break;

            default:
                break;
        }

        debugString += "Puzzle";
    }

    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        payTxt.text = ResourceManager.instance.Get_StaticWord(59);
    }

    #endregion
}
