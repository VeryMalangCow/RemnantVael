using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    #region Value

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] public Color RandomColor = Color.red;
    [HideInInspector] private Sequence RandomColorSetSeq;

    [Space(10)]
    [Header("=== Passing Data")]
    [SerializeField] public GameObject DesignatedPlayerPrefab;
    [SerializeField] public GameObject TitlePlayerPrefab;

    [Space(10)]
    [Header("=== Intro")]
    [SerializeField] public bool WasWatched = false;

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        
        Set_BaseOption();
        Set_RainbowColorDotween();
    }

    #endregion

    #region Option

    private void Set_BaseOption()
    {
        Application.targetFrameRate = 144;
    }

    #endregion

    #region Set

    // 무지개 컬러 Dotween
    private void Set_RainbowColorDotween()
    {
        RandomColorSetSeq = DOTween.Sequence();
        RandomColor = Color.red;

        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 0, 1), 0.5f).SetEase(Ease.Linear));

        RandomColorSetSeq.SetLoops(-1, LoopType.Restart);
    }

    #endregion
}

#region Static Caculate

public class StaticCaculator
{
    #region About Math

    public static bool Is_ChanceSuccess(float _Chance)
    {
        if (UnityEngine.Random.Range(0f, 1f) < _Chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region About Casting

    // 객체를 원하는 'T 타입'으로 캐스팅
    public static T Get_CastingTType<T>(object _Obj) where T : class
    {
        if (_Obj != null && _Obj is T objType)
        {
            return objType;
        }
        return default;
    }

    #endregion

    #region About Component

    // 객체에 'T 타입'이 있다면 변수에 할당
    public static void Set_ComponentTType<T>(ref T _Variable, GameObject _TargetGO) where T : Component
    {
        if (_Variable == null && _TargetGO.TryGetComponent(out T tTypeComponent))
        {
            _Variable = tTypeComponent;
        }
    }

    #endregion

    #region About List

    // 자식 객체들의 'T 타입' 리스트 가져오기
    public static List<T> Get_ChildList<T>(Transform _Parent) where T : Component
    {
        List<T> result = new List<T>();
        foreach (Transform TF in _Parent)
        {
            if (TF.TryGetComponent(out T type))
            {
                result.Add(type);
            }
        }
        return result;
    }

    // 'T 타입'의 List를 무작위 섞기
    public static List<T> Get_ShuffledList<T>(List<T> _TargetList)
    {
        List<T> result = new List<T>(_TargetList);

        int random1, random2;

        for (int i = 0; i < result.Count; ++i)
        {
            random1 = Random.Range(0, result.Count);
            random2 = Random.Range(0, result.Count);

            Set_Swap(result, random1, random2);
        }

        return result;
    }

    // 'T 타입'리스트의 두 값을 교체
    public static void Set_Swap<T>(List<T> _TargetList, int _Index1, int _Index2)
    {
        T temp = _TargetList[_Index1];
        _TargetList[_Index1] = _TargetList[_Index2];
        _TargetList[_Index2] = temp;
    }

    #endregion
}

#endregion

#region Enum

public enum eCombatMode
{ 
    Physics, Energy, Boost
}

public enum eMovementState
{
    Casting, IdleOrWalk, Dash
}

public enum eDamageType
{
    Physics, Energy
}

public enum eStatusEffect
{
    Flame, Cold, Electricity, Corrosion
}

public enum eEnemy
{
    Normal, Elite, SemiBoss, Boss
}

public enum eDashStyle
{
    OneWay, CanInputWay, Teleport
}

public enum eRoomType
{
    Completed, KillAll, Survived, BossKill
}



#endregion

#region Interface


public interface IInteract
{
    public void Play_Interact();
}


public interface IWhen
{
    public abstract void Play_When(EnemyController _EC = null);
}

public interface IWhen_Fire : IWhen { }

public interface IWhen_Hit : IWhen { }

public interface IWhen_CriticalHit : IWhen { }

public interface IWhen_GetElectricity : IWhen { }

#endregion

#region State Element

[System.Serializable]
public abstract class State
{
    public virtual void Reset_State() { }
}

[System.Serializable]
public class DmgState : State
{
    #region Value

    [SerializeField] public eDamageType DmgType;
    [SerializeField] public float Dmg;

    #endregion

    #region Constructor

    public DmgState(DmgState _State)
    {
        DmgType = _State.DmgType;
        Dmg = _State.Dmg;
    }

    public DmgState(eDamageType _DmgType, float _Dmg)
    {
        DmgType = _DmgType;
        Dmg = _Dmg;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        DmgType = eDamageType.Physics;
        Dmg = 0;
    }

    #endregion
}

[System.Serializable]
public class CriticalState : State
{
    #region Value

    [SerializeField] public float CC;
    [SerializeField] public float CD;

    #endregion

    #region Constructor

    public CriticalState(CriticalState _State)
    {
        CC = _State.CC;
        CD = _State.CD;
    }

    public CriticalState(float _CC, float _CD)
    {
        CC = _CC;
        CD = _CD;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        CC = 0;
        CD = 0;
    }

    #endregion
}

[System.Serializable]
public class KnockbackState : State
{
    #region Value

    [SerializeField] public bool CanKB;
    [SerializeField] public float KBPower;
    [SerializeField] public float KBTime;

    #endregion

    #region Constructor

    public KnockbackState(KnockbackState _State)
    {
        CanKB = _State.CanKB;
        KBPower = _State.KBPower;
        KBTime = _State.KBTime;
    }

    public KnockbackState(bool _CanKB, float _KBPower, float _KBTime)
    {
        CanKB = _CanKB;
        KBPower = _KBPower;
        KBTime = _KBTime;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        CanKB = false;
        KBPower = 0;
        KBTime = 0;
    }

    #endregion
}

#endregion

#region Combat State

[System.Serializable]
public class CombatState : State
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Combat State")]

    [Space(10)]
    [Header("=== Damage")]
    [SerializeField] public DmgState DmgState;

    [Space(10)]
    [Header("=== Critical")]
    [SerializeField] public CriticalState CriticalState;

    [Space(10)]
    [Header("=== Knockback")]
    [SerializeField] public KnockbackState KnockbackState;

    #endregion

    #region Constructor

    public CombatState(CombatState _State)
    {
        DmgState = new DmgState(_State.DmgState);
        CriticalState = new CriticalState(_State.CriticalState);
        KnockbackState = new KnockbackState(_State.KnockbackState);
    }

    public CombatState(DmgState _DmgState, CriticalState _CriticalState, KnockbackState _KnockbackState)
    {
        DmgState = new DmgState(_DmgState);
        CriticalState = new CriticalState(_CriticalState);
        KnockbackState = new KnockbackState(_KnockbackState);
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        DmgState.Reset_State();
        CriticalState.Reset_State();
        KnockbackState.Reset_State();
    }

    #endregion
}

[System.Serializable]
public class BulletState : CombatState
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bullet State")]
    [SerializeField] public bool IsCritical;
    [SerializeField] public float MuzzleSpeed;
    [SerializeField] public float AliveTime;

