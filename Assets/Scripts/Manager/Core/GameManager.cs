using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

#region Class : Caculate : Static

public class DevTool
{
    #region About Math

    // 퍼센트값을 도출
    public static float Get_Percent(float _Percent, float _Value)
    {
        return (_Percent / 100f) * _Value;
    }

    // 확률이 성공했는지를 반환
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

    // 실제 주소값 float에 추가
    public static void Add_RefValue(ref float _Variable, float _AddValue)
    {
        _Variable += _AddValue;
    }

    // X 피벗을 개수와 간격 수치로 계산 (float 반환 값을 모든 값에 빼주면 됨)
    public static float Get_MinusXPivot(float _IntervalX, int _MaxAmount)
    {
        return (_IntervalX / 2) * (_MaxAmount - 1);
    }

    // 
    #endregion

    #region About Casting

    // 'T 타입'이 Null이거나 Defualt가 아닌지?
    public static bool Is_Usable<T>(T _Value)
    {
        if (_Value is not null && !EqualityComparer<T>.Default.Equals(_Value, default))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 'T, U 타입' 두 값이 같은지 판별
    public static bool Is_Equal<T, U>(T _First, U _Second)
    {
        return _First?.Equals(_Second) ?? _Second is null;
    }

    // 'T, U 타입' 사용할 수 있는지 판별, 그 안의 2값이 같은지 판별
    public static bool Is_UsableAndEqual<T, U>(T _Value, U _InValue1, U _InValue2)
    {
        return Is_Equal(_InValue1, _InValue2) && Is_Usable(_Value);
    }


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

    // 게임 오브젝트 만들고, 컴포넌트 추가하기
    public static T Gen_Component<T>(Transform _ParentTF, string _Name) where T : Component
    {
        GameObject go = new GameObject(_Name);
        go.transform.SetParent(_ParentTF);
        T component = go.AddComponent<T>();
        return component;
    }

    // Gen SpriteRenderer
    public static SpriteRenderer Gen_Component_SR(Transform _ParentTF, string _Name, Sprite _Sprite, Material _Material, int _SortingOrder)
    {
        SpriteRenderer sr = Gen_Component<SpriteRenderer>(_ParentTF, _Name);
        Set_ComponentValue(sr, _Sprite, _Material, _SortingOrder);
        return sr;
    }

    // SpriteRenderer Value
    public static void Set_ComponentValue(SpriteRenderer _SR, Sprite _Sprite, Material _Material, int _SortingOrder)
    {
        _SR.sprite = _Sprite;
        _SR.material = _Material;
        _SR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region About List

    // 'T 타입' 리스트에 '새로' 추가 (처음에)
    public static void Add_FirstInListNew<T>(List<T> _TargetList, T _TargetValue)
    {
        Remove_InList(_TargetList, _TargetValue);
        _TargetList.Insert(0, _TargetValue);
    }

    // 'T 타입' 추가 시도 (처음에)
    public static void Add_FirstInList<T>(List<T> _TargetList, T _TargetValue)
    {
        if (!_TargetList.Contains(_TargetValue))
        {
            _TargetList.Insert(0, _TargetValue);
        }
    }

    // 'T 타입' 삭제 시도
    public static void Remove_InList<T>(List<T> _TargetList, T _TargetValue)
    {
        if (_TargetList.Contains(_TargetValue))
        {
            _TargetList.Remove(_TargetValue);
        }
    }

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
            random1 = UnityEngine.Random.Range(0, result.Count);
            random2 = UnityEngine.Random.Range(0, result.Count);

            Set_Swap(result, random1, random2);
        }

        return result;
    }

    // 'T 타입' 리스트의 두 값을 교체
    public static void Set_Swap<T>(List<T> _TargetList, int _Index1, int _Index2)
    {
        T temp = _TargetList[_Index1];
        _TargetList[_Index1] = _TargetList[_Index2];
        _TargetList[_Index2] = temp;
    }

    // 'T 타입' 리스트를 돌면서 실행
    public static void Set_ListDele<T>(List<T> _TargetList, Dele_T<T> _Dele)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _Dele(_TargetList[i]);
        }
    }

    // 'T, U 타입' 리스트를 돌면서 실행
    public static void Set_ListDele<T, U>(List<T> _TargetList, Dele_RefT_U<U, T> _Dele, ref U _Variable)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _Dele(ref _Variable, _TargetList[i]);
        }
    }

    public static void Set_ListDele<T, U>(List<T> _TargetList, Dele_T_U<T, U> _Dele, U _Value)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _Dele(_TargetList[i], _Value);
        }
    }

    #endregion

    #region About Position

    public static Vector2 Get_RandomDir()
    {
        float _X = Random.Range(-1.0f, 1.0f);
        float _Y = Random.Range(-1.0f, 1.0f);
        return new Vector2(_X, _Y).normalized;
    }

    #endregion

    #region About Rotation

    // 좌표값으로 회전축 값 가져오기
    public static Quaternion Get_RotFromDir(Vector2 _Dir)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
    }

    #endregion

    #region About Anim

    // 애니메이션을 코드상으로 변경하는 시스템
    public static void Set_Anim(ref AnimatorOverrideController _AOC, Animator _AT, AnimationClip _AC)
    {
        _AOC = new AnimatorOverrideController(_AT.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in _AOC.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        _AOC.ApplyOverrides(anims);
        _AT.runtimeAnimatorController = _AOC;
    }

    #endregion

}


