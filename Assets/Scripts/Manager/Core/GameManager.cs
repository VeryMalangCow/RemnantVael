using DG.Tweening;
using LeTai.TrueShadow;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Linq.Expressions;
using UnityEngine.InputSystem.HID;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : PersistentSingleton<GameManager>
{
    #region Value


    [Space(10)]
    [Header("=== Passing Data")]
    [SerializeField] public GameObject DesignatedPlayerPrefab;

    [Space(10)]
    [Header("=== Intro")]
    [SerializeField] public bool WasWatched = false;

    public static int LanguageID = 1;
    public readonly static List<string> KindOfLanguage = new List<string> { "Eng", "Kor" };

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

    #region Get

    // 퍼센트값을 도출
    public static float Get_Percent(float _Percent, float _Value)
    {
        return (_Percent / 100f) * _Value;
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
            return UnityEngine.Random.Range(-_RandomExtent * 0.5f, _RandomExtent * 0.5f);
        }
    }

    // 확률에 따른 등급
    public static int Get_Grade(List<int> _RankPercents)
    {
        float currentSum = 0f;
        float randomValue = UnityEngine.Random.Range(0f, Get_SumFloat(_RankPercents));

        for (int i = 0; i < _RankPercents.Count; i++)
        {
            currentSum += _RankPercents[i];
            if (currentSum > randomValue)
            {
                return i;
            }
        }

        return 0;
    }

    // 랭크
    public static int Get_Rank(List<int> _RankPercents)
    {
        return Get_Grade(_RankPercents) + 1;
    }

    // Round
    public static string Get_RoundFloatString(float _Value, int _RoundRange = 3)
    {
        return _Value >= 0 ? $"+{System.Math.Round(_Value, _RoundRange)}" : $"{System.Math.Round(_Value, _RoundRange)}";
    }


    #endregion

    #region Is

    // 확률이 성공했는지를 반환
    public static bool Is_ChanceSuccess(float _Chance)
    {
        if (UnityEngine.Random.Range(0f, 1f) < _Chance)
            return true;
        else
            return false;
        
    }

    // 일정 오차를 인정한다
    public static bool Is_InRange(float _Value, float _Criterion, float _Range)
    {
        return _Criterion - _Range <= _Value && _Value <= _Criterion + _Range ? true : false; 
    }

    #endregion

    #region Add

    // 실제 주소값 float에 추가
    public static void Add_RefValue(ref float _Variable, float _AddValue)
    {
        _Variable += _AddValue;
    }

    #endregion

    #endregion

    #region About String

    public static string Get_LengthString(int _Value, int _TargetLength)
    {
        return _Value.ToString().PadLeft(_TargetLength, '0');
    }

    #endregion

    #region About Casting

    #region Is

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

    #endregion

    #region Get

    // 객체를 원하는 'T 타입'으로 캐스팅
    public static T Get_CastingTType<T>(object _Obj)
    {
        if (_Obj != null && _Obj is T objType)
        {
            return objType;
        }
        Debug.Log("Default");
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

    #endregion

    #endregion

    #region About Component

    #region Set

    // 객체에 'T 타입'이 있다면 변수에 할당
    public static void Set_ComponentTType<T>(ref T _Variable, GameObject _TargetGO) where T : Component
    {
        if (_Variable == null && _TargetGO.TryGetComponent(out T tTypeComponent))
        {
            _Variable = tTypeComponent;
        }
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

    #region Get

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

    #endregion

    #region Gen 

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

    // Gen SpriteRenderer
    public static SpriteRenderer Gen_Component_SR(Transform _ParentTF, string _Name, Sprite _Sprite, Material _Material, int _SortingOrder)
    {
        SpriteRenderer sr = Gen_Component<SpriteRenderer>(_ParentTF, _Name);
        Set_ComponentValue(sr, _Sprite, _Material, _SortingOrder);
        return sr;
    }

    #endregion

    #region Remove

    // 게임 오브젝트에 컴포넌트 삭제하기
    public static void Remove_Component<T>(T _TargetComp) where T : Component
    {
        if (_TargetComp != null)
        {
            UnityEngine.Object.Destroy(_TargetComp);
        }
    }

    #endregion

    #endregion

    #region About Hash

    // 주변 좌표값을 가져오기
    public static HashSet<Vector2Int> Get_RoundVec(HashSet<Vector2Int> _TargetVec)
    {
        HashSet<Vector2Int> targetRoomVecRound = new HashSet<Vector2Int>();

        // 모든 타겟 좌표의 주변 좌표 추가
        foreach (var vec in _TargetVec)
        {
            List<Vector2Int> eachRoung = Get_RoundVec(vec);
            for (int i = 0; i < eachRoung.Count; i++)
                targetRoomVecRound.Add(eachRoung[i]);
        }

        // 원래 _TargetVec에 포함된 좌표 제거
        targetRoomVecRound.ExceptWith(_TargetVec);

        return targetRoomVecRound; // HashSet을 List로 변환 후 반환
    }

    #endregion

    #region About List

    #region Add

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

    #endregion

    #region Remove

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


    // 'T 타입' 중복 제거
    public static List<T> Remove_DuplicateInList<T>(List<T> _TargetList)
    {
        return _TargetList.Distinct().ToList();
    }

    #endregion

    #region Get 

    #region float

    public static float Get_SumFloat(List<int> _IntList)
    {
        float result = 0;
        for (int i = 0; i < _IntList.Count; i++)
        {
            result += _IntList[i];
        }
        return result;
    }

    #endregion

    #region Removed

    public static List<T> Get_RemovedList<T>(List<T> _TargetList, int _Index)
    {
        List<T> result = new List<T>(_TargetList);
        result.Remove(result[_Index]);
        return result;
    }
    public static List<T> Get_RemoveLastOneList<T>(List<T> _TargetList)
    {
        List<T> result = new List<T>(_TargetList);
        result.Remove(result[result.Count - 1]);
        return result;
    }

    #endregion

    #region Vector2Int

    // 해당 백터의 주변을 구하기
    public static List<Vector2Int> Get_RoundVec(Vector2Int _CenterVec)
    {
        return new List<Vector2Int>()
        {
            (_CenterVec + Vector2Int.up),
            (_CenterVec + Vector2Int.down),
            (_CenterVec + Vector2Int.left),
            (_CenterVec + Vector2Int.right)
        };
    }

    // 주변 좌표값을 가져오기
    public static List<Vector2Int> Get_RoundVec(List<Vector2Int> _TargetVec)
    {
        HashSet<Vector2Int> targetRoomVecRound = new HashSet<Vector2Int>();

        // 모든 타겟 좌표의 주변 좌표 추가
        foreach (var vec in _TargetVec)
        {
            List<Vector2Int> eachRoung = Get_RoundVec(vec);
            for (int i = 0; i < eachRoung.Count; i++)
                targetRoomVecRound.Add(eachRoung[i]);
        }
        
        // 원래 _TargetVec에 포함된 좌표 제거
        targetRoomVecRound.ExceptWith(_TargetVec);

        return targetRoomVecRound.ToList(); // HashSet을 List로 변환 후 반환
    }

    public static int Get_IntersectionAmount(List<Vector2Int> targetRoomVec, List<Vector2Int> existRoomVec)
    {
        return Get_Intersection(targetRoomVec, existRoomVec).Count;
    }

    public static List<Vector2Int> Get_Intersection(List<Vector2Int> targetRoomVec, List<Vector2Int> existRoomVec)
    {
        // existRoomVec의 요소를 Vector2Int로 변환
        HashSet<Vector2Int> existRoomSet = new HashSet<Vector2Int>(existRoomVec);

        // targetRoomVec과 existRoomVec의 교집합 반환
        return targetRoomVec.Where(room => existRoomSet.Contains(room)).ToList();
    }

    #endregion

    #region Random

    public static T Get_RandomInList<T>(List<T> _TargetList)
    {
        if (_TargetList == null || _TargetList.Count <= 0)
            return default;
        else
            return _TargetList[UnityEngine.Random.Range(0, _TargetList.Count)];
    }

    #endregion

    #region Child

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

    public static List<T> Get_AllChildList<T>(Transform _Parent) where T : Component
    {
        List<T> result = new List<T>();
        Transform[] allChildren = _Parent.GetComponentsInChildren<Transform>();
        foreach (Transform TF in allChildren)
        {
            if (TF == _Parent) continue;

            if (TF.TryGetComponent(out T type))
            {
                result.Add(type);
            }
        }

        return result;
    }

    public static List<T> Get_ChildList_OnlyOnceUnder<T>(Transform _Parent) where T : Component
    {
        List<T> result = new List<T>();
        if (_Parent.childCount > 0)
        {
            for (int i = 0; i < _Parent.childCount; i++)
            {
                if (_Parent.GetChild(i).gameObject.TryGetComponent(out T type))
                {
                    result.Add(type);
                }
            }
        }
        return result;
    }

    #endregion

    #region Double List


    // 'T 타입' 이중 리스트를 기본 리스트로 변경
    public static List<T> Get_List<T>(List<List<T>> _DoubleList) where T : class
    {
        List<T> result = new List<T>();
        for (int i = 0; i < _DoubleList.Count; i++)
        {
            result.AddRange(_DoubleList[i]);
        }
        return result;
    }


    // 'T 타입' 이중 리스트에서 리스트들중 마지막 부분만을 모아서 리턴
    public static List<T> Get_LastElementList<T>(List<List<T>> _DoubleList)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < _DoubleList.Count; i++)
        {
            result.Add(_DoubleList[i][_DoubleList[i].Count - 1]);
        }
        return result;
    }

    #endregion

    #region Casting + ByComponent

    // 게임 오브젝트 리스트에서 List T 타입 변형
    public static List<T> Get_ComponentTTypeList<T>(List<GameObject> _TargetList) where T : Component
    {
        List<T> resultList = new List<T>();
        for (int i = 0; i < _TargetList.Count; i++)
        {
            if (Get_ComponentTType(_TargetList[i], out T tType))
            {
                resultList.Add(tType);
            }
        }
        return resultList;
    }

    // 'T 타입' 리스트를 GO 리스트로 변경
    public static List<GameObject> Get_GOList<T>(List<T> _TargetList) where T : MonoBehaviour
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < _TargetList.Count; i++)
        {
            result.Add(_TargetList[i].gameObject);
        }
        return result;
    }

    // 강제 Parse
    public static List<int> Get_ParseIntList<T>(List<T> _TargetList)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < _TargetList.Count; i++)
        {
            result.Add(int.Parse(_TargetList[i].ToString()));
        }
        return result;
    }
    public static List<float> Get_ParseFloatList<T>(List<T> _TargetList)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < _TargetList.Count; i++)
        {
            result.Add(float.Parse(_TargetList[i].ToString()));
        }
        return result;
    }

    #endregion

    #region Row & Colume

    public static List<List<T>> Get_RowColumeList<T>(List<T> _List, int _Row)
    {
        int totalCount = _List.Count;
        int totalRows = (totalCount + _Row - 1) / _Row; // 올림 나눗셈

        List<List<T>> result = new List<List<T>>();

        for (int i = 0; i < totalRows; i++)
        {
            List<T> row = new List<T>();

            for (int j = 0; j < _Row; j++)
            {
                int index = i * _Row + j;
                if (index >= totalCount)
                    break;

                row.Add(_List[index]);
            }

            result.Add(row);
        }

        return result;
    }

    #endregion

    #region Unique

    // 'T 타입' List 두개를 합
    public static List<T> Get_CombineList<T>(List<T> _FirstList, List<T> _SecondList)
    {
        List<T> resultList = new List<T>(_FirstList);
        resultList.AddRange(_SecondList);
        return resultList;
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

    // 'T 타입' 리스트에서 랜덤으로 뽑기
    public static T Get_Random<T>(List<T> _TargetList)
    {
        if (_TargetList != null || _TargetList.Count > 0)
        {
            return _TargetList[UnityEngine.Random.Range(0, _TargetList.Count)];
        }
        return default;
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

    // 'T 타입' 두 리스트 중 교집합 가져오기
    public static List<T> Get_IntersectionList<T>(List<T> _List1, List<T> _List2)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < _List1.Count; i++)
        {
            if (_List2.Contains(_List1[i]))
            {
                result.Add(_List1[i]);
            }
        }
        return result;
    }

    #endregion

    #endregion

    #region Set

    // 'T 타입' 리스트의 두 값을 교체
    public static void Set_Swap<T>(List<T> _TargetList, int _Index1, int _Index2)
    {
        T temp = _TargetList[_Index1];
        _TargetList[_Index1] = _TargetList[_Index2];
        _TargetList[_Index2] = temp;
    }

    // 'T 타입' 리스트의 게임 오브젝트 모두 끄고 키기
    public static void Set_Active<T>(List<T> _TargetList, bool _Active) where T : Component
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _TargetList[i].gameObject.SetActive(_Active);
        }
    }

    // 'T 타입' 리스트를 모두 Null로 초기화
    public static void Set_Null<T>(List<T> _TargetList) where T : class
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _TargetList[i] = null;
        }
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

    // Comp
    public static void Set_SpriteList(List<Image> _TargetList, Sprite _Sprite)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _TargetList[i].sprite = _Sprite;
        }
    }
    public static void Set_SpriteNativeSize(List<Image> _TargetList)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            _TargetList[i].SetNativeSize();
        }
    }

    #endregion

    #endregion

    #region About Dictionary


    public static void Add_AmountForDict<T>(ref Dictionary<T, int> _Dict, T _ID, int _Amount)
    {
        if (_Dict.ContainsKey(_ID))
            _Dict[_ID] += _Amount; 
        else
            _Dict.Add(_ID, _Amount); 
    }

    #endregion

    #region About Vector

    #region Get

    public static float Get_Dis(GameObject _GO1, GameObject _GO2)
    {
        return Vector2.Distance(_GO1.transform.position, _GO2.transform.position);
    }

    public static Vector2 Get_RandomDir()
    {
        float _X = UnityEngine.Random.Range(-1.0f, 1.0f);
        float _Y = UnityEngine.Random.Range(-1.0f, 1.0f);
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
        return _TargetList.Count == 0 ?
            null :
            _TargetList.OrderBy(go => Vector3.Distance(_CenterGO.transform.position, go.transform.position)).First();
    }

    // 최대 거리의 객체 가져오기
    public static GameObject Get_FurthestGO(List<GameObject> _TargetList, GameObject _CenterGO)
    {
        return _TargetList.Count == 0 ? 
            null : 
            _TargetList.OrderBy(go => Vector3.Distance(_CenterGO.transform.position, go.transform.position)).Last();
    }

    // 범위 내 객체들 가져오기 (가까운 순서대로)
    public static List<GameObject> Get_CloserGOList(List<GameObject> _TargetList, GameObject _CenterGO, float _MaxDis)
    {
        return Get_RangeGOList(_TargetList, _CenterGO, 0, _MaxDis, _OrderByShortDis: true);
    }

    // 범위 밖 객체들 가져오기 (먼 순서대로)
    public static List<GameObject> Get_FurtherGOList(List<GameObject> _TargetList, GameObject _CenterGO, float _MinDis)
    {
        return Get_RangeGOList(_TargetList, _CenterGO, _MinDis, 0, _OrderByShortDis: false);
    }

    // 범위 조건 객체들 가져오기
    public static List<GameObject> Get_RangeGOList(List<GameObject> _TargetList, GameObject _CenterGO, float _MinDis, float _MaxDis, bool _OrderByShortDis)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < _TargetList.Count; i++)
        {
            if (_MinDis <= Vector2.Distance(_TargetList[i].transform.position, _CenterGO.transform.position) &&
                _MaxDis >= Vector2.Distance(_TargetList[i].transform.position, _CenterGO.transform.position))
            {
                result.Add(_TargetList[i]);
            }
        }

        return _OrderByShortDis ?
            result.OrderBy(obj => Vector2.Distance(obj.transform.position, _CenterGO.transform.position)).ToList() :
            result.OrderByDescending(obj => Vector2.Distance(obj.transform.position, _CenterGO.transform.position)).ToList();
    }

    #endregion

    #region Add

    // 실제 주소값 Vector에 추가
    public static void Add_RefValue(ref Vector2 _Variable, Vector2 _AddValue)
    {
        _Variable += _AddValue;
    }

    #endregion


    #endregion

    #region About Quaternion

    #region Get

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

    #region Add

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

    #endregion

    #endregion

    #region About Anim

    #region Set

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

    // 방향성 Anim 컨트롤러의 애니메이터들의 속도 조절
    public static void Set_AnimSpeed(List<DirectionalAnimController> _TargetList, float _Speed)
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            DevTool.Set_AnimSpeedAndSize(_TargetList[i].ThisComp, _Speed, _AnimSize: 1);
        }
    }

    // 지정 애니메이션 클립에서 마지막 프레임 스프라이트 반환
    public static Sprite Get_LastFrameSprite(AnimationClip _Clip, SpriteRenderer _TargetSR)
    {
        if (_Clip == null || _TargetSR == null)
            return null;

        // 마지막 프레임 시간 계산
        float epsilon = 1f / Mathf.Max(_Clip.frameRate, 30f) * 0.5f;
        float sampleTime = Mathf.Max(0f, _Clip.length - epsilon);

        // 현재 sprite 기억
        Sprite original = _TargetSR.sprite;

        // 샘플링해서 마지막 프레임 적용
        _Clip.SampleAnimation(_TargetSR.gameObject, sampleTime);
        Sprite last = _TargetSR.sprite;

        // 원래 sprite로 복구 (부작용 방지)
        _TargetSR.sprite = original;

        return last;
    }

    #endregion

    #region Is
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

    #endregion

    #endregion

    #region About Tween

    #region Play
    public static Tween Play_Tween(Tween _Tween, Dele _Start = null, Dele _Update = null, Dele _Complete = null)
    {
        _Tween
            .OnStart(() => { if (_Start != null) _Start(); })
            .OnUpdate(() => { if (_Update != null) _Update(); })
            .OnComplete(() => { if (_Complete != null) _Complete(); });

        return _Tween;
    }

    #endregion

    #region Set

    public static void Set_KillTween<T>(T _Comp)
    {
        if (_Comp != null && DOTween.IsTweening(_Comp))
        { DOTween.Kill(_Comp); }
    }

    public static void Set_KillTween(string _ID)
    {
        if (DOTween.IsTweening(_ID))
        { DOTween.Kill(_ID); }
    }

    public static void Set_KillTween(Sequence _Seq)
    {
        if (_Seq != null && DOTween.IsTweening(_Seq))
        { DOTween.Kill(_Seq); }
    }

    public static void Set_CompleteTween<T>(T _Comp)
    {
        if (_Comp != null && DOTween.IsTweening(_Comp))
        { DOTween.Complete(_Comp); }
    }

    public static void Set_CompleteTween(Sequence _Seq)
    {
        if (_Seq != null && DOTween.IsTweening(_Seq))
        { DOTween.Complete(_Seq); }
    }

    #endregion

    #endregion

    #region About Player

    public readonly static int SkillAmount = 2;
    public readonly static int BU_MaxLevel = 10;
    public readonly static int BU_LevelInterval = 3;

    #region Index from DmgType, Critical

    // 데미지와 크리티컬로 인덱스 구하기
    // 0: PB / 1: PC / 2: EB / 3: EC
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

    // 1:P / 2:E
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

    // 0,2: B / 1,3: C
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

    #region For Player

    public static Vector2 Get_DirForPlayer<T>(T _TType) where T : MonoBehaviour
    {
        return Get_Dir(_TType.gameObject, PlayerManager.Instance.PlayerController.gameObject);
    }

    public static float Get_DisForPlayer<T>(T _TType) where T : MonoBehaviour
    {
        return Get_Dis(_TType.gameObject, PlayerManager.Instance.PlayerController.gameObject);
    }

    #endregion

    #region Base Upgrade

    // 인풋의 EUI값으로 부터 맞는 BUState을 반환
    public static BUState<T> Get_ThisData<T>(List<BUShopData<T>> _ShopDataList, TxtAmountForBuyEUIController _InMTAFB)
    {
        for (int i = 0; i < _ShopDataList.Count; i++)
        {
            if (_ShopDataList[i].UpgradeEUI == _InMTAFB)
            {
                return _ShopDataList[i].State;
            }
        }
        return null;
    }

    #endregion

    #endregion

    #region About Buff

    // 냉기 데미지 갑소 계산 (효과)
    public static float Get_DmgEffectByCold(float _BaseDmg, EnemyBuffController _EnemyBuff)
    {
        return _BaseDmg * (1f - 
            (_EnemyBuff.ColdStack.CurrentStack * (_EnemyBuff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));
    }

    // 부식 데미지 증가 계산 (효과)
    public static float Get_DmgEffectByCorrosion(float _BaseDmg, EnemyBuffController _EnemyBuff)
    {
        return _BaseDmg *= (1f + 
            (_EnemyBuff.CorrosionStack.CurrentStack * (_EnemyBuff.DecayStack.CurrentStack + 1) * 0.01f));
    }


    // 화염
    public static float Get_FrameDmg(EnemyBuffController _Buff)
    {
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 0.01f
            * _Buff.FlameStack.CurrentStack
            * (_Buff.InfernoStack.CurrentStack + 1);
    }
    public static float Get_FlameExplDmg(out eDamageType _DmgType)
    {
        _DmgType = eDamageType.Physics;
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 10f;
    }


    // 냉기
    public static float Get_ColdExplDmg(out eDamageType _DmgType)
    {
        _DmgType = eDamageType.Energy;
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 7.5f;
    }


    // 전기
    public static float Get_ElectricityDmg(EnemyBuffController _Buff)
    {
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 0.005f
            * _Buff.ElectricityStack.CurrentStack
            * (_Buff.PlasmaStack.CurrentStack + 1);
    }
    public static float Get_ElectricityExplDmg(out eDamageType _DmgType)
    {
        _DmgType = eDamageType.Energy;
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 7.5f;
    }

    // 부식
    public static float Get_CorrosionExplDmg(out eDamageType _DmgType)
    {
        _DmgType = eDamageType.Physics;
        return PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 5f;
    }

    #endregion

    #region About Collider
    public static bool Can_Collding<T>(Collider2D _Col, string _Tag, HashSet<StaticDepthController> _AlreadyList, out T _TType) where T : StaticDepthController
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
/*
    public static bool Has_SolidColliderAt(Vector2 _WorldPos, LayerMask _LayerMask = default)
    {
        Collider2D hit;

        if (_LayerMask.value == 0) // 레이어 지정 없으면 전부 검사
            hit = Physics2D.OverlapPoint(_WorldPos);
        else
            hit = Physics2D.OverlapPoint(_WorldPos, _LayerMask);

        return hit != null && !hit.isTrigger;
    }
*/
    #endregion

    #region About Nav

    #region Is

    // 중간에 벽이 있는지
    public static bool Is_Exist_UseLine<T>(T _Start, T _End, string _LayerName) where T : MonoBehaviour
    {
        return Physics2D.Linecast(_Start.transform.position, _End.transform.position, LayerMask.GetMask(_LayerName)).collider != null;
    }

    public static bool Is_Exist_UseLine(Transform _Start, Transform _End, string _LayerName)
    {
        return Physics2D.Linecast(_Start.position, _End.position, LayerMask.GetMask(_LayerName)).collider != null;
    }

    public static bool Is_Exist_UseLine(Vector2 _Start, Vector2 _End, string _LayerName)
    {
        return Physics2D.Linecast(_Start, _End, LayerMask.GetMask(_LayerName)).collider != null;
    }


    public static bool Is_Exist_UseCircle(Transform _StartTF, Transform _EndTF, string _LayerName, float _Radius)
    {
        return Physics2D.CircleCast(_StartTF.position, _Radius, (_EndTF.position - _StartTF.position).normalized,
            Vector2.Distance(_StartTF.position, _EndTF.position), LayerMask.GetMask(_LayerName)).collider != null;
    }
    public static bool IsOnNavMesh(Vector2 _Point, float _MaxDistance = 0.1f, int _AreaMask = NavMesh.AllAreas, bool _PlaneXY = true)
    {
        // Vector2 → Vector3 변환
        Vector3 pos3 = _PlaneXY
            ? new Vector3(_Point.x, _Point.y, 0f) // XY 평면
            : new Vector3(_Point.x, 0f, _Point.y); // XZ 평면

        return NavMesh.SamplePosition(pos3, out _, _MaxDistance, _AreaMask);
    }
    #endregion

    #region Get

    public static bool TryGetDirNavMeshEnd(
        Vector2 start,
        Vector2 dir,
        out Vector2 endPoint,
        float maxDistance = 100f,
        int areaMask = NavMesh.AllAreas,
        bool planeXY = true)
    {
        endPoint = default;

        if (dir.sqrMagnitude < 1e-8f)
            return false;

        // 2D -> 3D 변환 (세팅에 따라 XY 또는 XZ)
        Vector3 To3(Vector2 v2) => planeXY
            ? new Vector3(v2.x, v2.y, 0f)
            : new Vector3(v2.x, 0f, v2.y);

        Vector2 To2(Vector3 v3) => planeXY
            ? new Vector2(v3.x, v3.y)
            : new Vector2(v3.x, v3.z);

        // 시작 지점을 NavMesh 위의 유효 지점으로 스냅
        Vector3 s3 = To3(start);
        if (!NavMesh.SamplePosition(s3, out var sHit, 0.2f, areaMask))
        {
            // 근처 반경을 조금 늘려 재시도
            if (!NavMesh.SamplePosition(s3, out sHit, 1.0f, areaMask))
                return false; // NavMesh 위에서 시작하지 않음
        }

        Vector3 dir3 = To3(dir.normalized) - To3(Vector2.zero);

        // 지수를 키우며(Raycast 실패 시) 멀리까지 쏴서 '첫 경계'를 찾는다
        float dist = Mathf.Max(1f, maxDistance);
        const float MaxCap = 100000f;   // 안전 상한
        const int MaxIters = 20;        // 안전 반복 상한

        for (int i = 0; i < MaxIters && dist <= MaxCap; i++)
        {
            Vector3 target = sHit.position + dir3 * dist;

            // NavMesh 직선 경로 상의 장애/경계 검사
            if (NavMesh.Raycast(sHit.position, target, out var hit, areaMask))
            {
                // 첫 번째 경계(또는 장애물) 지점
                endPoint = To2(hit.position);
                return true;
            }

            // 아직 경계에 닿지 않았다면 더 멀리
            dist *= 2f;
        }

        // 여기까지 왔다면 매우 멀리까지도 경계가 없었던 상황.
        // 마지막 타깃 근처의 NavMesh 유효 지점을 반환(사실상 무한 직선상 최원점).
        Vector3 farTarget = sHit.position + dir3 * Mathf.Min(dist, MaxCap);
        if (NavMesh.SamplePosition(farTarget, out var farHit, 2f, areaMask))
        {
            endPoint = To2(farHit.position);
            return true;
        }

        return false;
    }

    #endregion

    #endregion

    #region About UI

    #region Color

    public static void Set_Color<T>(Color _Clr, List<T> _TargetList) where T : Component
    {
        for (int i = 0; i < _TargetList.Count; i++)
        {
            switch (_TargetList[i]) 
            {
                case TMP_Text tmp:
                    Set_Color(_Clr, tmp);
                    break;

                case Image img:
                    Set_Color(_Clr, img);
                    break;


                default:
                    break;
            }
        }
    }

    public static void Set_Color(Color _Clr, TMP_Text _Comp)
    {
        if (_Comp == null) return;
        _Comp.color = new Color(_Clr.r, _Clr.g, _Clr.b, _Comp.color.a);
    }

    public static void Set_Color(Color _Clr, Image _Comp)
    {
        if (_Comp == null) return;
        _Comp.color = new Color(_Clr.r, _Clr.g, _Clr.b, _Comp.color.a);
    }

    public static void Set_AlphaColor(Light2D _Light, float _A)
    {
        _Light.color = Get_AlphaColor(_Light, _A);
    }

    public static void Set_AlphaColor(Image _Img, float _A)
    {
        _Img.color = Get_AlphaColor(_Img, _A);
    }

    public static void Set_AlphaColor(TMP_Text _Txt, float _A)
    {
        _Txt.color = Get_AlphaColor(_Txt, _A);
    }


    public static Color Get_AlphaColor(Light2D _Light, float _A)
    {
        Color clr = _Light.color;
        clr.a = _A;
        return clr;
    }

    public static Color Get_AlphaColor(Image _Img, float _A)
    {
        Color clr = _Img.color;
        clr.a = _A;
        return clr;
    }

    public static Color Get_AlphaColor(TMP_Text _Txt, float _A)
    {
        Color clr = _Txt.color;
        clr.a = _A;
        return clr;
    }


    public static void Set_TxtList(List<TMP_Text> _Txt, string _s)
    {
        for (int i = 0; i< _Txt.Count; i++)
        {
            _Txt[i].text = _s;
        }
    }

    #endregion

    #region Image

    // 커지고 작아지는 효과
    public static Tween Play_ScalePulse(RectTransform _RT, float _BigScale, float _OriginalScale = 1f, float _DurTime = 0.4f)
    {
        return _RT.DOScale(_BigScale, _DurTime * 0.5f)
            .OnComplete(() =>
            {
                _RT.DOScale(_OriginalScale, _DurTime * 0.5f);
            });
    }

    // 투명도 효과
    public static Tween Play_FadePulse(Image _Img, float _HighAplha = 1f, float _OriginalAlpha = 0.25f, float _DurTime = 0.4f)
    {
        return _Img.DOFade(_HighAplha, _DurTime * 0.5f)
           .OnComplete(() =>
           {
               _Img.DOFade(_OriginalAlpha, _DurTime * 0.5f);
           });
    }

    #endregion

    #region Interact

    public static string Get_InteractingAnnoTxt(IInteract _II, out bool _CanInteract)
    {
        _CanInteract = true;
        if (_II == null)
            return "";

        string result = _II.Get_InteractName(out bool canInteract);
        _CanInteract = canInteract;

        return result;
    }

    #endregion

    #region Dur

    public static void Set_Dur(int _DurAmount, List<Image> _ImgList, TMP_Text _Txt)
    {
        _Txt.text = _DurAmount.ToString();
        for (int i = 0; i < _ImgList.Count; i++)
        {
            if (_DurAmount > i) // On
            {
                _ImgList[i].color = Color.white;
            }
            else // Off
            {
                _ImgList[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    #endregion

    #endregion

    #region About Interface

    public static void Play_AllIWhen<T>(List<T> _IWhenList) where T : IWhen
    {
        if (_IWhenList.Count <= 0) return;

        for (int i = 0; i < _IWhenList.Count; i++)
            _IWhenList[i].Play_When();
    }

    public static T Get_SyncValue<T>(List<T> _TList, int _SyncRank)
    {
        return _TList[_SyncRank - 1];
    }

    #endregion

    #region About TrueShadow

    public static int Get_TSChildIndex<T>(T _T, int _Index) where T : MonoBehaviour
    {
        return Get_TSChildIndex(_T.gameObject, _Index);
    }

    public static int Get_TSChildIndex(GameObject _GO, int _Index)
    {
        return _GO.transform.GetChild(_Index).name != $"{_GO.name}'s Shadow" ?
            _Index : _Index + 1;
    }

    public static int Get_TSChildIndex(Transform _TF, int _Index)
    {
        return _TF.GetChild(_Index).name != $"{_TF.gameObject.name}'s Shadow" ?
            _Index : _Index + 1;
    }

    public static int Get_TSChildIndex(Component _Comp, int _Index)
    {
        return _Comp.gameObject.transform.GetChild(_Index).name != $"{_Comp.gameObject.name}'s Shadow" ?
            _Index : _Index + 1;
    }

    #endregion
}

#endregion

#region ========== CLASS

#region Class : Title UI

[System.Serializable]
public class TitleElement
{
    public RectTransform MovingRT;

    public float MovingPowerX;
    public float MovingPowerY;
}

[System.Serializable]
public class TitleTSTFElement
{
    public Transform ThisTSParentTF;

    public float Min;
    public float Max;

    public float DurTime;

    public List<TrueShadow> Get_TargetTSList()
    {
        return DevTool.Get_ChildList<TrueShadow>(ThisTSParentTF);
    }
}

[System.Serializable]
public class TitleTSElement
{
    public List<TrueShadow> ThisTSList;

    public float Min;
    public float Max;

    public float DurTime;
}

#endregion

#region Class : PublicData

[System.Serializable]
public class RefData<T>
{
    public T Value;

    public RefData(T _t)
    {
        Value = _t;
    }

}



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

    public CoupleData(CoupleData<T> _Data)
    {
        TypeBase = _Data.TypeBase;
        TypeSpecial = _Data.TypeSpecial;
    }

    public CoupleData(T _Base, T _Special)
    {
        TypeBase = _Base;
        TypeSpecial = _Special;
    }
    public T Get_Base(bool _Yes)
    {
        if (_Yes)
        {
            return TypeBase;
        }
        else
        {
            return TypeSpecial;
        }
    }

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
public class CouplePair<T>
{
    [SerializeField] public CoupleData<T> TypeBase;
    [SerializeField] public CoupleData<T> TypeSpecial;

    public CoupleData<T> Get_Base(bool _Yes)
    {
        if (_Yes)
        {
            return TypeBase;
        }
        else
        {
            return TypeSpecial;
        }
    }

    public CoupleData<T> Get_Special(bool _Yes)
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
}

[System.Serializable]
public class ChargeCooltimeData : CooltimeData
{
    public ChargeCooltimeData() : base() { }

    public ChargeCooltimeData(float _Max, float _Current = 0f) : base(_Max, _Current) { }

    public bool Is_Charge(float _DeltaTime)
    {
        if (Current >= Max)
        {
            Current = 0;
            return true;
        }
        else
        {
            Current += _DeltaTime;
            return false;
        }
    }
}

[System.Serializable]
public class AlwaysCooltimeData : CooltimeData
{
    public AlwaysCooltimeData() : base() { }

    public AlwaysCooltimeData(float _Max, float _Current = 0f) : base(_Max, _Current) { }

    public bool Is_Full(float _DeltaTime)
    {
        Current += _DeltaTime;

        if (Current >= Max)
        {
            Current -= Max;
            return true;
        }
        else
        {
            return false;
        }
    }
}


[System.Serializable]
public class IDWithClass<T>
{
    public int ID;
    public T TypeClass;
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


#region Class : Alive

[System.Serializable]
public class DeadParticleElement
{
    public Sprite Sprite;
    public Vector2 ShadowSize;
}


#endregion

#region Class : State : Combat

[System.Serializable]
public class CombatOwner
{
    public eCombatOwner Owner;
    public int ID;

    public CombatOwner(CombatOwner _Owner)
    {
        Owner = _Owner.Owner;
        ID = _Owner.ID;
    }

    public CombatOwner(eCombatOwner _TypeOwner, int _ID = -1)
    {
        Owner = _TypeOwner;
        ID = _ID;
    }
}

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
    [SerializeField] public CombatOwner OwnerData;

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
        OwnerData = new CombatOwner(_State.OwnerData);
        DmgState = new DmgState(_State.DmgState);
        CriticalState = new CriticalState(_State.CriticalState);
        KnockbackState = new KnockbackState(_State.KnockbackState);
    }

    public CombatState(CombatOwner _Owner, DmgState _DmgState, CriticalState _CriticalState, KnockbackState _KnockbackState)
    {
        OwnerData = new CombatOwner(_Owner);
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

    [SerializeField] public bool IsStatus = false;
    [SerializeField] public eStatusEffect StatusType;

    #endregion

    #region Constructor

    public BulletState(CombatState _State, bool _CheckIsCritical, float _MuzzleSpeed, float _AliveTime) : 
        base(_State)
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

    public BulletState(BulletState _State, bool _CheckIsCritical) : 
        base(_State.OwnerData, _State.DmgState, _State.CriticalState, _State.KnockbackState)
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

    #region Status

    public void Set_Status(bool _IsOn, eStatusEffect _StatueType)
    {
        IsStatus = _IsOn;
        StatusType = _StatueType;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        base.Reset_State();

        IsCritical = false;
        MuzzleSpeed = 0;
        AliveTime = 0;
        IsStatus = false;
    }

    #endregion
}

#endregion

#region Class : State : Combat : Attacker

[System.Serializable]
public class AttackerState : CombatState
{
    #region Constructor

    public AttackerState(CombatState _State) : 
        base(_State.OwnerData, _State.DmgState, _State.CriticalState, _State.KnockbackState) { }

    #endregion
}

#endregion

#region Class : State : Combat : Explosion

[System.Serializable]
public class ExplosionState : CombatState
{
    #region Value

    [Space(10)]
    [Header("=== AttackSize")]
    [SerializeField] public AttackSizeState AttackSizeState;

    public bool IsFire = false;
    public bool IsCold = false;
    public bool IsElectricity = false;
    public bool IsCorrosion = false;

    #endregion

    #region Constructor

    public ExplosionState(CombatState _State, AttackSizeState _SizeState, List<bool> _IsStatusList) : 
        base(_State.OwnerData, _State.DmgState, _State.CriticalState, _State.KnockbackState) 
    {
        AttackSizeState = new AttackSizeState(_SizeState);

        IsFire = _IsStatusList[0];
        IsCold = _IsStatusList[1];
        IsElectricity = _IsStatusList[2];
        IsCorrosion = _IsStatusList[3];
    }
    public ExplosionState(ExplosionState _State) : 
        base(_State.OwnerData, _State.DmgState, _State.CriticalState, _State.KnockbackState)
    {
        AttackSizeState = new AttackSizeState(_State.AttackSizeState);

        IsFire = _State.IsFire;
        IsCold = _State.IsCold;
        IsElectricity = _State.IsElectricity;
        IsCorrosion = _State.IsCorrosion;
    }

    #endregion

    #region Get 

    public List<bool> Get_AttributeCondition()
    {
        return new List<bool> { IsFire, IsCold, IsElectricity, IsCorrosion };
    }

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

[System.Serializable]
public class AttackSizeState : ElementState
{
    #region Value

    [SerializeField] public float Size;

    #endregion

    #region Constructor

    public AttackSizeState(AttackSizeState _State)
    {
        Size = _State.Size;
    }

    public AttackSizeState(float _Size)
    {
        Size = _Size;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        Size = 1f;
    }

    #endregion
}

#endregion


#region Class : State : Player : BU Shop

// Level, State을 저장 관리하고, 상점까지 통괄

[System.Serializable]
public class BUShopSkillData<T, U>
{
    public BUShopData<T> Skill_CooltimeShop;
    public BUShopData<T> Skill_PowerShop;
    public BUShopData<U> Skill_TierShop;
}

[System.Serializable]
public class BUShopData<T>
{
    #region Value

    #region - Insprector

    [SerializeField] public TxtAmountForBuyEUIController UpgradeEUI;

    #endregion

    #region - Hide

    [HideInInspector] public BUState<T> State;
    [HideInInspector] private BULevelData<T> LevelData;

    [HideInInspector] public string Name;
    [HideInInspector] public string Desc;

    #endregion

    #endregion

    #region Offset

    public void Offset(
        BUState<T> _State,
        BULevelData<T> _LevelData,
        List<BUShopData<T>> _AllList,
        BaseUpgradeUIController _Owner)
    {
        State = _State;
        LevelData = _LevelData;

        UpgradeEUI.Offset(_Owner);

        State.Offset(UpgradeEUI, LevelData);
        State.Set_BuffedState();

        _AllList.Add(this);
    }

    public void Set_LanguageTxt(string _Name, string _Desc)
    {
        Name = _Name;
        Desc = _Desc;

        UpgradeEUI.Set_LanguageTxt(Name, Desc);
    }

    #endregion

    #region Buy

    private bool Can_Buy()
    {
        return PlayerManager.Instance.PlayerController.Is_EnoughChargedBettery(LevelData.LevelDataList[State.CurrentLevel.Value].NeedEC_ForUpgrade) &&
            BaseUpgradeController.UsingShop != null &&
            BaseUpgradeController.UsingShop.CurrentDur > 0;
    }

    public void Try_Buy()
    {
        if (Can_Buy())
        {
            // Dur
            BaseUpgradeController.UsingShop.Take_Damage(_SpawnItem: false);

            // Cost
            PlayerManager.Instance.PlayerController.Use_ChargedBettery(LevelData.LevelDataList[State.CurrentLevel.Value].NeedEC_ForUpgrade);

            Set_LevelUp();
        }
    }

    private void Set_LevelUp()
    {
        // Lv Up
        State.CurrentLevel.Value++;
        State.ActualState.Value = LevelData.LevelDataList[State.CurrentLevel.Value - 1].UpgradeValue;
        State.Set_BuffedState();

        // Can Lv Up
        UpgradeEUI.BuyBtn.ThisBtn.interactable = DevTool.BU_MaxLevel <= State.CurrentLevel.Value ? false : true;

        // Desc
        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_Desc(UpgradeEUI, UpgradeEUI.SkillNameTxt.text);
    }

    #endregion
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
    [Header("=== Level Value")]
    public List<BUEachLevelData<T>> LevelDataList;

    // Base Offset
    public void Offset(BUState<T> _BaseValue)
    {
        LevelDataList = new List<BUEachLevelData<T>>();

        if (_BaseValue.BaseState.GetType() == typeof(float))
        {
            Offset_Float(
                float.Parse(_BaseValue.BaseState.ToString()),
                DevTool.Get_ParseFloatList(_BaseValue.UpgradeValueByLevelRange));
        }
        else if (_BaseValue.BaseState.GetType() == typeof(int))
        {
            Offset_Int(
                int.Parse(_BaseValue.BaseState.ToString()),
                DevTool.Get_ParseIntList(_BaseValue.UpgradeValueByLevelRange));
        }
    }

    // FLOAT
    public void Offset_Float(float _FloatValue, List<float> _UpgradeValue)
    {
        for (int i = 0; i < DevTool.BU_MaxLevel; i++)
        {
            BUEachLevelData<T> eachLevelData = new BUEachLevelData<T>();

            decimal stateValue = i == 0 ?
                 (decimal)(_FloatValue + _UpgradeValue[0]) :
                 (decimal)((float)LevelDataList[i - 1].Get_UpgradeValue() + _UpgradeValue[(int)(i / DevTool.BU_LevelInterval)]);

            eachLevelData.Set_UpgradeValue(Mathf.RoundToInt((float)stateValue * 100f) / 100f);
            eachLevelData.NeedEC_ForUpgrade = (int)(i / DevTool.BU_LevelInterval) + 1;

            LevelDataList.Add(eachLevelData);
        }
    }

    // INT
    public void Offset_Int(int _IntValue, List<int> _UpgradeValue)
    {
        for (int i = 0; i < DevTool.BU_MaxLevel; i++)
        {
            BUEachLevelData<T> test = new BUEachLevelData<T>();

            int stateValue = i == 0 ?
                (_IntValue + _UpgradeValue[0]) :
                ((int)LevelDataList[i - 1].Get_UpgradeValue() + _UpgradeValue[(int)(i / DevTool.BU_LevelInterval)]);
            
            test.Set_UpgradeValue((int)stateValue);
            test.NeedEC_ForUpgrade = (int)(i / DevTool.BU_LevelInterval) + 1; 
            
            LevelDataList.Add(test);
        }
    }
}

[System.Serializable]
public class BUEachLevelData<T>
{
    public T UpgradeValue;
    public int NeedEC_ForUpgrade;

    public void Set_UpgradeValue(object _Value)
    {
        UpgradeValue = (T)_Value;
    }

    public object Get_UpgradeValue()
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
    #region Value

    #region - Inspector

    [SerializeField] public T BaseState;
    [SerializeField] public List<T> UpgradeValueByLevelRange;
    [SerializeField] public ReactiveProperty<T> ActualState;

    #endregion

    #region - Hide

    [HideInInspector] public ReactiveProperty<int> CurrentLevel = new();
    [HideInInspector] private List<BuffState<T>> BuffList = new List<BuffState<T>>();

    [HideInInspector] public T BuffedState { get; set; }

    #endregion

    #endregion

    #region Offset

    public void Offset(
        TxtAmountForBuyEUIController EachEUI, 
        BULevelData<T> _UpgradeLevelData)
    {
        CurrentLevel.Value = 0;
        CurrentLevel
           .Subscribe(_CurrentLevel =>
           {
               if (_CurrentLevel < DevTool.BU_MaxLevel)
               {
                   EachEUI.Set(_CurrentLevel, _UpgradeLevelData.LevelDataList[_CurrentLevel].NeedEC_ForUpgrade);
               }
               else
               {
                   EachEUI.Set(_CurrentLevel, 0);
               }

               EachEUI.Set_InnerAlpha((float)_CurrentLevel / (float)DevTool.BU_MaxLevel);
           });
    }

    #endregion

    #region Gain / Lose
    // Gain
    public void Gain_Buff(BuffState<T> _Bs)
    {
        DevTool.Add_InList(BuffList, _Bs);
    }

    // Remove
    public void Lose_Buff(BuffState<T> _Bs)
    {
        DevTool.Remove_InList(BuffList, _Bs);
    }

    #endregion

    #region Buffed

    public void Set_BuffedState()
    {
        if (ActualState.Value.GetType() == typeof(float))
        {
            Set_BufftedState_Float();
        }
        else if (ActualState.Value.GetType() == typeof(int))
        {
            Set_BufftedState_Int();
        }
    }

    private void Set_BufftedState_Float()
    {
        float state = 1.0f;
        for (int i = 0; i < BuffList.Count; i++)
        {
            state += float.Parse(BuffList[i].ActualValue.ToString());
        }
        state *= float.Parse(ActualState.Value.ToString());
        BuffedState = (T)(object)state;
    }

    private void Set_BufftedState_Int()
    {
        int state = 0;
        for (int i = 0; i < BuffList.Count; i++)
        {
            state += int.Parse(BuffList[i].ActualValue.ToString());
        }
        state += int.Parse(ActualState.Value.ToString());
        BuffedState = (T)(object)state;
    }

    #endregion
}

#endregion



#region Class : State : Player : Module ItemData

[System.Serializable]
public class ItemData_Field
{
    [Header("=== ID")]
    public int ID;

    [HideInInspector] public int Rank = 1;

    public ItemData_Field(int _ID)
    {
        ID = _ID;
    }

    public ItemData_Field(int _ID, int _Rank)
    {
        ID = _ID;
        Rank = _Rank;
    }

    public ItemData_Field(ItemData_Field _Data)
    {
        ID = _Data.ID;
        Rank = _Data.Rank;
    }
}

[System.Serializable]
public class ItemData : ItemData_Field
{
    [Header("=== Info")]
    public string Name;
    public string Description;
    public string EquipDescription;
    public Sprite ItemIcon;

    [Header("=== MainChip")]
    public int R1_MainChipID;
    public int R3_MainChipID;
    public int R5_MainChipID;

    public ItemData(int _ID) : base(_ID) { }

    public ItemData(ItemData_Field _Data) : base(_Data) { }

    public ItemData(ItemData _ItemData) : base(_ItemData)
    {
        Set_LanguageTxt(_ItemData);
        ItemIcon = _ItemData.ItemIcon;

        R1_MainChipID = _ItemData.R1_MainChipID;
        R3_MainChipID = _ItemData.R3_MainChipID;
        R5_MainChipID = _ItemData.R5_MainChipID;
    }

    public void Set_LanguageTxt(ItemData _ItemData)
    {
        Name = _ItemData.Name;
        Description = _ItemData.Description;
        EquipDescription = _ItemData.EquipDescription;
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

    protected ModuleItemActivityManager.ActivityFuncDele_MI ThisActivityFuncDele;

    #endregion

    #region Constructor

    public ModuleState() { }

    public void Set_State(ItemData _ItemData)
    {
        ThisItemData = new ItemData(_ItemData);
        ThisActivityFuncDele = ModuleItemActivityManager.Instance.Get_CollectActivity_MI(ThisItemData.ID);
    }

    #endregion

    #region Get

    public static List<ModuleState> Get_AllModuleState()
    {
        return new List<ModuleState>()
        {
            new ModuleItem000(),
            new ModuleItem001(),
            new ModuleItem002(),
            new ModuleItem003(),
            new ModuleItem004(),
            new ModuleItem005(),
        };
    }

    protected int Get_Rank()
    {
        return ThisItemData.Rank;
    }

    #endregion

    #region Interface

    public virtual void Play_When(EnemyController _EC = null)
    {
        ThisActivityFuncDele(Get_Rank(), _EC);
    }

    #endregion
}

#endregion

#region Class : State : Player : MU Code

public class ModuleItem000 : ModuleState, IWhen_Fire
{ public ModuleItem000() : base() { } }

public class ModuleItem001 : ModuleState, IWhen_Fire
{ public ModuleItem001() : base() { } }

public class ModuleItem002 : ModuleState, IWhen_CriticalHit
{ public ModuleItem002() : base() { } }

public class ModuleItem003 : ModuleState, IWhen_CriticalHit
{ public ModuleItem003() : base() { } }

public class ModuleItem004 : ModuleState, IWhen_CriticalHit
{ public ModuleItem004() : base() { } }

public class ModuleItem005 : ModuleState, IWhen_CriticalHit
{ public ModuleItem005() : base() { } }

#endregion

#region Class : State : Player : MU UI

[System.Serializable]
class ForgeInteractPanel
{
    #region Value

    [Space(10)]
    public RectTransform PanelRT; 
    public OwnBtnEUIController PanelBtn;
    public TMP_Text PanelBtnTxt;

    [HideInInspector] public CanvasGroup PanelBtnCG;

    [Space(10)]
    public OwnBtnEUIController RoleBtn;
    public TMP_Text RoleBtnTxt;
    public TMP_Text RoleDescTxt;

    [HideInInspector] public RectTransform RoleBtnTxtRT;
    [HideInInspector] public Tween rtTween = null;

    [Space(10)]
    public List<Image> InnerImgs;

    #endregion

    public void Offset(ModuleUpgradeUIController _MUUC, string _BtnName, string _BtnDesc)
    {
        PanelBtn.Offset();

        PanelBtn.OwnerUIController = _MUUC;

        RoleBtn.Offset();
        RoleBtn.OwnerUIController = _MUUC;

        PanelBtnCG = DevTool.Get_ComponentTType<CanvasGroup>(PanelBtn.gameObject);

        RoleBtnTxtRT = DevTool.Get_ComponentTType<RectTransform>(RoleBtnTxt.gameObject);

        Set_LanguageTxt(_BtnName, _BtnDesc);

        rtTween = RoleBtnTxtRT.DOScale(1.15f, 1.0f)
                .OnPlay(() => { RoleBtnTxtRT.localScale = Vector2.one; })
                .OnKill(() => { RoleBtnTxtRT.localScale = Vector2.one; })
                .SetLoops(-1, LoopType.Yoyo);

        DOTween.Play(rtTween);
    }

    public void Set_LanguageTxt(string _BtnName, string _BtnDesc)
    {
        PanelBtnTxt.text = _BtnName;
        RoleBtnTxt.text = ">>  " + _BtnName + "  <<";
        RoleDescTxt.text = _BtnDesc;
    }
}

#endregion

#region Class : State : Player : MC

[System.Serializable]
public class SynchoronyState : IWhenSync
{
    #region Value

    [SerializeField] public int ID = 0;
    [SerializeField] public int SynergyRank = 0;
    protected ModuleItemActivityManager.ActivityFuncDele_MC ThisActivityFuncDele;

    #endregion

    #region Constructor

    public virtual void Set_State(int _ID, int _SynergyRank)
    {
        ID = _ID;
        SynergyRank = _SynergyRank;
        ThisActivityFuncDele = ModuleItemActivityManager.Instance.Get_CollectActivity_MC(ID);
    }

    #endregion

    #region Get

    public static List<SynchoronyState> Get_AllSynchoronyState()
    {
        return new List<SynchoronyState>()
        {
            new SynchoronyState000(),
            new SynchoronyState001(),
            new SynchoronyState002(),
            new SynchoronyState003(),
            new SynchoronyState004(),
            new SynchoronyState005(),
            new SynchoronyState006(),
            new SynchoronyState007(),
            new SynchoronyState008(),
        };
    }

    #endregion

    #region Play

    public void Play_When(EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        ThisActivityFuncDele(SynergyRank, _Enemy, _Bullet);
    }

    #endregion
}

public class SynchoronyState000 : SynchoronyState, IWhenSync_Fire
{ public SynchoronyState000() : base() { } }

public class SynchoronyState001 : SynchoronyState, IWhenSync_GetFire
{ public SynchoronyState001() : base() { } }
public class SynchoronyState002 : SynchoronyState, IWhenSync_GetCold
{ public SynchoronyState002() : base() { } }
public class SynchoronyState003 : SynchoronyState, IWhenSync_GetElectricity
{ public SynchoronyState003() : base() { } }
public class SynchoronyState004 : SynchoronyState, IWhenSync_GetCorrosion
{ public SynchoronyState004() : base() { } }

public class SynchoronyState005 : SynchoronyState, IWhenSync_CriticalHit
{ 
    public SynchoronyState005() : base() { }

    public static List<float> ValueList = new List<float> { 0.15f, 0.35f, 0.6f };
    public static List<float> CooltimeList = new List<float> { 5f, 6f, 7f };

    public override void Set_State(int _ID, int _SynergyRank)
    {
        base.Set_State(_ID, _SynergyRank);

        if (BuffManager.Instance.Get_CorrectBuff(1) is BuffDmgController dmgBuff)
        {
            dmgBuff.Set_Value(DevTool.Get_SyncValue(ValueList, _SynergyRank));
            dmgBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(CooltimeList, _SynergyRank));
        }
    }
}

public class SynchoronyState006 : SynchoronyState, IWhenSync_Start
{
    public SynchoronyState006() : base() { }

    public static List<float> ValueList = new List<float> { 0.2f, 0.5f, 1f };

    public override void Set_State(int _ID, int _SynergyRank)
    {
        base.Set_State(_ID, _SynergyRank);

        if (BuffManager.Instance.Get_CorrectBuff(8) is BuffCDController CDBuff)
        {
            CDBuff.Set_Value(DevTool.Get_SyncValue(ValueList, _SynergyRank));
        }
    }
}

public class SynchoronyState007 : SynchoronyState, IWhenSync_Hit
{
    public SynchoronyState007() : base() { }

    public static List<float> ValueList = new List<float> { 0.04f, 0.06f, 0.08f };
    public static List<int> MaxChargeList = new List<int> { 5, 7, 10 };
    public static List<float> CooltimeList = new List<float> { 3, 4, 5 };

    public override void Set_State(int _ID, int _SynergyRank)
    {
        base.Set_State(_ID, _SynergyRank);

        if (BuffManager.Instance.Get_CorrectBuff(9) is BuffRofController rofBuff)
        {
            rofBuff.Set_Value(DevTool.Get_SyncValue(ValueList, _SynergyRank));
            rofBuff.Set_MaxChargeValue(DevTool.Get_SyncValue(MaxChargeList, _SynergyRank));
            rofBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(CooltimeList, _SynergyRank));
        }
    }
}

public class SynchoronyState008 : SynchoronyState, IWhenSync_AfterFire
{
    public SynchoronyState008() : base() { }

    public static List<float> ValueList = new List<float> { 1f, 1.5f, 2f };
    public static List<int> MaxChargeList = new List<int> { 2, 3, 4 };
    public static List<float> CooltimeList = new List<float> { 2f, 1.5f, 1f };

    public override void Set_State(int _ID, int _SynergyRank)
    {
        base.Set_State(_ID, _SynergyRank);

        if (BuffManager.Instance.Get_CorrectBuff(10) is BuffDmgController dmgBuff)
        {
            dmgBuff.Set_Value(DevTool.Get_SyncValue(ValueList, _SynergyRank));
            dmgBuff.Set_MaxChargeValue(DevTool.Get_SyncValue(MaxChargeList, _SynergyRank));
            dmgBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(CooltimeList, _SynergyRank));

            dmgBuff.Max_Buff();
        }
    }
}

#endregion


#region Class : State : Ally : MU

public class CopyModuleState
{
    public ModuleState MS;
    public CoupleData<int> OriginalIndex;
    public bool IsEquipped;

    public CopyModuleState(ModuleState _MS, CoupleData<int> _OriginalIndex, bool _IsEquipped)
    {
        MS = _MS;
        OriginalIndex = _OriginalIndex;
        IsEquipped = _IsEquipped;
    }
}

#endregion

#region Class : State : Ally : Sync

public class AllySyncState : IWhenAlly
{
    #region Value

    [SerializeField] public int ID = 0;
    [SerializeField] public int SynergyRank = 0;

    protected AllyController ThisAlly;
    protected AllySyncManager.ActivityFuncDele_Sync ThisActivityFuncDele;

    #endregion

    #region Constructor

    public virtual void Set_State(AllyController _Ally, int _ID, int _SynergyRank)
    {
        ThisAlly = _Ally;
        ID = _ID;
        SynergyRank = _SynergyRank;
        ThisActivityFuncDele = AllySyncManager.Instance.Get_CollectActivity_Sync(ID);
    }

    #endregion

    #region Get

    public static List<AllySyncState> Get_AllSyncState()
    {
        return new List<AllySyncState>()
        {
            new AllySyncState000(),
            new AllySyncState001(),
            new AllySyncState002(),
            new AllySyncState003(),
            new AllySyncState004(),
            new AllySyncState005(),
            new AllySyncState006(),
            new AllySyncState007(),
            new AllySyncState008(),
        };
    }

    #endregion

    #region Play

    public void Play_When(
        EnemyController _Enemy = null, 
        BulletController _Bullet = null, 
        DroppingBombController _DroppingBullet = null)
    {
        ThisActivityFuncDele(ThisAlly, SynergyRank, _Enemy, _Bullet, _DroppingBullet);
    }

    #endregion
}
public class AllySyncState000 : AllySyncState, IWhenAlly_Fire 
{ public AllySyncState000() : base() { } }

public class AllySyncState001 : AllySyncState, IWhenAlly_GetFire
{ public AllySyncState001() : base() { } }
public class AllySyncState002 : AllySyncState, IWhenAlly_GetCold
{ public AllySyncState002() : base() { } }
public class AllySyncState003 : AllySyncState, IWhenAlly_GetElectricity
{ public AllySyncState003() : base() { } }
public class AllySyncState004 : AllySyncState, IWhenAlly_GetCorrosion
{ public AllySyncState004() : base() { } }

public class AllySyncState005 : AllySyncState, IWhenAlly_CriticalHit
{ 
    public AllySyncState005() : base() { }

    public override void Set_State(AllyController _Ally, int _ID, int _SynergyRank)
    {
        base.Set_State(_Ally, _ID, _SynergyRank);

        AllyBuff buff = _Ally.BuffController.Get_AllyBuff("Sync005");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState005.ValueList, _SynergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState005.CooltimeList, _SynergyRank));

        buff.SetOn_State(_ShowAlwaysOnOff: false);
    }
}
public class AllySyncState006 : AllySyncState, IWhenAlly_Start
{ 
    public AllySyncState006() : base() { }

    public override void Set_State(AllyController _Ally, int _ID, int _SynergyRank)
    {
        base.Set_State(_Ally, _ID, _SynergyRank);

        AllyBuff buff = _Ally.BuffController.Get_AllyBuff("Sync006");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState006.ValueList, _SynergyRank));

        buff.SetOn_State(_ShowAlwaysOnOff: false);
    }
}
public class AllySyncState007 : AllySyncState, IWhenAlly_AfterFire
{ 
    public AllySyncState007() : base() { }

