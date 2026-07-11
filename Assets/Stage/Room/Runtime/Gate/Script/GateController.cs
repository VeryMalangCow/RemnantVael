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
    [SerializeField] private int needKeyCardId = -1;
    [SerializeField] private SpriteRenderer keyCardIconSr;

    [Space(5)]
    [Header("-- Entrance")]
    [SerializeField] private SpriteRenderer nextMapIconSr;

    [Space(5)]
    [Header("-- Vec")]
    [SerializeField] public Vector2Int roomPosGate;
    [SerializeField] public Vector2Int gateDir;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public AnimationClip ac;

    [Space(10)]
    [Header("=== On / Off")]
    [SerializeField] public CoupleData<GameObject> thingsGo;
    [SerializeField] private GameObject entranceGo;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public List<SpriteRenderer> opacityLowerSrList;

    [Space(10)]
    [Header("=== Is Wall")]
    [SerializeField] public GameObject extraTargetObject;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public bool isOpen = false;
    [HideInInspector] public bool settedPos = false;

    // Controller
    [HideInInspector] public RoomController thisRoom;
    [HideInInspector] public GateController parterGate = null;

    // Animation
    [HideInInspector] public Animator at;
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region - Set Data

    [HideInInspector] private static float warpPointInterval = 1f;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        at = DevTool.Get_ComponentTType<Animator>(targetObject);
        ac = StageManager.instance.stageObjectGenerator.GetCurrentStageDoorAnim(gateDir);

        keyCardIconSr.gameObject.SetActive(false);
        keyCardIconSr.sortingOrder = SortingOrderManager.order_DoorIcon;
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // 같은 방법으로 Y축을 위로 올리는데, 다른 객체가 추가로 필요하니 이처럼 사용
        extraTargetObject.transform.position = Get_TargetPos();
    }

    private void LateUpdate()
    {
        // 문 애니메이션이 종료되면 애니메이터를 끈다.
        Try_AT_Disable();
    }

    #endregion

    #region On Off

    // 처음 문의 상태를 (벽이거나 문이거나) 판별해서 세팅
    public void Set_ExistDoorState(bool isExist, GateController parterGate)
    {
        thingsGo.typeBase.SetActive(!isExist);
        thingsGo.typeSpecial.SetActive(isExist);

        this.parterGate = parterGate;
    }


    // 문 열기/닫기
    public void SetOpen()
    {
        isOpen = true;
        
        if (!Can_Open_ByKeycard())
        {
            keyCardIconSr.sprite = StaticResourceManager.instance.ItemReso.keycardIcons[needKeyCardId].sprite;
            keyCardIconSr.gameObject.SetActive(true);
        }
        else
        {
            keyCardIconSr.gameObject.SetActive(false);
        }

        if (Can_Open())
        {
            at.enabled = true;
            DevTool.Set_Anim(ref aoc, at, ac);
            DevTool.Set_AnimSpeed(at, 1f);

            // 상호작용 판정을 가진 오브젝트
            entranceGo.SetActive(true);
        }
    }

    #endregion

    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        if (thingsGo.typeBase.activeSelf)
        {
            canInteract = false;
            return "";
        }
        else
        {
            canInteract = isOpen && Can_Open_ByKeycard();
            return StaticResourceManager.instance.staticWords.GetLanguage(1);
        }
    }

    public void PlayInteract()
    {
        if (isOpen && parterGate != null)
        {
            if (parterGate.thisRoom.roomRule is EntranceRuleController erc && erc.GetElevatorData() == 2)
            {
                Debug.Log("02 스테이지는 미구현");
                return;
            }
            if (needKeyCardId != -1)
            {
                PlayerController player = PlayerManager.instance.playerController;
                // 키카드 사용해서 열기
                if (player.CanUseKeyCard(needKeyCardId))
                {
                    player.UseKeyCard(needKeyCardId);
                    needKeyCardId = -1;
                    parterGate.needKeyCardId = -1;
                    SetOpen();

                    PlayerManager.instance.playerController.SetInteractable();
                }
            }
            else
            {
                if (parterGate.thisRoom.roomRule.Is_EliteEnemyRoom(out int eliteID))
                {
                    StageManager.instance.StartEliteRoom(this, eliteID);
                    SoundManager.instance.Pause_2D_BGM();

                }
                else if (parterGate.thisRoom.roomRule.Is_BossEnemyRoom(out int bossID))
                {
                    StageManager.instance.StartBossRoom(this, bossID);
                    SoundManager.instance.Pause_2D_BGM();
                }
                else
                {
                    EnterGate();
                }
            }
        }
    }

    public void EnterGate()
    {
        SoundManager.instance.PlayBuildSfx(transform.position, "EnterGate");
        PlayerManager.instance.playerController.SetOff_Trail();
        PlayerManager.instance.playerController.gameObject.transform.position = parterGate.Get_WarpPoint();
        StageManager.instance.StartCurrentRoom(parterGate.thisRoom);

        HudController hud = MainGameUIManager.instance.hud;
        if (hud.isTabInteracted.Value) hud.MinimapView.Reset_BookRoom();
    }

    #endregion

    #region Get

    public Vector2 Get_WarpPoint()
    {
        return (Vector2)gameObject.transform.position
            + new Vector2(-gateDir.x * warpPointInterval, -gateDir.y * warpPointInterval);
    }

    #endregion

    #region Anim

    // Animator 종료 시도
    private void Try_AT_Disable()
    {
        if (Can_AT_Disable())
        {
            at.enabled = false;
            thisSr.sprite = DevTool.Get_LastFrameSprite(ac, thisSr);
        }
    }

    #endregion

    #region Can

    public bool Can_Open_ByKeycard()
    {
        return needKeyCardId == -1;
    }

    private bool Can_Open()
    {
        return Can_Open_ByKeycard() &&
            thingsGo.typeSpecial.activeSelf && 
            parterGate != null &&
            isOpen;
    }

    // Animator가 종료될 수 있는가 판별
    private bool Can_AT_Disable()
    {
        return thingsGo.typeSpecial.activeSelf &&
            at.enabled &&
            DevTool.Is_AnimIsDone(at);
    }

    #endregion

    #region Key

    public void Set_NeedKeyCard(int id)
    {
        needKeyCardId = id;
    }

    #endregion

    #region Map

    public void Set_NextMap()
    {
        if (DevTool.Can_CastingTType(parterGate.thisRoom.roomRule, out EntranceRuleController erc))
        {
            int index = erc.GetElevatorData();
            nextMapIconSr.sprite = StaticResourceManager.instance.StageReso.GetStageIcon(index);
            nextMapIconSr.gameObject.SetActive(true);
        }
        else
        {
            nextMapIconSr.gameObject.SetActive(false);
        }
    }

    #endregion
}
