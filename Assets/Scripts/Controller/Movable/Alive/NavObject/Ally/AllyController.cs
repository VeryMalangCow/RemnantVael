using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

// Ally는 Follow를 기본으로 가짐 (플레이어에게 가는 것이 필요하기 때문)
public class AllyController : NavObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected AllyState MultipleAllyState;
    [SerializeField] private float MaxHP = 150f;
    [SerializeField] private float MaxEP = 100f;
    [SerializeField] protected float ForEnemyDis = 1.5f;
    [SerializeField] private ReactiveProperty<eAllyStateMode> AllyStateMode = new();

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup ThisSG;
    [SerializeField] private AllySolarController ThisSolar;
    [SerializeField] protected DirectionalAllyTypeImgController ThisDirImg;
    [SerializeField] public AllyBuffController BuffController;

    [Space(10)]
    [Header("=== Visible")]
    [SerializeField] protected Color ThisColor;
    [SerializeField] protected Color ThisExtraColor;
    [SerializeField] protected Gradient ThisExtraGradient;

    [Space(10)]
    [Header("=== HUD")]
    [SerializeField] public AllyHUDController HUD;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite FrontFaceSprite;

    #endregion

    #region - Hide

    // State
    [HideInInspector] protected float FollowInitDelay = 0.2f;

    [HideInInspector] protected AllyState ActualAllyState = new AllyState();

    // For Player
    [HideInInspector] protected PlayerController Player;
    [HideInInspector] protected float ForPlayerDis = 2.5f;

    // For Enemy
    [HideInInspector] protected EnemyController Enemy;

    [HideInInspector] private IEnumerator ThisMainCor = null;

    [HideInInspector] private readonly float NearPlayerDis = 0.5f;

    // Name
    [HideInInspector] private int NameID = -1;
    [HideInInspector] private List<string> Name = null;


    // Tuner
    [HideInInspector] public static readonly float MinLimitUpgradeValue = 0.01f;
    [SerializeField] private AllyState UpgradeAllyState = new AllyState();
    [HideInInspector] private List<AllyBaseTunerData> ThisTunerData;

    [HideInInspector] private Dictionary<string, RefData<float>> UpgradeStateDict;

    // Sync

    // ID, Amount
    // => 현재 가지고 있는 모든 Sync
    [HideInInspector] private Dictionary<int, int> SyncData;
    // ID, PlayerAmount 
    // => 현재 가지고 있는 Sync 중 플레이어가 Sync가 되어 있는                                         
    [HideInInspector] private Dictionary<int, int> ConnectSyncData;
    // ID, PlayerAmount =>
    // => 현재 가지고 있는 Sync 중 플레어가 가지고 있으며, 완성된 Ally Sync          
    [HideInInspector] private Dictionary<int, int> CompletelySyncData; 

    [HideInInspector] public static readonly int SyncMax = 3;
    [HideInInspector] public static readonly int NoneSyncNeedOneBuy = 3;
    [HideInInspector] private int HadNoneSyncAmount = 0;

    // Sync
    private List<IWhenAlly_Start> IWhenAlly_StartList = new List<IWhenAlly_Start>();

    private List<IWhenAlly_Fire> IWhenAlly_FireList = new List<IWhenAlly_Fire>();
    private List<IWhenAlly_AfterFire> IWhenAlly_AfterFireList = new List<IWhenAlly_AfterFire>();

    private List<IWhenAlly_Hit> IWhenAlly_HitList = new List<IWhenAlly_Hit>();
    private List<IWhenAlly_CriticalHit> IWhenAlly_CriticalHitList = new List<IWhenAlly_CriticalHit>();

    private List<IWhenAlly_GetFire> IWhenAlly_GetFireList = new List<IWhenAlly_GetFire>();
    private List<IWhenAlly_GetCold> IWhenAlly_GetColdList = new List<IWhenAlly_GetCold>();
    private List<IWhenAlly_GetElectricity> IWhenAlly_GetElectricityList = new List<IWhenAlly_GetElectricity>();
    private List<IWhenAlly_GetCorrosion> IWhenAlly_GetCorrosionList = new List<IWhenAlly_GetCorrosion>();

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Player = PlayerManager.Instance.PlayerController;
        DevTool.Add_InList(AllyManager.Instance.AllAllies, this);
        ID = AllyManager.Instance.AllAllies.IndexOf(this);

        ThisTunerData = new List<AllyBaseTunerData>();

        CurrentHP.Value = MaxHP;
        CurrentEP.Value = 0;

        Set_AllyStateMode(AllyStateMode.Value);

        Offset_UI();
        Offset_Subscribe();
        Offset_TunerUpgrade(); 
        Offset_SyncUpgrade();
        Offset_Comp();
    }

    private void Offset_UI()
    {
        NameID = AllyManager.Instance.Get_AllyNameID();
        Name = AllyManager.Instance.Get_AllyName(NameID);
        Set_Name();

        HUD.Offset();
    }

    private void Offset_Subscribe()
    {
        AllyStateMode
            .Subscribe(value =>
            {
                ThisSolar.Set_AllyStateMode(value);
            });


        CurrentSP
            .Subscribe(_CurrentSP =>
            {
                HUD.StateUI.SP_ProgressBar.Set_FillImgSmooth(CurrentSP.Value, MaxHP);

                if (CurrentSP.Value <= 0)
                {
                    CurrentSP.Value = 0;
                    HUD.StateUI.SP_ProgressBar.Set_NoNum();
                    HUD.StateUI.HP_ProgressBar.Set_FillImgSmooth(CurrentHP.Value, MaxHP);
                    HUD.StateUI.EP_ProgressBar.Set_FillImgSmooth(CurrentEP.Value, MaxEP);

                    //if (BuffController.ShieldBuff.IsOn)
                    //{ BuffController.ShieldBuff.Remove_AllStack(); }

                }
                else
                {
                    HUD.StateUI.HP_ProgressBar.Set_NoNum();
                    HUD.StateUI.EP_ProgressBar.Set_NoNum();
                }
            });

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HUD.StateUI.HP_ProgressBar.Set_FillImgSmooth(CurrentHP.Value, MaxHP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.HP_ProgressBar.Set_NoNum(); }
            });

        CurrentEP
            .Subscribe(_CurrentEP =>
            {
                HUD.StateUI.EP_ProgressBar.Set_FillImgSmooth(CurrentEP.Value, MaxEP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.EP_ProgressBar.Set_NoNum(); }
            });
    }

    private void Offset_TunerUpgrade()
    {
        UpgradeStateDict = new Dictionary<string, RefData<float>> 
        {
            { AllyManager.StateTypeList[0], UpgradeAllyState.Dmg },
            { AllyManager.StateTypeList[1], UpgradeAllyState.Rof },
            { AllyManager.StateTypeList[2], UpgradeAllyState.MovementSpeed },
            { AllyManager.StateTypeList[3], UpgradeAllyState.AttackSize },
            { AllyManager.StateTypeList[4], UpgradeAllyState.CC },
            { AllyManager.StateTypeList[5], UpgradeAllyState.CD },
        };
    }

    private void Offset_SyncUpgrade()
    {
        IWhenAlly_StartList = new List<IWhenAlly_Start>();

        IWhenAlly_FireList = new List<IWhenAlly_Fire>();
        IWhenAlly_AfterFireList = new List<IWhenAlly_AfterFire>();

        IWhenAlly_HitList = new List<IWhenAlly_Hit>();
        IWhenAlly_CriticalHitList = new List<IWhenAlly_CriticalHit>();

        IWhenAlly_GetFireList = new List<IWhenAlly_GetFire>();
        IWhenAlly_GetColdList = new List<IWhenAlly_GetCold>();
        IWhenAlly_GetElectricityList = new List<IWhenAlly_GetElectricity>();
        IWhenAlly_GetCorrosionList = new List<IWhenAlly_GetCorrosion>();

        SyncData = new Dictionary<int, int>();
        ConnectSyncData = new Dictionary<int, int>();
        CompletelySyncData = new Dictionary<int, int>();
    }

    private void Offset_Comp()
    {
        BuffController.Offset(this);
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        Set_AllState();

        Start_MainCor();
    }

    private void OnDisable()
    {
        //DevTool.Remove_InList(AllyManager.Instance.AllAllies, this);
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        Stop_MainCor();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Play_Movement(Time.fixedDeltaTime);
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Play_Walk(MoveAtDir, ActualAllyState.MovementSpeed.Value, _DeltaTime);
    }

    #endregion

    #region State (Enum)

    protected void Set_AllyStateMode(eAllyStateMode _Mode)
    {
        if (AllyStateMode.Value != _Mode)
        {
            AllyStateMode.Value = _Mode;
            ThisDirImg.Set_Type(_Mode);
        }
    }

    #endregion

    #region Set (State)

    public void Set_AllState() // 카드와 업그레이드 모두 적용
    {
        ActualAllyState = Get_AllBuffedState();
        ActualAllyState.Set_ValueLimitRange(MinLimitUpgradeValue);

        FollowInitDelay = 0.4f / ActualAllyState.MovementSpeed.Value;
    }

    private AllyState Get_AllBuffedState()
    {
        return AllyState.Get_Multiple(Get_AllBasicState(), BuffController.Get_BuffedState());
    }

    public AllyState Get_AllBasicState() // 카드와 업그레이드 모두 적용된 스탯
    {
        return AllyState.Get_Multiple(Get_CardState(), UpgradeAllyState);
    }

    public AllyState Get_CardState() // 카드만 적용된 스탯
    {
        return AllyState.Get_Multiple(MultipleAllyState, AllyManager.Instance.GetAllyState);
    }

    public AllyState Get_UpgradeAllState() // 업그레이드만 카드 적용된 스탯
    {
        return AllyState.Get_Subtraction(Get_AllBasicState(), Get_CardState());
    }

    #endregion

    #region EP

    public void AddCurrentEP(float _AddValue)
    {
        CurrentEP.Value = Math.Clamp(CurrentEP.Value + _AddValue, 0, MaxEP);
    }

    public void TakeDamage(float _DmgValue)
    {
        AddCurrentEP(-_DmgValue);
    }

    #endregion

    #region Play

    public void Start_MainCor()
    {
        if (!gameObject.activeSelf) return;

        ThisMainCor = Play_Main_Cor();
        StartCoroutine(ThisMainCor);
    }

    public void Stop_MainCor()
    {
        if (ThisMainCor == null)
            return;

        StopCoroutine(ThisMainCor);
        ThisMainCor = null;

        if (MoveAtDir != Vector2.zero)
            MoveAtDir = Vector2.zero;
    }

    protected virtual IEnumerator Play_Main_Cor()
    {
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region Is

    protected bool Is_FollowState(Transform _TargetTF, float _Dis, bool _CheckWall)
    {
        bool disCondition = _Dis < Vector2.Distance(_TargetTF.position, this.transform.position);
        bool wallCondition = _CheckWall ? Is_ExistWall(_TargetTF) : false;

        return disCondition || wallCondition;
    }

    protected void Stop_Follow()
    {
        if (MoveAtDir != Vector2.zero)
            MoveAtDir = Vector2.zero;
        
    }

    #endregion

    #region Set

    public void Set_PosRandomNearPlayer()
    {
        transform.position =
            (Vector2)PlayerManager.Instance.PlayerController.transform.position +
            (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * NearPlayerDis);
    }

    public void Set_TargetEnemy(EnemyController _Enemy)
    {
        Enemy = _Enemy;
    }

    #endregion

    #region Set (Sorting)

    public override void Set_SortingOrder(int _SortingOrder)
    {
        ThisSG.sortingOrder = _SortingOrder;

        HUD.ThisCanvas.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Dir

    // Not Normalize (최적화로 정규화를 하지않는 것이 더 도움이 됨)
    public Vector2 Get_ForPlayerDir()
    {
        return (Player.transform.position - this.transform.position);
    }

    public Vector2 Get_ForEnemyDir()
    {
        if (Enemy == null)
            return Vector2.down;

        return (Enemy.transform.position - this.transform.position);
    }


    #endregion

    #region Face

    public Sprite Get_FrontFaceImg()
    {
        return FrontFaceSprite;
    }

    #endregion

    #region Name

    public string Get_Name()
    {
        return Name[GameManager.LanguageID];
    }

    private void Set_Name()
    {
        HUD.Set_Name(Name[GameManager.LanguageID]);
    }

    #endregion

    #region Sync

    public Dictionary<int, int> Get_ThisSyncData()
    {
        return SyncData;
    }

    public void Add_Sync(List<int> _SyncList)
    {
        for (int i = 0; i < _SyncList.Count; i++)
        {
            if (SyncData.ContainsKey(_SyncList[i]))
            {
                int currentAmount = SyncData[_SyncList[i]];

                if (currentAmount < SyncMax) // 추가
                {
                    SyncData[_SyncList[i]] = currentAmount + 1;
                }
                else // 초과라면
                {
                    HadNoneSyncAmount++;
                }
            }
            else
            {
                SyncData.Add(_SyncList[i], 1);
            }
        }

        Set_ActingSync();
    }

    private Dictionary<int, int> Get_ConnectingSync()
    {
        Dictionary<int, int> playerSyncDataDict = ModuleItemManager.Instance.Get_CurrentSyncData();
        List<int> allyIds = SyncData.Keys.ToList();

        Dictionary<int, int> resultSyncDataDict = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> playerSyncData in playerSyncDataDict)
        {
            if (allyIds.Contains(playerSyncData.Key))
            {
                resultSyncDataDict.Add(playerSyncData.Key, SyncData[playerSyncData.Key]);
            }
        }

        return resultSyncDataDict;
    }

    private Dictionary<int, int> Get_CompletelySync()
    {
        Dictionary<int, int> resultSyncDataDict = new Dictionary<int, int>();
        foreach (KeyValuePair<int,  int> connectSync in ConnectSyncData)
        {
            if (SyncData[connectSync.Key] >= SyncMax)
            {
                resultSyncDataDict.Add(connectSync.Key, ModuleItemManager.Instance.Get_CurrentSyncData()[connectSync.Key]);
            }
        }
        return resultSyncDataDict;
    }

    public void Set_ActingSync()
    {
        ConnectSyncData = Get_ConnectingSync();
        CompletelySyncData = Get_CompletelySync();

        Set_Interface();
    }

    public List<int> Get_ConnectingSyncToKeyList()
    {
        return ConnectSyncData.Keys.ToList();
    }

    public List<int> Get_CompletelySyncToKeyList()
    {
        return CompletelySyncData.Keys.ToList();
    }


    #region None Sync

    // 아직 완성하지 못한 Sync 가져오기
    public Dictionary<int, int> Get_NoFullSyncData()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> pair in SyncData)
        {
            if (pair.Value < SyncMax)
            {
                result.Add(pair.Key, pair.Value);
            }
        }
        return result;
    }

    public int Get_HadNoneSyncAmount()
    {
        return HadNoneSyncAmount;
    }

    public void Use_HadNoneSyncAmount(int _Amount)
    {
        HadNoneSyncAmount -= _Amount;
    }

    #endregion

    #endregion

    #region Sync (Interface)

    private void Reset_Interface()
    {
        IWhenAlly_StartList.Clear();

        IWhenAlly_FireList.Clear();
        IWhenAlly_AfterFireList.Clear();

        IWhenAlly_HitList.Clear();
        IWhenAlly_CriticalHitList.Clear();

        IWhenAlly_GetFireList.Clear();
        IWhenAlly_GetColdList.Clear();
        IWhenAlly_GetElectricityList.Clear();
        IWhenAlly_GetCorrosionList.Clear();

        BuffController.Reset_SyncState();
    }

    private void Set_Interface()
    {
        Reset_Interface();

        foreach (KeyValuePair<int, int> syncData in CompletelySyncData) // ID, Amount
        {
            AllySyncState state = AllySyncState.Get_AllSyncState()[syncData.Key];
            state.Set_State(this, syncData.Key, syncData.Value);
            Try_AddIWhenAlly(state);
        }

        ActiveAlly_Start();
    }

    private void Try_AddIWhenAlly(AllySyncState _State)
    {
        if (_State is IWhenAlly_Start iStart) DevTool.Add_InList(IWhenAlly_StartList, iStart);

        else if (_State is IWhenAlly_Fire iFire) DevTool.Add_InList(IWhenAlly_FireList, iFire);
        else if (_State is IWhenAlly_AfterFire iAfterFire) DevTool.Add_InList(IWhenAlly_AfterFireList, iAfterFire);
                                
        else if (_State is IWhenAlly_Hit iHit) DevTool.Add_InList(IWhenAlly_HitList, iHit);
        else if (_State is IWhenAlly_CriticalHit iCriticalHit) DevTool.Add_InList(IWhenAlly_CriticalHitList, iCriticalHit);
                                
        else if (_State is IWhenAlly_GetFire iGetFire) DevTool.Add_InList(IWhenAlly_GetFireList, iGetFire);
        else if (_State is IWhenAlly_GetCold iGetCold) DevTool.Add_InList(IWhenAlly_GetColdList, iGetCold);
        else if (_State is IWhenAlly_GetElectricity iGetElectricity) DevTool.Add_InList(IWhenAlly_GetElectricityList, iGetElectricity);
        else if (_State is IWhenAlly_GetCorrosion iGetCorrosion) DevTool.Add_InList(IWhenAlly_GetCorrosionList, iGetCorrosion);
    }

    #endregion

    #region Active

    public void ActiveAlly_Start()
    {
        ActiveAlly(IWhenAlly_StartList);
    }


    public void ActiveAlly_Fire(BulletController _Bullet)
    {
        ActiveAlly(IWhenAlly_FireList, null, _Bullet);
    }

    public void ActiveAlly_AfterFire()
    {
        ActiveAlly(IWhenAlly_AfterFireList);
    }


    public void ActiveAlly_Hit()
    {
        ActiveAlly(IWhenAlly_HitList);
    }

    public void ActiveAlly_CriticalHit()
    {
        ActiveAlly(IWhenAlly_CriticalHitList);
    }


    public void ActiveAlly_EnemyTakingFire(EnemyController _Enemy)
    {
        ActiveAlly(IWhenAlly_GetFireList, _Enemy);
    }

    public void ActiveAlly_EnemyTakingCold(EnemyController _Enemy)
    {
        ActiveAlly(IWhenAlly_GetColdList, _Enemy);
    }

    public void ActiveAlly_EnemyTakingElectricity(EnemyController _Enemy)
    {
        ActiveAlly(IWhenAlly_GetElectricityList, _Enemy);
    }

    public void ActiveAlly_EnemyTakingCorrosion(EnemyController _Enemy)
    {
        ActiveAlly(IWhenAlly_GetCorrosionList, _Enemy);
    }


    // Base
    private void ActiveAlly<T>(List<T> _IWhenList, EnemyController _Enemy = null, BulletController _Bullet = null) where T : IWhenAlly
    {
        if (_IWhenList.Count <= 0) return;

        for (int i = 0; i < _IWhenList.Count; i++)
            _IWhenList[i].Play_When(_Enemy, _Bullet);
    }


    #endregion

    #region Tuner

    public List<AllyBaseTunerData> Get_ThisTunerData()
    {
        return ThisTunerData;
    }

    public void Add_Tuner(AllyTunerData _Data)
    {
        AllyBaseTunerData addData = new AllyBaseTunerData(_Data);

        ThisTunerData.Add(addData);

        Set_TunerUpgradeState();
        Set_AllState();
    }

    private void Set_TunerUpgradeState()
    {
        UpgradeAllyState.Reset();

        for (int i = 0; i < ThisTunerData.Count; i++)
        {
            Add_UpgradeState(ThisTunerData[i].Positive0.Type, ThisTunerData[i].Positive0.Rank);
            Add_UpgradeState(ThisTunerData[i].Positive1.Type, ThisTunerData[i].Positive1.Rank);
            Add_UpgradeState(ThisTunerData[i].Negative.Type, -ThisTunerData[i].Negative.Rank);
        }
    }

    private void Add_UpgradeState(string _Type, int _Rank)
    {
        UpgradeStateDict[_Type].Value += AllyManager.Get_AllyTunerStateMultiple(_Type) * _Rank;
    }

    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        Set_Name();
    }

    #endregion
}