    public override void Set_State(AllyController _Ally, int _ID, int _SynergyRank)
    {
        base.Set_State(_Ally, _ID, _SynergyRank);

        AllyBuff buff = _Ally.BuffController.Get_AllyBuff("Sync007");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState007.ValueList, _SynergyRank));
        buff.Set_MaxAmount(DevTool.Get_SyncValue(SynchoronyState007.MaxChargeList, _SynergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState007.CooltimeList, _SynergyRank));

        buff.SetOn_State(_ShowAlwaysOnOff: false);
    }
}

public class AllySyncState008 : AllySyncState, IWhenAlly_Hit
{
    public AllySyncState008() : base() { }

    public override void Set_State(AllyController _Ally, int _ID, int _SynergyRank)
    {
        base.Set_State(_Ally, _ID, _SynergyRank);

        AllyBuff buff = _Ally.BuffController.Get_AllyBuff("Sync008");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState008.ValueList, _SynergyRank));
        buff.Set_MaxAmount(DevTool.Get_SyncValue(SynchoronyState008.MaxChargeList, _SynergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState008.CooltimeList, _SynergyRank));

        buff.SetOn_State(_ShowAlwaysOnOff: true);
    }
}

#endregion

#region Class : State : Ally : Buff

[Serializable]
public class OriginalAllyBuff
{
    [SerializeField] public string Type;
    [SerializeField] public Sprite IconSprite;

