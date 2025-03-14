using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    #region Value


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
    }

    #endregion

    #region Option

    private void Set_BaseOption()
    {
        Application.targetFrameRate = 144;
    }

    #endregion
}

#region ========== DEV TOOL

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
        if (Random.Range(0f, 1f) < _Chance)
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

    // float 랜덤값을 0을 기준으로 돌리기 (음수 양수의 범위)
    public static float Get_RandomValueBaseZero(float _RandomExtent)
    {
        if (_RandomExtent == 0)
        { 
            return 0;
        }
        else
        {
            return Random.Range(-_RandomExtent * 0.5f, _RandomExtent * 0.5f);
        }
    }


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
    public static T Get_CastingTType<T>(object _Obj)
    {
        if (_Obj != null && _Obj is T objType)
        {
            return objType;
        }
        return default;
    }
    public static bool Can_CastingTType<T>(object _Obj, out T _TType) where T : class
    {
        if (_Obj != null && _Obj is T objType)
        {
            _TType = objType;
            return true;
        }

        _TType = null;
        return false;
    }

    // 객체를 원하는 'T 타입'으로 Out 빼기
    public static bool Can_CastingTType<T>(object _Obj)
    {
        if (_Obj != null && _Obj is T)
        {
            return true;
        }
        return false;
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
    public static T Get_ComponentTType<T>(GameObject _TargetGO)
    {
        if (_TargetGO != null && _TargetGO.TryGetComponent(out T tTypeComponent))
        {
            return tTypeComponent;
        }
        return default;
    }

    public static bool Get_ComponentTType<T>(GameObject _TargetGO, out T _TType)
    {
        if (_TargetGO != null && _TargetGO.TryGetComponent(out T tTypeComponent))
        {
            _TType = tTypeComponent;
            return true;
        }
        _TType = default;
        return false;
    }

    // 게임 오브젝트 만들고, 컴포넌트 추가하기
    public static T Gen_Component<T>(Transform _ParentTF, string _Name) where T : Component
    {
        GameObject go = new GameObject(_Name);
        go.transform.SetParent(_ParentTF);
        T component = go.AddComponent<T>();
        return component;
    }

    // 게임 오브젝트에 컴포넌트 추가하기
    public static T Gen_Component<T>(GameObject _TargetGO) where T : Component
    {
        T component = _TargetGO.AddComponent<T>();
        return component;
    }

    // 게임 오브젝트에 컴포넌트 삭제하기
    public static void Remove_Component<T>(T _TargetComp) where T : Component
    {
        if (_TargetComp != null)
        {
            Object.Destroy(_TargetComp);
        }
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


    // 구조체 값에서 값을 넣기
    public static void Set_TF_FromStruct(Transform _TF, State_TF2D _StructTF2D)
    {
        _TF.position = _StructTF2D.Pos;
        _TF.rotation = _StructTF2D.Rot;
        _TF.localScale = _StructTF2D.LocalScale;
    }

    public static void Set_MatAndClr_FromStruct(SpriteRenderer _SR, State_Sprite _SpriteExtra)
    {
        _SR.material = _SpriteExtra.Mat;
        _SR.color = _SpriteExtra.Clr;
    }


    #endregion

    #region About List

    // 'T 타입' 리스트에 '새로' 추가
    public static bool Add_InList<T>(List<T> _TargetList, T _TargetValue)
    {
        if (!_TargetList.Contains(_TargetValue))
        {
            _TargetList.Add(_TargetValue);
            return true;
        }
        return false;
    }

    // 'T 타입' 삭제 시도
    public static bool Remove_InList<T>(List<T> _TargetList, T _TargetValue)
    {
        if (_TargetList.Contains(_TargetValue))
        {
            _TargetList.Remove(_TargetValue);
            return true;
        }
        return false;
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
            random1 = Random.Range(0, result.Count);
            random2 = Random.Range(0, result.Count);

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

    // 'T 타입' 맞는 인덱스 찾기
    public static int Get_IndexInList<T>(List<T> _TargetList, T _Target) where T : class
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            if (_TargetList[i] == _Target)
            {
                return i;
            }
        }
        return -1;
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
    public static void Set_ListDele<T, U>(List<T> _TargetList, Dele_RefT_U<U, T> _Dele, ref U _Variable, 
        int _StartIndex = 0)
    {
        for (int i = _StartIndex; i < _TargetList.Count; i++)
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



    // 'T 타입' 리스트에서 랜덤으로 뽑기
    public static T Get_Random<T>(List<T> _TargetList)
    {
        if (_TargetList != null || _TargetList.Count > 0)
        {
            return _TargetList[Random.Range(0, _TargetList.Count)];
        }
        return default;
    }

    // 'T 타입' 이중 리스트를 기본 리스트로 변경
    public static List<T> Get_List<T>(List<List<T>> _DoubleList)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < _DoubleList.Count; i++)
        {
            result.AddRange(_DoubleList[i]);
        }
        return result;
    }
    // 'T 타입' 중복 제거
    public static List<T> Remove_DuplicateInList<T>(List<T> _TargetList)
    {
        return _TargetList.Distinct().ToList();
    }

    // 'T 타입' List를 특정 T 리스트로 변경
    public static List<U> Get_ConvertTTypeList<T, U>(List<T> _FromList) where T : class where U : class
    {
        List<U> resultList = new List<U>();
        for (int i = 0; i < _FromList.Count; i++)
        {
            if (Can_CastingTType(_FromList[i], out U uType))
            {
                resultList.Add(uType);
            }
        }
        return resultList;
    }


    #endregion

    #region About Vector2

    public static Vector2 Get_RandomDir()
    {
        float _X = Random.Range(-1.0f, 1.0f);
        float _Y = Random.Range(-1.0f, 1.0f);
        return new Vector2(_X, _Y).normalized;
    }

    public static Vector2 Get_Dir(GameObject _FromGO, GameObject _ToGO)
    {
        return Get_Dir(_FromGO.transform.position, _ToGO.transform.position);
    }

    public static Vector2 Get_Dir(GameObject _FromGO, Vector2 _ToPos)
    {
        return Get_Dir(_FromGO.transform.position, _ToPos);
    }

    public static Vector2 Get_Dir(Vector2 _FromPos, Vector2 _ToPos)
    {
        return (_ToPos - _FromPos).normalized;
    }


    // 실제 주소값 Vector에 추가
    public static void Add_RefValue(ref Vector2 _Variable, Vector2 _AddValue)
    {
        _Variable += _AddValue;
    }

    // 방향에 의한 Img, Anim 변환 // SolarSystem에서 사용
    // 위 사항에 사용될 Index 값
    public static int Get_Index(float _EulerAngleY)
    {
        return (int)((_EulerAngleY + 67.5f) % 360 * 0.0222222f);
    }

    // 위 함수의 인트값을 다시 벡터로 가져오기
    public static Vector2Int Get_NormalizedVec(int _Index)
    {
        Vector2Int[] directions = {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0),
            new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0)};

        return (_Index >= 0 && _Index < directions.Length) ? directions[_Index] : Vector2Int.zero;
    }

    // 플레이어의 사격을 위해 너무 가까우면 X로 발사되는 것을 방지하기 위한 값 계산
    private static float FireMinDisLimit = 4;
    public static Vector2 Get_MinFireDir(Vector2 _SpawnPos)
    {
        Vector2 targetPos = InputManager.Instance.MousePosByWorld;
        if (FireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position +
                InputManager.Instance.DirFromPlayerPos.normalized * FireMinDisLimit;
        }

        return (targetPos - _SpawnPos).normalized;
    }

    // 최소 거리의 객체 가져오기
    public static GameObject Get_ClosetGO(List<GameObject> _TargetList, GameObject _CenterGO)
    {
        // 초기 설정
        GameObject resultGO = _TargetList[0];
        float shortestDis = Vector3.Distance(_CenterGO.transform.position, _TargetList[0].transform.position);

        for (int i = 1; i < _TargetList.Count; i++)
        {
            float currentDistance = Vector3.Distance(_CenterGO.transform.position, _TargetList[i].transform.position);
            if (currentDistance < shortestDis)
            {
                resultGO = _TargetList[i].gameObject;
                shortestDis = currentDistance;
            }
        }

        return resultGO;
    }

    #endregion

    #region About Quaternion

    // 좌표값 (Vector2:Dir)
    // => 회전값 (Quaternion:Rot)
    public static Quaternion Get_RotFromDir(Vector2 _Dir)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
    }
    
    public static Quaternion Get_RotFromDir_Solar(Vector2 _Dir)
    {
        return Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f);
    }

    // 각값 (float:Angle)
    // => 좌표값 (Vector2:Dir) : transform.eulerAngles.z값을 인자로 받는 것이 보편적으로 좋음
    public static Vector2 Get_DirFromAngle(float _Angle)
    {
        return new Vector2(
                    Mathf.Cos((_Angle + 90) * Mathf.Deg2Rad),
                    Mathf.Sin((_Angle + 90) * Mathf.Deg2Rad)).normalized;
    }
    // 좌표값 (Vecto2:Dir)
    // => 각값(float:Angle)
    public static float Get_AngleFromDir(Vector2 _Dir)
    {
        return Mathf.Atan2(_Dir.y, _Dir.x) * Mathf.Rad2Deg - 90f;
    }

    // 회전값에 값을 더하기
    // TF값, 월드기준
    public static void Add_RotZValue(Transform _TF, float _ZValue)
    {
        Vector3 currentRotation = _TF.eulerAngles;

        currentRotation.z += _ZValue;
        _TF.eulerAngles = currentRotation;
    }

    // TF값, 로컬기준
    public static void Add_LocalRotZValue(Transform _TF, float _ZValue)
    {
        Vector3 currentRotation = _TF.localEulerAngles;

        currentRotation.z += _ZValue;
        _TF.localEulerAngles = currentRotation;
    }

    // Quat값, 월드 기준
    public static Quaternion Add_RotZValue(Quaternion _Rotation, float _ZValue)
    {
        Vector3 currentRotation = _Rotation.eulerAngles;
        currentRotation.z += _ZValue;

        Quaternion q = Quaternion.identity;
        q.eulerAngles = currentRotation;

        return q;
    }


    // 반대 방향의 회전값 구하기
    public static Quaternion Get_FlipRotation(Quaternion _Rotation)
    {
        Vector3 currentRotation = _Rotation.eulerAngles;
        currentRotation.z += 180;

        Quaternion q = Quaternion.identity;
        q.eulerAngles = currentRotation;

        return q;
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

    // 애니메이션의 속도와 크기 조절
    public static void Set_AnimSpeedAndSize(Animator _AT, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        Set_AnimSpeed(_AT, _AnimSpeed);
        Set_AnimSize(_AT, _AnimSize);
    }

    // 애니메이션의 속도 조절
    public static void Set_AnimSpeed(Animator _AT, float _AnimSpeed)
    {
        _AT.speed = _AnimSpeed;
    }

    // 애니메이션의 크기 조절
    public static void Set_AnimSize(Animator _AT, float _AnimSize)
    {
        _AT.transform.localScale = Vector2.one * _AnimSize;
    }

    // 애니메이션이 끝났는지 판별
    public static bool Is_AnimIsDone(Animator _AT)
    {
        // 현재 애니메이터 상태 정보 가져오기가 1이상(1번이상 진행?)
        // 애니메이션이 종료되었는지 판별
        if (_AT.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && !_AT.IsInTransition(0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 방향성 Anim 컨트롤러의 애니메이터들의 속도 조절
    public static void Set_AnimSpeed(List<DirectionalAnimController> _TargetList, float _Speed)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            DevTool.Set_AnimSpeedAndSize(_TargetList[i].ThisComp, _Speed, _AnimSize: 1);
        }
    }


    #endregion

    #region About Tween

    public static void Play_Tween(Tween _Tween, Dele _Start, Dele _Update, Dele _Complete)
    {
        _Tween
            .OnStart(() => { _Start(); })
            .OnUpdate(() => { _Update(); })
            .OnComplete(() => { _Complete(); });
    }

    public static void Set_CompleteTween<T>(T _Comp)
    {
        if (DOTween.IsTweening(_Comp))
        { DOTween.Complete(_Comp); }
    }

    public static void Set_CompleteTween(Sequence _Seq)
    {
        if (_Seq != null && DOTween.IsTweening(_Seq))
        { DOTween.Complete(_Seq); }
    }

    #endregion

    #region About Player

    public readonly static int SkillAmount = 2;

    public static int Get_IndexOfDmgTypeAndCritical(eDamageType _DmgType, bool _IsCritical)
    {
        if (_DmgType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { return 0; }
            else
            { return 1; }
        }
        else
        {
            if (!_IsCritical)
            { return 2; }
            else
            { return 3; }
        }
    }

    public static eDamageType Get_DmgTypeFromIndex(int _Index)
    {
        if (_Index == 0 || _Index == 1)
        {
            return eDamageType.Physics;
        }
        else
        {
            return eDamageType.Energy;
        }
    }

    public static bool Get_CriticalFromIndex(int _Index)
    {
        if (_Index == 0 || _Index == 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region About Buff

    public static float Get_DmgEffectByCold(float _BaseDmg, EnemyBuffController _EnemyBuff)
    {
        return _BaseDmg * (1f - 
            (_EnemyBuff.ColdStack.CurrentStack * (_EnemyBuff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));
    }

    public static float Get_DmgEffectByCorrosion(float _BaseDmg, EnemyBuffController _EnemyBuff)
    {
        return _BaseDmg *= (1f + 
            (_EnemyBuff.CorrosionStack.CurrentStack * (_EnemyBuff.DecayStack.CurrentStack + 1) * 0.01f));
    }

    #endregion

    #region About Collider

    public static bool Can_Collding<T>(Collider2D _Col, string _Tag, List<StaticDepthController> _AlreadyList, out T _TType) where T : StaticDepthController
    {
        _TType = null;

        return _Col.tag == _Tag &&
            _Col.transform.parent.TryGetComponent(out _TType) &&
            !_AlreadyList.Contains(_TType);
    }

    public static bool Can_Collding<T>(Collider2D _Col, string _Tag, out T _TType) where T : StaticDepthController
    {
        _TType = null;

        return _Col.tag == _Tag &&
            _Col.transform.parent.TryGetComponent(out _TType);
    }
    #endregion

    #region About Nav

    // 중간에 벽이 있는지
    public static bool Is_Exist_UseCircle(Transform _StartTF, Transform _EndTF, string _LayerName, float _Radius)
    {
        Vector2 dirVec = Get_Dir(_StartTF.position, _EndTF.position);
        return Physics2D.CircleCast(_StartTF.position, _Radius, dirVec, dirVec.sqrMagnitude, LayerMask.GetMask(_LayerName)).collider != null ?
            true : false;
    }

    #endregion
}

#endregion

#region ========== CLASS

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
    [SerializeField] public float Max;
    [SerializeField] public float Current;

    public CooltimeData()
    {
        Max = 0f;
        Current = 0f;
    }

    public CooltimeData(float _Max, float _Current = 0f)
    {
        Max = _Max;
        Current = _Current;
    }

    public bool Is_Charge(float _DeltaTime)
    {
        if (Current >= Max)
        {
            return true;
        }
        else
        {
            Current += Time.deltaTime;
            return false;
        }
    }
}

#endregion


#region Class : Movable


[System.Serializable]
public class CurrentKnockbackState
{
    public Vector2 Dir;
    public float Power;
    public float Time;

    public CurrentKnockbackState(Vector2 _KnockbackDir, float _KnockbackPower, float _KnockbackTime)
    {
        Dir = _KnockbackDir;
        Power = _KnockbackPower;
        Time = _KnockbackTime;
    }

    public Tween Start_Knockback()
    {
        return DOTween.To(() => Power, x => Power = x, 0, Time);
    }

    public Vector2 Get_Knockback()
    {
        return Dir.normalized * Power;
    }
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

#endregion

#region Class : State : Combat : Bullet

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

#endregion

#region Class : State : Combat : Attacker

[System.Serializable]
public class AttackerState : CombatState
{
    #region Constructor

    public AttackerState(CombatState _State) : base(_State.DmgState, _State.CriticalState, _State.KnockbackState) { }

    #endregion
}

#endregion

#region Class : State : CombatElement

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


#region Class : State : Player : BU Shop
// Level, State을 저장 관리하고, 상점까지 통괄

[System.Serializable]
public class BUShopSkillData<T, U>
{
    public BUShopEachData<T> Skill_CooltimeShop;
    public BUShopEachData<T> Skill_PowerShop;
    public BUShopEachData<U> Skill_TierShop;
}

[System.Serializable]
public class BUShopEachData<T>
{
    [SerializeField] public TxtAmountForBuyEUIController Upgrade_MTAFB;
    [SerializeField] public OwnBtnEUIController Upgrade_BuyBtn;

    [HideInInspector] public BUState<T> Upgrade_BUS;
    [HideInInspector] private BULevelData<T> Upgrade_BUOTD;

    public void Offset(BUState<T> _Upgrade_BUS, BULevelData<T> _Upgrade_BUOTD, BaseUpgradeUIController _Owner)
    {
        Upgrade_MTAFB.Offset();

        Upgrade_BUS = _Upgrade_BUS;
        Upgrade_BUOTD = _Upgrade_BUOTD;

        Upgrade_BUS.BuffedState = Upgrade_BUS.ActualState.Value;

        Upgrade_MTAFB.SkillNameTxt.text = Upgrade_BUS.Name;
        Upgrade_MTAFB.SkillOpenSimpleTxt.text = Upgrade_BUS.Desc;

        if (Upgrade_BuyBtn != null)
        {
            Upgrade_BuyBtn.Offset();
            Upgrade_BuyBtn.OwnerUIController = _Owner;
        }


        Upgrade_BUS.CurrentLevel
           .Subscribe(_CurrentLevel =>
           {
               int currentLv = _CurrentLevel;
               if (currentLv < Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, Upgrade_BUOTD.BU_EachLevelDataList[currentLv].NeedEC_ForUpgrade);
               }
               else if (currentLv == Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, 0);
               }
               Upgrade_MTAFB.Set_InnerAlpha((float)currentLv / (float)Upgrade_BUOTD.BU_EachLevelDataList.Count);
           });

        _Owner.MainColorCompList.Add(Upgrade_MTAFB.SkillNameTxt);
        _Owner.SubColorCompList.Add(Upgrade_MTAFB.SkillLvTxt);
        _Owner.SubColorCompList.AddRange(Upgrade_MTAFB.ThisMIAAT.Img_List);
        _Owner.SubColorCompList.AddRange(Upgrade_MTAFB.InnerImgList);
        _Owner.MainColorCompList.Add(Upgrade_MTAFB.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
        _Owner.MainColorCompList.Add(Upgrade_MTAFB.SimpleDescTxt);
        _Owner.MainColorCompList.Add(Upgrade_BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
    }

    public void TryBuy()
    {
        int index = Upgrade_BUS.CurrentLevel.Value;
        int needEC = Upgrade_BUOTD.BU_EachLevelDataList[index].NeedEC_ForUpgrade;
        int hadEC = PlayerManager.Instance.PlayerController.CurrentEC.Value;
        if (needEC <= hadEC)
        {
            Buy(needEC, Upgrade_BUOTD.BU_EachLevelDataList.Count, Upgrade_BUOTD.BU_EachLevelDataList[index].UpgradeValue);
        }
    }

    private void Buy(int _UseEC, int _MaxUpgradeLevel, T _SetValue)
    {
        BaseUpgradeController.UsingShop.Take_Damage(false);

        Upgrade_BUS.CurrentLevel.Value++;
        Upgrade_BUS.ActualState.Value = _SetValue;
        PlayerManager.Instance.PlayerController.CurrentEC.Value -= _UseEC;
        if (_MaxUpgradeLevel <= Upgrade_BUS.CurrentLevel.Value)
        {
            Upgrade_BuyBtn.ThisBtn.interactable = false;
        }

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_Desc(Upgrade_MTAFB);
    }


    public static BUState<T> GetThisData(List<BUShopEachData<T>> _ShopDataList, TxtAmountForBuyEUIController _InMTAFB)
    {
        foreach (BUShopEachData<T> Data in _ShopDataList)
        {
            if (Data.Upgrade_MTAFB == _InMTAFB)
            {
                return Data.Upgrade_BUS;
            }
        }
        return null;
    }
}

#endregion

#region Class : State : Player : BU Level
// BU Manager로 미리 수치를 저장하기 위함

[System.Serializable]
public class BULevelSkillData<T, U>
{
    public BULevelData<T> Skill_Cooltime_BUData;
    public BULevelData<T> Skill_Power_BUData;
    public BULevelData<U> Skill_Tier_BUData;
}

[System.Serializable]
public class BULevelData<T>
{
    [Header("=== No Input")]
    public List<BUEachLevelData<T>> BU_EachLevelDataList;

    public void Offset(BUState<T> _BaseValue)
    {
        if (_BaseValue.BaseState.GetType() == typeof(float))
        {
            // Base
            float float_BaseValue = float.Parse(_BaseValue.BaseState.ToString());

            // Upgrade
            List<float> float_UpgradeValues = new List<float>();
            for (int i = 0; i < _BaseValue.UpgradeValueByLevelRange.Count; i++)
            {
                float float_EachUpgradeValue = float.Parse(_BaseValue.UpgradeValueByLevelRange[i].ToString());
                float_UpgradeValues.Add(float_EachUpgradeValue);
            }

            Offset(float_BaseValue, float_UpgradeValues, _BaseValue.NeedPayByLevelRange);
        }
        else if (_BaseValue.BaseState.GetType() == typeof(int))
        {
            // Base
            int int_BaseValue = int.Parse(_BaseValue.BaseState.ToString());

            // Upgrade
            List<int> int_UpgradeValues = new List<int>();
            for (int i = 0; i < _BaseValue.UpgradeValueByLevelRange.Count; i++)
            {
                int int_EachUpgradeValue = int.Parse(_BaseValue.UpgradeValueByLevelRange[i].ToString());
                int_UpgradeValues.Add(int_EachUpgradeValue);
            }

            Offset(int_BaseValue, int_UpgradeValues, _BaseValue.NeedPayByLevelRange);
        }
    }

    public void Offset(float _FloatValue, List<float> _UpgradeValue, List<int> _NeedPay)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i == 0)
            {
                decimal d = (decimal)(_FloatValue + _UpgradeValue[0]);
                BU_EachLevelDataList[i].SetUpgradeValue(Mathf.RoundToInt((float)d * 100f) / 100f);
            }
            else
            {
                decimal d = (decimal)((float)BU_EachLevelDataList[i - 1].GetUpgradeValue() + _UpgradeValue[(int)(i / 3)]);
                BU_EachLevelDataList[i].SetUpgradeValue(Mathf.RoundToInt((float)d * 100f) / 100f);
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = _NeedPay[(int)(i / 3)];
        }
    }

    public void Offset(int _IntValue, List<int> _UpgradeValue, List<int> _NeedPay)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i == 0)
            {
                int _intager = (_IntValue + _UpgradeValue[0]);
                BU_EachLevelDataList[i].SetUpgradeValue((int)_intager);
            }
            else
            {
                int _intager = ((int)BU_EachLevelDataList[i - 1].GetUpgradeValue() + _UpgradeValue[(int)(i / 3)]);
                BU_EachLevelDataList[i].SetUpgradeValue((int)_intager);
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = _NeedPay[(int)(i / 3)];
        }
    }
}

