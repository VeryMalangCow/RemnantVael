using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

    private void Update()
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

        PlayerInput.actions["Walk"].performed += InputWalk;
    }

    private void OnDisableInput()
    {
        PlayerInput.actions["Walk"].Disable();

        PlayerInput.actions["Walk"].performed -= InputWalk;
    }


    #endregion

    #region Movement

    public void InputWalk(InputAction.CallbackContext inputValue)
    {
        InputMoveDir = inputValue.ReadValue<Vector2>().normalized;
    }

    #endregion
}