    [SerializeField] public float BuffValue = 0;
    [SerializeField] public int BuffMaxAmount = 0;
    [SerializeField] public float MaxCooltime = 0;
    [SerializeField] public bool IsIncrease = false;
    [SerializeField] public bool IsPermanent = false;
}


[Serializable]
public class AllyBuff : OriginalAllyBuff
{
    #region Value

    private AllyController Ally;
    private BuffIconEUIController BuffIconUI = null;

    [SerializeField] private int BuffAmount = 0;
    private RefData<float> ActualBuffValue;

    private bool IsOn = false;
    private bool IsAlwaysShowUI = false;

    [SerializeField] private float CurrentCooltime = 0;

    #endregion

    #region Constructor

    public AllyBuff(AllyController _Ally, OriginalAllyBuff _Original) : base()
    {
        Ally = _Ally;

        Type = _Original.Type;
        IconSprite = _Original.IconSprite;
        BuffValue = _Original.BuffValue;
        BuffMaxAmount = _Original.BuffMaxAmount;
        MaxCooltime = _Original.MaxCooltime;
        IsIncrease = _Original.IsIncrease;
        IsPermanent = _Original.IsPermanent;

        IsOn = false;
        IsAlwaysShowUI = false;
        BuffAmount = 0;
        CurrentCooltime = 0;
        ActualBuffValue = new RefData<float>(0);
    }