[System.Serializable]
public class BUEachLevelData<T>
{
    public T UpgradeValue;
    public int NeedEC_ForUpgrade;

    public void SetUpgradeValue(object _Value)
    {
        UpgradeValue = (T)_Value;
    }

    public object GetUpgradeValue()
    {
        return UpgradeValue;
    }
}

#endregion

#region Class : State : Player : BU State
// BU 강화를 위한 수치들

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

#endregion

#region Class : State : Player : Other

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

#region Class : State : Player : ItemData

[System.Serializable]
public class ItemData
{
    [Header("=== ID")]
    public int ID;

    [Header("=== Info")]
    public string Name;
    public string Description;
    public string EquipDescription;
    public Sprite ItemIcon;

    [Header("=== MainChip")]
    public int R1_MainChipID;
    public int R3_MainChipID;
    public int R5_MainChipID;

    [Header("=== Level")]
    public int BoostLv = 1;
    public int Rank = 1;

    public ItemData(int _ID) 
    {
        ID = _ID;
    }

    public ItemData(ItemData _ItemData)
    {
        ID = _ItemData.ID;

        Name = _ItemData.Name;
        Description = _ItemData.Description;
        EquipDescription = _ItemData.EquipDescription;
        ItemIcon = _ItemData.ItemIcon;

        R1_MainChipID = _ItemData.R1_MainChipID;
        R3_MainChipID = _ItemData.R3_MainChipID;
        R5_MainChipID = _ItemData.R5_MainChipID;

        BoostLv = _ItemData.BoostLv;
        Rank = _ItemData.Rank;
    }
}

