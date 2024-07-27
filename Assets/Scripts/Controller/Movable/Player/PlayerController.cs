using UnityEngine;

public class PlayerController : MovableObject
{
    #region Value
    [Space(20)] [Header("<><><><><> Player")]

    [Header("=== Movement")]

    [Header("-- Dash")]
    [SerializeField] private int CurrentDashCharge = 0;
    [SerializeField] private float CurrentDashCooltime = 0;
    [SerializeField] private Vector2 DashTargetDir;

    [Header("=== Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;

    #endregion

    #region Framework

    private void FixedUpdate()
    {
        Movement();
    }

    #endregion

    #region Movement
    
    private void Movement()
    {
        DashCaculate();

        if (MovementState == eMovementState.Dash)
        { Dash(DashTargetDir, PlayerManager.Instance.MovementState.dashDur); }
        else
        { Walk(InputManager.Instance.InputMoveDir, PlayerManager.Instance.MovementState.walkSpeed, AccelerationSpeed); }
    }


    

    private void DashCaculate()
    {
        if (CurrentDashCharge >= PlayerManager.Instance.MovementState.maxDashCharge)
        { return; }

        if (CurrentDashCooltime >= PlayerManager.Instance.MovementState.dashCooltime)
        {
            CurrentDashCharge++;
            CurrentDashCooltime = 0f;
            Debug.Log("´ë½¬·® : " + CurrentDashCharge);
        }
        else
        {
            CurrentDashCooltime += Time.deltaTime;
        }
    }

    public void CanDashCheck()
    {
        if (MovementState == eMovementState.Dash || CurrentDashCharge <= 0)
        {
            return;
        }

        DashTargetDir = InputManager.Instance.DirFromPlayerPos.normalized;
        CurrentDashCharge--;
        MovementState = eMovementState.Dash;
    }

    #endregion
}