    #endregion

    #region Set

    public void SetOn_State(bool _ShowAlwaysOnOff)
    {
        BuffAmount = 0;
        CurrentCooltime = 0;
        ActualBuffValue.Value = 0;

        Ally.BuffController.BuffingState.Add_List(this, Type);
        Set_OnOff(true);
        Set_AlwaysShowUI(_ShowAlwaysOnOff); 

        Ally.BuffController.BuffingState.Set_BuffedAllyState();
        Ally.Set_AllState();
    }

    public void SetOff_State()
    {
        BuffAmount = 0;
        CurrentCooltime = 0;
        ActualBuffValue.Value = 0;

        Ally.BuffController.BuffingState.Remove_List(this, Type);
        Set_OnOff(false);
        Set_AlwaysShowUI(false); 

        Ally.BuffController.BuffingState.Set_BuffedAllyState();
        Ally.Set_AllState();
    }

    private void Set_OnOff(bool _OnOff)
    {
        if (IsOn == _OnOff) return;
        
        IsOn = _OnOff;

        if (!IsOn && BuffIconUI != null) // 꺼짐
        {
            Ally.HUD.TemporaryBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }
    }

    private void Set_AlwaysShowUI(bool _OnOff)
    {
        if (IsAlwaysShowUI == _OnOff) return;

        IsAlwaysShowUI = _OnOff;

        if (IsAlwaysShowUI) // 항상 켜짐
        {
            BuffIconUI = Ally.HUD.TemporaryBuffUI.Get_BuffIconUI();
            BuffIconUI.SetOn(IconSprite, IsAlwaysShowUI ? true : !(BuffAmount == 0));
        }
    }