[System.Serializable]
public class MainChipData
{
    public Sprite ThisIcon;
    public int ID;
    public string Name;
    public List<string> AmalgamationDescList;
}

#endregion

#region Class : State : Player : MU

public class ModuleState : IWhen
{
    #region Value

    public ItemData ThisItemData;

    public List<InventoryItemEUIController> InventoryUI_Equip = new List<InventoryItemEUIController>();
    public List<InventoryItemEUIController> InventoryUI_Forge = new List<InventoryItemEUIController>();

    protected ModuleItemActivityManager.ActivityFuncDele ThisActivityFuncDele;

    #endregion

    #region Constructor

    public ModuleState(int _ID)
    {
        ThisItemData = new ItemData(_ID);
        ThisActivityFuncDele = ModuleItemActivityManager.Instance.Get_CollectActivity(ThisItemData.ID);
    }

    #endregion

    #region Get

    public static List<ModuleState> Get_AllModuleState()
    {
        return new List<ModuleState>()
        {
            new ModuleItem000(0),
            new ModuleItem001(1),
            new ModuleItem002(2),
            new ModuleItem003(3),
            new ModuleItem004(4),
            new ModuleItem005(5),
        };
    }

    protected int Get_Rank()
    {
        return ThisItemData.Rank;
    }

