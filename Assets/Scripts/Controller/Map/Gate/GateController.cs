using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : StaticDepthController, IInteract
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Gate")]

    [Space(10)]
    [Header("=== Data")]

    [Space(5)]
    [Header("-- KeyCard")]
    [SerializeField] private int NeedKeyCardID = -1;
    [SerializeField] private SpriteRenderer KeyCardIconSR;

    [Space(5)]
    [Header("-- Entrance")]
    [SerializeField] private SpriteRenderer NextMapIconSR;

    [Space(5)]
    [Header("-- Vec")]
    [SerializeField] public Vector2Int RoomPosGate;
    [SerializeField] public Vector2Int GateDir;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public AnimationClip ThisAC;

    [Space(10)]
    [Header("=== On / Off")]
    [SerializeField] public CoupleData<GameObject> ThingsGO;
    [SerializeField] private GameObject EntranceGO;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public List<SpriteRenderer> OpacityLowerSRList;

    [Space(10)]
    [Header("=== Is Wall")]
    [SerializeField] public GameObject ExtraTargetObject;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public bool IsOpen = false;
    [HideInInspector] public bool SettedPos = false;

    // Controller
    [HideInInspector] public RoomController ThisRoom;
    [HideInInspector] public GateController ParterGate = null;

    // Animation
    [HideInInspector] public Animator ThisAnimator;
    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region - Set Data

    [HideInInspector] private static float WarpPointInterval = 1f;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        ThisAnimator = DevTool.Get_ComponentTType<Animator>(TargetObject);
        StageManager.Instance.Set_StageDoorAnim(this, DevTool.Get_ComponentTType<SpriteRenderer>(TargetObject), GateDir);

        KeyCardIconSR.gameObject.SetActive(false);
        KeyCardIconSR.sortingOrder = LayerOrderManager.Order_DoorIcon;
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // 같은 방법으로 Y축을 위로 올리는데, 다른 객체가 추가로 필요하니 이처럼 사용
        ExtraTargetObject.transform.position = Get_TargetPos();
    }

    private void LateUpdate()
    {
        // 문 애니메이션이 종료되면 애니메이터를 끈다.
        Try_AT_Disable();
    }

    #endregion

    #region On Off

    // 처음 문의 상태를 (벽이거나 문이거나) 판별해서 세팅
    public void Set_ExistDoorState(bool _IsExist)
    {
        ThingsGO.TypeBase.SetActive(!_IsExist);
        ThingsGO.TypeSpecial.SetActive(_IsExist);

    }


    // 문 열기/닫기
    public void Set_Open()
    {
        IsOpen = true;
        
        if (!Can_Open_ByKeycard())
        {
            KeyCardIconSR.sprite = MainGameUIManager.Instance.Get_KeyCardSprite(NeedKeyCardID);
            KeyCardIconSR.gameObject.SetActive(true);
        }
        else
        {
            KeyCardIconSR.gameObject.SetActive(false);
        }

        if (Can_Open())
        {
            ThisAnimator.enabled = true;
            DevTool.Set_Anim(ref AOC, ThisAnimator, ThisAC);
            DevTool.Set_AnimSpeed(ThisAnimator, 1f);

            // 상호작용 판정을 가진 오브젝트
            EntranceGO.SetActive(true);
        }
    }

    #endregion

    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        if (ThingsGO.TypeBase.activeSelf)
        {
            _CanInteract = false;
            return "";
        }
        else
        {
            _CanInteract = IsOpen && Can_Open_ByKeycard();
            return ResourceManager.Instance.Get_StaticWord(1);
        }
    }

    public void Play_Interact()
    {
        if (IsOpen && ParterGate != null)
        {
            if (NeedKeyCardID != -1)
            {
                // 키카드 사용해서 열기
                if (PlayerManager.Instance.Can_UseKeyCard(NeedKeyCardID))
                {
                    PlayerManager.Instance.Use_KeyCard(NeedKeyCardID);
                    NeedKeyCardID = -1;
                    ParterGate.NeedKeyCardID = -1;
                    Set_Open();

                    MainGameUIManager.Instance.PlayerHUD_UIController.Set_InteractUI(); 
                    MainGameUIManager.Instance.InteractAnno_UIController.Set_UI();
                }
            }
            else
            {
                if (ParterGate.ThisRoom.RoomRuleController.Is_EliteEnemyRoom(out int eliteID))
                {
                    StageManager.Instance.Get_CurrentStageData().EnemyData.StageEliteEnemyList[eliteID].TryGetComponent(out EliteEnemyController eliteEnemy);
                    StageManager.Instance.Play_GoInBossRoom(this, eliteEnemy);
                }
                else if (ParterGate.ThisRoom.RoomRuleController.Is_BossEnemyRoom(out int bossID))
                {
                    StageManager.Instance.Get_CurrentStageData().EnemyData.StageBossEnemyList[bossID].TryGetComponent(out BossEnemyController bossEnemy);
                    StageManager.Instance.Play_GoInBossRoom(this, bossEnemy);
                }
                else
                {
                    PassGateForBossRoom();
                }

            }
        }
    }

    public void PassGateForBossRoom()
    {
        PlayerManager.Instance.PlayerController.SetOff_Trail();
        PlayerManager.Instance.PlayerController.gameObject.transform.position = ParterGate.Get_WarpPoint();
        StageManager.Instance.Play_CurrentRoom(ParterGate.ThisRoom);
    }

    #endregion

    #region Get

    public Vector2 Get_WarpPoint()
    {
        return (Vector2)gameObject.transform.position
            + new Vector2(-GateDir.x * WarpPointInterval, -GateDir.y * WarpPointInterval);
    }

    #endregion

    #region Anim

    // Animator 종료 시도
    private void Try_AT_Disable()
    {
        if (Can_AT_Disable())
        {
            ThisAnimator.enabled = false;
            ThisSR.sprite = DevTool.Get_LastFrameSprite(ThisAC, ThisSR);
        }
    }

    #endregion

    #region Can

    public bool Can_Open_ByKeycard()
    {
        return NeedKeyCardID == -1;
    }

    private bool Can_Open()
    {
        return Can_Open_ByKeycard() &&
            ThingsGO.TypeSpecial.activeSelf && 
            ParterGate != null &&
            IsOpen;
    }

    // Animator가 종료될 수 있는가 판별
    private bool Can_AT_Disable()
    {
        return ThingsGO.TypeSpecial.activeSelf &&
            ThisAnimator.enabled &&
            DevTool.Is_AnimIsDone(ThisAnimator);
    }

    #endregion

    #region Key

    public void Set_NeedKeyCard(int _ID)
    {
        NeedKeyCardID = _ID;

    }

    #endregion

    #region Map

    public void Set_NextMap()
    {
        if (DevTool.Can_CastingTType(ParterGate.ThisRoom.RoomRuleController, out EntranceRuleController erc))
        {
            int index = erc.Get_ElevatorData();
            NextMapIconSR.sprite = StageManager.Instance.StageIconDict[index];
            NextMapIconSR.gameObject.SetActive(true);
        }
        else
        {
            NextMapIconSR.gameObject.SetActive(false);
        }
    }

    #endregion
}
