using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    #region - Inspector

    [Header("=== Mouse")]
    [SerializeField] private RectTransform MousePointerRT;

    [Header("=== Buffered")]
    [Tooltip("선입력 가능 시간의 적용(최대) 시간")]
    [SerializeField] private float EndBufferedInputTime = 0.5f;

    #endregion

    #region - Hide

    // Buffered
    [HideInInspector] public bool IsPlayingBuffered = false;
    [HideInInspector] private float CurrentBufferedInputTime = 0f;

    // Mouse Vec
    [HideInInspector] public bool CanMouseInput = false;
    [HideInInspector] public Vector2 MousePos; // 현재 마우스 위치
    [HideInInspector] public Vector2 MousePosByWorld; // 세상 기준 마우스 위치
    [HideInInspector] public Vector2 DirFromPlayerPos; // 플레이어부터 마우스까지의 Vec

    // Movement Vec
    [HideInInspector] public Vector2 InputMoveDir;
    [HideInInspector] public Vector2Int InputArrowDir;

    // Input
    [HideInInspector] public PlayerInput PlayerInput;

    // Aim
    [HideInInspector] public AimController AimController;
    [HideInInspector] public AimRoundController AimRoundController;
    [HideInInspector] private bool IsAim = false;

    // Dele
    [HideInInspector] private Dele CurrentBufferedDele = null;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        Set_AllPointer(false);
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
    }

    private void Start()
    {
        Offset();
    }

    private void Update()
    {
        Set_MousePos();
    }

    private void LateUpdate()
    {
        Caculate_BufferedInput(Time.deltaTime);
    }

    #endregion

    #region Buffered Input

    private void Caculate_BufferedInput(float _DeltaTime)
    {
        if (CurrentBufferedDele != null)
        {
            CurrentBufferedInputTime += Time.deltaTime;

            if (CurrentBufferedInputTime >= EndBufferedInputTime)
            {
                SetOff_BufferedInput();
            }

            if (!IsPlayingBuffered && CurrentBufferedDele != null)
            {
                CurrentBufferedDele();
                SetOff_BufferedInput();
            }
        }
    }

    private void SetOn_BufferedInput(Dele _Buffered)
    {
        CurrentBufferedDele = _Buffered;
        CurrentBufferedInputTime = 0f;
    }

    private void SetOff_BufferedInput()
    {
        CurrentBufferedDele = null;
        CurrentBufferedInputTime = 0f;
    }

    private void Play_BuffedApplyInput(Dele _Func)
    {
        if (IsPlayingBuffered) SetOn_BufferedInput(_Func);
        else _Func();
    }

    #endregion

    #region Aim & Mouse

    public void Set_AllPointer(bool _OnOff)
    {
        Set_AllPointer(_OnOff, _OnOff);
    }

    public void Set_AllPointer(bool _Aim, bool _Mouse)
    {
        Set_AimPointer(_Aim);
        Set_MousePointer(_Mouse);
    }

    private void Set_AimPointer(bool _IsOn)
    {
        AimController.gameObject.SetActive(_IsOn);
        AimRoundController.gameObject.SetActive(_IsOn);
        IsAim = _IsOn;
    }

    private void Set_MousePointer(bool _IsOn)
    {
        MousePointerRT.gameObject.SetActive(_IsOn);
    }


    #endregion

    #region Mouse

    private void Set_MousePos()
    {
        if (!CanMouseInput) return; 

        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);
        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerManager.Instance.PlayerController.gameObject.transform.position;

        if (!IsAim)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                DevTool.Get_ComponentTType<RectTransform>(MousePointerRT.transform.parent.gameObject), // 변환할 UI(RectTransform)
                MousePos, // 현재 마우스 좌표 (Screen Space)
                MainGameUIManager.Instance.UICamera, // Canvas의 카메라 (Render Mode 따라 null 가능)
                out Vector2 localPoint); // 변환된 Local 좌표

            MousePointerRT.anchoredPosition = localPoint;
        }
    }

    public void Play_MousePointerClick()
    {
        DevTool.Set_KillTween(MousePointerRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(MousePointerRT.DOScale(1.4f, 0.05f));
        seq.Append(MousePointerRT.DOScale(1f, 0.05f));
    }

    #endregion

    #region Input Set

    public void SetOnOff_InputAction(int _CurrentStageID, bool _OnOff)
    {
        if (_CurrentStageID == 99)
        {
            if (_OnOff)
                SetOn_InputAction_InLobby();
            else
                SetOff_InputAction_InLobby();
        }
        else
        {
            if (_OnOff)
                SetOn_InputAction_MainGame();
            else
                SetOff_InputAction_MainGame();
        }
    }


    private void SetOn_InputAction_InLobby()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input;

        // Player
        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Arrow"].performed += Input_Arrow;

        PlayerInput.actions["Interact"].performed += Input_Interact;
        PlayerInput.actions["TabInteract"].performed += Input_Tab;
        PlayerInput.actions["OutMainGame"].performed += Input_OMGUI;

        // Out Main Game UI
        PlayerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    private void SetOff_InputAction_InLobby()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input;

        // Player
        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Arrow"].performed -= Input_Arrow;

        PlayerInput.actions["Interact"].performed -= Input_Interact;
        PlayerInput.actions["TabInteract"].performed -= Input_Tab;
        PlayerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        // Out Main Game UI
        PlayerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }

    private void SetOn_InputAction_MainGame()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input; 

        // Player
        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Arrow"].performed += Input_Arrow;
        PlayerInput.actions["Fire"].performed += Input_Fire;
        PlayerInput.actions["Dash"].performed += Input_Dash;

        PlayerInput.actions["CombatMode"].performed += Input_CombatMode;
        PlayerInput.actions["ChargeBettery"].performed += Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed += Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed += Input_Skill_1;

        PlayerInput.actions["STAlly"].performed += Input_STAlly;
        PlayerInput.actions["UTAlly"].performed += Input_UTAlly;
        PlayerInput.actions["NTAlly"].performed += Input_NTAlly;

        PlayerInput.actions["Interact"].performed += Input_Interact;
        PlayerInput.actions["TabInteract"].performed += Input_Tab;
        PlayerInput.actions["OutMainGame"].performed += Input_OMGUI;

        PlayerInput.actions["Ping"].performed += Input_Ping;

        // BU UI
        PlayerInput.actions["BUUI_Select"].performed += Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed += Input_BUUIOutPanel;

        // MU UI
        PlayerInput.actions["MUUI_Select"].performed += Input_MUUIClick;
        PlayerInput.actions["MUUI_SelectSub"].performed += Input_MUUIClickSub;
        PlayerInput.actions["MUUI_OutPanel"].performed += Input_MUUIOutPanel;
        PlayerInput.actions["MUUI_Drag"].performed += Input_MUUIDrag;

        // A BU UI
        PlayerInput.actions["ABUUI_Select"].performed += Input_ABUUIClick;
        PlayerInput.actions["ABUUI_OutPanel"].performed += Input_ABUUIOutPanel;

        // A MU UI
        PlayerInput.actions["AMUUI_Select"].performed += Input_AMUUIClick;
        PlayerInput.actions["AMUUI_OutPanel"].performed += Input_AMUUIOutPanel;

        // Ally Card
        PlayerInput.actions["AllyCard_Select"].performed += Input_AllyCardClick;

        // Box Line Connector
        PlayerInput.actions["BoxLineConnector_RightRoll"].performed += Input_BoxLineConnector_RightRoll;
        PlayerInput.actions["BoxLineConnector_LeftRoll"].performed += Input_BoxLineConnector_LeftRoll;
        PlayerInput.actions["BoxLineConnector_TryUnlock"].performed += Input_BoxLineConnector_TryUnlock;

        // Num Shape Color Password
        PlayerInput.actions["NumShapeColorPassword_RollForDown"].performed += Input_NumShapeColorPassword_RollForDown;
        PlayerInput.actions["NumShapeColorPassword_RollForUp"].performed += Input_NumShapeColorPassword_RollForUp;
        PlayerInput.actions["NumShapeColorPassword_TryUnlock"].performed += Input_NumShapeColorPassword_TryUnlock;

        // In Order Locker
        PlayerInput.actions["InOrderLocker_Interact"].performed += Input_InOrderLocker_Interact;
        PlayerInput.actions["InOrderLocker_TryUnlock"].performed += Input_InOrderLocker_TryUnlock;

        // Out Main Game UI
        PlayerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    private void SetOff_InputAction_MainGame()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input;

        // Player
        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Arrow"].performed -= Input_Arrow;
        PlayerInput.actions["Fire"].performed -= Input_Fire;
        PlayerInput.actions["Dash"].performed -= Input_Dash;

        PlayerInput.actions["CombatMode"].performed -= Input_CombatMode;
        PlayerInput.actions["ChargeBettery"].performed -= Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed -= Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed -= Input_Skill_1;

        PlayerInput.actions["STAlly"].performed -= Input_STAlly;
        PlayerInput.actions["UTAlly"].performed -= Input_UTAlly;
        PlayerInput.actions["NTAlly"].performed -= Input_NTAlly;

        PlayerInput.actions["Interact"].performed -= Input_Interact;
        PlayerInput.actions["TabInteract"].performed -= Input_Tab;
        PlayerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        PlayerInput.actions["Ping"].performed -= Input_Ping;

        // BU UI
        PlayerInput.actions["BUUI_Select"].performed -= Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed -= Input_BUUIOutPanel;

        // MU UI
        PlayerInput.actions["MUUI_Select"].performed -= Input_MUUIClick;
        PlayerInput.actions["MUUI_SelectSub"].performed -= Input_MUUIClickSub;
        PlayerInput.actions["MUUI_OutPanel"].performed -= Input_MUUIOutPanel;
        PlayerInput.actions["MUUI_Drag"].performed -= Input_MUUIDrag;

        // Ally Card
        PlayerInput.actions["AllyCard_Select"].performed -= Input_AllyCardClick;

        // Box Line Connector
        PlayerInput.actions["BoxLineConnector_RightRoll"].performed -= Input_BoxLineConnector_RightRoll;
        PlayerInput.actions["BoxLineConnector_LeftRoll"].performed -= Input_BoxLineConnector_LeftRoll;
        PlayerInput.actions["BoxLineConnector_TryUnlock"].performed -= Input_BoxLineConnector_TryUnlock;

        // Num Shape Color Password
        PlayerInput.actions["NumShapeColorPassword_RollForDown"].performed -= Input_NumShapeColorPassword_RollForDown;
        PlayerInput.actions["NumShapeColorPassword_RollForUp"].performed -= Input_NumShapeColorPassword_RollForUp;
        PlayerInput.actions["NumShapeColorPassword_TryUnlock"].performed -= Input_NumShapeColorPassword_TryUnlock;

        // In Order Locker
        PlayerInput.actions["InOrderLocker_Interact"].performed -= Input_InOrderLocker_Interact;
        PlayerInput.actions["InOrderLocker_TryUnlock"].performed -= Input_InOrderLocker_TryUnlock;

        // Out Main Game UI
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
            if (IsPlayingBuffered)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_Dash);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_Dash();
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
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_CombatModeCheck);
    }

    #endregion

    #region Skill

    private void Input_Skill_0(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_Skill0);
    }

    private void Input_Skill_1(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_Skill1);
    }

    #endregion

    #region Ally

    private void Input_STAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_STAllyLvUp();
    }

    private void Input_UTAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_UTAllyLvUp();
    }

    private void Input_NTAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_NTAllyLvUp();
    }

    #endregion

    #region Fire

    private void Input_Fire(InputAction.CallbackContext _InputValue)
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.IsInputed = _InputValue.ReadValueAsButton();
    }

    #endregion

    #region Ping

    public void Input_Ping(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_PingEnemy(AimController);
    }

    #endregion

    #region Charge Bettery

    private void Input_ChargeBettery(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_ChargeBettery);
    }

    #endregion

    #region Interact

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_Interact();
    }


    #endregion

    #region Tab

    private void Input_Tab(InputAction.CallbackContext _InputValue)
    {
        MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = 
            _InputValue.ReadValueAsButton();
    }

    #endregion

    #endregion

    #region BUUI

    private void Input_BUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BaseUpgrade_UIController.Try_Interact();
    }

    private void Input_BUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion
    
    #region MUUI

    private void Input_MUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_Interact();
    }
    private void Input_MUUIClickSub(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractSub();
    }
    private void Input_MUUIDrag(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractDragOn();
        else
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractDragOff();
    }

    private void Input_MUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region ABUUI

    private void Input_ABUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.Try_Interact();
    }

    private void Input_ABUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region AMUUI
    private void Input_AMUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyModuleUpgrade_UIController.Try_Interact();
    }

    private void Input_AMUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyModuleUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region AllyCard UI

    private void Input_AllyCardClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyCard_UIController.Try_Interact();
    }

    #endregion

    #region Puzzle UI

    #region Box Line Connector

    private void Input_BoxLineConnector_RightRoll(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BoxLineConnector_UIController.Try_Interact();
    }
    private void Input_BoxLineConnector_LeftRoll(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BoxLineConnector_UIController.Try_InteractSub();
    }

    private void Input_BoxLineConnector_TryUnlock(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BoxLineConnector_UIController.Try_InteractUnlock();
    }

    #endregion

    #region Num Shape Color Password

    private void Input_NumShapeColorPassword_RollForDown(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.NumShapeColorPassword_UIController.Try_Interact();
    }
    private void Input_NumShapeColorPassword_RollForUp(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.NumShapeColorPassword_UIController.Try_InteractSub();
    }
    private void Input_NumShapeColorPassword_TryUnlock(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.NumShapeColorPassword_UIController.Try_InteractUnlock();
    }

    #endregion

    #region In Order Locker

    private void Input_InOrderLocker_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.InOrderLocker_UIController.Try_Interact();
    }
    private void Input_InOrderLocker_TryUnlock(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.InOrderLocker_UIController.Try_InteractUnlock();
    }

    #endregion

    #endregion

    #region OutMainGame UI

    private void Input_OMGUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.SetOn_ThisPanel();
    }

    private void Input_OMGUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.Try_Interact();
    }

    private void Input_OMGUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.Try_InteractBack();
    }

    #endregion
}