    protected int Get_BoostLv()
    {
        int targetBoostLv = PlayerManager.Instance.PlayerController.CurrentBoostLv.Value;
        if (targetBoostLv > ThisItemData.BoostLv)
        {
            targetBoostLv = ThisItemData.BoostLv;
        }
        return targetBoostLv;
    }

    #endregion

    #region Interface

    public virtual void Play_When(EnemyController _EC = null)
    {
        ThisActivityFuncDele(Get_Rank(), Get_BoostLv(), _EC);
    }

    #endregion
}

#endregion

#region Class : State : Player : MU Code

public class ModuleItem000 : ModuleState, IWhen_Fire
{ public ModuleItem000(int _ID) : base(_ID) { } }

public class ModuleItem001 : ModuleState, IWhen_Fire
{ public ModuleItem001(int _ID) : base(_ID) { } }

public class ModuleItem002 : ModuleState, IWhen_CriticalHit
{ public ModuleItem002(int _ID) : base(_ID) { } }

public class ModuleItem003 : ModuleState, IWhen_CriticalHit
{ public ModuleItem003(int _ID) : base(_ID) { } }

public class ModuleItem004 : ModuleState, IWhen_CriticalHit
{ public ModuleItem004(int _ID) : base(_ID) { } }

public class ModuleItem005 : ModuleState, IWhen_CriticalHit
{ public ModuleItem005(int _ID) : base(_ID) { } }

