using UniRx;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;
using System;

public class PlayerController : AliveObjectController
{
    #region Value

    // Ep
    public event Action<float, float> OnEpChanged;
    public event Action<float, float> OnMaxEpChanged;

    // Bettery
    public int betteryShard { get; private set; } = 0;
    public event Action<int> OnBetteryShardChanged;
    public int emptyBettery { get; private set; } = 0;
    public event Action<int> OnEmptyBetteryChanged;
    public int chargedBettery { get; private set; } = 0;
    public event Action<int> OnChargedBetteryChanged;

    // Module
    public int moduleShard = 0;
    public event Action<int> OnModuleShardChanged;
    
    // Overrider
    public int overrider = 0;
    public event Action<int> OnOverriderChanged;
    
    // Credit
    public int credit = 0;
    public event Action<int> OnCreditChanged;

    // keycard
    private Dictionary<int, int> keycardDict = new Dictionary<int, int>();
    public Action<Dictionary<int, int>> OnKeycardChanged;

    // boost
    public int boostLv { get; private set; } = 0;
    public readonly int maxBoostLv = 3;
    public event Action<int> OnBoostLvChanged;

    // Interact
    private List<GameObject> interactableGoList = new List<GameObject>();
    public IInteract interactable { get; private set; } = null;
    public event Action<IInteract> OnInteractableChanged;

    // Presence
    public int strikeTeamPresence { get; private set; } = 0;
    public int needStrikeTeamPresence { get; private set; } = 0;

    public event Action<int, int> OnStrikePresenceChanged;


    public int uplinkTeamPresence { get; private set; } = 0;
    public int needUplinkTeamPresence { get; private set; } = 0;

    public event Action<int, int> OnUplinkPresenceChanged;


    public int neoTeamPresence { get; private set; } = 0;
    public int needNeoTeamPresence { get; private set; } = 0;

    public event Action<int, int> OnNeoPresenceChanged;

    public readonly int needIntervalPresence = 5;


    // Reputation
    public float reputation { get; private set; } = 0f;

    public event Action<float> OnReputationChanged;

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int nameId;
    public int GetNameID => nameId;

    [Space(10)]
    [Header("=== Controller & Generator")]
    [SerializeField] public PlayerWeaponController baseWeapon;
    [SerializeField] public SkillWeaponController skillWeapon;
    [SerializeField] public PlayerDashController dash;
    [SerializeField] public RigidbodyAnimSolarController rbLower;
    [SerializeField] public AfterImgGenerator afterImgGenerator;

    [Space(10)]
    [Header("=== BU State")]
    [SerializeField] public BUState<float> maxEP;
    [SerializeField] public BUState<float> avoidChance;
    [SerializeField] public BUState<float> takingDmgMultiple;
    [SerializeField] public BUState<float> spawnESMultiple;
    [SerializeField] public BUState<float> needEP_ForSkillMultiple;
    [SerializeField] public BUState<float> walkSpeed;
    [SerializeField] public BUState<float> walkSpeedWhenShotMultiple;

    [Space(10)]
    [Header("=== Visual Comp")]

