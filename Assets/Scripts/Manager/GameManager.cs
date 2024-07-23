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
        Offset();
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        InputManager.Instance.Offset();
        ObjectPoolingManager.Instance.Offset();

        PlayerController.Instance.Offset();
        CameraController.Instance.Offset();
    }

    #endregion

    #region Option

    private void SetBaseOption()
    {
        Application.targetFrameRate = 144;
    }

    #endregion
}