#endregion


#region Class : State : Enemy : Pattern

[System.Serializable]
public class ContinuousEnemyPattern
{
    public List<EnemyPattern> EnemyPatternList;
}

[System.Serializable]
public class OrderOfPriorityEnemyPattern
{
    public List<ContinuousEnemyPattern> EnemyPatternList;
}

#endregion

#region Class : Satellite

[System.Serializable]
public abstract class SatelliteController
{
    #region Value

    [Space(5)]
    [Header("<><><><><> Satellite")]

    [SerializeField] public Transform Target;
    [SerializeField] public DepthController Follower;

    [SerializeField] public int UpperOrder;

    #endregion

    #region Func

    public void Set_Pos()
    {
        Follower.transform.position = Target.position;
    }

    public abstract void Set_SortingOrder(int _ObjectSortOrder);

    #endregion
}

[System.Serializable]
public class SatelliteSideController : SatelliteController
{
    #region Value

    [Space(5)]
    [Header("<><><><><> Side")]

    [SerializeField] public int FarFromCenter;

    #endregion

    #region Func

    public override void Set_SortingOrder(int _ObjectSortOrder)
    {
        Follower.Set_SortingOrder(_ObjectSortOrder + UpperOrder + 
            (Is_LocalUpper(Follower.transform) ? -FarFromCenter : FarFromCenter));
    }

