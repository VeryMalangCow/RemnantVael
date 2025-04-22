using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumShapeColorPasswordUIController : PuzzleUIController
{
    #region Value



    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (Is_Interact_Click()) return;
    }

    #endregion

    #region Click

    private bool Is_Interact_Click()
    {
        if (true)
            return false;

        return true;
    }

    #endregion

    #region Success

    public override bool Can_Success()
    {
        return false;
    }

    #endregion
}