    [Space(5)]
    [Header("-- SG")]
    [SerializeField] private SortingGroup bodySg;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] private StateAnimController stateAnim;
    [SerializeField] private StateAnimController moveDirStateAnim;
    [SerializeField] private List<MovableDepthController> baseAnimDepthList;
    [SerializeField] private CoupleData<List<StateAnimController>> boostStateAnimController;

    [Space(5)]
    [Header("-- VFX")]
    [SerializeField] private TrailRenderer trail;

    [Space(10)]
    [Header("=== Visual Reso")]

    [SerializeField] private AnimationClip moveDirAc;
    [SerializeField] private Sprite changeState_DamageType;

    [SerializeField] public List<Material> materialList;
    [SerializeField] private List<AnimationClip> boostVFXAnimList;

    [SerializeField] private PlayerVisual<Color> clr;
    [SerializeField] private PlayerVisual<Gradient> gradient;

    [SerializeField] private PlayerVisual<AnimationClip> hittedPointAc;
    [SerializeField] private TrioData<AnimationClip> dmgTypeStateAc;
    [SerializeField] private CoupleData<AnimationClip> boostOnOffAc;

    [SerializeField] private CoupleData<Sprite> changeState_BoostUpDown;
    [SerializeField] private CoupleData<Sprite> changeState_Skill;

    [SerializeField] public GameObject aimPrefab;
    [SerializeField] public GameObject aimRoundPrefab;

    [SerializeField] public Sprite battleProdSprite;

    [Space(10)]
    [Header("=== Sound")]
    [SerializeField] private ASQueueSet audioQueueSet;
    [SerializeField] private AudioSource movementAs;

    #endregion

    #region - Hide

    // Lower
    [HideInInspector] private bool isLowerTweening = false;

    // Stage Type
    [HideInInspector] private SortingGroup sg;
    [HideInInspector] private SpriteRenderer shadowSr;

    // Combat Mode
    [HideInInspector] private eDamageType dmgMode = eDamageType.Physics;

    // Invincible
    [HideInInspector] private bool isInvincible = false;

    // Movement
    [HideInInspector] public eMovementState movementState = eMovementState.IdleOrWalk;

    // Shield
    [HideInInspector] public List<Shield> shieldElements = new List<Shield>();

    // Buff
    [HideInInspector] public List<BuffController> currentBuffs = new List<BuffController>();


    // BaseAnim
    [HideInInspector] private Sequence baseSeq = null;
    [HideInInspector] private readonly float baseYLimit = 0.02f;
    [HideInInspector] private readonly float baseTweenReTime = 0.25f;
    [HideInInspector] private readonly float wheelLowestAnimSpeed = 0.5f;

    // Book
    [HideInInspector] private eDamageType targetDmgMode = eDamageType.Physics;
    [HideInInspector] private ChargeCooltimeData castingTime = new ChargeCooltimeData();
    [HideInInspector] private Dele reservationDele = null;
    [HideInInspector] private const float invincibleTime = 0.5f;

    #endregion

    #region - Data

    [HideInInspector] public static readonly int maxRank = 5;
    [HideInInspector] public readonly int needBS_ForMakeBC = 4;
    [HideInInspector] public readonly float needEP_ForMakeEC = 5f;
    
    #endregion

    #endregion

    #region Offset

    public void TestStart()
    {
        Offset_FirstSetting();
        Offset_Controller();

        enabled = true;
    }

    private void Offset_FirstSetting()
    {
        // State Anim
        Reset_StateAnim();

        // State Anim : Dmg Type
        stateAnim.Set_Anim(new State_Anim(dmgTypeStateAc.typeA, 0.8f), 1f);

        // State Anim : Boost
        SetBoostLv(0);

        // State Anim: Room Move
        moveDirStateAnim.Set_Anim(new State_Anim(moveDirAc));
        SetOff_RoomMoveDir();

        // Base Tween
        baseSeq = DOTween.Sequence();

        baseSeq.Join(DOTween.To(() => targetRange, x => targetRange = x, targetRange + baseYLimit, baseTweenReTime)
            .SetEase(Ease.Linear));

        if (baseAnimDepthList != null && baseAnimDepthList.Count > 0)
        {
            for (int i = 0; i < baseAnimDepthList.Count; i++)
            {
                MovableDepthController movable = baseAnimDepthList[i];
                baseSeq.Join(DOTween.To(() => movable.targetRange, x => movable.targetRange = x, movable.targetRange + baseYLimit, baseTweenReTime)
                    .SetEase(Ease.Linear));
            }
        }

        baseSeq.SetLoops(-1, LoopType.Yoyo);

        PrisonBuild.unlockedClr = Get_CorrectColor(eDamageType.Energy, false);

        // Item
        chargedBettery = 0;
        credit = 0;
        overrider = 0;
        moduleShard = 0;

        // Presence
        needStrikeTeamPresence = needIntervalPresence;
        needUplinkTeamPresence = needIntervalPresence;
        needNeoTeamPresence = needIntervalPresence;

        strikeTeamPresence = 0;
        uplinkTeamPresence = 0;
        neoTeamPresence = 0;

        reputation = 1f;
    }



    private void Offset_Controller()
    {
        keycardDict = new Dictionary<int, int>();

        int amount = StaticResourceManager.instance.ItemReso.keycardIcons.Length;
        for (int i = 0; i < amount; i++)
            keycardDict.Add(i, 0);

        dash.Offset();
        sg = DevTool.Get_ComponentTType<SortingGroup>(gameObject); 
        shadowSr = DevTool.Get_ComponentTType<SpriteRenderer>(transform.GetChild(0).gameObject);

        audioQueueSet.Offset(); 
        movementAs.volume = 0.2f;
    }

    #endregion

    #region Mono

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Update_Caculate(Time.fixedDeltaTime);
        Play_Movement(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        MainGameUIManager.instance.interactAnnoUi.Set_PosIfNot(interactable);
        Set_Tween(rbLower.transform, rb.velocity);
    }

    private void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        bodySg.sortingOrder = sortingOrder;

        trail.sortingOrder = sortingOrder - 1;
        // base.Set_SortingOrder(_SortingOrder);
    }

    #endregion

    #region Stage Part Time

    // 스테이지 시작 전
    public void Set_PastStartStage()
    {
        sg.enabled = true;
        sg.sortingOrder = -3002;

        thisSr.sortingOrder = 2;

        shadowSr.sortingOrder = -10;

        stateAnim.transform.parent.transform.gameObject.SetActive(false);
    }

    // 스테이지 시작
    public void Set_StartStage()
    {
        sg.enabled = false;
        sg.sortingOrder = 0;
        thisSr.sortingOrder = 0;

        stateAnim.transform.parent.transform.gameObject.SetActive(true); 

        AddSortingLayer();

        SetOn_Trail();
    }

    // 스테이지 끝
    public void Set_EndStage()
    {
        sg.enabled = true;
        sg.sortingOrder = 3001;

        stateAnim.transform.parent.transform.gameObject.SetActive(false);

        RemoveSortingLayer();

        SetOff_Trail();
    }

    #endregion

    #region Shield Point

    // 현재 총 쉴드값
    private float Get_TotalShield()
    {
        if (shieldElements.Count <= 0)
        { return 0; }

        float totalShield = 0;
        DevTool.Set_ListDele(shieldElements, new Dele_RefT_U<float, Shield>(Add_ShieldValue), ref totalShield);
        return totalShield;

        void Add_ShieldValue(ref float _Variable, Shield _Shield)
        {
            DevTool.Add_RefValue(ref _Variable, _Shield.shieldCurrentValue);
        }
    }

    // 쉴드 획득
    public void Gain_Shield(Shield shield)
    {
        DevTool.Remove_InList(shieldElements, shield);

        shieldElements.Insert(0, shield);
        currentSP.Value = Get_TotalShield();
    }

    // 쉴드 제거
    public void Remove_Shield(Shield shield)
    {
        if (shieldElements.Contains(shield))
        {
            DevTool.Set_ListDele(currentBuffs, new Dele_T_U<BuffController, Shield>(End_ShieldBuff), shield);
            shieldElements.Remove(shield);
        }
        currentSP.Value = Get_TotalShield();

        void End_ShieldBuff(BuffController buff, Shield _shield)
        {
            BuffShieldController shieldBuff = DevTool.Get_CastingTType<BuffShieldController>(buff);
            if (DevTool.Is_UsableAndEqual(shieldBuff, shieldBuff.thisShield, _shield))
            {
                shieldBuff.End_Buff();
            }
        }
    }

    #endregion

    #region Current EP

    // 에너지 획득
    public void AddCurrentEp(float addValue)
    {
        if (addValue == 0)
            return;

        float maxHp = maxEP.actualState;
        AddCurrentEp(addValue, maxHp); // 상속 클래스 함수
        OnEpChanged?.Invoke(currentEp, maxHp); 

        CheckIsDead(currentEp);
    }

    public void FullEp()
    {
        float maxHp = maxEP.actualState;
        SetCurrentEp(maxHp);
        OnEpChanged?.Invoke(currentEp, maxHp);
    }

    public float GetPercentEP(float percent)
    {
        return DevTool.GetPercent(percent, maxEP.actualState);
    }

    #endregion

    #region Max EP

    public void SetMaxEp()
    {
        OnMaxEpChanged?.Invoke(currentEp, maxEP.actualState);
    }

    #endregion

    #region Item - Bettery

    // Bettery Shard
    public void GainBetteryShard(int gainValue)
    {
        betteryShard = Mathf.Min(betteryShard + gainValue, 9999);

        if (betteryShard >= needBS_ForMakeBC)
        {
            MainGameUIManager.instance.hud.BetteryShardView.currentEmptyBc.Set_Complete(
            fadeInTime: 0.3f,
            stayTime: 0.1f,
            fadeOutTime: 0.5f);

            int BSAmount = betteryShard / needBS_ForMakeBC;
            betteryShard -= needBS_ForMakeBC * BSAmount;
            GainEmptyBettery(BSAmount);
        }
        else
        {
            SetBetteryShardUI();
        }
    }

    public void SetBetteryShardUI()
    {
        OnBetteryShardChanged?.Invoke(betteryShard);
    }

    // Empty Bettery
    public void GainEmptyBettery(int gainValue)
    {
        emptyBettery = Mathf.Min(emptyBettery + gainValue, 9999); 
        SetEmptyBetteryUI();
    }

    public void UseEmptyBettery(int useValue)
    {
        emptyBettery = Mathf.Max(emptyBettery - useValue, 0); 
        SetEmptyBetteryUI();
    }

    public void SetEmptyBetteryUI()
    {
        OnEmptyBetteryChanged?.Invoke(emptyBettery);
    }

    // Charged Bettery
    public void GainChargedBettery(int gainValue)
    {
        chargedBettery = Mathf.Min(chargedBettery + gainValue, 9999);
        SetChargedBetteryUI();
    }

    public void UseChargedBettery(int useValue)
    {
        chargedBettery = Math.Max(chargedBettery - useValue, 0); 
        SetChargedBetteryUI();
    }

    public void SetChargedBetteryUI()
    {
        OnChargedBetteryChanged?.Invoke(chargedBettery);
    }

    // Charge
    public bool IsEnoughChargedBettery(int needAmount)
        => chargedBettery >= needAmount;
    
    private void MakeChargedBettery()
    {
        AddCurrentEp(-needEP_ForMakeEC, maxEP.actualState);
        UseEmptyBettery(1);
        GainChargedBettery(1);
    }

    #endregion

    #region Item - Credit

    public void GainCredit(int gainValue)
    {
        credit = Mathf.Min(credit + gainValue, 9999);
        SetCreditUI();
    }
    public void UseCredit(int useValue)
    {
        credit = Mathf.Max(credit - useValue, 0);
        SetCreditUI();
    }

    public void SetCreditUI()
    {
        OnCreditChanged?.Invoke(credit);
    }

    #endregion

    #region Item - Overrider

    public void GainOverrider(int gainValue)
    {
        overrider = Mathf.Min(overrider + gainValue, 9999);
        SetOverriderUI();
    }

    public void UseOverrider(int useValue)
    {
        overrider = Mathf.Max(overrider - useValue, 0);
        SetOverriderUI();
    }

    public void SetOverriderUI()
    {
        OnOverriderChanged?.Invoke(overrider);
    }

    #endregion

    #region Item - Module Shard

    public void GainModuleShard(int gainValue)
    {
        moduleShard = Mathf.Min(moduleShard + gainValue, 9999);
        SetModuleShardUI();
    }
    public void UseModuleShard(int useValue)
    {
        moduleShard = Mathf.Max(moduleShard - useValue, 0);
        SetModuleShardUI();
    }

    public void SetModuleShardUI()
    {
        OnModuleShardChanged?.Invoke(moduleShard);
    }



    #endregion

    #region Item - KeyCard

    public void GainKeyCard(int keyCardID, int amount = 1)
    {
        if (keycardDict.ContainsKey(keyCardID))
        {
            keycardDict[keyCardID] = Mathf.Min(keycardDict[keyCardID] + amount, 99);
            MainGameUIManager.instance.hud.KeyView.EffectKeyIcon(keyCardID);
            OnKeycardChanged?.Invoke(keycardDict);
        }
    }

    public void UseKeyCard(int keyCardID, int amount = 1)
    {
        if (keycardDict.ContainsKey(keyCardID))
        {
            keycardDict[keyCardID] = Mathf.Max(keycardDict[keyCardID] - amount, 0);
            SoundManager.instance.Play_2D_SFX_Build("UseKeycard");
            OnKeycardChanged?.Invoke(keycardDict);
        }
    }

    public bool CanUseKeyCard(int keyCardID)
        => keycardDict.ContainsKey(keyCardID) &&
        keycardDict[keyCardID] > 0;

    #endregion

    #region Boost Lv

    public void SetBoostLv(int value)
    {
        boostLv = value;
        // 플레이어 위
        Set_BoostAnim_StateWheel(value);
        Set_BoostAnim_StateExtraVFX(value);

        OnBoostLvChanged?.Invoke(value);
    }

    // 스탯의 위아래로 도는 바퀴
    private void Set_BoostAnim_StateWheel(int index)
    {
        for (int i = 0; i < maxBoostLv; i++)
        {
            // 부스팅 : 언부스팅
            AnimationClip ac = i < index ?
                boostOnOffAc.typeSpecial : boostOnOffAc.typeBase;
            float animSpeed = i < index ?
                wheelLowestAnimSpeed * (index - i + 1) : wheelLowestAnimSpeed;

            boostStateAnimController.typeBase[i].Set_Anim(new State_Anim(ac, animSpeed));
        }
    }

    // 스탯의 좌우로 도는 이펙트
    private void Set_BoostAnim_StateExtraVFX(int index)
    {
        boostStateAnimController.typeSpecial[0].gameObject.SetActive(false);
        boostStateAnimController.typeSpecial[1].gameObject.SetActive(false);

        float animSpeed = index * 0.5f;
        if (index > 0)
        { 
            Set_EachBoostAnim_StateExtraVFX(0, animSpeed); 
        }
        if (index > 2)
        {
            Set_EachBoostAnim_StateExtraVFX(1, animSpeed);
        }
    }

    private void Set_EachBoostAnim_StateExtraVFX(int index, float animSpeed)
    {
        boostStateAnimController.typeSpecial[index].gameObject.SetActive(true);
        boostStateAnimController.typeSpecial[index].Set_Anim(new State_Anim(boostVFXAnimList[index], animSpeed));

    }

    #endregion

    #region Ally Presence

    // Gain
    public void GainStrikePresence(int value)
    {
        strikeTeamPresence += value;
        OnStrikePresenceChanged?.Invoke(strikeTeamPresence, needStrikeTeamPresence);
    }
    public void GainUplinkPresence(int value)
    {
        uplinkTeamPresence += value;
        OnUplinkPresenceChanged?.Invoke(uplinkTeamPresence, needUplinkTeamPresence);
    }
    public void GainNeoPresence(int value)
    {
        neoTeamPresence += value;
        OnNeoPresenceChanged?.Invoke(neoTeamPresence, needNeoTeamPresence);
    }

    public void ActPresence()
    {
        OnStrikePresenceChanged?.Invoke(strikeTeamPresence, needStrikeTeamPresence);
        OnUplinkPresenceChanged?.Invoke(uplinkTeamPresence, needUplinkTeamPresence);
        OnNeoPresenceChanged?.Invoke(neoTeamPresence, needNeoTeamPresence);
    }

    // Try Lv Up
    public void Try_STAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.roomRule.roomType != eRoomType.Completed)
            return;

        if (needStrikeTeamPresence <= strikeTeamPresence)
        {
            needStrikeTeamPresence += needIntervalPresence; 
            OnStrikePresenceChanged?.Invoke(strikeTeamPresence, needStrikeTeamPresence);

            MainGameUIManager.instance.allyCardUi.typeIndex = 0;
            MainGameUIManager.instance.allyCardUi.SetOnThisPanel();
        }
    }

    public void Try_UTAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.roomRule.roomType != eRoomType.Completed)
            return;

        if (needUplinkTeamPresence <= uplinkTeamPresence)
        {
            needUplinkTeamPresence += needIntervalPresence;
            OnUplinkPresenceChanged?.Invoke(uplinkTeamPresence, needUplinkTeamPresence);

            MainGameUIManager.instance.allyCardUi.typeIndex = 1;
            MainGameUIManager.instance.allyCardUi.SetOnThisPanel();
        }
    }

    public void Try_NTAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.roomRule.roomType != eRoomType.Completed)
            return;

        if (needNeoTeamPresence <= neoTeamPresence)
        {
            needNeoTeamPresence += needIntervalPresence;
            OnNeoPresenceChanged?.Invoke(neoTeamPresence, needNeoTeamPresence);

            MainGameUIManager.instance.allyCardUi.typeIndex = 2;
            MainGameUIManager.instance.allyCardUi.SetOnThisPanel();
        }
    }

    #endregion

    #region Reputation

    public void GainReputation(float value)
    {
        reputation = Mathf.Min(reputation + value, 100f);
        OnReputationChanged?.Invoke(reputation);
    }

    public void ReduceReputation(float value)
    {
        reputation = Mathf.Max(reputation - value, 0f);
        OnReputationChanged?.Invoke(reputation);
    }
    public void ActReputation()
    {
        OnReputationChanged?.Invoke(reputation);
    }

    #endregion

    #region Interact

    public void SetInteractable()
    {
        OnInteractableChanged?.Invoke(interactable);
        Set_MoveDir();
    }

    public void SetInteractable(IInteract interactable)
    {
        this.interactable = interactable;
        SetInteractable();
    }

    #endregion

    #region Movement

    private void Play_Movement(float deltaTime)
    {
        if (movementState == eMovementState.IdleOrWalk)
        {
            Play_Walk(deltaTime);
        }
        else if (movementState == eMovementState.Dash)
        {
            dash.Play_Dash(deltaTime);
        }
    }

    private void Play_Walk(float deltaTime)
    {
        float multiple =
            baseWeapon.isShooting ? walkSpeedWhenShotMultiple.actualState : 1f;
        Play_Walk(
            InputManager.instance.inputMoveDir, walkSpeed.actualState * multiple, deltaTime);
    }

    public void Try_Dash()
    {
        if (Can_Change() && dash.Is_EnoughEP())
        {
            InputManager.instance.isPlayingBuffered = true;
            afterImgGenerator.Start_Gen(0.7f, 0.03f, 0.5f);
            AddCurrentEp(-dash.Get_ActualNeedEP());
            movementState = eMovementState.Dash;
        }
    }

    #endregion

    #region Movement Lower (Tween)

    private void Set_Tween(Transform targetTf, Vector2 rbVel)
    {
        if (rbVel != Vector2.zero && isLowerTweening == false) // On
        {
            SetOn_Tween(targetTf);
        }
        else if (rbVel == Vector2.zero && isLowerTweening == true) // Off
        {
            SetOff_Tween(targetTf);
        }
    }

    private void SetOn_Tween(Transform targetTf)
    {
        isLowerTweening = true;
        targetTf.DOShakePosition(1f, 0.01f, 20, 0, false, false)
            .SetLoops(-1, LoopType.Restart)
            .OnStart(() => movementAs.volume = 0.25f )
            .OnKill(() => movementAs.volume = 0.1f );
    }

    private void SetOff_Tween(Transform targetTf)
    {
        isLowerTweening = false;
        DOTween.Kill(targetTf);
    }

    #endregion

    #region Casting

    // 상태 변경이나 스킬, 대시 등을 사용할 수 있는 상황인가?
    private bool Can_Change()
    {
        if (movementState != eMovementState.IdleOrWalk)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private const float combatModeInterval = 0.25f;
    private const float boostModeInterval = 0.1f;
    private const float unBoostModeInterval = 0.1f;
    private const float chargeBetteryInterval = 1f;
    public readonly float skillInterval = 0.5f;

    // 조건: + None
    public void Try_CombatModeCheck()
    {
        if (!Can_Change()) 
        { return; }

        stateAnim.Set_Anim(
            new State_Anim(dmgTypeStateAc.typeSpecial, 2f), 
            innerSprite: changeState_DamageType);

        targetDmgMode = targetDmgMode == eDamageType.Physics ?
            eDamageType.Energy : eDamageType.Physics;

        Start_Casting(combatModeInterval);
    }

    // 조건: + EP, BC가 충분한가?
    public void Try_ChargeBettery()
    {
        if (!Can_Change() ||
            needEP_ForMakeEC >= this.currentEp ||
            emptyBettery <= 0) 
        { return; }

        reservationDele = MakeChargedBettery;

        Start_Casting(chargeBetteryInterval);
    }

    // 조건: + ex) 스킬을 사용할 수 없다면, Error 문구
    public void Try_Skill0()
    {
        Try_Skill(0, sprite: changeState_Skill.typeBase);
    }

    public void Try_Skill1()
    {
        Try_Skill(1, sprite: changeState_Skill.typeSpecial);
    }

    private void Try_Skill(int index, Sprite sprite)
    {
        if (!Can_Change())
        { return; }

        if (!skillWeapon.skillList[index].Can_Active())
        {
            MainGameUIManager.instance.hud.SkillView.skillList[index].Play_ErrorUI();
            return;
        }

        reservationDele = skillWeapon.skillList[index].Active_Skill;
        stateAnim.Set_Anim(
            new State_Anim(dmgTypeStateAc.typeSpecial, 2f),
            sprite);

        Start_Casting(skillInterval);
    }


    public void Start_Casting(float castingTime)
    {
        this.castingTime.max = castingTime;
        movementState = eMovementState.Casting;
        rb.velocity = Vector2.zero;

        InputManager.instance.isPlayingBuffered = true;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (interactableGoList.Count <= 0 || interactable == null)
            return; 

        interactable.PlayInteract();
        
        MainGameUIManager.instance.hud.InteractView.UseInteractUI(interactable);
    }

    #endregion

    #region Update Caculate

    private void Update_Caculate(float deltaTime)
    {
        Caculate_Casting(deltaTime);
        //Caculate_Boosting(_DeltaTime, CurrentBoostLv.Value);
    }

    private void Caculate_Casting(float deltaTime)
    {
        if (movementState != eMovementState.Casting)
        { return; }

        if (castingTime.Is_Charge(deltaTime)) // 캐스팅 완료
        {
            movementState = eMovementState.IdleOrWalk;
            InputManager.instance.isPlayingBuffered = false;

            Set_CombatMode();
            Set_Skill();
        }
    }

/*
    private void Caculate_Boosting(float _DeltaTime, int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            Add_CurrentEP(-decValue * _DeltaTime);
        }
    }
*/

    #endregion

    #region Check CastingType

    // 캐스팅에서 바뀐부분을 찾아서 적용
    
    private void Set_CombatMode()
    {
        if(dmgMode != targetDmgMode)
        {
            dmgMode = targetDmgMode;
            baseWeapon.dmgType = targetDmgMode;

            Reset_StateAnim();
            InputManager.instance.aimController.Set_DmgType(targetDmgMode);
        }
    }

    private void Set_Skill()
    {
        if(reservationDele != null)
        {
            reservationDele();
            reservationDele = null;

            Reset_StateAnim();
        }
    }

    // 모든 캐스팅이 끝나면, 본래의 스탯애니메이션으로 돌아옴
    private void Reset_StateAnim()
    {
        stateAnim.Set_Anim(
                new State_Anim(targetDmgMode == eDamageType.Physics ?
                    dmgTypeStateAc.typeA : dmgTypeStateAc.typeB, 0.8f));
    }

    #endregion

    #region Room Move State

    private void SetOn_RoomMoveDir(Vector2 dir)
    {
        moveDirStateAnim.gameObject.SetActive(true);
        moveDirStateAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));
    }

    private void SetOff_RoomMoveDir()
    {
        moveDirStateAnim.gameObject.SetActive(false);
    }


    private void Set_MoveDir()
    {
        if (DevTool.Can_CastingTType(interactable, out GateController gate) && gate.isOpen)
        { SetOn_RoomMoveDir(gate.gateDir); }
        else
        { SetOff_RoomMoveDir(); }
    }

    #endregion

    #region Visual

    public Color Get_CorrectColor(eDamageType damageType, bool isCritical)
    {
        return Get_CorrectTComponent(clr, damageType, isCritical);
    }

    public Gradient Get_CorrectGradient(eDamageType damageType, bool isCritical)
    {
        return Get_CorrectTComponent(gradient, damageType, isCritical);
    }

    public AnimationClip Get_CorrectAC(eDamageType damageType, bool isCritical)
    {
        return Get_CorrectTComponent(hittedPointAc, damageType, isCritical);
    }

    private T Get_CorrectTComponent<T>(PlayerVisual<T> t, eDamageType damageType, bool isCritical)
    {
        return t.Get_CorrectType(damageType).Get_Special(isCritical);
    }

    #endregion

    #region Trigger

    // Enter
    protected override void OnTriggerEnter2D(Collider2D col) // 판별을 위한 IInteract GO 추가
    {
        base.OnTriggerEnter2D(col);

        GameObject targetGO = col.gameObject.transform.parent.gameObject;
        if (DevTool.Get_ComponentTType<IInteract>(targetGO) != null)
        {
            DevTool.Add_InList(interactableGoList, targetGO);
        }
    }

    // Exit
    private void OnTriggerExit2D(Collider2D col) // 판별에 필요없는 IInteract GO 삭제
    {
        GameObject targetGo = col.gameObject.transform.parent.gameObject;
        if (DevTool.Get_ComponentTType<IInteract>(targetGo) != null)
        {
            DevTool.Remove_InList(interactableGoList, targetGo);

            // 모두 삭제되었다면
            if (interactableGoList.Count <= 0)
            {
                SetInteractable(null);
            }
        }
    }

    // Stay: Set Current IInteract
    private void OnTriggerStay2D(Collider2D col)
    {
        // None
        if (interactableGoList.Count <= 0)
        {
            SetInteractable(null);
        }

        // 하나만 존재
        else if (interactableGoList.Count == 1)
        {
            if (DevTool.Get_ComponentTType(interactableGoList[0], out IInteract i))
            {
                SetInteractable(i);
            }
        }
        else // 다수 존재
        {
            if (DevTool.Get_ComponentTType(
                    DevTool.Get_ClosetGO(interactableGoList, this.gameObject),
                    out IInteract i))
            {
                SetInteractable(i);
            }
        }
    }

    #endregion

    #region Hitted

    // 타격: 총알
    public void Try_Hitted(EnemyBulletController bullet)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        if (isInvincible || isDead)
        { return; }

        isInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            BulletState state = bullet.state;
            
            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, bullet.ownEnemy.buff),
                DevTool.Get_Dir(bullet.gameObject, gameObject),
                state.knockbackState);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyAttackerController attacker)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        if (isInvincible || isDead)
        { return; }

        isInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            AttackerState state = attacker.attackerState;

            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, attacker.enemy.buff),
                DevTool.Get_Dir(attacker.gameObject, gameObject),
                state.knockbackState);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyExplosionController explosion)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        if (isInvincible || isDead)
        { return; }

        isInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            ExplosionState state = explosion.state;

            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, explosion.enemy.buff),
                DevTool.Get_Dir(explosion.gameObject, gameObject),
                state.knockbackState);
        }
    }


    // 타격: 건물어택커
