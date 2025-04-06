using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSetSpriteController : BuildSpriteController
{
    #region Set

    private void Add_CurrentSetSprite()
    {
        StageManager.Instance.Add_SetSprite(this);
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        Add_CurrentSetSprite();
    }

    #endregion
}
