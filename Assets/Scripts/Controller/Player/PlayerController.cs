using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    #region Value

    [Header("=== State")]
    [SerializeField] private PlayerState PlayerState;

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    [Header("=== Movement")]
    [SerializeField] private float AccelerationSpeed = 12;

    [Header("=== Weapon")]
    [SerializeField] public WeaponController RightWeapon;
    [SerializeField] public WeaponController LeftWeapon;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        Walk();
    }

    #endregion

    #region Movement
    
    private void Walk()
    {
        Vector2 moveVelocity = InputManager.Instance.InputMoveDir * PlayerState.WalkSpeed;
        Vector2 currentVelocity = ThisRb.velocity;

        moveVelocity = Vector2.Lerp(currentVelocity, moveVelocity, AccelerationSpeed * Time.deltaTime);
        ThisRb.velocity = moveVelocity;
    }

    #endregion
}

[System.Serializable]
public class PlayerState
{
    [SerializeField] public float WalkSpeed = 1f;    
}

