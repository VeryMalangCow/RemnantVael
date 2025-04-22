using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonPuzzleOperatorController : PrisonOperatorController
{
    #region Offset

    protected override void Offset()
    {
        base.Offset();

        PayTxt.text = CSVManager.Instance.Get_StaticWord(59);
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

    public override void Play_Interact()
    {
        if (TargetPrison == null) return;

        string debugString = "";
        switch (TargetPrison)
        {
            case StrikeTeamPrisonController:
                MainGameUIManager.Instance.BoxLineConnector_UIController.Offset_FirstValue(TargetPrison);
                MainGameUIManager.Instance.BoxLineConnector_UIController.SetOn_ThisPanel();
                break;
            case UplinkTeamPrisonController:
                break;
            case NeoTeamPrisonController:
                break;

            default:
                break;
        }

        debugString += "Puzzle";
    }

    #endregion
}
