using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int AllyTypeIndex = 0;

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

    [Space(10)]
    [Header("=== HUD")]
    [SerializeField] private AllyHUDController HUD;

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
    [HideInInspector] protected float ForPlayerDis = 1f;

    // For Enemy
    [HideInInspector] protected EnemyController Enemy;

    [HideInInspector] private IEnumerator ThisMainCor = null;

    [HideInInspector] private readonly float NearPlayerDis = 0.5f;

    // Name
    [HideInInspector] private int NameID = -1;
    [HideInInspector] private List<string> Name = null;


    // Tuner
    [HideInInspector] public static readonly float MinLimitUpgradeValue = 0.01f;
    [HideInInspector] public static readonly float AllyTunerStateMultiple = 0.05f;
    [SerializeField] private AllyState UpgradeAllyState = new AllyState();
    [HideInInspector] private List<AllyBaseTunerData> ThisTunerData;

    [HideInInspector] private Dictionary<string, RefData<float>> UpgradeStateDict;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Player = PlayerManager.Instance.PlayerController;
        DevTool.Add_InList(AllyManager.Instance.AllAllies, this);

        ThisTunerData = new List<AllyBaseTunerData>();

        CurrentHP.Value = MaxHP;
        CurrentEP.Value = 0;

        Set_AllyStateMode(AllyStateMode.Value);

        Offset_UI();
        Offset_Subscribe();
        Offset_TunerUpgrade();
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
            { AllyManager.TunerTypeList[0], UpgradeAllyState.Dmg },
            { AllyManager.TunerTypeList[1], UpgradeAllyState.Rof },
            { AllyManager.TunerTypeList[2], UpgradeAllyState.MovementSpeed },
        };
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
        ActualAllyState = Get_AllState();
        ActualAllyState.Set_ValueLimitRange(MinLimitUpgradeValue);

        FollowInitDelay = 0.4f / ActualAllyState.MovementSpeed.Value;
    }

    public AllyState Get_AllState() // 카드와 업그레이드 모두 적용된 스탯
    {
        return Get_ApplyMultipleState(Get_CardState(), UpgradeAllyState);
    }

    public AllyState Get_CardState() // 카드만 적용된 스탯
    {
        return Get_ApplyMultipleState(MultipleAllyState, AllyManager.Instance.GetAllyState);
    }

    public AllyState Get_UpgradeAllState() // 업그레이드만 카드 적용된 스탯
    {
        return AllyState.Get_Subtraction(Get_AllState(), Get_CardState());
    }


    private AllyState Get_ApplyMultipleState(AllyState _State1, AllyState _State2)
    {
        AllyState result = new AllyState();

        result.MovementSpeed.Value = _State1.MovementSpeed.Value * _State2.MovementSpeed.Value;
        result.Dmg.Value = _State1.Dmg.Value * _State2.Dmg.Value;
        result.Rof.Value = _State1.Rof.Value * _State2.Rof.Value;

        return result;
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
            Add_UpgradeState(ThisTunerData[i].Positive0.Type, Get_TunerState(ThisTunerData[i].Positive0.Rank));
            Add_UpgradeState(ThisTunerData[i].Positive1.Type, Get_TunerState(ThisTunerData[i].Positive1.Rank));
            Add_UpgradeState(ThisTunerData[i].Negative.Type, -Get_TunerState(ThisTunerData[i].Negative.Rank));
        }
    }

    private void Add_UpgradeState(string _Type, float _Value)
    {
        UpgradeStateDict[_Type].Value += _Value;
    }

    private float Get_TunerState(int _Rank)
    {
        return AllyTunerStateMultiple * _Rank;
    }

    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        Set_Name();
    }

    #endregion
}
