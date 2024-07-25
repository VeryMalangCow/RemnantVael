using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    [Header("=== Mouse")]
    [SerializeField] public Vector2 MousePos;
    [SerializeField] public Vector2 MousePosByWorld;
    [SerializeField] public Vector2 DirFromPlayerPos;

    [Header("=== Component")]
    [SerializeField] private PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        OnEnableInput();
    }

    private void OnDisable()
    {
        OnDisableInput();
    }

    private void FixedUpdate()
    {
        SetMousePos();
    }

    #endregion

    #region Mouse

    private void SetMousePos()
    {
        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);
        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerController.Instance.transform.position;
    }

    #endregion

    #region Input Set

    private void OnEnableInput()
    {
        PlayerInput.actions["Walk"].Enable();

        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Fire00"].performed += Input_Fire00;
        PlayerInput.actions["Fire01"].performed += Input_Fire01;
        PlayerInput.actions["Dash"].performed += Input_Dash;
    }

    private void OnDisableInput()
    {
        PlayerInput.actions["Walk"].Disable();

        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Fire00"].performed -= Input_Fire00;
        PlayerInput.actions["Fire01"].performed -= Input_Fire01;
        PlayerInput.actions["Dash"].performed -= Input_Dash;
    }


    #endregion

    #region Movement

    public void Input_Walk(InputAction.CallbackContext inputValue)
    {
        InputMoveDir = inputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext inputValue)
    {
        if(inputValue.ReadValueAsButton())
        {
            PlayerController.Instance.CanDashCheck();
        }
    }

    #endregion

    #region Fire

    private void Input_Fire00(InputAction.CallbackContext inputValue)
    {
        PlayerController.Instance.RightWeapon.IsInputed = inputValue.ReadValueAsButton();
    }
    private void Input_Fire01(InputAction.CallbackContext inputValue)
    {
        PlayerController.Instance.LeftWeapon.IsInputed = inputValue.ReadValueAsButton();
    }

    #endregion
}
