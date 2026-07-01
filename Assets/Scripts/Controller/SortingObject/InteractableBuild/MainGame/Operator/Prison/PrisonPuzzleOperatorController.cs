

public class PrisonPuzzleOperatorController : PrisonOperatorController
{
    #region Offset

    protected override void Offset()
    {
        base.Offset();
        Set_Language();
    }
    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        var prefab = StaticResourceManager.instance.BuildPrefab.prisonPuzzleOperPrefab; 

        iconStateAnim.Set_Anim(new State_Anim(prefab.animation, 1f), 1f);
        iconStateAnim.sr.material = prefab.panelMaterial;

        annoIcon.sprite = prefab.annoIcon;
        annoIcon.material = prefab.annoMaterial;
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
                MainGameUIManager.instance.puzzleBlcUi.SetPrison(targetPrison);
                MainGameUIManager.instance.puzzleBlcUi.SetOnThisPanel();
                break;
            case UplinkTeamPrisonController:
                MainGameUIManager.instance.puzzleNscUi.SetPrison(targetPrison);
                MainGameUIManager.instance.puzzleNscUi.SetOnThisPanel();
                break;
            case NeoTeamPrisonController:
                MainGameUIManager.instance.puzzleIolUi.SetPrison(targetPrison);
                MainGameUIManager.instance.puzzleIolUi.SetOnThisPanel();
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
