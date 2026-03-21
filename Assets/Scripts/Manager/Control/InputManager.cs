using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    #region - Inspector

    [Header("=== Mouse")]
    [SerializeField] private RectTransform mousePointerRT;

    [Header("=== Buffered")]
    [Tooltip("선입력 가능 시간의 적용(최대) 시간")]
    [SerializeField] private float endBufferedInputTime = 0.5f;

    #endregion

    #region - Hide

    // Buffered
    [HideInInspector] public bool isPlayingBuffered = false;
    [HideInInspector] private float currentBufferedInputTime = 0f;

    // Mouse Vec
    [HideInInspector] public bool canMouseInput = false;
    [HideInInspector] public Vector2 mousePos; // 현재 마우스 위치
    [HideInInspector] public Vector2 mousePosByWorld; // 세상 기준 마우스 위치
    [HideInInspector] public Vector2 dirFromPlayerPos; // 플레이어부터 마우스까지의 Vec

    // Movement Vec
    [HideInInspector] public Vector2 inputMoveDir;
    [HideInInspector] public Vector2Int inputArrowDir;

    // Input
    [HideInInspector] public PlayerInput playerInput;

    // Aim
    [HideInInspector] public AimController aimController;
    [HideInInspector] public AimRoundController aimRoundController;
    [HideInInspector] private bool isAim = false;

    // Dele
    [HideInInspector] private Dele currentBufferedDele = null;

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

    private void Caculate_BufferedInput(float deltaTime)
    {
        if (currentBufferedDele != null)
        {
            currentBufferedInputTime += Time.deltaTime;

            if (currentBufferedInputTime >= endBufferedInputTime)
            {
                SetOff_BufferedInput();
            }

            if (!isPlayingBuffered && currentBufferedDele != null)
            {
                currentBufferedDele();
                SetOff_BufferedInput();
            }
        }
    }

    private void SetOn_BufferedInput(Dele buffered)
    {
        currentBufferedDele = buffered;
        currentBufferedInputTime = 0f;
    }

    private void SetOff_BufferedInput()
    {
        currentBufferedDele = null;
        currentBufferedInputTime = 0f;
    }

    private void Play_BuffedApplyInput(Dele func)
    {
        if (isPlayingBuffered) SetOn_BufferedInput(func);
        else func();
    }

    #endregion

    #region Aim & Mouse

    public void Set_AllPointer(bool onOff)
    {
        Set_AllPointer(onOff, onOff);
    }

    public void Set_AllPointer(bool aim, bool mouse)
    {
        Set_AimPointer(aim);
        Set_MousePointer(mouse);
    }

    private void Set_AimPointer(bool isOn)
    {
        aimController.gameObject.SetActive(isOn);
        aimRoundController.gameObject.SetActive(isOn);
        isAim = isOn;
    }

    private void Set_MousePointer(bool isOn)
    {
        mousePointerRT.gameObject.SetActive(isOn);
    }


    #endregion

    #region Mouse

    private void Set_MousePos()
    {
        if (!canMouseInput) return; 

        mousePos = Input.mousePosition;
        mousePosByWorld = Camera.main.ScreenToWorldPoint(mousePos);
        dirFromPlayerPos = mousePosByWorld - (Vector2)PlayerManager.instance.playerController.gameObject.transform.position;

        if (!isAim)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                DevTool.Get_ComponentTType<RectTransform>(mousePointerRT.transform.parent.gameObject), // 변환할 UI(RectTransform)
                mousePos, // 현재 마우스 좌표 (Screen Space)
                MainGameUIManager.instance.uiCamera, // Canvas의 카메라 (Render Mode 따라 null 가능)
                out Vector2 localPoint); // 변환된 Local 좌표

            mousePointerRT.anchoredPosition = localPoint;
        }
    }

    public void Play_MousePointerClick()
    {
        DevTool.Set_KillTween(mousePointerRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(mousePointerRT.DOScale(1.4f, 0.05f));
        seq.Append(mousePointerRT.DOScale(1f, 0.05f));
        seq.SetUpdate(true);
    }

    #endregion

    #region Input Set

    public void SetOnOff_InputAction(int currentStageID, bool onOff)
    {
        if (currentStageID == 99)
        {
            if (onOff)
                SetOn_InputAction_InLobby();
            else
                SetOff_InputAction_InLobby();
        }
        else
        {
            if (onOff)
                SetOn_InputAction_MainGame();
            else
                SetOff_InputAction_MainGame();
        }
    }


    private void SetOn_InputAction_InLobby()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.instance.playerController.gameObject, out PlayerInput input))
            playerInput = input;

        // Player
        playerInput.actions["Walk"].performed += Input_Walk;
        playerInput.actions["Arrow"].performed += Input_Arrow;

        playerInput.actions["Interact"].performed += Input_Interact;
        playerInput.actions["TabInteract"].performed += Input_Tab;
        playerInput.actions["OutMainGame"].performed += Input_OMGUI;

        // Out Main Game UI
        playerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        playerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    private void SetOff_InputAction_InLobby()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.instance.playerController.gameObject, out PlayerInput input))
            playerInput = input;

        // Player
        playerInput.actions["Walk"].performed -= Input_Walk;
        playerInput.actions["Arrow"].performed -= Input_Arrow;

        playerInput.actions["Interact"].performed -= Input_Interact;
        playerInput.actions["TabInteract"].performed -= Input_Tab;
        playerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        // Out Main Game UI
        playerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        playerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }

    private void SetOn_InputAction_MainGame()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.instance.playerController.gameObject, out PlayerInput input))
            playerInput = input; 

        // Player
        playerInput.actions["Walk"].performed += Input_Walk;
        playerInput.actions["Arrow"].performed += Input_Arrow;
        playerInput.actions["Fire"].performed += Input_Fire;
        playerInput.actions["Dash"].performed += Input_Dash;

        playerInput.actions["CombatMode"].performed += Input_CombatMode;
        playerInput.actions["ChargeBettery"].performed += Input_ChargeBettery;

        playerInput.actions["Skill_0"].performed += Input_Skill_0;
        playerInput.actions["Skill_1"].performed += Input_Skill_1;

        playerInput.actions["STAlly"].performed += Input_STAlly;
        playerInput.actions["UTAlly"].performed += Input_UTAlly;
        playerInput.actions["NTAlly"].performed += Input_NTAlly;

        playerInput.actions["Interact"].performed += Input_Interact;
        playerInput.actions["TabInteract"].performed += Input_Tab;
        playerInput.actions["OutMainGame"].performed += Input_OMGUI;

        playerInput.actions["Ping"].performed += Input_Ping;

        // BU UI
        playerInput.actions["BUUI_Select"].performed += Input_BUUIClick;
        playerInput.actions["BUUI_OutPanel"].performed += Input_BUUIOutPanel;

        // MU UI
        playerInput.actions["MUUI_Select"].performed += Input_MUUIClick;
        playerInput.actions["MUUI_SelectSub"].performed += Input_MUUIClickSub;
        playerInput.actions["MUUI_OutPanel"].performed += Input_MUUIOutPanel;
        playerInput.actions["MUUI_Drag"].performed += Input_MUUIDrag;

        // A BU UI
        playerInput.actions["ABUUI_Select"].performed += Input_ABUUIClick;
        playerInput.actions["ABUUI_OutPanel"].performed += Input_ABUUIOutPanel;

        // A MU UI
        playerInput.actions["AMUUI_Select"].performed += Input_AMUUIClick;
        playerInput.actions["AMUUI_OutPanel"].performed += Input_AMUUIOutPanel;

        // Ally Card
        playerInput.actions["AllyCard_Select"].performed += Input_AllyCardClick;

        // Box Line Connector
        playerInput.actions["BoxLineConnector_RightRoll"].performed += Input_BoxLineConnector_RightRoll;
        playerInput.actions["BoxLineConnector_LeftRoll"].performed += Input_BoxLineConnector_LeftRoll;
        playerInput.actions["BoxLineConnector_TryUnlock"].performed += Input_BoxLineConnector_TryUnlock;

        // Num Shape Color Password
        playerInput.actions["NumShapeColorPassword_RollForDown"].performed += Input_NumShapeColorPassword_RollForDown;
        playerInput.actions["NumShapeColorPassword_RollForUp"].performed += Input_NumShapeColorPassword_RollForUp;
        playerInput.actions["NumShapeColorPassword_TryUnlock"].performed += Input_NumShapeColorPassword_TryUnlock;

        // In Order Locker
        playerInput.actions["InOrderLocker_Interact"].performed += Input_InOrderLocker_Interact;
        playerInput.actions["InOrderLocker_TryUnlock"].performed += Input_InOrderLocker_TryUnlock;

        // Cvt
        // PremiumCredit
        playerInput.actions["CPCUI_Select"].performed += Input_Cvt_PC_Click;
        playerInput.actions["CPCUI_OutPanel"].performed += Input_Cvt_PC_OutPanel;
        // ProtoCore
        playerInput.actions["CPUI_Select"].performed += Input_Cvt_P_Click;
        playerInput.actions["CPUI_OutPanel"].performed += Input_Cvt_P_OutPanel;
        // EtherCore
        playerInput.actions["CEUI_Select"].performed += Input_Cvt_E_Click;
        playerInput.actions["CEUI_OutPanel"].performed += Input_Cvt_E_OutPanel;
        // ProtoCore
        playerInput.actions["COUI_Select"].performed += Input_Cvt_O_Click;
        playerInput.actions["COUI_OutPanel"].performed += Input_Cvt_O_OutPanel;

        // Out Main Game UI
        playerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        playerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    private void SetOff_InputAction_MainGame()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.instance.playerController.gameObject, out PlayerInput input))
            playerInput = input;

        // Player
        playerInput.actions["Walk"].performed -= Input_Walk;
        playerInput.actions["Arrow"].performed -= Input_Arrow;
        playerInput.actions["Fire"].performed -= Input_Fire;
        playerInput.actions["Dash"].performed -= Input_Dash;

        playerInput.actions["CombatMode"].performed -= Input_CombatMode;
        playerInput.actions["ChargeBettery"].performed -= Input_ChargeBettery;

        playerInput.actions["Skill_0"].performed -= Input_Skill_0;
        playerInput.actions["Skill_1"].performed -= Input_Skill_1;

        playerInput.actions["STAlly"].performed -= Input_STAlly;
        playerInput.actions["UTAlly"].performed -= Input_UTAlly;
        playerInput.actions["NTAlly"].performed -= Input_NTAlly;

        playerInput.actions["Interact"].performed -= Input_Interact;
        playerInput.actions["TabInteract"].performed -= Input_Tab;
        playerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        playerInput.actions["Ping"].performed -= Input_Ping;

        // BU UI
        playerInput.actions["BUUI_Select"].performed -= Input_BUUIClick;
        playerInput.actions["BUUI_OutPanel"].performed -= Input_BUUIOutPanel;

        // MU UI
        playerInput.actions["MUUI_Select"].performed -= Input_MUUIClick;
        playerInput.actions["MUUI_SelectSub"].performed -= Input_MUUIClickSub;
        playerInput.actions["MUUI_OutPanel"].performed -= Input_MUUIOutPanel;
        playerInput.actions["MUUI_Drag"].performed -= Input_MUUIDrag;

        // Ally Card
        playerInput.actions["AllyCard_Select"].performed -= Input_AllyCardClick;

        // Box Line Connector
        playerInput.actions["BoxLineConnector_RightRoll"].performed -= Input_BoxLineConnector_RightRoll;
        playerInput.actions["BoxLineConnector_LeftRoll"].performed -= Input_BoxLineConnector_LeftRoll;
        playerInput.actions["BoxLineConnector_TryUnlock"].performed -= Input_BoxLineConnector_TryUnlock;

        // Num Shape Color Password
        playerInput.actions["NumShapeColorPassword_RollForDown"].performed -= Input_NumShapeColorPassword_RollForDown;
        playerInput.actions["NumShapeColorPassword_RollForUp"].performed -= Input_NumShapeColorPassword_RollForUp;
        playerInput.actions["NumShapeColorPassword_TryUnlock"].performed -= Input_NumShapeColorPassword_TryUnlock;

        // In Order Locker
        playerInput.actions["InOrderLocker_Interact"].performed -= Input_InOrderLocker_Interact;
        playerInput.actions["InOrderLocker_TryUnlock"].performed -= Input_InOrderLocker_TryUnlock;

        // Cvt
        // PremiumCredit
        playerInput.actions["CPCUI_Select"].performed -= Input_Cvt_PC_Click;
        playerInput.actions["CPCUI_OutPanel"].performed -= Input_Cvt_PC_OutPanel;
        // ProtoCore
        playerInput.actions["CPUI_Select"].performed -= Input_Cvt_P_Click;
        playerInput.actions["CPUI_OutPanel"].performed -= Input_Cvt_P_OutPanel;
        // EtherCore
        playerInput.actions["CEUI_Select"].performed -= Input_Cvt_E_Click;
        playerInput.actions["CEUI_OutPanel"].performed -= Input_Cvt_E_OutPanel;
        // ProtoCore
        playerInput.actions["COUI_Select"].performed -= Input_Cvt_O_Click;
        playerInput.actions["COUI_OutPanel"].performed -= Input_Cvt_O_OutPanel;

        // Out Main Game UI
        playerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        playerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }

    #endregion

    #region Player

    #region Movement

    public void Input_Walk(InputAction.CallbackContext inputValue)
    {
        inputMoveDir = inputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
        {
            if (isPlayingBuffered)
            {
                SetOn_BufferedInput(PlayerManager.instance.playerController.Try_Dash);
                return;
            }

            PlayerManager.instance.playerController.Try_Dash();
        }
    }

    public void Input_Arrow(InputAction.CallbackContext inputValue)
    {
        Vector2 v2 = inputValue.ReadValue<Vector2>();
        if (v2.x != 0)
        {
            inputArrowDir = v2.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (v2.y != 0)
        {
            inputArrowDir = v2.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
        else
        {
            inputArrowDir = Vector2Int.zero;
        }
    }

    #endregion

    #region Combat

    private void Input_CombatMode(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.instance.playerController.Try_CombatModeCheck);
    }

    #endregion

    #region Skill

    private void Input_Skill_0(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.instance.playerController.Try_Skill0);
    }

    private void Input_Skill_1(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.instance.playerController.Try_Skill1);
    }

    #endregion

    #region Ally

    private void Input_STAlly(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            PlayerManager.instance.playerController.Try_STAllyLvUp();
    }

    private void Input_UTAlly(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            PlayerManager.instance.playerController.Try_UTAllyLvUp();
    }

    private void Input_NTAlly(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            PlayerManager.instance.playerController.Try_NTAllyLvUp();
    }

    #endregion

    #region Fire

    private void Input_Fire(InputAction.CallbackContext inputValue)
    {
        PlayerManager.instance.playerController.baseWeapon.isInputed = inputValue.ReadValueAsButton();
    }

    #endregion

    #region Ping

    public void Input_Ping(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            PlayerManager.instance.playerController.Try_PingEnemy(aimController);
    }

    #endregion

    #region Charge Bettery

    private void Input_ChargeBettery(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.instance.playerController.Try_ChargeBettery);
    }

    #endregion

    #region Interact

    private void Input_Interact(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            PlayerManager.instance.playerController.Try_Interact();
    }


    #endregion

    #region Tab

    private void Input_Tab(InputAction.CallbackContext inputValue)
    {
        MainGameUIManager.instance.playerHUD_UIController.IsTabInputed = 
            inputValue.ReadValueAsButton();
    }

    #endregion

    #endregion

    #region BUUI

    private void Input_BUUIClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.baseUpgrade_UIController.Try_Interact();
    }

    private void Input_BUUIOutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.baseUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion
    
    #region MUUI

    private void Input_MUUIClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.moduleUpgrade_UIController.Try_Interact();
    }
    private void Input_MUUIClickSub(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.moduleUpgrade_UIController.Try_InteractSub();
    }
    private void Input_MUUIDrag(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.moduleUpgrade_UIController.Try_InteractDragOn();
        else
            MainGameUIManager.instance.moduleUpgrade_UIController.Try_InteractDragOff();
    }

    private void Input_MUUIOutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.moduleUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region ABUUI

    private void Input_ABUUIClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.allyBaseUpgrade_UIController.Try_Interact();
    }

    private void Input_ABUUIOutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.allyBaseUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region AMUUI
    private void Input_AMUUIClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.allyModuleUpgrade_UIController.Try_Interact();
    }

    private void Input_AMUUIOutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.allyModuleUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region AllyCard UI

    private void Input_AllyCardClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.allyCard_UIController.Try_Interact();
    }

    #endregion

    #region Puzzle UI

    #region Box Line Connector

    private void Input_BoxLineConnector_RightRoll(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.boxLineConnector_UIController.Try_Interact();
    }
    private void Input_BoxLineConnector_LeftRoll(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.boxLineConnector_UIController.Try_InteractSub();
    }

    private void Input_BoxLineConnector_TryUnlock(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.boxLineConnector_UIController.Try_InteractUnlock();
    }

    #endregion

    #region Num Shape Color Password

    private void Input_NumShapeColorPassword_RollForDown(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.numShapeColorPassword_UIController.Try_Interact();
    }
    private void Input_NumShapeColorPassword_RollForUp(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.numShapeColorPassword_UIController.Try_InteractSub();
    }
    private void Input_NumShapeColorPassword_TryUnlock(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.numShapeColorPassword_UIController.Try_InteractUnlock();
    }

    #endregion

    #region In Order Locker

    private void Input_InOrderLocker_Interact(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.inOrderLocker_UIController.Try_Interact();
    }
    private void Input_InOrderLocker_TryUnlock(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.inOrderLocker_UIController.Try_InteractUnlock();
    }

    #endregion

    #endregion

    #region Cvt

    // PC (Premium Credit)
    private void Input_Cvt_PC_Click(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.premiumCreditCvt_UIController.Try_Interact();
    }


    private void Input_Cvt_PC_OutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.premiumCreditCvt_UIController.SetOff_ThisPanel();
    }


    // P (Proto Core)
    private void Input_Cvt_P_Click(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.protoCoreCvt_UIController.Try_Interact();
    }


    private void Input_Cvt_P_OutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.protoCoreCvt_UIController.SetOff_ThisPanel();
    }


    // E (Ether Core)
    private void Input_Cvt_E_Click(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.etherCoreCvt_UIController.Try_Interact();
    }


    private void Input_Cvt_E_OutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.etherCoreCvt_UIController.SetOff_ThisPanel();
    }


    // O (Origin Core)
    private void Input_Cvt_O_Click(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.originCoreCvt_UIController.Try_Interact();
    }


    private void Input_Cvt_O_OutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.originCoreCvt_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region OutMainGame UI

    private void Input_OMGUI(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.outMainGame_UIController.SetOn_ThisPanel();
    }

    private void Input_OMGUIClick(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.outMainGame_UIController.Try_Interact();
    }

    private void Input_OMGUIOutPanel(InputAction.CallbackContext inputValue)
    {
        if (inputValue.ReadValueAsButton())
            MainGameUIManager.instance.outMainGame_UIController.Try_InteractBack();
    }

    #endregion
}
