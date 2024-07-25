using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    [Header("=== Movement")]
    [Header("-- Walk")]
    [SerializeField] private float accelerationSpeed = 12;

    [Header("-- Dash")]
    [SerializeField] public bool isDashing = false;
    [SerializeField] private int currentDashCharge = 0;
    [SerializeField] private float currentDashCooltime = 0;
    [SerializeField] private float currentDashProcess = 0;
    [SerializeField] private Vector2 dashTargetDir;

    [Header("=== Weapon")]
    [SerializeField] public PlayerWeaponController RightWeapon;
    [SerializeField] public PlayerWeaponController LeftWeapon;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    #endregion

    #region Movement
    
    private void Movement()
    {
        DashCaculate();

        if (isDashing)
        { Dash(); }
        else
        { Walk(); }
    }

    private void Walk()
    {
        Vector2 moveVelocity = InputManager.Instance.InputMoveDir * PlayerManager.Instance.MovementState.walkSpeed;
        Vector2 currentVelocity = ThisRb.velocity;

        moveVelocity = Vector2.Lerp(currentVelocity, moveVelocity, accelerationSpeed * Time.deltaTime);
        ThisRb.velocity = moveVelocity;
    }

    private void Dash()
    {
        if (currentDashProcess < PlayerManager.Instance.MovementState.dashDur)
        {
            currentDashProcess += Time.deltaTime;
            ThisRb.velocity = dashTargetDir * PlayerManager.Instance.MovementState.dashSpeed;
        }
        else
        {
            currentDashProcess = 0;
            isDashing = false;
        }
    }

    private void DashCaculate()
    {
        if (currentDashCharge >= PlayerManager.Instance.MovementState.maxDashCharge)
        { return; }

        if (currentDashCooltime >= PlayerManager.Instance.MovementState.dashCooltime)
        {
            currentDashCharge++;
            currentDashCooltime = 0f;
            Debug.Log("´ë½¬·® : " + currentDashCharge);
        }
        else
        {
            currentDashCooltime += Time.deltaTime;
        }
    }

    public void CanDashCheck()
    {
        if (isDashing || currentDashCharge <= 0)
        {
            return;
        }

        dashTargetDir = InputManager.Instance.DirFromPlayerPos.normalized;
        currentDashCharge--;
        isDashing = true;
    }

    #endregion
}



