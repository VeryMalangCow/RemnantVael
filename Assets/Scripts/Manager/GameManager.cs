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

    #region Module

    // Get Type if it Can Cast
    public static T CastIfPossible<T>(object input) where T : class
    {
        if (input is T variable)
        {
            return variable;
        }
        else
        {
            return null;
        }
    }

    // Set List by Component
    public static List<T> SetList<T>(Transform _Parent)
    {
        List<T> result = new List<T>();
        foreach (Transform TF in _Parent)
        {
            if (TF.TryGetComponent(out T type))
            {
                result.Add(type);
            }
        }
        return result;
    }

    #endregion
}

public enum eCombatMode
{ 
    Physics, Energy, Boost
}

public enum eMovementState
{
    Stop, IdleOrWalk, Dash
}

public enum eDamageType
{
    Physics, Energy
}

public enum eEnemy
{
    Normal, Elite, Boss
}