    private bool Is_LocalUpper(Transform _TargetTF)
    {
        return _TargetTF.localPosition.y > 0;
    }

    #endregion
}

[System.Serializable]
public class SatelliteCenterController : SatelliteController
{
    #region Func

    public override void Set_SortingOrder(int _ObjectSortOrder)
    {
        Follower.Set_SortingOrder(_ObjectSortOrder + UpperOrder);
    }

    #endregion
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

#endregion

#region ========== STRUCT

#region Struct : PublicData

public struct State_TF2D
{
    #region Value

    public Vector2 Pos;
    public Quaternion Rot;
    public Vector2 LocalScale;

    #endregion

    #region Constructor

    public State_TF2D(Vector2 _Pos, Quaternion _Rot, Vector2 _LocalScale)
    {
        Pos = _Pos;
        Rot = _Rot;
        LocalScale = _LocalScale;
    }

    #endregion
}

#endregion

#region Struct : BulletState

public struct BulletState_PosAndRot
{
    #region Value

    public Vector2 SpawnPos;
    public Vector2 Dir;
    public float SpreadAngle;

    #endregion

    #region Constructor

    public BulletState_PosAndRot(Vector2 _SpawnPos, Vector2 _Dir, float _SpreadAngle)
    {
        SpawnPos = _SpawnPos;
        Dir = _Dir;
        SpreadAngle = _SpreadAngle;
    }

