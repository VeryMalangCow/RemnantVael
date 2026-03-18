using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Set_TargetBuild(PrisonController _TargetPrison)
    {
        base.Set_TargetBuild(_TargetPrison);

        _TargetPrison.PuzzleOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        //base.Get_InteractName();
        _CanInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(59);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        if (TargetPrison == null || TargetPrison.IsOn) return;

        string debugString = "";
        switch (TargetPrison)
        {
            case StrikeTeamPrisonController:
                MainGameUIManager.instance.boxLineConnector_UIController.Offset_FirstValue(TargetPrison);
                MainGameUIManager.instance.boxLineConnector_UIController.SetOn_ThisPanel();
                break;
            case UplinkTeamPrisonController:
                MainGameUIManager.instance.numShapeColorPassword_UIController.Offset_FirstValue(TargetPrison);
                MainGameUIManager.instance.numShapeColorPassword_UIController.SetOn_ThisPanel();
                break;
            case NeoTeamPrisonController:
                MainGameUIManager.instance.inOrderLocker_UIController.Offset_FirstValue(TargetPrison);
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
        PayTxt.text = ResourceManager.instance.Get_StaticWord(59);
    }

    #endregion
}
