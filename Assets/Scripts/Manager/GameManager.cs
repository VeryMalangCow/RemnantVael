using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        if(GameManager.Instance == this)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        

        SetBaseOption();
    }

    #endregion

    #region Option

    private void SetBaseOption()
    {
        Application.targetFrameRate = 144;
    }

    #endregion
}