    #endregion
}

public struct BulletState_Size
{
    #region Value

    public Vector2 ObjSize;
    public Vector2 ColSize;

    #endregion

    #region Constructor 

    public BulletState_Size(Vector2 _ObjScale, Vector2 _ColSize)
    {
        ObjSize = _ObjScale;
        ColSize = _ColSize;
    }

    #endregion
}

#endregion

#region Struct : AttackerState

public struct AttackerState_EndTF
{
    #region Value

    public State_TF2D TF;

    public float Time;

    #endregion

    #region Constructor

    public AttackerState_EndTF(Vector2 _Pos, Quaternion _Rot, Vector2 _Size, float _Time)
    {
        TF.Pos = _Pos;
        TF.Rot = _Rot;
        TF.LocalScale = _Size;

        Time = _Time;
    }

    #endregion
}

public struct AttackerState_Juge<T> where T : Collider2D
{
    #region Value

    public Vector2 ColSize;
    public bool IsVertical;

    #endregion

    #region Constructor

    public AttackerState_Juge(Vector2 _ColSize, bool _IsVertical = false)
    {
        ColSize = _ColSize;
        IsVertical = _IsVertical;
    }

    #endregion
}

#endregion

#region Struct : Visual

public struct State_Sprite
{
    #region Value

    public Material Mat;
    public Color Clr;

    #endregion

    #region Constructor

    public State_Sprite(Material _Mat, Color _Clr)
    {
        Mat = _Mat;
        Clr = _Clr;
    }

    #endregion
}

public struct State_Anim
{
    #region Value

    public AnimationClip AC;
    public float Speed;

    #endregion

    #region Constructor

    public State_Anim(AnimationClip _AC)
    {
        AC = _AC;
        Speed = 1f;
    }

    public State_Anim(AnimationClip _AC, float _Speed)
    {
        AC = _AC;
        Speed = _Speed;
    }

    #endregion
}

#endregion

#region Struct : Visual : Explosion

public struct ExplState_Base
{
    #region Value

    public Vector2 SpawnPos;
    public int SpawnAmount;

    #endregion

    #region Constructor

    public ExplState_Base(Vector2 _SpawnPos, int _SpawnAmount)
    {
        SpawnPos = _SpawnPos;
        SpawnAmount = _SpawnAmount;
    }