/*    public void Try_Hitted(TrapObjectController attacker)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        if (isInvincible || isDead)
        { return; }

        isInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
        }
    }*/

    #endregion

    #region Damaged

    // 데미지 계산
    private void Take_Damaged(float dmgValue, Vector2 hittedDir, KnockbackState kbState)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        // Multiple
        dmgValue *= takingDmgMultiple.buffedState;

        // Effect
        // Knockback
        if (kbState.canKB)
        { 
            Gain_Knockback(new CurrentKnockbackState(hittedDir, kbState.kbPower, kbState.kbTime));
        }

        // Damage
        Take_Damaged(dmgValue, hittedDir, showHUDEffect: true);
    }

    // 오직 데미지만 계산 (넉백, 애니메이션 등 설정)
    public void Take_Damaged(float dmgValue, Vector2 hittedDir, bool showHUDEffect = true)
    {
        if (TestThings.isUltraMode)
        {
            Debug.Log("울트라 모드");
            return;
        }

        AllyRequestManager.instance.Play_TakingDamage();

        if (showHUDEffect)
        {
            MainGameUIManager.instance.hud.HittedView.PlayHittedPlayScreen(dmgValue, 0.1f);
            MainGameUIManager.instance.hud.HittedView.PlayHittedPlayInfo(dmgValue, invincibleTime);
        }

        if (hittedDir != Vector2.zero)
        {
            PlayerManager.instance.cameraController.Play_DamagedAnim(invincibleTime, dmgValue * 0.1f, hittedDir);
        }
       

        if (shieldElements.Count > 0)
        {
            for (int i = shieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (shieldElements[i].shieldCurrentValue <= dmgValue)
                {
                    dmgValue -= shieldElements[i].shieldCurrentValue;

                    Remove_Shield(shieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    shieldElements[i].shieldCurrentValue -= dmgValue;
                    dmgValue = 0;
                    currentSP.Value = Get_TotalShield();
                    return;
                }
            }
        }
        currentSP.Value = Get_TotalShield();
        AddCurrentEp(-dmgValue);
    }

    #endregion

    #region Died

    // Dead!
    protected override void Set_Die()
    {
        base.Set_Die();

        SoundManager.instance.Play_2D_SFX_Player("Killed");

        EventManager.instance.Set_Input(false);

        EnemyManager.instance.SetOff_AllEnemyPattern();

        this.gameObject.SetActive(false);

        MainGameUIManager.instance.Play_DeadProd();
    }

    #endregion

    #region Avoid

    // 회피?
    public bool Is_Avoid()
    {
        // 회피
        if (DevTool.Is_ChanceSuccess(avoidChance.actualState))
        {
            Play_Avoid();
            SoundManager.instance.Play_2D_SFX_Player(audioQueueSet.Get_T(), "Avoid");
            return true;
        }
        else
        {
            SetOn_Invincible();
            SoundManager.instance.Play_2D_SFX_Player(audioQueueSet.Get_T(), "Hitted");
            return false;
        }
    }

    // 회피
    private void Play_Avoid()
    {
        PlayerManager.instance.cameraController.Play_AvoidAnim(invincibleTime);
        VFXManager.instance.player_ExplImgGenerator.Expl_Player_Avoid(id, targetObject.transform.position);
        MainGameUIManager.instance.hud.HittedView.PlayAvoidPlayInfo(invincibleTime);
    }
    
    // 회피하지 못함 => 무적
    private void SetOn_Invincible()
    {
        float intervalTime = invincibleTime * 0.125f; // (1/8)
        for (int i = 0; i < afterImgGenerator.targetSRList.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(afterImgGenerator.targetSRList[i].DOFade(0, 0));
            seq.AppendInterval(intervalTime);
            seq.Append(afterImgGenerator.targetSRList[i].DOFade(1, 0));
            seq.AppendInterval(intervalTime);
            seq.SetLoops(4, LoopType.Restart);
        }
    }

    // 회피 끝
    public void SetOff_Invincible()
    {
        isInvincible = false;
    }

    #endregion

    #region Prison Panelty

    public void Set_PrisonPanelty()
    {
        UseCredit((int)(credit * 0.2f));
        UseOverrider((int)(overrider * 0.2f));
        UseModuleShard((int)(moduleShard * 0.2f));

        Take_Damaged(currentEp * 0.2f, Vector2.zero, false);

        MainGameUIManager.instance.hud.HittedView.PlayPrisonPanelty();
    }

    #endregion

    #region Ping

    public void Try_PingEnemy(AimController aim)
    {
        EnemyController enemy = EnemyManager.instance.Get_ClosestEnemy(aim.gameObject, out float dis);
        if (enemy != null && dis <= 3)
            PlayerManager.instance.SetOn_PingEnemy(enemy);
        else
            PlayerManager.instance.SetOff_PingEnemy();
    }

    #endregion

    #region Trail

    public void SetOn_Trail()
    {
        trail.Clear();
        trail.emitting = true;
    }

    public void SetOff_Trail()
    {
        trail.emitting = false;
        trail.Clear();
    }

    #endregion


    #region Sound

    public AudioSource Get_AS()
    {
        return audioQueueSet.Get_T();
    }

    #endregion
}
