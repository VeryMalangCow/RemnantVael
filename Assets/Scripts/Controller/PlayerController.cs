using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    #region Value

    [Header("=== State")]
    [SerializeField] private PlayerState PlayerState;

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    [Header("=== Movement")]
    [SerializeField] private Vector2 ActualMoveDir;
    [SerializeField] private float AccelerationSpeed = 12;
    [SerializeField] private float CurrentSpeed = 0;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }


    private void Update()
    {
        Walk();
    }

    #endregion


    #region Movement
    

    private void Walk()
    {
        SetActualWalkDir();
        SetCurrentWalkSpeed();

        ThisRb.velocity = ActualMoveDir * CurrentSpeed * Time.deltaTime * 1000f;
    }

    private void SetActualWalkDir()
    {
        if (CurrentSpeed == 0)
        {
            ActualMoveDir = Vector2.zero;
        }
        else
        {
            ActualMoveDir = Vector2.Lerp(ActualMoveDir, InputManager.Instance.InputMoveDir, AccelerationSpeed * Time.deltaTime);
        }
    }

    private void SetCurrentWalkSpeed()
    {
        if (InputManager.Instance.InputMoveDir == Vector2.zero && CurrentSpeed > 0)
        {
            CurrentSpeed -= PlayerState.WalkSpeed * (AccelerationSpeed * Time.deltaTime);
            if (CurrentSpeed < 0) { CurrentSpeed = 0; }
        }
        else if (InputManager.Instance.InputMoveDir != Vector2.zero && CurrentSpeed < PlayerState.WalkSpeed)
        {
            CurrentSpeed += PlayerState.WalkSpeed * (AccelerationSpeed * Time.deltaTime);
            if (CurrentSpeed > PlayerState.WalkSpeed) { CurrentSpeed = PlayerState.WalkSpeed; }
        }
    }

    
    #endregion
}

[System.Serializable]
public class PlayerState
{
    [SerializeField] public float WalkSpeed = 1f;    
}