    public ExplState_Base(ExplState_Base _State)
    {
        SpawnPos = _State.SpawnPos;
        SpawnAmount = _State.SpawnAmount;
    }

    #endregion
}

public struct ExplState_Sprite
{
    #region Value

    public List<Sprite> Sprite;
    public Material Material;

    #endregion

    #region Constructor

    public ExplState_Sprite(List<Sprite> _Sprite, Material _Material)
    {
        Sprite = _Sprite;
        Material = _Material;
    }
    public ExplState_Sprite(ExplState_Sprite _State)
    {
        Sprite = _State.Sprite;
        Material = _State.Material;
    }

    #endregion
}

public struct ExplState_MoveAndScale
{
    #region Value

    public Vector2 Dir;
    public float Dis;
    public float Scale;
    public float Time;
    public float RandomDelayTime;

    #endregion

    #region Constructor

    public ExplState_MoveAndScale(Vector2 _Dir, float _Dis, float _Scale, float _Time, float _RandomDelayTime)
    {
        Dir = _Dir;
        Dis = _Dis;
        Scale = _Scale;
        Time = _Time;
        RandomDelayTime = _RandomDelayTime;
    }

    public ExplState_MoveAndScale(ExplState_MoveAndScale _State)
    {
        Dir = _State.Dir;
        Dis = _State.Dis;
        Scale = _State.Scale;
        Time = _State.Time;
        RandomDelayTime = _State.RandomDelayTime;
    }

    #endregion
}


public struct ExplState
{
    public ExplState_Base BaseState;
    public ExplState_Sprite SpriteState;
    public ExplState_MoveAndScale FirstState;
    public ExplState_MoveAndScale SecondState;

    private ExplState_MoveAndScale OriginalFirstState;
    private ExplState_MoveAndScale OriginalSecondState;

    public ExplState(ExplState_Base _BaseState, ExplState_Sprite _SpriteState, ExplState_MoveAndScale _FirstState, ExplState_MoveAndScale _SecondState)
    {
        BaseState = new ExplState_Base(_BaseState);
        SpriteState = new ExplState_Sprite(_SpriteState);
        FirstState = new ExplState_MoveAndScale(_FirstState);
        SecondState = new ExplState_MoveAndScale(_SecondState);

        OriginalFirstState = new ExplState_MoveAndScale(_FirstState);
        OriginalSecondState = new ExplState_MoveAndScale(_SecondState);
    }

    public void Set_AllDir(Vector2 _Dir)
    {
        FirstState.Dir = _Dir;
        SecondState.Dir = _Dir;

        OriginalFirstState.Dir = _Dir;
        OriginalSecondState.Dir = _Dir;
    }

    public void Set_MultipleAllDir(Vector2 _Dir)
    {
        FirstState.Dir *= _Dir;
        SecondState.Dir *= _Dir;

        OriginalFirstState.Dir *= _Dir;
        OriginalSecondState.Dir *= _Dir;
    }

    public void Set_RandomValue()
    {
        FirstState.Time += DevTool.Get_RandomValueBaseZero(OriginalFirstState.RandomDelayTime);
        SecondState.Time += DevTool.Get_RandomValueBaseZero(OriginalFirstState.RandomDelayTime);
    }

    public void Set_RandomAngleValue_PivotZero(float _AngleExtent)
    {
        float randomAngle = DevTool.Get_RandomValueBaseZero(_AngleExtent);
        FirstState.Dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(OriginalFirstState.Dir));
        SecondState.Dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(OriginalSecondState.Dir));
    }

    public void Set_RandomAngleValue_JustAdd(float _AngleExtent)
    {
        float randomAngle = Random.Range(0, _AngleExtent);
        FirstState.Dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(OriginalFirstState.Dir));
        SecondState.Dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(OriginalSecondState.Dir));
    }
}

#endregion

#endregion

#region ========== INTERFACE

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

#endregion

#region ========== DELEGATE

public delegate void Dele();

public delegate void Dele_T<T>(T _Item);

public delegate void Dele_RefT_T<T>(ref T _Item1, T _Item2);

public delegate void Dele_T_U<T, U>(T _Item1, U _Item2);
public delegate void Dele_RefT_U<T, U>(ref T _Item1, U _Item2);


#endregion

#region ========== ENUM

#region About Combat

public enum eDamageType
{
    Physics, Energy
}

public enum eStatusEffect
{
    Flame, Cold, Electricity, Corrosion
}

#endregion

#region About Movement

public enum eMovementState
{
    Casting, IdleOrWalk, Dash
}

public enum eDashStyle
{
    OneWay, CanInputWay, Teleport
}

#endregion

#region Enemy

public enum eEnemy
{
    Normal, Elite, SemiBoss, Boss
}

#endregion

#region Room

public enum eRoomType
{
    Completed, KillAll, Survived, BossKill
}

#endregion

#endregion