    #endregion

    #region Set (Change Value)

    public void Set_Value(float _Value)
    {
        BuffValue = _Value;
    }

    public void Set_MaxAmount(int _Value)
    {
        BuffMaxAmount = _Value;
    }

    public void Set_Cooltime(float _Value)
    {
        MaxCooltime = _Value;
    }

    #endregion

    public void SetAndGain_Buff(int _Stack = 1)
    {
        Ally.BuffController.BuffingState.Add_List(this, Type);
        Set_OnOff(true);
        Set_AlwaysShowUI(false);
        Gain_Buff(_Stack);
    }

    #region Gain Loss

    public void Gain_Buff(int _Stack = 1)
    {
        // ui first
        if (BuffIconUI == null)
        {
            BuffIconUI = Ally.HUD.TemporaryBuffUI.Get_BuffIconUI();
        }

        // value
        BuffAmount = Mathf.Min(BuffAmount + _Stack, BuffMaxAmount);
        Set_Value();
        Ally.BuffController.BuffingState.Set_BuffedAllyState();
        Ally.Set_AllState();

        if (!IsIncrease) CurrentCooltime = 0;

        // ui
        BuffIconUI.SetOn(IconSprite, IsAlwaysShowUI ? true : !(BuffAmount == 0));
    }

    public void Reduce_Buff(int _Stack = 1)
    {
        // value
        BuffAmount = Mathf.Max(BuffAmount - _Stack, 0);
        Set_Value();
        Ally.BuffController.BuffingState.Set_BuffedAllyState();
        Ally.Set_AllState();

        if (IsIncrease) CurrentCooltime = 0;

        // ui
        BuffIconUI.SetOn(IconSprite, IsAlwaysShowUI ? true : !(BuffAmount == 0));

        // ui last
        if (BuffAmount == 0 && !IsAlwaysShowUI && BuffIconUI != null)
        {
            Ally.HUD.TemporaryBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }
    }

    #endregion

    #region Cooltime

    public void Caculate_Cooltime(float _DeltaTime)
    {
        if (!IsOn || IsPermanent) return;

        if (IsIncrease)
            Caculate_Cooltime_Increase(_DeltaTime);
        else
            Caculate_Cooltime_Decrease(_DeltaTime);

        if (BuffIconUI != null)
        {
            BuffIconUI.Set_Cooltime(CurrentCooltime / MaxCooltime);
            BuffIconUI.Set_Icon(BuffAmount, BuffMaxAmount);
        }
    }

    private void Caculate_Cooltime_Increase(float _DeltaTime)
    {
        if (BuffAmount >= BuffMaxAmount) return;

        if (MaxCooltime <= CurrentCooltime) // 스택 감소
        {
            CurrentCooltime -= MaxCooltime;
            Gain_Buff(1);
        }
        else // 쿨타임 돌림
        {
            CurrentCooltime += _DeltaTime;
        }
    }

    private void Caculate_Cooltime_Decrease(float _DeltaTime)
    {
        if (BuffAmount <= 0) return;

        if (MaxCooltime <= CurrentCooltime) // 스택 감소
        {
            CurrentCooltime -= MaxCooltime;
            Reduce_Buff(1);
        }
        else // 쿨타임 돌림
        {
            CurrentCooltime += _DeltaTime;
        }
    }

    #endregion

    #region Value

    private void Set_Value()
    {
        ActualBuffValue.Value = BuffAmount * BuffValue;
    }
    
    public float Get_Value()
    {
        return ActualBuffValue.Value;
    }

    #endregion

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



#region Class : Spawn : Enemy

[System.Serializable]
public class EnemySpot
{
    [SerializeField] public int EnemyID;
    [SerializeField] public Transform EnemySpawnTF;
}



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


#region Class : State : Enemy : Buff : Status Effect 

[Serializable]
public class StatusEffect
{
    #region Value

    public delegate void EffectDele();

    [HideInInspector] public EnemyController Enemy;
    [HideInInspector] public BuffIconEUIController BuffIconUI = null;
    [HideInInspector] public Sprite IconSprite;

    public bool IsOn;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect(EnemyController _Enemy, Sprite _IconSprite)
    {
        IsOn = false;

        Enemy = _Enemy;
        IconSprite = _IconSprite;
    }

    #endregion

    #region Clear

    public virtual void Set_Clear()
    {
        IsOn = false;
    }

    #endregion
}

#endregion

#region Class : State : Enemy : Buff : Temporary Effect

[Serializable]
public class StatusEffect_Temporary : StatusEffect
{
    #region Value

    public float MaxCooltime;
    public float CurrentCooltime;

    protected EffectDele GainDele = null;
    protected EffectDele ReduceDele = null;

    #endregion

    #region Contruct
    // 생성자
    public StatusEffect_Temporary(
        EnemyController _Enemy, float _MaxCooltime,
        EffectDele _GainFunc, EffectDele _ReduceFunc, Sprite _IconSprite)
        : base(_Enemy, _IconSprite)
    {
        Enemy = _Enemy;

        MaxCooltime = _MaxCooltime;
        CurrentCooltime = 0;

        GainDele = _GainFunc;
        ReduceDele = _ReduceFunc;

        IconSprite = _IconSprite;
    }
    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int _GainAmount, bool _ShowTxt, CombatOwner _CombatOwner)
    {
        if (BuffIconUI == null)
        { Start_FirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    public virtual void Reduce_Stack(int _GainAmount)
    {
        if (ReduceDele != null)
        { ReduceDele(); }
    }

    // 버프 시작
    protected virtual void Start_FirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.TemporaryBuffUI.Get_BuffIconUI();
            BuffIconUI.SetOn(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.TemporaryBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
        CurrentCooltime = 0;
    }




    public virtual void Caculate_Cooltime(float _DeltaTime)
    {
        BuffIconUI.Set_Cooltime(CurrentCooltime / MaxCooltime);
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Temporary_WithAmount : StatusEffect_Temporary
{
    #region Value

    public eStatusEffect StatusType;

    public int MaxStack;
    public int CurrentStack;
    public int OnceTimeReduceAmount;

    private bool IsResetWhenGain;

    protected EffectDele FullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Temporary_WithAmount(
        EnemyController _Enemy, eStatusEffect _StatusType, int _MaxStack, float _MaxCooltime, int _OnceTimeReduceAmount, bool _IsResetWhenGain,
        EffectDele _GainFunc, EffectDele _ReduceFunc, EffectDele _FullStack,
        Sprite _IconSprite)
        : base(_Enemy, _MaxCooltime, _GainFunc, _ReduceFunc, _IconSprite)
    {
        StatusType = _StatusType;

        MaxStack = _MaxStack;
        CurrentStack = 0;
        OnceTimeReduceAmount = _OnceTimeReduceAmount;

        IsResetWhenGain = _IsResetWhenGain;

        FullStack = _FullStack;
    }

    #endregion

    #region Active Func

    private DeleEnemy Get_PlayerIDele()
    {
        switch (StatusType)
        {
            case eStatusEffect.Flame:
                return new DeleEnemy(ModuleItemManager.Instance.ActiveSync_EnemyTakingFire);

            case eStatusEffect.Cold:
                return new DeleEnemy(ModuleItemManager.Instance.ActiveSync_EnemyTakingCold);

            case eStatusEffect.Electricity:
                return new DeleEnemy(ModuleItemManager.Instance.ActiveSync_EnemyTakingElectricity);

            case eStatusEffect.Corrosion:
                return new DeleEnemy(ModuleItemManager.Instance.ActiveSync_EnemyTakingCorrosion);
        }
        return null;
    }

    private DeleEnemy Get_AllyIDele(int _ID)
    {
        AllyController ally = AllyManager.Instance.AllAlly[_ID];
        switch (StatusType)
        {
            case eStatusEffect.Flame:
                return new DeleEnemy(ally.ActiveAlly_EnemyTakingFire);

            case eStatusEffect.Cold: 
                return new DeleEnemy(ally.ActiveAlly_EnemyTakingCold);

            case eStatusEffect.Electricity: 
                return new DeleEnemy(ally.ActiveAlly_EnemyTakingElectricity);

            case eStatusEffect.Corrosion: 
                return new DeleEnemy(ally.ActiveAlly_EnemyTakingCorrosion);
        }
        return null;
    }

    #endregion

    #region Func

    public int CurrentGainStack = 0;
    // 버프 증가
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt, CombatOwner _CombatOwner)
    {
        CurrentStack = System.Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        // 플레이어 속성
        CurrentGainStack = _GainAmount;
        if (_CombatOwner.Owner == eCombatOwner.Player)
            Get_PlayerIDele()(Enemy);
        else if (_CombatOwner.Owner == eCombatOwner.Ally)
            Get_AllyIDele(_CombatOwner.ID)(Enemy);

        if (IsResetWhenGain)
        {
            CurrentCooltime = 0;
        }

        base.Gain_Stack(_GainAmount, _ShowTxt, _CombatOwner);

        if (CurrentStack >= MaxStack && FullStack != null)
        {
            FullStack();
        }
    }

    public void ReGain_Stack()
    {
        CurrentStack = System.Math.Clamp(CurrentStack + CurrentGainStack, 0, MaxStack);
    }

    // 버프 감소
    public override void Reduce_Stack(int _ReduceAmount)
    {
        base.Reduce_Stack(_ReduceAmount);

        CurrentStack = System.Math.Clamp(CurrentStack - _ReduceAmount, 0, MaxStack);
        if (CurrentStack <= 0)
        {
            Remove_AllStack();
        }
    }


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
        CurrentStack = 0;
    }

    // 쿨타임
    public override void Caculate_Cooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                CurrentCooltime -= MaxCooltime;
                Reduce_Stack(1);
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }

        if (BuffIconUI != null)
        {
            base.Caculate_Cooltime(_DeltaTime);
            BuffIconUI.Set_Icon(CurrentStack, MaxStack);
        }

    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Temporary_WithoutAmount : StatusEffect_Temporary
{
    #region Contruct

    public StatusEffect_Temporary_WithoutAmount(
        EnemyController _Enemy, float _MaxCooltime,
        EffectDele _GainFunc, EffectDele _ReduceFunc,
        Sprite _IconSprite)
        : base(_Enemy, _MaxCooltime, _GainFunc, _ReduceFunc, _IconSprite)
    { }

    #endregion

    #region Func

    // 버프 획득
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt, CombatOwner _CombatOwner)
    {
        CurrentCooltime = 0;
        base.Gain_Stack(_GainAmount, _ShowTxt, _CombatOwner);
    }

    // 버프 제거
    public override void Remove_AllStack()
    {
        base.Reduce_Stack(0);
        base.Remove_AllStack();
    }

    // 쿨타임
    public override void Caculate_Cooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                Remove_AllStack();
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }

