using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    [Header("=== State")]
    [SerializeField] private bool OnAim = true;
    [SerializeField] private bool OnMouse = false;

    [Header("=== Mouse")]
    [SerializeField] private RectTransform MousePointerRT;
    [HideInInspector] public bool CanMouseInput = false;
    [SerializeField] public Vector2 MousePos;
    [SerializeField] public Vector2 MousePosByWorld;
    [SerializeField] public Vector2 DirFromPlayerPos;

    [Header("=== Aim")]
    [SerializeField] public AimController AimController;

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;
    [SerializeField] public Vector2Int InputArrowDir;

    [Header("=== First Input")] 
    [SerializeField] public bool IsPlayingSkill = false;
    [SerializeField] private float EndBufferedInputTime = 0.5f;
    [SerializeField] private float CurrentBufferedInputTime = 0f;


    private delegate void FirstInputDele();
    private FirstInputDele CurrentBufferedInputDele = null;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
    }

    private void Start()
    {
        SetOff_AllPointer();
    }

    private void FixedUpdate()
    {
        Set_MousePos();
    }

    private void Update()
    {
        Caculate_BufferedInput();
    }


    #endregion

    #region Buffered Input

    private void Caculate_BufferedInput()
    {
        if (CurrentBufferedInputDele != null)
        {
            CurrentBufferedInputTime += Time.deltaTime;

            if (CurrentBufferedInputTime >= EndBufferedInputTime)
            {
                SetOff_BufferedInput();
            }

            if (!IsPlayingSkill)
            {
                CurrentBufferedInputDele();
                SetOff_BufferedInput();
            }
        }
    }

    private void SetOn_BufferedInput(FirstInputDele _Skill)
    {
        CurrentBufferedInputDele = _Skill;
        CurrentBufferedInputTime = 0f;
    }

    private void SetOff_BufferedInput()
    {
        CurrentBufferedInputDele = null;
        CurrentBufferedInputTime = 0f;
    }

    #endregion

    #region Aim & Mouse

    public void SetOn_AllPointer()
    {
        Set_AimPointer(true);
        Set_MousePointer(true);
    }

    public void SetOff_AllPointer()
    {
        Set_AimPointer(false);
        Set_MousePointer(false);
    }


    public void SetOn_AimPointer()
    {
        Set_AimPointer(true);
        Set_MousePointer(false);
    }

    public void SetOn_MousePointer()
    {
        Set_AimPointer(false);
        Set_MousePointer(true);
    }


    private void Set_AimPointer(bool _IsOn)
    {
        OnAim = _IsOn;

        AimController.gameObject.SetActive(_IsOn);
        PlayerManager.Instance.PlayerController.AimRoundController.gameObject.SetActive(_IsOn);
    }

    private void Set_MousePointer(bool _IsOn)
    {
        OnMouse = _IsOn;

        MousePointerRT.gameObject.SetActive(_IsOn);
    }


    #endregion

    #region Mouse

    private void Set_MousePos()
    {
        if (!CanMouseInput)
        { return; }

        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);
        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerManager.Instance.PlayerController.gameObject.transform.position;
        MousePointerRT.anchoredPosition = MousePos;
    }

    public Quaternion Get_Rot_DirFromPlayerPos()
    {
        return StaticCaculator.Get_RotFromDir(DirFromPlayerPos);
    }

    #endregion

    #region Input Set

    public void SetOn_InputAction()
    {
        if (PlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Arrow"].performed += Input_Arrow;
        PlayerInput.actions["Fire"].performed += Input_Fire;
        PlayerInput.actions["Dash"].performed += Input_Dash;

        PlayerInput.actions["CombatMode"].performed += Input_CombatMode;
        PlayerInput.actions["BoostMode"].performed += Input_BoostMode;
        PlayerInput.actions["UnBoostMode"].performed += Input_UnBoostMode;
        PlayerInput.actions["ChargeBettery"].performed += Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed += Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed += Input_Skill_1;

        PlayerInput.actions["Interact"].performed += Input_Interact;
        PlayerInput.actions["TabInteract"].performed += Input_Tab;
        PlayerInput.actions["OutMainGame"].performed += Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed += Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed += Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed += Input_MUUIClick;
        PlayerInput.actions["MUUI_OutPanel"].performed += Input_MUUIOutPanel;

        PlayerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    public void SetOff_InputAction()
    {
        if (PlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Arrow"].performed -= Input_Arrow;
        PlayerInput.actions["Fire"].performed -= Input_Fire;
        PlayerInput.actions["Dash"].performed -= Input_Dash;

        PlayerInput.actions["CombatMode"].performed -= Input_CombatMode;
        PlayerInput.actions["BoostMode"].performed -= Input_BoostMode;
        PlayerInput.actions["UnBoostMode"].performed -= Input_UnBoostMode;
        PlayerInput.actions["ChargeBettery"].performed -= Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed -= Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed -= Input_Skill_1;

        PlayerInput.actions["Interact"].performed -= Input_Interact;
        PlayerInput.actions["TabInteract"].performed -= Input_Tab;
        PlayerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed -= Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed -= Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed -= Input_MUUIClick;
        PlayerInput.actions["MUUI_OutPanel"].performed -= Input_MUUIOutPanel;

        PlayerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }

    #endregion

    #region Player

    #region Movement

    public void Input_Walk(InputAction.CallbackContext _InputValue)
    {
        InputMoveDir = _InputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Set_DashCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.Set_DashCheck();
        }
    }

    public void Input_Arrow(InputAction.CallbackContext _InputValue)
    {
        Vector2 v2 = _InputValue.ReadValue<Vector2>();
        if (v2.x != 0)
        {
            InputArrowDir = v2.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (v2.y != 0)
        {
            InputArrowDir = v2.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
        else
        {
            InputArrowDir = Vector2Int.zero;
        }
    }

    #endregion

    #region Combat

    private void Input_CombatMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_CombatModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_CombatModeCheck();
        }
    }

    private void Input_BoostMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_BoostModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_BoostModeCheck(); 
        }
    }

    private void Input_UnBoostMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_UnBoostModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_UnBoostModeCheck();
        }
    }

    #endregion

    #region Skill

    private void Input_Skill_0(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_Skill0);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_Skill0();
        }
    }

    private void Input_Skill_1(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_Skill1);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_Skill1();
        }
    }

    #endregion

    #region Fire

    private void Input_Fire(InputAction.CallbackContext _InputValue)
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.IsInputed = _InputValue.ReadValueAsButton();
    }

    #endregion

    #region Charge Bettery

    private void Input_ChargeBettery(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_ChargeBettery);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_ChargeBettery();
        }
    }

    #endregion

    #region Interact

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            PlayerManager.Instance.PlayerController.Try_Interact();
        }
    }

    #endregion

    #region Tab

    private void Input_Tab(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = true;
        }
        else
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = false;
        }
    }

    #endregion

    #endregion

    #region BaseUpgrade UI

    private void Input_BUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.Try_Interact();
        }
    }

    private void Input_BUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
        }
    }

    #endregion
    
    #region ModuleUpgrade UI

    private void Input_MUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_Interact();
        }
    }

    private void Input_MUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOff_ThisPanel();
        }
    }

    #endregion

    #region OutMainGame UI

    private void Input_OMGUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.SetOn_ThisPanel();
        }
    }

    private void Input_OMGUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.Try_Interact();
        }
    }

    private void Input_OMGUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.SetOff_ThisPanel();
        }
    }

    #endregion
}