    #endregion

    #region Constructor

    public BulletState(CombatState _State, bool _CheckIsCritical, float _MuzzleSpeed, float _AliveTime) : base(_State)
    {
        if (_CheckIsCritical)
        {
            IsCritical = StaticCaculator.Is_ChanceSuccess(_State.CriticalState.CC);
        }
        else
        {
            IsCritical = false;
        }

        MuzzleSpeed = _MuzzleSpeed;
        AliveTime = _AliveTime;
    }

    public BulletState(BulletState _State, bool _CheckIsCritical) : base(_State.DmgState, _State.CriticalState, _State.KnockbackState)
    {
        if (_CheckIsCritical)
        {
            IsCritical = StaticCaculator.Is_ChanceSuccess(_State.CriticalState.CC);
        }
        else
        {
            IsCritical = _State.IsCritical;
        }

        MuzzleSpeed = _State.MuzzleSpeed;
        AliveTime = _State.AliveTime;
    }


    #endregion

    #region Reset

    public override void Reset_State()
    {
        base.Reset_State();

        IsCritical = false;
        MuzzleSpeed = 0;
        AliveTime = 0;
    }

    #endregion
}

[System.Serializable]
public class AttackerState
{
    [SerializeField] public eDamageType DmgType;
    [SerializeField] public float Dmg;

    [SerializeField] public bool CanKB;
    [SerializeField] public float KBPower;
    [SerializeField] public float KBTime;

    [SerializeField] public float CC;
    [SerializeField] public float CD;

    public AttackerState() { }

    public AttackerState(AttackerState _AttakerState)
    {
        DmgType = _AttakerState.DmgType;
        Dmg = _AttakerState.Dmg;

        CanKB = _AttakerState.CanKB;
        KBPower = _AttakerState.KBPower;
        KBTime = _AttakerState.KBTime;

        CC = _AttakerState.CC;
        CD = _AttakerState.CD;
    }
    public AttackerState(eDamageType _DamageType, float _BaseDamage, bool _AbleKnockback, float _KnockbackPower, float _KnockbackTime, float _CC, float _CD)
    {
        DmgType = _DamageType;
        Dmg = _BaseDamage;

        CanKB = _AbleKnockback;
        KBPower = _KnockbackPower;
        KBTime = _KnockbackTime;

        CC = _CC;
        CD = _CD;
    }
}

#endregion


