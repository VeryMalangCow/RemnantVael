

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

    public override void PlayInteract()
    {
        base.PlayInteract();

        if (targetPrison == null || targetPrison.isOn) return;

        string debugString = "";
        switch (targetPrison)
        {
            case StrikeTeamPrisonController:
                MainGameUIManager.instance.boxLineConnectorUi.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.boxLineConnectorUi.SetOn_ThisPanel();
                break;
            case UplinkTeamPrisonController:
                MainGameUIManager.instance.numShapeColorPasswordUi.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.numShapeColorPasswordUi.SetOn_ThisPanel();
                break;
            case NeoTeamPrisonController:
                MainGameUIManager.instance.inOrderLockerUi.Offset_FirstValue(targetPrison);
                MainGameUIManager.instance.inOrderLockerUi.SetOn_ThisPanel();
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
