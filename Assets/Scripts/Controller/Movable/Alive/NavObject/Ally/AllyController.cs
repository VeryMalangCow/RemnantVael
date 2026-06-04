using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class AllyController : NavObjectController, IPoolable
{
    #region Value


    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected AllyState multipleAllyState;
    [SerializeField] protected float maxHP = 150f;
    [HideInInspector] protected static readonly float maxEP = 100f;
    [SerializeField] protected ReactiveProperty<eAllyStateMode> allyStateMode = new();

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public AllyBuffController buffController;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite frontFaceSprite;

    [Space(10)]
    [Header("=== Visible")]
    [SerializeField] protected Color clr;
    [SerializeField] protected Color extraClr;
    [SerializeField] protected Gradient extraGradient;

    [Space(10)]
    [Header("=== HUD")]
    [SerializeField] public AllyHUDController hud;

    #endregion

    #region - Hide

    // State
    [HideInInspector] protected AllyState actualAllyState = new AllyState();
    [HideInInspector] public AllyState Get_ActaulAllyState() => actualAllyState;

    // Player
    [HideInInspector] protected PlayerController player;

    // Enemy
    [HideInInspector] protected EnemyController enemy;

    // Main Cor
    [HideInInspector] private IEnumerator thisMainCor = null;


    // Name
    [HideInInspector] protected int nameId = -1;
    [HideInInspector] protected string[] _name = null;


    // Sync
    private List<IWhenAlly_Start> iWhenAlly_StartList = new List<IWhenAlly_Start>();

    private List<IWhenAlly_Fire> iWhenAlly_FireList = new List<IWhenAlly_Fire>();
    private List<IWhenAlly_AfterFire> iWhenAlly_AfterFireList = new List<IWhenAlly_AfterFire>();

    private List<IWhenAlly_Hit> iWhenAlly_HitList = new List<IWhenAlly_Hit>();
    private List<IWhenAlly_CriticalHit> iWhenAlly_CriticalHitList = new List<IWhenAlly_CriticalHit>();

    private List<IWhenAlly_GetFire> iWhenAlly_GetFireList = new List<IWhenAlly_GetFire>();
    private List<IWhenAlly_GetCold> iWhenAlly_GetColdList = new List<IWhenAlly_GetCold>();
    private List<IWhenAlly_GetElectricity> iWhenAlly_GetElectricityList = new List<IWhenAlly_GetElectricity>();
    private List<IWhenAlly_GetCorrosion> iWhenAlly_GetCorrosionList = new List<IWhenAlly_GetCorrosion>();


    // ID, Amount
    // => 현재 가지고 있는 모든 Sync
    [HideInInspector] private Dictionary<int, int> syncData;
    // ID, PlayerAmount 
    // => 현재 가지고 있는 Sync 중 플레이어가 Sync가 되어 있는                                         
    [HideInInspector] private Dictionary<int, int> connectSyncData;
    // ID, PlayerAmount =>
    // => 현재 가지고 있는 Sync 중 플레어가 가지고 있으며, 완성된 Ally Sync          
    [HideInInspector] private Dictionary<int, int> completelySyncData;

    [HideInInspector] public static readonly int syncMax = 3;
    [HideInInspector] public static readonly int noneSyncNeedOneBuy = 3;
    [HideInInspector] private int hadNoneSyncAmount = 0;

    // Tuner
    [HideInInspector] public static readonly float minLimitUpgradeValue = 0.01f;
    [SerializeField] private AllyState upgradeAllyState = new AllyState();
    [HideInInspector] private List<AllyBaseTunerData> thisTunerData;

    [HideInInspector] private Dictionary<string, RefData<float>> upgradeStateDict;

    // Ally Request
    [HideInInspector] private AllyRequest request = null;

    // Pool
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
        Set_SpawnFirst();
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

        Offset_Base();
        Offset_SyncUpgrade();
        Offset_TunerUpgrade();
        Offset_Name();
        Offset_Subscribe();
    }

    private void Offset_Base()
    {
        currentHP.Value = maxHP;
        SetCurrentEp(0f);

        player = PlayerManager.instance.playerController;

        DevTool.Add_InList(AllyManager.instance.allAlly, this);
        id = AllyManager.instance.allAlly.IndexOf(this);

        Set_AllyStateMode(allyStateMode.Value);

        buffController.Offset(this);
    }

    private void Offset_SyncUpgrade()
    {
        iWhenAlly_StartList = new List<IWhenAlly_Start>();

        iWhenAlly_FireList = new List<IWhenAlly_Fire>();
        iWhenAlly_AfterFireList = new List<IWhenAlly_AfterFire>();

        iWhenAlly_HitList = new List<IWhenAlly_Hit>();
        iWhenAlly_CriticalHitList = new List<IWhenAlly_CriticalHit>();

        iWhenAlly_GetFireList = new List<IWhenAlly_GetFire>();
        iWhenAlly_GetColdList = new List<IWhenAlly_GetCold>();
        iWhenAlly_GetElectricityList = new List<IWhenAlly_GetElectricity>();
        iWhenAlly_GetCorrosionList = new List<IWhenAlly_GetCorrosion>();

        syncData = new Dictionary<int, int>();
        connectSyncData = new Dictionary<int, int>();
        completelySyncData = new Dictionary<int, int>();
    }

    private void Offset_TunerUpgrade()
    {
        thisTunerData = new List<AllyBaseTunerData>();

        upgradeStateDict = new Dictionary<string, RefData<float>>
        {
            { AllyManager.stateTypeList[0], upgradeAllyState.dmg },
            { AllyManager.stateTypeList[1], upgradeAllyState.rof },
            { AllyManager.stateTypeList[2], upgradeAllyState.movementSpeed },
            { AllyManager.stateTypeList[3], upgradeAllyState.attackSize },
            { AllyManager.stateTypeList[4], upgradeAllyState.criticalChacne },
            { AllyManager.stateTypeList[5], upgradeAllyState.criticalDmg },
            { AllyManager.stateTypeList[6], upgradeAllyState.muzzleSpeed },
            { AllyManager.stateTypeList[7], upgradeAllyState.kbPower },
            { AllyManager.stateTypeList[8], upgradeAllyState.dur }
        };
    }

    private void Offset_Name()
    {
        nameId = AllyManager.instance.Get_AllyNameID();
        _name = AllyManager.instance.Get_AllyName(nameId);

        Set_Name();
    }

    private void Offset_Subscribe()
    {
        hud.Offset();

        hud.stateUi.spProgressBar.SetFillImgSmooth(currentSP.Value, maxHP);
        hud.stateUi.hpProgressBar.SetFillImgSmooth(currentHP.Value, maxHP);
        hud.stateUi.epProgressBar.SetFillImgSmooth(currentEp, maxEP);

        currentSP
            .Subscribe(_CurrentSP =>
            {
                hud.stateUi.spProgressBar.SetFillImgSmooth(currentSP.Value, maxHP);

                if (currentSP.Value <= 0)
                {
                    currentSP.Value = 0;
                    hud.stateUi.spProgressBar.SetNoNum();
                    hud.stateUi.hpProgressBar.SetFillImgSmooth(currentHP.Value, maxHP);
                    hud.stateUi.epProgressBar.SetFillImgSmooth(currentEp, maxEP);
                }
                else
                {
                    hud.stateUi.hpProgressBar.SetNoNum();
                    hud.stateUi.epProgressBar.SetNoNum();
                }
            });

        currentHP
            .Subscribe(_CurrentHP =>
            {
                hud.stateUi.hpProgressBar.SetFillImgSmooth(currentHP.Value, maxHP);

                if (currentSP.Value > 0)
                { hud.stateUi.hpProgressBar.SetNoNum(); }
            });

        //currentEp
        //    .Subscribe(_CurrentEP =>
        //    {
        //        hud.stateUi.epProgressBar.Set_FillImgSmooth(currentEP.Value, maxEP);
        //
        //        if (currentSP.Value > 0)
        //        { hud.stateUi.epProgressBar.Set_NoNum(); }
        //    });
    }

    #endregion

    #region Framework

    private void Awake()
    {
        StartNew_Request();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Set_AllState();

        Start_MainCor();

        AddSortingLayer();
    }

    protected virtual void OnDisable()
    {
        Stop_MainCor();

        RemoveSortingLayer();
    }

    #endregion

    #region Enemy

    public void Set_TargetEnemy(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    #endregion

    #region State (Enum)

    protected virtual void Set_AllyStateMode(eAllyStateMode mode)
    {
        if (allyStateMode.Value != mode)
        {
            allyStateMode.Value = mode;
        }
    }

    #endregion

    #region Face

    public Sprite Get_FrontFaceImg()
    {
        return frontFaceSprite;
    }

    #endregion

    #region Play

    public virtual void Start_MainCor()
    {
        if (!gameObject.activeSelf) return;

        thisMainCor = Play_Main_Cor();
        StartCoroutine(thisMainCor);
    }

    public void Stop_MainCor()
    {
        if (thisMainCor == null)
            return;

        StopCoroutine(thisMainCor);
        thisMainCor = null;

        if (moveAtDir != Vector2.zero)
            moveAtDir = Vector2.zero;
    }

    protected virtual IEnumerator Play_Main_Cor()
    {
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region Name

    public string Get_Name()
    {
        return _name[GameManager.languageID];
    }

    protected virtual void Set_Name()
    {
        hud.Set_Name(_name[GameManager.languageID]);
    }


    #endregion

    #region Set

    public virtual void Set_SpawnFirst()
    {

    }

    #endregion

    #region Set (State)

    public virtual void Set_AllState() // 카드와 업그레이드 모두 적용
    {
        actualAllyState = Get_AllBuffedState();
        actualAllyState.Set_ValueLimitRange(minLimitUpgradeValue);
    }

    private AllyState Get_AllBuffedState()
    {
        return AllyState.Get_Multiple(Get_AllBasicState(), buffController.Get_BuffedState());
    }

    public AllyState Get_AllBasicState() // 카드와 업그레이드 모두 적용된 스탯
    {
        return AllyState.Get_Multiple(Get_CardState(), upgradeAllyState);
    }

    public AllyState Get_CardState() // 카드만 적용된 스탯
    {
        return AllyState.Get_Multiple(multipleAllyState, AllyManager.instance.getAllyState);
    }

    public AllyState Get_UpgradeAllState() // 업그레이드만 카드 적용된 스탯
    {
        return AllyState.Get_Subtraction(Get_AllBasicState(), Get_CardState());
    }

    #endregion

    #region Tuner

    public List<AllyBaseTunerData> Get_ThisTunerData()
    {
        return thisTunerData;
    }

    public void Add_Tuner(AllyTunerData data)
    {
        AllyBaseTunerData addData = new AllyBaseTunerData(data);

        thisTunerData.Add(addData);

        Set_TunerUpgradeState();
        Set_AllState();
    }

    private void Set_TunerUpgradeState()
    {
        upgradeAllyState.Reset();

        for (int i = 0; i < thisTunerData.Count; i++)
        {
            Add_UpgradeState(thisTunerData[i].positive0.type, thisTunerData[i].positive0.rank);
            Add_UpgradeState(thisTunerData[i].positive1.type, thisTunerData[i].positive1.rank);
            Add_UpgradeState(thisTunerData[i].negative.type, -thisTunerData[i].negative.rank);
        }
    }

    private void Add_UpgradeState(string type, int rank)
    {
        upgradeStateDict[type].value += AllyManager.Get_AllyTunerStateMultiple(type) * rank;
    }

    #endregion

    #region Sync

    public Dictionary<int, int> Get_ThisSyncData()
    {
        return syncData;
    }

    public void Add_Sync(List<int> syncList)
    {
        for (int i = 0; i < syncList.Count; i++)
        {
            if (syncData.ContainsKey(syncList[i]))
            {
                int currentAmount = syncData[syncList[i]];

                if (currentAmount < syncMax) // 추가
                {
                    syncData[syncList[i]] = currentAmount + 1;
                }
                else // 초과라면
                {
                    hadNoneSyncAmount++;
                }
            }
            else
            {
                syncData.Add(syncList[i], 1);
            }
        }

        Set_ActingSync();
    }

    private Dictionary<int, int> Get_ConnectingSync()
    {
        Dictionary<int, int> playerSyncDataDict = ModuleItemManager.instance.Get_CurrentSyncData();
        List<int> allyIds = syncData.Keys.ToList();

        Dictionary<int, int> resultSyncDataDict = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> playerSyncData in playerSyncDataDict)
        {
            if (allyIds.Contains(playerSyncData.Key))
            {
                resultSyncDataDict.Add(playerSyncData.Key, syncData[playerSyncData.Key]);
            }
        }

        return resultSyncDataDict;
    }

    private Dictionary<int, int> Get_CompletelySync()
    {
        Dictionary<int, int> resultSyncDataDict = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> connectSync in connectSyncData)
        {
            if (syncData[connectSync.Key] >= syncMax)
            {
                resultSyncDataDict.Add(connectSync.Key, ModuleItemManager.instance.Get_CurrentSyncData()[connectSync.Key]);
            }
        }
        return resultSyncDataDict;
    }

    public void Set_ActingSync()
    {
        connectSyncData = Get_ConnectingSync();
        completelySyncData = Get_CompletelySync();

        Set_Interface();
    }

    public List<int> Get_ConnectingSyncToKeyList()
    {
        return connectSyncData.Keys.ToList();
    }

    public List<int> Get_CompletelySyncToKeyList()
    {
        return completelySyncData.Keys.ToList();
    }


    #region None Sync

    // 아직 완성하지 못한 Sync 가져오기
    public Dictionary<int, int> Get_NoFullSyncData()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> pair in syncData)
        {
            if (pair.Value < syncMax)
            {
                result.Add(pair.Key, pair.Value);
            }
        }
        return result;
    }

    public int Get_HadNoneSyncAmount()
    {
        return hadNoneSyncAmount;
    }

    public void Use_HadNoneSyncAmount(int amount)
    {
        hadNoneSyncAmount -= amount;
    }

    #endregion

    #endregion

    #region Active

    public void ActiveAlly_Start()
    {
        ActiveAlly(iWhenAlly_StartList);
    }


    public void ActiveAlly_Fire(BulletController bullet, DroppingBombController droppingBullet)
    {
        ActiveAlly(iWhenAlly_FireList, null, bullet, droppingBullet);
    }

    public void ActiveAlly_AfterFire()
    {
        ActiveAlly(iWhenAlly_AfterFireList);
    }


    public void ActiveAlly_Hit()
    {
        ActiveAlly(iWhenAlly_HitList);
    }

    public void ActiveAlly_CriticalHit()
    {
        ActiveAlly(iWhenAlly_CriticalHitList);
    }


    public void ActiveAlly_EnemyTakingFire(EnemyController enemy)
    {
        ActiveAlly(iWhenAlly_GetFireList, enemy);
    }

    public void ActiveAlly_EnemyTakingCold(EnemyController enemy)
    {
        ActiveAlly(iWhenAlly_GetColdList, enemy);
    }

    public void ActiveAlly_EnemyTakingElectricity(EnemyController enemy)
    {
        ActiveAlly(iWhenAlly_GetElectricityList, enemy);
    }

    public void ActiveAlly_EnemyTakingCorrosion(EnemyController enemy)
    {
        ActiveAlly(iWhenAlly_GetCorrosionList, enemy);
    }


    // Base
    private void ActiveAlly<T>(List<T> iWhenList, EnemyController enemy = null, BulletController bullet = null, DroppingBombController droppingBullet = null) where T : IWhenAlly
    {
        if (iWhenList.Count <= 0) return;

        for (int i = 0; i < iWhenList.Count; i++)
            iWhenList[i].Play_When(enemy, bullet);
    }


    #endregion

    #region Sync (Interface)

    private void Reset_Interface()
    {
        iWhenAlly_StartList.Clear();

        iWhenAlly_FireList.Clear();
        iWhenAlly_AfterFireList.Clear();

        iWhenAlly_HitList.Clear();
        iWhenAlly_CriticalHitList.Clear();

        iWhenAlly_GetFireList.Clear();
        iWhenAlly_GetColdList.Clear();
        iWhenAlly_GetElectricityList.Clear();
        iWhenAlly_GetCorrosionList.Clear();

        buffController.Reset_SyncState();
    }

    private void Set_Interface()
    {
        Reset_Interface();

        foreach (KeyValuePair<int, int> syncData in completelySyncData) // ID, Amount
        {
            AllySyncState state = AllySyncState.Get_AllSyncState()[syncData.Key];
            state.Set_State(this, syncData.Key, syncData.Value);
            Try_AddIWhenAlly(state);
        }

        ActiveAlly_Start();
    }

    private void Try_AddIWhenAlly(AllySyncState state)
    {
        if (state is IWhenAlly_Start iStart) DevTool.Add_InList(iWhenAlly_StartList, iStart);

        else if (state is IWhenAlly_Fire iFire) DevTool.Add_InList(iWhenAlly_FireList, iFire);
        else if (state is IWhenAlly_AfterFire iAfterFire) DevTool.Add_InList(iWhenAlly_AfterFireList, iAfterFire);

        else if (state is IWhenAlly_Hit iHit) DevTool.Add_InList(iWhenAlly_HitList, iHit);
        else if (state is IWhenAlly_CriticalHit iCriticalHit) DevTool.Add_InList(iWhenAlly_CriticalHitList, iCriticalHit);

        else if (state is IWhenAlly_GetFire iGetFire) DevTool.Add_InList(iWhenAlly_GetFireList, iGetFire);
        else if (state is IWhenAlly_GetCold iGetCold) DevTool.Add_InList(iWhenAlly_GetColdList, iGetCold);
        else if (state is IWhenAlly_GetElectricity iGetElectricity) DevTool.Add_InList(iWhenAlly_GetElectricityList, iGetElectricity);
        else if (state is IWhenAlly_GetCorrosion iGetCorrosion) DevTool.Add_InList(iWhenAlly_GetCorrosionList, iGetCorrosion);
    }

    #endregion

    #region HP

    public void TakeDamage(float dmgValue)
    {
        Add_CurrentHP(-dmgValue, maxHP);
    }

    #endregion

    #region Trust (EP)

    public void Gain_Trust(float value)
    {
        AddCurrentEp(value, maxEP);
    }

    public void Reduce_Trust(float value)
    {
        AddCurrentEp(-value, maxEP);
    }


    #endregion

    #region Request

    public void StartNew_Request()
    {
        request = AllyRequest.Get_AllyRequestType(this);

        hud.requestUi.Set_RequestTxt_Language(request);
    }


    public void DataOff_Request()
    {
        request = null;
        StartNew_Request();
    }

    #endregion

    #region Set (Language)

    public virtual void Set_Language()
    {
        Set_Name();
        hud.requestUi.Set_RequestTxt_Language(request);
    }

    #endregion
}
