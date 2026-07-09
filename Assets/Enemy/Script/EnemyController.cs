using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class EnemyController : NavObjectController, IPoolable
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup sg;
    [SerializeField] public EnemyBuffController buff;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eEnemy enemyType;
    [SerializeField] private float maxHP;
    [SerializeField] private float maxEP = 100f;
    [SerializeField] private float chargeEPSpeed = 1f;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] protected EnemyDropItemPercent enemyDropItemPercent;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public EnemyHUDController hud;

    [Space(10)]
    [Header("=== Satellite")]
    [SerializeField] private EnemySolarController lookingSatellite;
    [SerializeField] private RigidbodyAnimSolarController walkingSatellite;

    [Space(10)]
    [Header("=== Ping Data")]
    [SerializeField] public Vector2 pingOffsetVec;
    [SerializeField] public Vector2 pingSizeVec;

    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> orderOfPriorityEnemyPatternList;
    [SerializeField] protected ContinuousEnemyPattern specialPattern;

    #endregion

    #region - Hide


    // Discharge
    [HideInInspector] private bool isDischarge = false;
    [HideInInspector] private float dischargeDelayTime = 1f;
    [HideInInspector] private float dischargeDelayCurrentTime = 0f;

    // FullCharge
    [HideInInspector] protected bool isFullCharge = false;
    [HideInInspector] private bool isPlayingSpecialPattern = false;

    // 패턴
    [HideInInspector] private EnemyPattern currentEnemyPattern = null;

    // 움직임을 통제
    [HideInInspector] private GameObject target;

    [HideInInspector] public Vector2 lookAtPoint = Vector2.zero;
    [HideInInspector] public Vector2 lookAtDir = Vector2.zero;


    // 패턴
    [HideInInspector] private ContinuousEnemyPattern currentContinuousEnemyPattern = null;
    [HideInInspector] public bool isPlayingPattern = false;
    [HideInInspector] public IEnumerator currentPatternCor = null;

    // 방
    [HideInInspector] private RoomController currentRoomController;

    // 죽음
    [HideInInspector] private eDamageType dieStateType;

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;

    #endregion
    #endregion

    #region Pool

    public void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        Reset_State();
        DevTool.Add_InList(EnemyManager.instance.currentEnemies, this);
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_FirstSetting();
        Offset_Subscribe();
        Offset_Controller();
    }

    private void Offset_FirstSetting()
    {
        hud.Offset();
    }

    private void Offset_Subscribe()
    {
        currentSP
            .Subscribe(_currentSP =>
            {
                hud.stateUi.spProgressBar.SetFillImg(currentSP.Value, maxHP);

                if (currentSP.Value <= 0)
                {
                    currentSP.Value = 0;
                    hud.stateUi.spProgressBar.SetNoNum();
                    hud.stateUi.hpProgressBar.SetFillImg(currentHP.Value, maxHP);
                    hud.stateUi.epProgressBar.SetFillImg(currentEp, maxEP);

                    if (buff.shieldBuff.isOn)
                    { buff.shieldBuff.Remove_AllStack(); }

                }
                else
                {
                    hud.stateUi.hpProgressBar.SetNoNum();
                    hud.stateUi.epProgressBar.SetNoNum();
                }
            });

        currentHP
            .Subscribe(_currentHP =>
            {
                hud.stateUi.hpProgressBar.SetFillImg(currentHP.Value, maxHP);

                if (currentSP.Value > 0)
                { hud.stateUi.hpProgressBar.SetNoNum(); }
            });

        //currentEP
        //    .Subscribe(_currentEP =>
        //    {
        //        hud.stateUi.epProgressBar.Set_FillImgSmooth(currentEP.Value, maxEP);
        //
        //        if (currentSP.Value > 0)
        //        { hud.stateUi.epProgressBar.Set_NoNum(); }
        //    });
    }

    private void Offset_Controller()
    {
        buff.Offset(this);
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        AddSortingLayer();
    }

    private void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion

    #region Movement

    public void HandleMovement(float fdt)
    {
        Play_Walk(moveAtDir, moveSpeed, fdt);
    }

    #endregion

    #region Energy

    public void HandleChargeSkill(float deltaTime)
    {
        if (isDischarge) // 방전 회복 딜레이
        {
            Caculate_DischargeDelay(deltaTime);
        }
        else if (!isFullCharge) // 충전
        {
            Add_CurrentEP(chargeEPSpeed * deltaTime);
        }
    }

    private void Caculate_DischargeDelay(float deltaTime)
    {
        dischargeDelayCurrentTime += deltaTime;
        if (dischargeDelayTime <= dischargeDelayCurrentTime)
        {
            dischargeDelayCurrentTime = 0f;
            isDischarge = false;
        }
    }

    public void Reset_ChargeState()
    {
        isDischarge = true;
        dischargeDelayCurrentTime = 0f;
        SetCurrentEp(0f);
        isFullCharge = false;
        isPlayingSpecialPattern = false;

        hud.Reset_HUD();
    }

    #endregion

    #region HP

    // HP
    protected float Get_PercentHP()
    {
        return (currentHP.Value / maxHP) * 100f;
    }

    #endregion

    #region Reset

    private void Reset_State()
    {
        base.isDead = false;
        currentSP.Value = 0;
        currentHP.Value = maxHP;

        // Energy
        SetCurrentEp(0f);

        // Discharge
        Reset_ChargeState();

        if (target == null) // 타겟 Player
        { target = PlayerManager.instance.playerController.gameObject; }

        if (currentRoomController == null) // 현재 Room
        { currentRoomController = StageManager.instance.currentRoomController; }

    }

    #endregion

    #region Look

    public void HandleLookAtTarget()
    {
        if (isDead) return;

        // 바라볼 방향값 계산
        lookAtDir = target != null ? 
            DevTool.Get_Dir(this.gameObject, target) : Vector2.down;
    }

    #endregion

    #region Life State Gain

    // 쉴드 설정
    // => 낮은 값이 설정되어도 적용할 것인가?
    public void Set_CurrentSP(float setValue, bool lesserIsOk = false)
    {
        Set_CurrentSP(setValue, maxHP, lesserIsOk);
    }
    public void Set_CurrentSP_Zero()
    {
        Set_CurrentSP(0, maxHP, lesserIsOk: true);
    }

    private void Add_CurrentSP(float addValue)
    {
        Add_CurrentSP(addValue, maxHP);
    }

    private void Add_CurrentHP(float addValue)
    { 
        Add_CurrentHP(addValue, maxHP);
        CheckIsDead(currentHP.Value);
    }

    private void Add_CurrentEP(float addValue)
    {
        if (isFullCharge) return;

        AddCurrentEp(addValue, maxEP);

        // 방전
        if (currentEp <= 0)
        {
            isDischarge = true;
            dischargeDelayCurrentTime = 0f;
        }

        // 풀 충전
        if (currentEp >= maxEP)
        {
            isFullCharge = true;
            hud.Set_Charged(isFullCharge);
        }
    }

    public float Get_PercentHP(float percent)
    {
        return DevTool.GetPercent(percent, maxHP);
    }

    #endregion

    #region Hitted

    // 총알 데미지
    public void Try_Hitted(BulletController bullet)
    {
        if (isDead)
        { return; }

        BulletState state = bullet.state;

        // Damage
        Take_Damaged(
            state, 
            state.isCritical, 
            DevTool.Get_DirFromAngle(bullet.transform.eulerAngles.z));

        if (state.isStatus)
        {
            switch (state.statusType)
            {
                case eStatusEffect.Flame:
                    buff.flameStack.Gain_Stack(1, true, state.ownerData);
                    break;
                case eStatusEffect.Cold:
                    buff.coldStack.Gain_Stack(1, true, state.ownerData);
                    break;
                case eStatusEffect.Electricity:
                    buff.electricityStack.Gain_Stack(1, true, state.ownerData);
                    break;
                case eStatusEffect.Corrosion:
                    buff.corrosionStack.Gain_Stack(1, true, state.ownerData);
                    break;
            }
        }
    }

    // 어택커 데미지
    public void Try_Hitted(AttackerController attacker)
    {
        if (isDead)
        { return; }

        AttackerState state = attacker.attackerState;

        // Damage
        Take_Damaged(
            state,
            DevTool.Is_ChanceSuccess(state.criticalState.criticalChance),
            DevTool.Get_Dir(attacker.gameObject, this.gameObject));
    }

    // 폭발 데미지
    public void Try_Hitted(ExplosionController explosion)
    {
        if (isDead)
        { return; }

        ExplosionState state = explosion.state;

        // Damage
        Take_Damaged(
            state,
            DevTool.Is_ChanceSuccess(state.criticalState.criticalChance),
            DevTool.Get_Dir(explosion.gameObject, this.gameObject));

        Try_GainStack(state.isFire, buff.flameStack, state.ownerData);
        Try_GainStack(state.isCold, buff.coldStack, state.ownerData);
        Try_GainStack(state.isElectricity, buff.electricityStack, state.ownerData);
        Try_GainStack(state.isCorrosion, buff.corrosionStack, state.ownerData);
    }

    private void Try_GainStack(bool isGain, StatusEffect_Temporary_WithAmount targetDebuff, CombatOwner combatOwner)
    {
        if (isGain) targetDebuff.Gain_Stack(1, true, combatOwner);
    }

    #endregion

    #region Damaged (Type)

    // 데미지, 넉백, 크리티컬, 모듈 호과 등
    private void Take_Damaged(CombatState state, bool isCritical, Vector2 dirKb)
    {
        float actualDmg = state.dmgState.dmg;

        // INTERFACE: 맞을 때 효과 
        if (state.ownerData.owner == eCombatOwner.Player)
        {
            ModuleItemManager.instance.Active_Hit(this);
            ModuleItemManager.instance.ActiveSync_Hit();
        }
        else if (state.ownerData.owner == eCombatOwner.Ally)
        {
            AllyManager.instance.allAlly[state.ownerData.id].ActiveAlly_Hit();
        }

        // KB
        if (state.knockbackState.canKB)
        {
            Gain_Knockback(new CurrentKnockbackState(dirKb, state.knockbackState.kbPower, state.knockbackState.kbTime));
        }
        // 치명타 계산
        if (isCritical)
        {
            actualDmg *= state.criticalState.criticalDmg;

            // INTERFACE: 치명타를 맞을 때 효과 
            if (state.ownerData.owner == eCombatOwner.Player)
            {
                ModuleItemManager.instance.Active_CriticalHit(this);
                ModuleItemManager.instance.ActiveSync_CriticalHit();
            }
            else if (state.ownerData.owner == eCombatOwner.Ally)
            {
                AllyManager.instance.allAlly[state.ownerData.id].ActiveAlly_CriticalHit();
            }
        }

        // 데미지 구현 (Dmg: 적의 부식 디버프 계산)
        Take_Damage(DevTool.Get_DmgEffectByCorrosion(actualDmg, buff),
            state.dmgState.dmgType,
            isCritical);

        // 사운드
        if (!isDead)
        { SoundManager.instance.PlayEnemySfx(transform.position, "Hitted"); }
        else
        { SoundManager.instance.PlayEnemySfx(transform.position, "Killed"); }
    }

    #endregion

    #region Damaged (Caculate)

    // 오직 데미지만을 계산
    public void Take_Damage(float dmgValue, eDamageType dmgType, bool isCritical = false)
    {
        // 쉴드 계산
        if (currentSP.Value > 0)
        {
            float uiTxt = 0f;
            float uiX = 0f;
            if (currentSP.Value > dmgValue)
            {
                uiTxt = dmgValue;
                uiX = 0.2f;

                Add_CurrentSP(-dmgValue);
                dmgValue = 0f;
            }
            else
            {
                uiTxt = currentSP.Value;
                uiX = 0.1f;

                dmgValue -= currentSP.Value;
                Set_CurrentSP_Zero();
            }

            VfxManager.instance.SpawnDmgTxtCanvas().Offset_ByShieldDmg(
                    (Vector2)targetObject.transform.position + new Vector2(uiX, 0.2f),
                    uiTxt, isCritical);
        }

        if (dmgValue <= 0)
        { return; }

        // 직접 데미지
        // 물리 값
        if (dmgType == eDamageType.Physics) 
        {
            Take_Damaged_Physics(dmgValue, isCritical);
        }
        // 에너지 값
        else 
        {
            Take_Damaged_Energy(dmgValue, isCritical);
        }
    }

    // 물리 데미지를 받음
    private void Take_Damaged_Physics(float dmgValue, bool isCritical)
    {
        // UI
        VfxManager.instance.SpawnDmgTxtCanvas().Offset_ByPhysicDmg(
            (Vector2)targetObject.transform.position + new Vector2(-0.2f, 0.2f),
            dmgValue, isCritical);

        Add_CurrentHP(-dmgValue);
    }

    // 에너지 데미지를 받음
    private void Take_Damaged_Energy(float dmgValue, bool isCritical)
    {
        // UI
        VfxManager.instance.SpawnDmgTxtCanvas().Offset_ByEnergyDmg(
            (Vector2)targetObject.transform.position + new Vector2(-0.2f, 0.2f),
            dmgValue, isCritical);

        Add_CurrentEP(-dmgValue);
    }

    #endregion

    #region Die

    // 죽음
    protected override void Set_Die()
    {
        base.Set_Die();

        Set_Die_GenItem();
        Set_Die_Effect();
        Set_Die_Data();

        EnemyManager.instance.RemoveEnemy(this, enemyType, Get_ID());
    }

    protected virtual void Set_Die_GenItem()
    {
        EnemyDropItemPercent genP = enemyDropItemPercent;
        Gen_BS(Random.Range(
            genP.bsAmountMinMax.typeBase, genP.bsAmountMinMax.typeSpecial)); // 베터리 조각
        Gen_MS(Random.Range(
            genP.msAmountMinMax.typeBase, genP.msAmountMinMax.typeSpecial)); // 모듈 조각
        Gen_Credit(Random.Range(
            genP.creditAmountMinMax.typeBase, genP.creditAmountMinMax.typeSpecial)); // 크레딧
        Gen_Overrider(Random.Range(
            genP.overriderAmountMinMax.typeBase, genP.overriderAmountMinMax.typeSpecial)); // 오버라이더
        Gen_J(Random.Range(
            genP.jouleAmountMinMax.typeBase, genP.jouleAmountMinMax.typeSpecial)
            * PlayerManager.instance.playerController.spawnESMultiple.actualState); // 줄
        
        // Drop Module Item
        if (DevTool.Is_ChanceSuccess(genP.moduleDropPercent))
            Gen_ModuleItem(DevTool.Get_Rank(genP.moduleRankPercents));
        
        // Drop Keycard
        if (DevTool.Is_ChanceSuccess(genP.keycardDropPercent))
            Gen_KeycardItem(Random.Range(1, StaticResourceManager.instance.ItemReso.keycardIcons.Length));
    }

    private void Set_Die_Effect()
    {
        // Effect
        PlayerManager.instance.cameraController.Play_KillAnim(dur: 0.2f);
        VfxManager.instance.onceTime_AnimGenerator.Anim_Attacked_BigSlice(targetObject.transform.position);
        VfxManager.instance.enemy_ExplImgGenerator.Expl_Enemy(targetObject.transform.position);
    }

    private void Set_Die_Data()
    {
        EndAll_Pattern();
        End_Nav();
        hud.Reset_HUD();

        // Remove
        DevTool.Remove_InList(EnemyManager.instance.currentEnemies, this);

        // Check Room State
        StageManager.instance.CompleteRoomKillAll();

        // Ping
        if (PlayerManager.instance.Is_PingedEnemy(this))
            PlayerManager.instance.SetOff_PingEnemy();
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        sg.sortingOrder = sortingOrder;

        hud.canvas.sortingOrder = sortingOrder;

        PlayerManager.instance.Set_SortingOrderPing(this, sortingOrder);
    }

    #endregion

    #region Pattern

    public void EndAll_Pattern()
    {
        if (currentEnemyPattern != null)
            currentEnemyPattern.End_Pattern();
        
        if (currentPatternCor != null)
            StopCoroutine(currentPatternCor);

        currentContinuousEnemyPattern = null;
        currentEnemyPattern = null;

        lookAtPoint = Vector2.zero;
        lookAtDir = Vector2.zero;
    }

    protected void Start_PatternFromNone()
    {
        orderOfPriorityEnemyPatternList[orderOfPriorityEnemyPatternList.Count - 1].enemyPatternList[0].enemyPatternList[0].Start_Pattern();
    }

    private int Get_NextPatternIndex()
    {
        int orderOfPattern = -1;
        if (currentEnemyPattern != null)
        {
            // 패턴의 순서 값을 저장해 활용
            orderOfPattern = currentContinuousEnemyPattern.enemyPatternList.IndexOf(currentEnemyPattern);

            // 마지막 패턴 이었다면 (끝내기)
            if (orderOfPattern == currentContinuousEnemyPattern.enemyPatternList.Count - 1)
            {
                currentEnemyPattern = null;
                currentContinuousEnemyPattern = null;
                orderOfPattern = -1;
            }
        }
        return orderOfPattern;
    }

    public virtual void Play_Pattern()
    {
        if (isDead) return;

        if (TryPlay_ChargeStatePattern()) return;

        // 이미 있는지 진행 중인 패턴이 있는지 확인
        int orderOfPattern = Get_NextPatternIndex();

        if (orderOfPattern == -1) // 다음 패턴이 => 없다면 랜덤 패턴을 찾아서 실행.
        {
            Play_NewPattern();
        }
        else // => 있다면 다음 패턴을 찾아서 실행.
        {
            Play_NextPattern(orderOfPattern);
        }
    }

    private void Play_NewPattern()
    {
        for (int i = 0; i < orderOfPriorityEnemyPatternList.Count; i++)
        {
            // 같은 우선도에 있는 패턴 랜덤으로 섞기
            List<ContinuousEnemyPattern> patternList = DevTool.Get_ShuffledList(orderOfPriorityEnemyPatternList[i].enemyPatternList);

            // 만약 사용 가능한 패턴이 있다면 시작
            for (int j = 0; j < patternList.Count; j++)
            {
                if (patternList[j].enemyPatternList[0].Can_PlayPattern())
                {
                    currentContinuousEnemyPattern = patternList[j];
                    currentEnemyPattern = patternList[j].enemyPatternList[0];
                    currentEnemyPattern.Start_Pattern();

                    return;
                }
            }
        }
    }

    private void Play_NextPattern(int orderOfPattern)
    {
        currentEnemyPattern = currentContinuousEnemyPattern.enemyPatternList[orderOfPattern + 1];
        currentEnemyPattern.Start_Pattern();
    }

    private bool TryPlay_ChargeStatePattern()
    {
        if (isFullCharge && !isPlayingSpecialPattern) // 풀 차징 시
        {
            isPlayingSpecialPattern = true;
            currentContinuousEnemyPattern = specialPattern;
            currentEnemyPattern = specialPattern.enemyPatternList[0];
            currentEnemyPattern.Start_Pattern();

            return true;
        }

        return false;
    }

    #endregion

}