#endregion

#region Class : State : Element

[System.Serializable]
public abstract class ElementState
{
    public virtual void Reset_State() { }
}

[System.Serializable]
public class DmgState : ElementState
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
public class CriticalState : ElementState
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
public class KnockbackState : ElementState
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

#region Class : State : Combat

[System.Serializable]
public abstract class State
{
    public virtual void Reset_State() { }
}

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
            IsCritical = DevTool.Is_ChanceSuccess(_State.CriticalState.CC);
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
            IsCritical = DevTool.Is_ChanceSuccess(_State.CriticalState.CC);
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
public class AttackerState : CombatState
{
    #region Constructor

    public AttackerState(CombatState _State) : base(_State.DmgState, _State.CriticalState, _State.KnockbackState) { }

    #endregion
}

#endregion

#region Class : State : Player


[System.Serializable]
public class BUState<T>
{
    public T BaseState;
    public ReactiveProperty<int> CurrentLevel;
    public List<T> UpgradeValueByLevelRange;
    public List<int> NeedPayByLevelRange;
    public ReactiveProperty<T> ActualState;

    public List<BuffState<T>> BuffList = new List<BuffState<T>>();

    public string Name;
    [TextArea]
    public string Desc;


    // Gain
    public void GainBuff(BuffState<T> _Bs)
    {
        if (!BuffList.Contains(_Bs))
        {
            BuffList.Add(_Bs);
        }
    }

    // Remove
    public void RemoveBuff(BuffState<T> _Bs)
    {
        if (BuffList.Contains(_Bs))
        {
            BuffList.Remove(_Bs);
        }
    }

    public T BuffedState
    { get; set; }



    public void SetBuffedState()
    {
        if (ActualState.Value.GetType() == typeof(float))
        {
            float state = 1.0f;
            for (int i = 0; i < BuffList.Count; i++)
            {
                state += float.Parse(BuffList[i].ActualValue.ToString());
            }
            state *= float.Parse(ActualState.Value.ToString());
            BuffedState = (T)(object)state;
            return;
        }

#if UNITY_EDITOR
        Debug.Assert(false, "Player Buff의 자료형이 구현되어 있지 않습니다.");
#endif

        return;
    }

}


[System.Serializable]
public class Shield
{
    public string ShieldID;
    public float ShieldMaxValue;
    public float ShieldCurrentValue;
}


[System.Serializable]
public class BuffState<T>
{
    public string BuffID;
    public T BaseValue;
    public T ActualValue;
}

#endregion

#region Class : Visual

[System.Serializable]
public class PlayerVisual<T>
{
    [SerializeField] public CoupleData<T> Physics;
    [SerializeField] public CoupleData<T> Energy;

    public CoupleData<T> Get_CorrectType(eDamageType _DmgType)
    {
        if (_DmgType == eDamageType.Physics)
        {
            return Physics;
        }
        else
        {
            return Energy;
        }
    }
}


#endregion

#region Class : PublicData

[System.Serializable]
public class TrioData<T>
{
    [SerializeField] public T TypeA;
    [SerializeField] public T TypeSpecial;
    [SerializeField] public T TypeB;
}

[System.Serializable]
public class CoupleData<T>
{
    [SerializeField] public T TypeBase;
    [SerializeField] public T TypeSpecial;

    public T Get_Special(bool _Yes)
    {
        if (_Yes)
        {
            return TypeSpecial;
        }
        else
        {
            return TypeBase;
        }
    }
}

[System.Serializable]
public class CooltimeData
{
    [SerializeField] public float CurrentCooltime = 0;
    [SerializeField] public float MaxCooltime = 0;
}

#endregion

#region Interface : Interact


public interface IInteract
{
    public void Play_Interact();
}

#endregion

#region Interface : When

public interface IWhen
{
    public abstract void Play_When(EnemyController _EC = null);
}

public interface IWhen_Fire : IWhen { }

public interface IWhen_Hit : IWhen { }

public interface IWhen_CriticalHit : IWhen { }

public interface IWhen_GetElectricity : IWhen { }

#endregion

#region Delegate

public delegate void Dele_T<T>(T _Item);
public delegate void Dele_RefT_T<T>(ref T _Item1, T _Item2);
public delegate void Dele_T_U<T, U>(T _Item1, U _Item2);
public delegate void Dele_RefT_U<T, U>(ref T _Item1, U _Item2);


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