        if (BuffIconUI != null)
        {
            base.Caculate_Cooltime(_DeltaTime);
        }
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

#endregion

#region Class : State : Enemy : Buff : Permanent Effect

[Serializable]
public class StatusEffect_Permanent : StatusEffect
{
    #region Value

    protected EffectDele GainDele = null;

    #endregion

    #region Contruct

    public StatusEffect_Permanent(
        EnemyController _Enemy, Sprite _IconSprite,
        EffectDele _GainFunc)
        : base(_Enemy, _IconSprite)
    {
        GainDele = _GainFunc;
    }

    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        if (BuffIconUI == null)
        { Start_FirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    // 필요 없음!

    // 버프 시작
    protected virtual void Start_FirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.PermanentBuffUI.Get_BuffIconUI();
            BuffIconUI.SetOn(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.PermanentBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Permanent_WithAmount : StatusEffect_Permanent
{
    #region Value

    public int MaxStack;
    public int CurrentStack;

    protected EffectDele FullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Permanent_WithAmount(
        EnemyController _Enemy, Sprite _IconSprite,
        EffectDele _GainFunc, EffectDele _FullStack,
        int _MaxStack)
        : base(_Enemy, _IconSprite, _GainFunc)
    {
        MaxStack = _MaxStack;
        FullStack = _FullStack;
        CurrentStack = 0;
    }
    #endregion

    #region Func

    // 버프 증가
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        CurrentStack = System.Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        base.Gain_Stack(_GainAmount, _ShowTxt);
        BuffIconUI.Set_Icon(CurrentStack, MaxStack);

        if (CurrentStack >= MaxStack && FullStack != null)
        {
            FullStack();
        }
    }

    // 버프 감소
    // 필요 없음!


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
        CurrentStack = 0;
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Permanent_WithoutAmount : StatusEffect_Permanent
{
    #region Contruct

    public StatusEffect_Permanent_WithoutAmount(
        EnemyController _Enemy, Sprite _IconSprite, EffectDele _GainFunc)
        : base(_Enemy, _IconSprite, _GainFunc)
    {

    }

    #endregion

    #region Func

    // 버프 증가
    // 필요 없음! 

    // 버프 감소
    // 필요 없음!


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

#endregion


#region Class : Enemy : BossPhase

[System.Serializable]
public class BossPhaseData
{
    public int ThisPhase;
    public float ThisPhaseLimitPercentHP;
    public List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    public ContinuousEnemyPattern SpecialPattern;

}

#endregion

#region Class : Enemy : DropItem

[System.Serializable]
public class EnemyDropItemPercent
{
    [Space(5)]
    [Header("-- Module")]
    [SerializeField] public float ModuleDropPercent = 0.0f;
    [SerializeField] public List<int> ModuleRankPercents;

    [Space(5)]
    [Header("-- Keycard")]
    [SerializeField] public float keycardDropPercent = 0.0f;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] public CoupleData<int> BSAmountMinMax;
    [SerializeField] public CoupleData<int> MSAmountMinMax;
    [SerializeField] public CoupleData<int> CreditAmountMinMax;
    [SerializeField] public CoupleData<int> OverriderAmountMinMax;
    [SerializeField] public CoupleData<float> JouleAmountMinMax;
}

[System.Serializable]
public class CoreDropItemPercent
{
    [Space(5)]
    [Header("-- Core")]
    [SerializeField] public int CoreItemID = 0;
    [SerializeField] public float CoreItemPercent = 0.0f;
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

    public abstract void Set_SortingOrder();

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

    public override void Set_SortingOrder()
    {
        Follower.Set_SortingOrder(UpperOrder + 
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

    public override void Set_SortingOrder()
    {
        Follower.Set_SortingOrder(UpperOrder);
    }

    #endregion
}

#endregion


#region Class : Stage

[System.Serializable]
public class StageData
{
    public string MapIndexName;

    [Space(5)]
    public StageInfo InfoData;

    [Space(5)]
    public StageRoom RoomData;

    [Space(5)]
    public StageEnemy EnemyData;


    [Space(5)]
    public List<Material> MapMaterialUnclear;
    public List<Material> MapMaterialClear;
    public List<StageDoorAnim> MapDoorAnim;

    [HideInInspector] public List<Sprite> AllMapSprite;
    [HideInInspector] public StageMapSprite MapSpriteReso;
    
    public void Offset(List<Sprite> _AllSprite, List<int> _MaterialIndexList)
    {
        MapSpriteReso.Offset(_AllSprite, _MaterialIndexList, MapIndexName);
    }
}

#endregion

#region Class : Stage : StageInfo

[System.Serializable]
public class StageInfo
{
    public int StageID;
}

#endregion

#region Class : Stage : Room

[System.Serializable]
public class StageRoom
{
    public List<GenRoomData> RoomAmount;

    [Space(10)]
    public List<GenDesignatedRoom> DesignatedRoom; 

    [Space(10)]
    public List<GenSpecialRoomData> EntranceRoom;

    [Space(10)]
    public List<GenSpecialRoomData> VaultRoom;

    [Space(10)]
    public List<GenSpecialRoomData> ShopRoom;

    [Space(10)]
    public List<GenSpecialRoomData> AllyShopRoom;

    [Space(10)]
    public List<GenPrisonRoomData> PrisonRoom;
}

[System.Serializable]
public class GenRoomData
{
    public int ID;
    public int Amount;
}

[System.Serializable]
public class GenDesignatedRoom
{
    public int ID;
    public int RuleID;
}

[System.Serializable]
public class GenSpecialRoomData
{
    public int ID;
    public int RuleID;
}

[System.Serializable]
public class GenPrisonRoomData : GenSpecialRoomData
{
    public int TypeID;
}

#endregion

#region Class : Stage : Enemy

[System.Serializable]
public class StageEnemy
{
    public List<GameObject> StageEnemyList;
    public List<GameObject> StageEliteEnemyList;
    public List<GameObject> StageBossEnemyList;
}

#endregion

#region Class : Stage : NextIndex

[System.Serializable]
public class MapNextIndex
{
    public int PastIndex;
    public List<int> NextIndexList;

    public MapNextIndex(int _PastIndex, int _NextIndex)
    {
        PastIndex = _PastIndex;
        NextIndexList = new List<int> { _NextIndex };
    }
}

#endregion  

#region Class : Passage : Middle

public class AllPassageMiddleSpriteData
{
    private Dictionary<string, EachPassageMiddleSpriteData> PassageMiddleSpriteDict;

    public AllPassageMiddleSpriteData(List<List<Sprite>> _AllSprite)
    {
        PassageMiddleSpriteDict = new Dictionary<string, EachPassageMiddleSpriteData>();
        for (int i = 0; i < _AllSprite.Count; i++)
        {
            for (int j = 0; j < _AllSprite.Count; j++)
            {
                string[] fullName = _AllSprite[i][j].name.Split("_");

                PassageMiddleSpriteDict.Add(
                    $"{fullName[1]}_{fullName[3]}_{fullName[5]}", 
                    new EachPassageMiddleSpriteData(_AllSprite[i][j], i));
            }
        }
    }

    public Sprite Get_CorrectSprite(string _Key, out int _MaterialIndex)
    {
        _MaterialIndex = -1;
        if (PassageMiddleSpriteDict.ContainsKey(_Key))
        {
            EachPassageMiddleSpriteData data = PassageMiddleSpriteDict[_Key];
            _MaterialIndex = data.Get_MaterialIndex();
            return data.Get_Sprite();
        }
        return null;
    }
}

public class EachPassageMiddleSpriteData
{
    private Sprite Sprite;
	private int MaterialIndex;

    public EachPassageMiddleSpriteData(Sprite _Sprite, int _MaterialIndex)
    {
        Sprite = _Sprite;
        MaterialIndex = _MaterialIndex;
    }

    public Sprite Get_Sprite()
    {
        return Sprite;
    }

    public int Get_MaterialIndex()
    {
        return MaterialIndex;
    }
}

#endregion


#region Class : AllyUpgrade : Card

[System.Serializable]
public class AllyCardBaseData
{
    public int ID;
    public int Rank;
    public int EssentialID;

    public AllyCardBaseData(int _ID, int _Rank, int _EssentialID)
    {
        ID = _ID;
        Rank = _Rank;
        EssentialID = _EssentialID;
    }
}

[System.Serializable]
public class AllyCardData
{
    public int ID;
    public int Rank;
    public int EssentialID;

    public string Name;
    public string Desc;

    public AllyCardData(AllyCardBaseData _BaseData, string _Name, string _Desc)
    {
        ID = _BaseData.ID;
        Rank = _BaseData.Rank;
        EssentialID = _BaseData.EssentialID;

        Set_LanguageTxt(_Name, _Desc);
    }

    public void Set_LanguageTxt(string _Name, string _Desc)
    {
        Name = _Name;
        Desc = _Desc;
    }
}

#endregion

#region Class : AllyUpgrade : Base(Tuner)

[System.Serializable]
public class AllyBaseTunerData
{
    public AllyEachBaseTunerData Positive0;
    public AllyEachBaseTunerData Positive1;
    public AllyEachBaseTunerData Negative;

    public AllyBaseTunerData(AllyTunerData _Data)
    {
        Positive0 = new AllyEachBaseTunerData(_Data.Positive0);
        Positive1 = new AllyEachBaseTunerData(_Data.Positive1);
        Negative = new AllyEachBaseTunerData(_Data.Negative);
    }
}

[System.Serializable]
public class AllyEachBaseTunerData
{
    public string Type = "";
    public int Rank = 0;

    public AllyEachBaseTunerData(AllyEachTunerData _Data)
    {
        Type = _Data.Type;
        Rank = _Data.Rank;
    }
}


[System.Serializable]
public class AllyBaseUpradeTunerSet
{
    public List<AllyTunerData> AllyTunerDataList;

    public void Offset(int _Amount, List<string> _TypeList, List<int> _RankPercent)
    {
        AllyTunerDataList = new List<AllyTunerData>();
        for (int i = 0; i < _Amount; i++)
        {
            AllyTunerDataList.Add(new AllyTunerData(_TypeList, _RankPercent));
        }
    }
}

[System.Serializable]
public class AllyTunerData
{
    public AllyEachTunerData Positive0;
    public AllyEachTunerData Positive1;
    public AllyEachTunerData Negative;

    public int NeedPay = 0;

    public AllyTunerData(List<string> _TypeList, List<int> _RankPercent)
    {
        Set_Data(_TypeList, _RankPercent);
    }

    public void Set_Data(List<string> _TypeList, List<int> _RankPercent)
    {
        Positive0 = new AllyEachTunerData(_TypeList, _RankPercent);
        Positive1 = new AllyEachTunerData(_TypeList, _RankPercent);
        Negative = new AllyEachTunerData(_TypeList, _RankPercent);

        NeedPay = (int)((Positive0.Rank + Positive1.Rank - Negative.Rank + 5) * 0.5f);
    }
}

[System.Serializable]
public class AllyEachTunerData
{
    public string Type = "";
    public int Rank = 0;

    public AllyEachTunerData(List<string> _TypeList, List<int> _RankPercent)
    {
        Set_Data(_TypeList, _RankPercent);
    }

    public void Set_Data(List<string> _TypeList, List<int> _RankPercent)
    {
        Type = _TypeList[UnityEngine.Random.Range(0, _TypeList.Count)];
        Rank = DevTool.Get_Rank(_RankPercent);
    }
}

#endregion


#region Class : Ally State


[System.Serializable]
public class AllyState
{
    #region Value

    public RefData<float> Dmg;
    public RefData<float> Rof;
    public RefData<float> MovementSpeed;
    public RefData<float> AttackSize;
    public RefData<float> CC;
    public RefData<float> CD;
    public RefData<float> MuzzleSpeed;
    public RefData<float> KBPower;
    public RefData<float> Dur;

    #endregion

    #region Constructor

    public AllyState()
    {
        Dmg = new RefData<float>(1);
        Rof = new RefData<float>(1);
        MovementSpeed = new RefData<float>(1);
        AttackSize = new RefData<float>(1);
        CC = new RefData<float>(1);
        CD = new RefData<float>(1);
        MuzzleSpeed = new RefData<float>(1);
        KBPower = new RefData<float>(1);
        Dur = new RefData<float>(1);
    }

    public AllyState(AllyState _StateValue)
    {
        Dmg = new RefData<float>(_StateValue.Dmg.Value);
        Rof = new RefData<float>(_StateValue.Rof.Value);
        MovementSpeed = new RefData<float>(_StateValue.MovementSpeed.Value);
        AttackSize = new RefData<float>(_StateValue.AttackSize.Value);
        CC = new RefData<float>(_StateValue.CC.Value);
        CD = new RefData<float>(_StateValue.CD.Value);
        MuzzleSpeed = new RefData<float>(_StateValue.MuzzleSpeed.Value);
        KBPower = new RefData<float>(_StateValue.KBPower.Value);
        Dur = new RefData<float>(_StateValue.Dur.Value);
    }

    public void Reset()
    {
        Dmg.Value = 1;
        Rof.Value = 1;
        MovementSpeed.Value = 1;
        AttackSize.Value = 1;
        CC.Value = 1;
        CD.Value = 1;
        MuzzleSpeed.Value = 1;
        KBPower.Value = 1;
        Dur.Value = 1;
    }

    public static AllyState Get_Multiple(AllyState _State0, AllyState _State1)
    {
        AllyState result = new AllyState();

        result.Dmg = new RefData<float>(_State0.Dmg.Value * _State1.Dmg.Value);
        result.Rof = new RefData<float>(_State0.Rof.Value * _State1.Rof.Value);
        result.MovementSpeed = new RefData<float>(_State0.MovementSpeed.Value * _State1.MovementSpeed.Value);
        result.AttackSize = new RefData<float>(_State0.AttackSize.Value * _State1.AttackSize.Value);
        result.CC = new RefData<float>(_State0.CC.Value * _State1.CC.Value);
        result.CD = new RefData<float>(_State0.CD.Value * _State1.CD.Value);
        result.MuzzleSpeed = new RefData<float>(_State0.MuzzleSpeed.Value * _State1.MuzzleSpeed.Value);
        result.KBPower = new RefData<float>(_State0.KBPower.Value * _State1.KBPower.Value);
        result.Dur = new RefData<float>(_State0.Dur.Value * _State1.Dur.Value);

        return result;
    }

    public static AllyState Get_Subtraction(AllyState _Original, AllyState _Exclude)
    {
        AllyState result = new AllyState();

        result.Dmg = new RefData<float>(_Original.Dmg.Value - _Exclude.Dmg.Value);
        result.Rof = new RefData<float>(_Original.Rof.Value - _Exclude.Rof.Value);
        result.MovementSpeed = new RefData<float>(_Original.MovementSpeed.Value - _Exclude.MovementSpeed.Value);
        result.AttackSize = new RefData<float>(_Original.AttackSize.Value - _Exclude.AttackSize.Value);
        result.CC = new RefData<float>(_Original.CC.Value - _Exclude.CC.Value);
        result.CD = new RefData<float>(_Original.CD.Value - _Exclude.CD.Value);
        result.MuzzleSpeed = new RefData<float>(_Original.MuzzleSpeed.Value - _Exclude.MuzzleSpeed.Value);
        result.KBPower = new RefData<float>(_Original.KBPower.Value - _Exclude.KBPower.Value);
        result.Dur = new RefData<float>(_Original.Dur.Value - _Exclude.Dur.Value);

        return result;
    }

    public void Set_ValueLimitRange(float _Min)
    {
        Dmg.Value = Mathf.Max(_Min, Dmg.Value);
        Rof.Value = Mathf.Max(_Min, Rof.Value);
        MovementSpeed.Value = Mathf.Max(_Min, MovementSpeed.Value);
        AttackSize.Value = Mathf.Max(_Min, AttackSize.Value);
        CC.Value = Mathf.Max(_Min, CC.Value);
        CD.Value = Mathf.Max(_Min, CD.Value);
        MuzzleSpeed.Value = Mathf.Max(_Min, MuzzleSpeed.Value);
        KBPower.Value = Mathf.Max(_Min, KBPower.Value);
        Dur.Value = Mathf.Max(_Min, Dur.Value);
    }

    #endregion
}

[System.Serializable]
public class AllyBuffState : AllyState
{
    [HideInInspector] public List<AllyBuff> Dmg_BuffList;
    [HideInInspector] public List<AllyBuff> Rof_BuffList;
    [HideInInspector] public List<AllyBuff> MovementSpeed_BuffList;
    [HideInInspector] public List<AllyBuff> AttackSize_BuffList;
    [HideInInspector] public List<AllyBuff> CC_BuffList;
    [HideInInspector] public List<AllyBuff> CD_BuffList;
    [HideInInspector] public List<AllyBuff> MuzzleSpeed_BuffList;
    [HideInInspector] public List<AllyBuff> KBPower_BuffList;
    [HideInInspector] public List<AllyBuff> Dur_BuffList;

    [HideInInspector] private RefData<bool> Dmg_IsExist;
    [HideInInspector] private RefData<bool> Rof_IsExist;
    [HideInInspector] private RefData<bool> MovementSpeed_IsExist;
    [HideInInspector] private RefData<bool> AttackSize_IsExist;
    [HideInInspector] private RefData<bool> CC_IsExist;
    [HideInInspector] private RefData<bool> CD_IsExist;
    [HideInInspector] private RefData<bool> MuzzleSpeed_IsExist;
    [HideInInspector] private RefData<bool> KBPower_IsExist;
    [HideInInspector] private RefData<bool> Dur_IsExist;

    [HideInInspector] Dictionary<string, List<AllyBuff>> BuffDict;
    [HideInInspector] Dictionary<string, RefData<bool>> BuffIsOnDict;

    public AllyBuffState() : base()
    {
        Dmg_BuffList = new List<AllyBuff>();
        Rof_BuffList = new List<AllyBuff>();
        MovementSpeed_BuffList = new List<AllyBuff>();
        AttackSize_BuffList = new List<AllyBuff>();
        CC_BuffList = new List<AllyBuff>();
        CD_BuffList = new List<AllyBuff>();
        MuzzleSpeed_BuffList = new List<AllyBuff>();
        KBPower_BuffList = new List<AllyBuff>();
        Dur_BuffList = new List<AllyBuff>();

        Dmg_IsExist = new RefData<bool>(false);
        Rof_IsExist = new RefData<bool>(false);
        MovementSpeed_IsExist = new RefData<bool>(false);
        AttackSize_IsExist = new RefData<bool>(false);
        CC_IsExist = new RefData<bool>(false);
        CD_IsExist = new RefData<bool>(false);
        MuzzleSpeed_IsExist = new RefData<bool>(false);
        KBPower_IsExist = new RefData<bool>(false);
        Dur_IsExist = new RefData<bool>(false);

        BuffDict = new Dictionary<string, List<AllyBuff>>
        {
            { AllyManager.StateTypeList[0], Dmg_BuffList },
            { AllyManager.StateTypeList[1], Rof_BuffList },
            { AllyManager.StateTypeList[2], MovementSpeed_BuffList },
            { AllyManager.StateTypeList[3], AttackSize_BuffList },
            { AllyManager.StateTypeList[4], CC_BuffList },
            { AllyManager.StateTypeList[5], CD_BuffList },
            { AllyManager.StateTypeList[6], MuzzleSpeed_BuffList },
            { AllyManager.StateTypeList[7], KBPower_BuffList },
            { AllyManager.StateTypeList[8], Dur_BuffList }
        };

        BuffIsOnDict = new Dictionary<string, RefData<bool>>
        {
            { AllyManager.StateTypeList[0], Dmg_IsExist },
            { AllyManager.StateTypeList[1], Rof_IsExist },
            { AllyManager.StateTypeList[2], MovementSpeed_IsExist },
            { AllyManager.StateTypeList[3], AttackSize_IsExist },
            { AllyManager.StateTypeList[4], CC_IsExist },
            { AllyManager.StateTypeList[5], CD_IsExist },
            { AllyManager.StateTypeList[6], MuzzleSpeed_IsExist },
            { AllyManager.StateTypeList[7], KBPower_IsExist },
            { AllyManager.StateTypeList[8], Dur_IsExist }
        };
    }

    public void Add_List(AllyBuff _Buff, string _Type)
    {
        DevTool.Add_InList(BuffDict[_Type], _Buff);

        if (!BuffIsOnDict[_Type].Value)
            BuffIsOnDict[_Type].Value = true;
    }

    public void Remove_List(AllyBuff _Buff, string _Type)
    {
        DevTool.Remove_InList(BuffDict[_Type], _Buff);

        if (BuffDict[_Type].Count <= 0)
            BuffIsOnDict[_Type].Value = false;
    }

    public AllyState Get_BuffedAllyState()
    {
        return this;
    }

    public void UpdateData(float _DeltaTime)
    {
        foreach(var data in BuffDict)
            if (BuffIsOnDict[data.Key].Value)
                UpdateData(data.Value);
            
        void UpdateData(List<AllyBuff> _BuffList)
        {
            for (int i = 0; i < _BuffList.Count; i++)
                _BuffList[i].Caculate_Cooltime(_DeltaTime);
        }
    }



    public void Set_BuffedAllyState()
    {
        MovementSpeed.Value = Get_BuffValue(MovementSpeed_BuffList);
        Dmg.Value = Get_BuffValue(Dmg_BuffList);
        Rof.Value = Get_BuffValue(Rof_BuffList);
        AttackSize.Value = Get_BuffValue(AttackSize_BuffList);
        CC.Value = Get_BuffValue(CC_BuffList);
        CD.Value = Get_BuffValue(CD_BuffList);
        MuzzleSpeed.Value = Get_BuffValue(MuzzleSpeed_BuffList);
        KBPower.Value = Get_BuffValue(KBPower_BuffList);
        Dur.Value = Get_BuffValue(Dur_BuffList);
    }

    private float Get_BuffValue(List<AllyBuff> _BuffList)
    {
        float result = 1;
        for (int i = 0; i < _BuffList.Count; i++)
        {
            result += _BuffList[i].Get_Value();
        }
        return result;
    }
}


#endregion


#region Class : Ally Sprite

[System.Serializable]
public class AllySpriteSet
{
    public List<Sprite> AllyIdle;
    public List<Sprite> AllyMove;
    public List<Sprite> AllyAttack;

    public AllySpriteSet(string _Name)
    {
        AllyIdle = ResourceManager.Instance.Get_AllySprite(_Name, "Idle");
        AllyMove = ResourceManager.Instance.Get_AllySprite(_Name, "Move");
        AllyAttack = ResourceManager.Instance.Get_AllySprite(_Name, "Attack");
    }
}

#endregion


#region Class : Ally Request

public class AllyCompleteList<T> where T : IWhen_Request
{
    public List<T> List = new List<T>();

    public void Play_Request()
    {
        if (List.Count <= 0) return;

        for (int i = 0; i < List.Count; i++)
            List[i].Play_When_Request();
    }

}

public class AllyFailList<T> where T : IWhen_Fail
{
    public List<T> List = new List<T>();

    // KillEnemy
    public void Start_Fail(T _Request)
    {
        List.Add(_Request);
    }

    public void Play_Request()
    {
        if (List.Count <= 0) return;

        for (int i = 0; i < List.Count; i++)
            List[i].Play_When_Fail();
    }
}


public abstract class AllyRequest
{
    #region Variable

    protected AllyController Ally;

    protected float CompleteProgress = 0;
    protected float MaxCompleteProgress = 0;
    protected float GainCompleteOnceProgress = 0;

    protected float FailProgress = 0;
    protected float MaxFailProgress = 0;
    protected float GainFailOnceProgress = 0;

    protected int Rank = 0;

    private float Point = 0;

    #endregion

    #region Constructor

    public AllyRequest(AllyController _Ally)
    {
        Ally = _Ally;

        Rank = Get_RandomRank();

        CompleteProgress = 0;
        FailProgress = 0;

        Point = (Rank + 1) * 4;

        Set_IWhenAdd();

        Ally.HUD.RequestUI.Set_Request_CompleteProgress(0);
        Ally.HUD.RequestUI.Set_Request_FailProgress(0);
    }

    #endregion

    #region Extra Constructor Func (Static)

    private static List<Func<AllyController, AllyRequest>> RequestTypeList = new()
    {
        Get_AllyRequestType_000,
        Get_AllyRequestType_001
    };

    private static AllyRequest Get_AllyRequestType_000(AllyController _Ally) => new AllyRequest_Slayer(_Ally);
    private static AllyRequest Get_AllyRequestType_001(AllyController _Ally) => new AllyRequest_BountyHunter(_Ally);


    public static AllyRequest Get_AllyRequestType(AllyController _Ally)
    {
        return RequestTypeList[UnityEngine.Random.Range(0, RequestTypeList.Count)](_Ally);
    }


    #endregion

    #region Rank


    // Min: 0 <-> Max: 4
    private static List<int> RankPercent = new List<int>() { 10, 6, 3, 2, 1 };
    public static int Get_RandomRank()
    {
        return DevTool.Get_Grade(RankPercent);
    }

    #endregion

    #region Progress

    protected virtual void Inc_CompleteProgress()
    {
        CompleteProgress = Mathf.Min(CompleteProgress + GainCompleteOnceProgress, MaxCompleteProgress);
        Ally.HUD.RequestUI.Set_Request_CompleteProgress(CompleteProgress / MaxCompleteProgress);

        if (CompleteProgress >= MaxCompleteProgress) Complete();
    }

    protected virtual void Inc_FailProgress()
    {
        FailProgress = Mathf.Min(FailProgress + GainFailOnceProgress, MaxFailProgress);
        Ally.HUD.RequestUI.Set_Request_FailProgress(FailProgress / MaxFailProgress);

        if (FailProgress >= MaxFailProgress) Fail();
    }

    #endregion

    #region Result

    public void Complete()
    {
        Ally.Gain_Trust(Point);
        Ally.DataOff_Request();
        Set_IWhenRemove();
        Ally = null;
    }

    public void Fail()
    {
        Ally.Reduce_Trust(Point);
        Ally.DataOff_Request();
        Set_IWhenRemove();
        Ally = null;
    }

    #endregion

    #region Get (Abstract)

    public abstract string Get_Name();
    public abstract string Get_CompleteDesc();
    public abstract string Get_FailDesc();

    #endregion

    #region Set (Abstract)

    public abstract void Set_IWhenAdd();
    public abstract void Set_IWhenRemove();
    

    #endregion
}


// Type

public class AllyRequest_Slayer : AllyRequest, IWhen_Complete_KillNormalEnemy, IWhen_Fail_TakingDamage
{
    #region Constructor

    public AllyRequest_Slayer(AllyController _Ally) : base(_Ally)
    {
        MaxCompleteProgress = (Rank + 1) * 5; // 총 처치 수
        GainCompleteOnceProgress = 1;

        MaxFailProgress = 6 - Rank; // 실패 피격 수
        GainFailOnceProgress = 1; 
        
        Ally.HUD.RequestUI.Set_Request_CompleteTxt(0, $"{CompleteProgress}/{MaxCompleteProgress}");
        Ally.HUD.RequestUI.Set_Request_FailTxt(0, $"{FailProgress}/{MaxFailProgress}");
    }

    #endregion

    #region Play

    public void Play_When_Request()
    {
        Inc_CompleteProgress();
    }

    public void Play_When_Fail()
    {
        Inc_FailProgress();
    }

    #endregion

    #region Progress

    protected override void Inc_CompleteProgress()
    {
        base.Inc_CompleteProgress();
        Ally.HUD.RequestUI.Set_Request_CompleteTxt(CompleteProgress / MaxCompleteProgress, $"{CompleteProgress}/{MaxCompleteProgress}");
    }

    protected override void Inc_FailProgress()
    {
        base.Inc_FailProgress();
        Ally.HUD.RequestUI.Set_Request_FailTxt(FailProgress / MaxFailProgress, $"{FailProgress}/{MaxFailProgress}");
    }

    #endregion

    #region Get

    public override string Get_Name() { return ResourceManager.Instance.Get_RequestName(0); }
    public override string Get_CompleteDesc() { return ((IWhen_Complete_KillNormalEnemy)this).Get_WhenDesc(); }
    public override string Get_FailDesc() { return ((IWhen_Fail_TakingDamage)this).Get_WhenDesc(); }

    #endregion

    #region Set

    public override void Set_IWhenAdd()
    {
        ((IWhen_Complete_KillNormalEnemy)this).Add_IWhenList();
        ((IWhen_Fail_TakingDamage)this).Add_IWhenList();
    }

    public override void Set_IWhenRemove()
    {
        ((IWhen_Complete_KillNormalEnemy)this).Remove_IWhenList();
        ((IWhen_Fail_TakingDamage)this).Remove_IWhenList();
    }

    #endregion
}

public class AllyRequest_BountyHunter: AllyRequest, IWhen_Complete_KillEliteEnemy, IWhen_Fail_TakingDamage
{
    #region Constructor

    public AllyRequest_BountyHunter(AllyController _Ally) : base(_Ally)
    {
        MaxCompleteProgress = (Rank + 1); // 총 처치 수
        GainCompleteOnceProgress = 1;

        MaxFailProgress = 8 - Rank; // 실패 피격 수
        GainFailOnceProgress = 1;

        Ally.HUD.RequestUI.Set_Request_CompleteTxt(0, $"{CompleteProgress}/{MaxCompleteProgress}");
        Ally.HUD.RequestUI.Set_Request_FailTxt(0, $"{FailProgress}/{MaxFailProgress}");
    }

    #endregion

    #region Play

    public void Play_When_Request()
    {
        Inc_CompleteProgress();
    }

    public void Play_When_Fail()
    {
        Inc_FailProgress();
    }

    #endregion

    #region Progress

    protected override void Inc_CompleteProgress()
    {
        Ally.HUD.RequestUI.Set_Request_CompleteTxt(CompleteProgress / MaxCompleteProgress, $"{CompleteProgress}/{MaxCompleteProgress}");

        base.Inc_CompleteProgress();
    }

    protected override void Inc_FailProgress()
    {
        Ally.HUD.RequestUI.Set_Request_FailTxt(FailProgress / MaxFailProgress, $"{FailProgress}/{MaxFailProgress}");

        base.Inc_FailProgress();
    }
    #endregion

    #region Get

    public override string Get_Name() { return ResourceManager.Instance.Get_RequestName(1); }
    public override string Get_CompleteDesc() { return ((IWhen_Complete_KillEliteEnemy)this).Get_WhenDesc(); }
    public override string Get_FailDesc() { return ((IWhen_Fail_TakingDamage)this).Get_WhenDesc(); }

    #endregion

    #region Set

    public override void Set_IWhenAdd()
    {
        ((IWhen_Complete_KillEliteEnemy)this).Add_IWhenList();
        ((IWhen_Fail_TakingDamage)this).Add_IWhenList();
    }
    public override void Set_IWhenRemove()
    {
        ((IWhen_Complete_KillEliteEnemy)this).Remove_IWhenList();
        ((IWhen_Fail_TakingDamage)this).Remove_IWhenList();
    }

    #endregion
}

#endregion

#region Class : UI

[System.Serializable]
public class MinimapIcon
{
    public CouplePair<Sprite> MinimapElementIcon;
    public List<Vector2Int> RoomVec;
    public Vector2 SpritePivot;
}

[System.Serializable]
public class LanguageTxt
{
    public int ID;
    public List<TMP_FontAsset> FontAssets;
}


#endregion


#region Class : Stage : Reso

[System.Serializable]
public class StageMapSprite
{
    [Header("=== Sprtie: Based on the outer surface")]

    public Dictionary<string, SpriteMaterial> MapSprite = new Dictionary<string, SpriteMaterial>();

    public void Offset(List<Sprite> _AllSprite, List<int> _MaterialIndexList, string _MapIndexName)
    {
        for (int i = 0; i < _AllSprite.Count; i++)
        {
            if (_AllSprite[i].name.Length > 5)
            {
                if (_AllSprite[i].name[5] == 'A') continue; 
                
                MapSprite.Add(
                    _AllSprite[i].name.Substring(5, _AllSprite[i].name.Length - 5),
                    new SpriteMaterial(_AllSprite[i], _MaterialIndexList[i]));
            }
        }
    }
}

[System.Serializable]
public class SpriteMaterial
{
    public Sprite Sprite;
    public int MaterialIndex;

    public SpriteMaterial(Sprite _Sprite, int _MaterialIndex)
    {
        Sprite = _Sprite;
        MaterialIndex = _MaterialIndex;
    }
}


[System.Serializable]
public class StageDoorAnim
{
    public Vector2Int Dir;
    public AnimationClip DoorAnim;
    public int MaterialIndex;
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


#region Class : Build : Converter

[System.Serializable]
public class ConverterReso
{
    public List<EachConverterReso> ConverterResoList;
}

[System.Serializable]
public class EachConverterReso
{
    public AnimationClip AC;
    public Material Material;
}

#endregion


#region Class : Ally : Prison

[System.Serializable]
public class PrisonAllySprite
{
    public Sprite Bind;
    public Sprite Fall;
    public Sprite Stand;
    public Sprite Salute;
}

#endregion


#region Class : Puzzle : NSC

[System.Serializable]
public class NSCAnswerSpriteSet
{
    public int ShapeIndex;
    public List<Sprite> AllAnswerSet;
}

#endregion


#region Class : CSV : Word

[System.Serializable]
public class WordData
{
    public List<WordElementData> AllWordData;

    public WordData(List<WordElementData> _AllMapNameData)
    {
        AllWordData = _AllMapNameData;
    }

    public string Get_Word(int _ID)
    {
        for (int i = 0; i < AllWordData.Count; i++)
            if (AllWordData[i].ID == _ID)
                return AllWordData[i].Word[GameManager.LanguageID];

        return "";
    }
}

[System.Serializable]
public class WordElementData
{
    public int ID;
    public List<string> Word;

    public WordElementData(int _ID, List<string> _MapName)
    {
        ID = _ID;
        Word = _MapName;
    }
}

#endregion

#region Class : CSV : ModuleInfo

[System.Serializable]
public class ModuleBaseData
{
    public int ID;
    public List<int> ModuleMainChip;

    public ModuleBaseData(int _ID, List<int> _MainChip)
    {
        ID = _ID;
        ModuleMainChip = _MainChip;
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
    public float Dis;

    #endregion

    #region Constructor

    public BulletState_PosAndRot(Vector2 _SpawnPos, Vector2 _Dir, float _SpreadAngle, float _Dis = 0)
    {
        SpawnPos = _SpawnPos;
        Dir = _Dir;
        SpreadAngle = _SpreadAngle;
        Dis = _Dis;
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

public struct BulletState_Effect
{
    #region Value

    public int ExplAmount;
    public float TrailTime;

    #endregion

    #region Constructor

    public BulletState_Effect(int _ExplAmount, float _TrailTime)
    {
        ExplAmount = _ExplAmount;
        TrailTime = _TrailTime;
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


#region Struct : ItemData


public struct ItemData_UIVisual
{
    public Sprite Icon;
    public int Rank;
    public Sprite RankIcon;

    public ItemData_UIVisual(Sprite _Icon, int _Rank)
    {
        Icon = _Icon;
        Rank = _Rank;
        RankIcon = ModuleItemManager.Instance.Get_CorrectRankIcon(Rank);
    }

    public ItemData_UIVisual(ItemData _ItemData)
    {
        Icon = _ItemData.ItemIcon;
        Rank = _ItemData.Rank;
        RankIcon = ModuleItemManager.Instance.Get_CorrectRankIcon(Rank);
    }
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
        float randomAngle = UnityEngine.Random.Range(0, _AngleExtent);
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

    public string Get_InteractName(out bool _CanInteract);
}

#endregion

#region Interface : When (Base)

public interface IWhen
{
    public abstract void Play_When(EnemyController _EC = null);
}

public interface IWhen_Fire : IWhen { }

public interface IWhen_Hit : IWhen { }

public interface IWhen_CriticalHit : IWhen { }

public interface IWhen_GetElectricity : IWhen { }


#endregion

#region Interface : When (Request)

// Request
public interface IWhen_Request
{
    public abstract void Play_When_Request();
}

public interface IWhen_Complete_KillNormalEnemy : IWhen_Request 
{
    public string Get_WhenDesc() { return ResourceManager.Instance.Get_RequestCompleteDesc(0); }
    public void Add_IWhenList() { AllyRequestManager.Instance.Add_RequestComplete("KillNormalEnemy", this); }
    public void Remove_IWhenList() { AllyRequestManager.Instance.Remove_RequestComplete("KillNormalEnemy", this); }
}
public interface IWhen_Complete_KillEliteEnemy : IWhen_Request
{
    public string Get_WhenDesc() { return ResourceManager.Instance.Get_RequestCompleteDesc(1); }
    public void Add_IWhenList() { AllyRequestManager.Instance.Add_RequestComplete("KillEliteEnemy", this); }
    public void Remove_IWhenList() { AllyRequestManager.Instance.Remove_RequestComplete("KillEliteEnemy", this); }
}


// Fail
public interface IWhen_Fail
{
    public abstract void Play_When_Fail();
}

public interface IWhen_Fail_TakingDamage : IWhen_Fail 
{
    public string Get_WhenDesc() { return ResourceManager.Instance.Get_RequestFailDesc(0); }
    public void Add_IWhenList() { AllyRequestManager.Instance.Add_RequestFail("TakingDamage", this); }
    public void Remove_IWhenList() { AllyRequestManager.Instance.Remove_RequestFail("TakingDamage", this); }
}
public interface IWhen_Fail_UsingSkill : IWhen_Fail
{
    public string Get_WhenDesc() { return ResourceManager.Instance.Get_RequestFailDesc(1); }
    public void Add_IWhenList() { AllyRequestManager.Instance.Add_RequestFail("UsingSkill", this); }
    public void Remove_IWhenList() { AllyRequestManager.Instance.Remove_RequestFail("UsingSkill", this); }
}

#endregion

#region Interface : When (Synchrony)

public interface IWhenSync
{
    public abstract void Play_When(EnemyController _Enemy = null, BulletController _Bullet = null);
}

public interface IWhenSync_Start : IWhenSync { }

public interface IWhenSync_Fire : IWhenSync { }
public interface IWhenSync_AfterFire : IWhenSync { }

public interface IWhenSync_Hit : IWhenSync { }
public interface IWhenSync_CriticalHit : IWhenSync { }

public interface IWhenSync_GetFire : IWhenSync { }
public interface IWhenSync_GetCold : IWhenSync { }
public interface IWhenSync_GetElectricity : IWhenSync { }
public interface IWhenSync_GetCorrosion : IWhenSync { }

#endregion

#region Interface : When (Ally Sync)

public interface IWhenAlly
{
    public abstract void Play_When(
        EnemyController _Enemy = null,
        BulletController _Bullet = null,
        DroppingBombController _DroppingBullet = null);
}

public interface IWhenAlly_Start : IWhenAlly { }

public interface IWhenAlly_Fire : IWhenAlly { }
public interface IWhenAlly_AfterFire : IWhenAlly { }

public interface IWhenAlly_Hit : IWhenAlly { }
public interface IWhenAlly_CriticalHit : IWhenAlly { }

public interface IWhenAlly_GetFire : IWhenAlly { }
public interface IWhenAlly_GetCold : IWhenAlly { }
public interface IWhenAlly_GetElectricity : IWhenAlly { }
public interface IWhenAlly_GetCorrosion : IWhenAlly { }
#endregion


#endregion

#region ========== DELEGATE

public delegate void Dele();

public delegate void Dele_T<T>(T _Item);

public delegate void Dele_RefT_T<T>(ref T _Item1, T _Item2);

public delegate void Dele_T_U<T, U>(T _Item1, U _Item2);
public delegate void Dele_RefT_U<T, U>(ref T _Item1, U _Item2);



public delegate void DeleEnemy(EnemyController _Enemy);

#endregion

#region ========== ENUM

#region About Combat

public enum eCombatOwner
{
    Player, Ally, Enemy
}

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
    Normal, Elite, Boss
}

#endregion

#region Room

public enum eRoomType
{
    Completed, KillAll, Survived, BossKill
}

#endregion

#region Puzzle

public enum eNSCPuzzleType
{
    Num, Shape, Color
}

#endregion

#region Out Main Game UI

public enum OutMainGameUIType
{
    BasePanel, OptionPanel
}

#endregion

#region Sound

public enum SoundType
{
    SFX, BGM
}

#endregion

#region Ally

public enum eAllyStateMode
{
    Idle, Move, Attack
}

#endregion

#endregion