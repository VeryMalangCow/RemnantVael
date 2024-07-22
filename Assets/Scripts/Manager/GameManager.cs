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

        //Don't Destroy On Load, When Only Once.
        GameManager[] obj = FindObjectsOfType<GameManager>();
        if (obj.Length == 1)
        { DontDestroyOnLoad(gameObject); }
        else
        { Destroy(gameObject); }
    }

    #endregion
}
