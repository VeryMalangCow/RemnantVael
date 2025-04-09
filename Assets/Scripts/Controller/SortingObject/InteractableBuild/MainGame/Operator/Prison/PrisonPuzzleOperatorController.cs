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

        Debug.Log("Puzzle");
    }

    #endregion
}
