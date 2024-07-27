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

    #endregion
}


public enum eMovementState
{
    IdleOrWalk, Dash
}

public enum eDamageType
{
    Physics, Energy
}

public enum eEnemy
{
    Normal, Elite, Boss
}