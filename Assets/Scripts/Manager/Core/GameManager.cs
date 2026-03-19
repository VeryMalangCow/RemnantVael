using DG.Tweening;
using LeTai.TrueShadow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : PersistentSingleton<GameManager>
{
    #region Value


    [Space(10)]
    [Header("=== Passing Data")]
    [SerializeField] public GameObject designatedPlayerPrefab;

    [Space(10)]
    [Header("=== Intro")]
    [SerializeField] public bool wasWatched = false;

    public static int languageID = 1;
    public static eScreenMode screenMode = eScreenMode.fullScreen;
    public static eResolution resolutionMode = eResolution.w1920h1080;
    public static eFPS fps = eFPS.f144;
    public readonly static string[] kindOfLanguage = new string[] { "Eng", "Kor" };

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        
    }

    #endregion

    #region Option

    public void Set_BaseOption()
    {
        OptionJsonData savedData = SaveDataManager.instance.jsonData.optionData;

        ResourceManager.instance.Set_LanguageFont(savedData.languageID);

        Set_Screen(savedData.resolutionMode, savedData.screenMode);
        Set_FPS(savedData.fps);
        SoundManager.instance.Set_BgmVolume(savedData.bgmVolume);
        SoundManager.instance.Set_SfxVolume(savedData.sfxVolume);
    }

    public void Set_Screen(eResolution resolutionMode, eScreenMode screenMode)
    {
        GameManager.resolutionMode = resolutionMode;
        SaveDataManager.instance.jsonData.optionData.resolutionMode = resolutionMode;
        GameManager.screenMode = screenMode;
        SaveDataManager.instance.jsonData.optionData.screenMode = screenMode;

        string[] reso = resolutionMode.ToString().Split("h");
        reso[0] = reso[0].Replace("w", "");
        FullScreenMode mode = FullScreenMode.MaximizedWindow;
        if (screenMode == eScreenMode.fullScreen) mode = FullScreenMode.FullScreenWindow;
        else if (screenMode == eScreenMode.window) mode = FullScreenMode.Windowed;
        else mode = FullScreenMode.MaximizedWindow;

        Screen.SetResolution(Convert.ToInt32(reso[0]), Convert.ToInt32(reso[1]), mode);
    }

    public void Set_FPS(eFPS mode)
    {
        GameManager.fps = mode;
        SaveDataManager.instance.jsonData.optionData.fps = mode;

        int fps = Convert.ToInt32(GameManager.fps.ToString().Replace("f", "")); 
        Application.targetFrameRate = fps;
    }

    #endregion
}

#region ========== DEV TOOL

public class DevTool
{
    #region About Math

    #region Get

    // 퍼센트값을 도출
    public static float Get_Percent(float percent, float value)
    {
        return (percent / 100f) * value;
    }

    // X 피벗을 개수와 간격 수치로 계산 (float 반환 값을 모든 값에 빼주면 됨)
    public static float Get_MinusXPivot(float intervalX, int maxAmount)
    {
        return (intervalX / 2) * (maxAmount - 1);
    }

    // float 랜덤값을 0을 기준으로 돌리기 (음수 양수의 범위)
    public static float Get_RandomValueBaseZero(float randomExtent)
    {
        if (randomExtent == 0)
        {
            return 0;
        }
        else
        {
            return UnityEngine.Random.Range(-randomExtent * 0.5f, randomExtent * 0.5f);
        }
    }

    // 확률에 따른 등급
    public static int Get_Grade(List<int> rankPercents)
    {
        float currentSum = 0f;
        float randomValue = UnityEngine.Random.Range(0f, Get_SumFloat(rankPercents));

        for (int i = 0; i < rankPercents.Count; i++)
        {
            currentSum += rankPercents[i];
            if (currentSum > randomValue)
            {
                return i;
            }
        }

        return 0;
    }

    // 랭크
    public static int Get_Rank(List<int> rankPercents)
    {
        return Get_Grade(rankPercents) + 1;
    }

    // Round
    public static string Get_RoundFloatString(float value, int roundRange = 3)
    {
        return value >= 0 ? $"+{System.Math.Round(value, roundRange)}" : $"{System.Math.Round(value, roundRange)}";
    }


    #endregion

    #region Is

    // 확률이 성공했는지를 반환
    public static bool Is_ChanceSuccess(float chance)
    {
        if (UnityEngine.Random.Range(0f, 1f) < chance)
            return true;
        else
            return false;
        
    }

    // 일정 오차를 인정한다
    public static bool Is_InRange(float value, float criterion, float range)
    {
        return criterion - range <= value && value <= criterion + range ? true : false; 
    }

    #endregion

    #region Add

    // 실제 주소값 float에 추가
    public static void Add_RefValue(ref float variable, float addValue)
    {
        variable += addValue;
    }

    #endregion

    #endregion

    #region About String

    public static string Get_LengthString(int value, int targetLength)
    {
        return value.ToString().PadLeft(targetLength, '0');
    }

    public static string Get_MemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression)memberExpression.Body).Member.Name;

    #endregion

    #region About Casting

    #region Is

    // 'T 타입'이 Null이거나 Defualt가 아닌지?
    public static bool Is_Usable<T>(T value)
    {
        if (value is not null && !EqualityComparer<T>.Default.Equals(value, default))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 'T, U 타입' 두 값이 같은지 판별
    public static bool Is_Equal<T, U>(T first, U second)
    {
        return first?.Equals(second) ?? second is null;
    }

    // 'T, U 타입' 사용할 수 있는지 판별, 그 안의 2값이 같은지 판별
    public static bool Is_UsableAndEqual<T, U>(T value, U inValue1, U inValue2)
    {
        return Is_Equal(inValue1, inValue2) && Is_Usable(value);
    }

    #endregion

    #region Get

    // 객체를 원하는 'T 타입'으로 캐스팅
    public static T Get_CastingTType<T>(object obj)
    {
        if (obj != null && obj is T objType)
        {
            return objType;
        }
        Debug.Log("Default");
        return default;
    }
    public static bool Can_CastingTType<T>(object obj, out T tType) where T : class
    {
        if (obj != null && obj is T objType)
        {
            tType = objType;
            return true;
        }

        tType = null;
        return false;
    }

    #endregion

    #endregion

    #region About Component

    #region Set

    // 객체에 'T 타입'이 있다면 변수에 할당
    public static void Set_ComponentTType<T>(ref T variable, GameObject targetGO) where T : Component
    {
        if (variable == null && targetGO.TryGetComponent(out T tTypeComponent))
        {
            variable = tTypeComponent;
        }
    }

    // SpriteRenderer Value
    public static void Set_ComponentValue(SpriteRenderer sr, Sprite sprite, Material material, int sortingOrder)
    {
        sr.sprite = sprite;
        sr.material = material;
        sr.sortingOrder = sortingOrder;
    }


    // 구조체 값에서 값을 넣기
    public static void Set_TF_FromStruct(Transform tf, State_TF2D structTF2D)
    {
        tf.position = structTF2D.Pos;
        tf.rotation = structTF2D.Rot;
        tf.localScale = structTF2D.LocalScale;
    }

    public static void Set_MatAndClr_FromStruct(SpriteRenderer sr, State_Sprite spriteExtra)
    {
        sr.material = spriteExtra.Mat;
        sr.color = spriteExtra.Clr;
    }

    #endregion

    #region Get

    public static T Get_ComponentTType<T>(GameObject targetGO)
    {
        if (targetGO != null && targetGO.TryGetComponent(out T tTypeComponent))
        {
            return tTypeComponent;
        }
        return default;
    }

    public static bool Get_ComponentTType<T>(GameObject targetGO, out T tType)
    {
        if (targetGO != null && targetGO.TryGetComponent(out T tTypeComponent))
        {
            tType = tTypeComponent;
            return true;
        }
        tType = default;
        return false;
    }

    #endregion

    #region Gen 

    // 게임 오브젝트 만들고, 컴포넌트 추가하기
    public static T Gen_Component<T>(Transform parentTF, string _Name) where T : Component
    {
        GameObject go = new GameObject(_Name);
        go.transform.SetParent(parentTF);
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
    public static HashSet<Vector2Int> Get_RoundVec(HashSet<Vector2Int> targetVec)
    {
        HashSet<Vector2Int> targetRoomVecRound = new HashSet<Vector2Int>();

        // 모든 타겟 좌표의 주변 좌표 추가
        foreach (var vec in targetVec)
        {
            List<Vector2Int> eachRoung = Get_RoundVec(vec);
            for (int i = 0; i < eachRoung.Count; i++)
                targetRoomVecRound.Add(eachRoung[i]);
        }

        // 원래 _TargetVec에 포함된 좌표 제거
        targetRoomVecRound.ExceptWith(targetVec);

        return targetRoomVecRound; // HashSet을 List로 변환 후 반환
    }

    #endregion

    #region About List

    #region Add

    // 'T 타입' 리스트에 '새로' 추가
    public static bool Add_InList<T>(List<T> targetList, T targetValue)
    {
        if (!targetList.Contains(targetValue))
        {
            targetList.Add(targetValue);
            return true;
        }
        return false;
    }

    #endregion

    #region Remove

    // 'T 타입' 삭제 시도
    public static bool Remove_InList<T>(List<T> targetList, T targetValue)
    {
        if (targetList.Contains(targetValue))
        {
            targetList.Remove(targetValue);
            return true;
        }
        return false;
    }


    // 'T 타입' 중복 제거
    public static List<T> Remove_DuplicateInList<T>(List<T> targetList)
    {
        return targetList.Distinct().ToList();
    }

    #endregion

    #region Get 

    #region float

    public static float Get_SumFloat(List<int> intList)
    {
        float result = 0;
        for (int i = 0; i < intList.Count; i++)
        {
            result += intList[i];
        }
        return result;
    }

    #endregion

    #region Removed

    public static List<T> Get_RemovedList<T>(List<T> targetList, int index)
    {
        List<T> result = new List<T>(targetList);
        result.Remove(result[index]);
        return result;
    }
    public static List<T> Get_RemoveLastOneList<T>(List<T> targetList)
    {
        List<T> result = new List<T>(targetList);
        result.Remove(result[result.Count - 1]);
        return result;
    }

    #endregion

    #region Vector2Int

    // 해당 백터의 주변을 구하기
    public static List<Vector2Int> Get_RoundVec(Vector2Int centerVec)
    {
        return new List<Vector2Int>()
        {
            (centerVec + Vector2Int.up),
            (centerVec + Vector2Int.down),
            (centerVec + Vector2Int.left),
            (centerVec + Vector2Int.right)
        };
    }

    // 주변 좌표값을 가져오기
    public static List<Vector2Int> Get_RoundVec(List<Vector2Int> targetVec)
    {
        HashSet<Vector2Int> targetRoomVecRound = new HashSet<Vector2Int>();

        // 모든 타겟 좌표의 주변 좌표 추가
        foreach (var vec in targetVec)
        {
            List<Vector2Int> eachRoung = Get_RoundVec(vec);
            for (int i = 0; i < eachRoung.Count; i++)
                targetRoomVecRound.Add(eachRoung[i]);
        }
        
        // 원래 _TargetVec에 포함된 좌표 제거
        targetRoomVecRound.ExceptWith(targetVec);

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

    public static T Get_RandomInList<T>(List<T> targetList)
    {
        if (targetList == null || targetList.Count <= 0)
            return default;
        else
            return targetList[UnityEngine.Random.Range(0, targetList.Count)];
    }

    public static T Get_RandomInList<T>(T[] targetArr)
    {
        if (targetArr == null || targetArr.Length <= 0)
            return default;
        else
            return targetArr[UnityEngine.Random.Range(0, targetArr.Length)];
    }

    #endregion

    #region Child

    // 자식 객체들의 'T 타입' 리스트 가져오기
    public static List<T> Get_ChildList<T>(Transform parent) where T : Component
    {
        List<T> result = new List<T>();
        foreach (Transform TF in parent)
        {
            if (TF.TryGetComponent(out T type))
            {
                result.Add(type);
            }
        }
        return result;
    }

    public static List<T> Get_AllChildList<T>(Transform parent) where T : Component
    {
        List<T> result = new List<T>();
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>();
        foreach (Transform TF in allChildren)
        {
            if (TF == parent) continue;

            if (TF.TryGetComponent(out T type))
            {
                result.Add(type);
            }
        }

        return result;
    }

    public static List<T> Get_ChildList_OnlyOnceUnder<T>(Transform parent) where T : Component
    {
        List<T> result = new List<T>();
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).gameObject.TryGetComponent(out T type))
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
    public static List<T> Get_List<T>(List<List<T>> doubleList) where T : class
    {
        List<T> result = new List<T>();
        for (int i = 0; i < doubleList.Count; i++)
        {
            result.AddRange(doubleList[i]);
        }
        return result;
    }


    // 'T 타입' 이중 리스트에서 리스트들중 마지막 부분만을 모아서 리턴
    public static List<T> Get_LastElementList<T>(List<List<T>> doubleList)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < doubleList.Count; i++)
        {
            result.Add(doubleList[i][doubleList[i].Count - 1]);
        }
        return result;
    }

    #endregion

    #region Casting + ByComponent

    // 게임 오브젝트 리스트에서 List T 타입 변형
    public static List<T> Get_ComponentTTypeList<T>(List<GameObject> targetList) where T : Component
    {
        List<T> resultList = new List<T>();
        for (int i = 0; i < targetList.Count; i++)
        {
            if (Get_ComponentTType(targetList[i], out T tType))
            {
                resultList.Add(tType);
            }
        }
        return resultList;
    }

    // 'T 타입' 리스트를 GO 리스트로 변경
    public static List<GameObject> Get_GOList<T>(List<T> targetList) where T : MonoBehaviour
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < targetList.Count; i++)
        {
            result.Add(targetList[i].gameObject);
        }
        return result;
    }

    // 강제 Parse
    public static List<int> Get_ParseIntList<T>(List<T> targetList)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < targetList.Count; i++)
        {
            result.Add(int.Parse(targetList[i].ToString()));
        }
        return result;
    }
    public static List<float> Get_ParseFloatList<T>(List<T> targetList)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < targetList.Count; i++)
        {
            result.Add(float.Parse(targetList[i].ToString()));
        }
        return result;
    }

    #endregion

    #region Row & Colume

    public static List<List<T>> Get_RowColumeList<T>(List<T> list, int row)
    {
        int totalCount = list.Count;
        int totalRows = (totalCount + row - 1) / row; // 올림 나눗셈

        List<List<T>> result = new List<List<T>>();

        for (int i = 0; i < totalRows; i++)
        {
            List<T> rowList = new List<T>();

            for (int j = 0; j < row; j++)
            {
                int index = i * row + j;
                if (index >= totalCount)
                    break;

                rowList.Add(list[index]);
            }

            result.Add(rowList);
        }

        return result;
    }

    #endregion

    #region Unique

    // 'T 타입' List 두개를 합
    public static List<T> Get_CombineList<T>(List<T> firstList, List<T> secondList)
    {
        List<T> resultList = new List<T>(firstList);
        resultList.AddRange(secondList);
        return resultList;
    }

    // 'T 타입'의 List를 무작위 섞기
    public static List<T> Get_ShuffledList<T>(List<T> targetList)
    {
        List<T> result = new List<T>(targetList);

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
    public static T Get_Random<T>(List<T> targetList)
    {
        if (targetList != null || targetList.Count > 0)
        {
            return targetList[UnityEngine.Random.Range(0, targetList.Count)];
        }
        return default;
    }


    // 'T 타입' 맞는 인덱스 찾기
    public static int Get_IndexInList<T>(List<T> targetList, T target) where T : class
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] == target)
            {
                return i;
            }
        }
        return -1;
    }

    // 'T 타입' 두 리스트 중 교집합 가져오기
    public static List<T> Get_IntersectionList<T>(List<T> list1, List<T> list2)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < list1.Count; i++)
        {
            if (list2.Contains(list1[i]))
            {
                result.Add(list1[i]);
            }
        }
        return result;
    }

    #endregion

    #endregion

    #region Set

    // 'T 타입' 리스트의 두 값을 교체
    public static void Set_Swap<T>(List<T> targetList, int index1, int index2)
    {
        T temp = targetList[index1];
        targetList[index1] = targetList[index2];
        targetList[index2] = temp;
    }

    // 'T 타입' 리스트의 게임 오브젝트 모두 끄고 키기
    public static void Set_Active<T>(List<T> targetList, bool active) where T : Component
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].gameObject.SetActive(active);
        }
    }

    // 'T 타입' 리스트를 모두 Null로 초기화
    public static void Set_Null<T>(List<T> targetList) where T : class
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i] = null;
        }
    }


    // 'T 타입' 리스트를 돌면서 실행
    public static void Set_ListDele<T>(List<T> targetList, Dele_T<T> dele)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            dele(targetList[i]);
        }
    }

    // 'T, U 타입' 리스트를 돌면서 실행
    public static void Set_ListDele<T, U>(List<T> targetList, Dele_RefT_U<U, T> dele, ref U variable,
        int startIndex = 0)
    {
        for (int i = startIndex; i < targetList.Count; i++)
        {
            dele(ref variable, targetList[i]);
        }
    }

    public static void Set_ListDele<T, U>(List<T> targetList, Dele_T_U<T, U> dele, U value)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            dele(targetList[i], value);
        }
    }

    // Comp
    public static void Set_SpriteList(List<Image> targetList, Sprite sprite)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].sprite = sprite;
        }
    }
    public static void Set_SpriteNativeSize(List<Image> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].SetNativeSize();
        }
    }

    #endregion

    #endregion

    #region About Dictionary


    public static void Add_AmountForDict<T>(ref Dictionary<T, int> dict, T id, int amount)
    {
        if (dict.ContainsKey(id))
            dict[id] += amount; 
        else
            dict.Add(id, amount); 
    }

    #endregion

    #region About Vector

    #region Get

    public static float Get_Dis(GameObject go1, GameObject go2)
    {
        return Vector2.Distance(go1.transform.position, go2.transform.position);
    }

    public static Vector2 Get_RandomDir()
    {
        float _X = UnityEngine.Random.Range(-1.0f, 1.0f);
        float _Y = UnityEngine.Random.Range(-1.0f, 1.0f);
        return new Vector2(_X, _Y).normalized;
    }

    public static Vector2 Get_Dir(GameObject fromGO, GameObject toGO)
    {
        return Get_Dir(fromGO.transform.position, toGO.transform.position);
    }

    public static Vector2 Get_Dir(GameObject fromGO, Vector2 toPos)
    {
        return Get_Dir(fromGO.transform.position, toPos);
    }

    public static Vector2 Get_Dir(Vector2 fromPos, Vector2 toPos)
    {
        return (toPos - fromPos).normalized;
    }


    // 방향에 의한 Img, Anim 변환 // SolarSystem에서 사용
    // 위 사항에 사용될 Index 값
    public static int Get_Index(float eulerAngleY)
    {
        return (int)((eulerAngleY + 67.5f) % 360 * 0.0222222f);
    }

    // 위 함수의 인트값을 다시 벡터로 가져오기
    public static Vector2Int Get_NormalizedVec(int index)
    {
        Vector2Int[] directions = {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0),
            new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0)};

        return (index >= 0 && index < directions.Length) ? directions[index] : Vector2Int.zero;
    }

    // 플레이어의 사격을 위해 너무 가까우면 X로 발사되는 것을 방지하기 위한 값 계산
    private static float fireMinDisLimit = 4;
    public static Vector2 Get_MinFireDir(Vector2 spawnPos)
    {
        Vector2 targetPos = InputManager.instance.mousePosByWorld;
        if (fireMinDisLimit > Vector3.Magnitude(InputManager.instance.dirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.instance.playerController.transform.position +
                InputManager.instance.dirFromPlayerPos.normalized * fireMinDisLimit;
        }

        return (targetPos - spawnPos).normalized;
    }


    // 최소 거리의 객체 가져오기
    public static GameObject Get_ClosetGO(List<GameObject> targetList, GameObject centerGO)
    {
        return targetList.Count == 0 ?
            null :
            targetList.OrderBy(go => Vector3.Distance(centerGO.transform.position, go.transform.position)).First();
    }

    // 최대 거리의 객체 가져오기
    public static GameObject Get_FurthestGO(List<GameObject> targetList, GameObject centerGO)
    {
        return targetList.Count == 0 ? 
            null : 
            targetList.OrderBy(go => Vector3.Distance(centerGO.transform.position, go.transform.position)).Last();
    }

    // 범위 내 객체들 가져오기 (가까운 순서대로)
    public static List<GameObject> Get_CloserGOList(List<GameObject> targetList, GameObject centerGO, float maxDis)
    {
        return Get_RangeGOList(targetList, centerGO, 0, maxDis, orderByShortDis: true);
    }

    // 범위 밖 객체들 가져오기 (먼 순서대로)
    public static List<GameObject> Get_FurtherGOList(List<GameObject> targetList, GameObject centerGO, float minDis)
    {
        return Get_RangeGOList(targetList, centerGO, minDis, 0, orderByShortDis: false);
    }

    // 범위 조건 객체들 가져오기
    public static List<GameObject> Get_RangeGOList(List<GameObject> targetList, GameObject centerGO, float minDis, float maxDis, bool orderByShortDis)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < targetList.Count; i++)
        {
            if (minDis <= Vector2.Distance(targetList[i].transform.position, centerGO.transform.position) &&
                maxDis >= Vector2.Distance(targetList[i].transform.position, centerGO.transform.position))
            {
                result.Add(targetList[i]);
            }
        }

        return orderByShortDis ?
            result.OrderBy(obj => Vector2.Distance(obj.transform.position, centerGO.transform.position)).ToList() :
            result.OrderByDescending(obj => Vector2.Distance(obj.transform.position, centerGO.transform.position)).ToList();
    }

    #endregion

    #region Add

    // 실제 주소값 Vector에 추가
    public static void Add_RefValue(ref Vector2 variable, Vector2 addValue)
    {
        variable += addValue;
    }

    #endregion


    #endregion

    #region About Quaternion

    #region Get

    // 좌표값 (Vector2:Dir)
    // => 회전값 (Quaternion:Rot)
    public static Quaternion Get_RotFromDir(Vector2 dir)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));
    }
    
    public static Quaternion Get_RotFromDir_Solar(Vector2 dir)
    {
        return Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, dir), 0f);
    }

    // 각값 (float:Angle)
    // => 좌표값 (Vector2:Dir) : transform.eulerAngles.z값을 인자로 받는 것이 보편적으로 좋음
    public static Vector2 Get_DirFromAngle(float angle)
    {
        return new Vector2(
                    Mathf.Cos((angle + 90) * Mathf.Deg2Rad),
                    Mathf.Sin((angle + 90) * Mathf.Deg2Rad)).normalized;
    }
    // 좌표값 (Vecto2:Dir)
    // => 각값(float:Angle)
    public static float Get_AngleFromDir(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
    }


    // 반대 방향의 회전값 구하기
    public static Quaternion Get_FlipRotation(Quaternion rotation)
    {
        Vector3 currentRotation = rotation.eulerAngles;
        currentRotation.z += 180;

        Quaternion q = Quaternion.identity;
        q.eulerAngles = currentRotation;

        return q;
    }

    #endregion

    #region Add

    // 회전값에 값을 더하기
    // TF값, 월드기준
    public static void Add_RotZValue(Transform tf, float z)
    {
        Vector3 currentRotation = tf.eulerAngles;

        currentRotation.z += z;
        tf.eulerAngles = currentRotation;
    }

    // TF값, 로컬기준
    public static void Add_LocalRotZValue(Transform tf, float z)
    {
        Vector3 currentRotation = tf.localEulerAngles;

        currentRotation.z += z;
        tf.localEulerAngles = currentRotation;
    }

    // Quat값, 월드 기준
    public static Quaternion Add_RotZValue(Quaternion rotation, float z)
    {
        Vector3 currentRotation = rotation.eulerAngles;
        currentRotation.z += z;

        Quaternion q = Quaternion.identity;
        q.eulerAngles = currentRotation;

        return q;
    }

    #endregion

    #endregion

    #region About Anim

    #region Set

    // 애니메이션을 코드상으로 변경하는 시스템
    public static void Set_Anim(ref AnimatorOverrideController aoc, Animator at, AnimationClip ac)
    {
        aoc = new AnimatorOverrideController(at.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, ac));
        aoc.ApplyOverrides(anims);
        at.runtimeAnimatorController = aoc;
    }

    // 애니메이션의 속도와 크기 조절
    public static void Set_AnimSpeedAndSize(Animator at, float animSpeed = 1f, float animSize = 1f)
    {
        Set_AnimSpeed(at, animSpeed);
        Set_AnimSize(at, animSize);
    }

    // 애니메이션의 속도 조절
    public static void Set_AnimSpeed(Animator at, float animSpeed)
    {
        at.speed = animSpeed;
    }

    // 애니메이션의 크기 조절
    public static void Set_AnimSize(Animator at, float animSize)
    {
        at.transform.localScale = Vector2.one * animSize;
    }

    // 방향성 Anim 컨트롤러의 애니메이터들의 속도 조절
    public static void Set_AnimSpeed(List<DirectionalAnimController> targetList, float speed)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            DevTool.Set_AnimSpeedAndSize(targetList[i].ThisComp, speed, animSize: 1);
        }
    }

    // 지정 애니메이션 클립에서 마지막 프레임 스프라이트 반환
    public static Sprite Get_LastFrameSprite(AnimationClip ac, SpriteRenderer sr)
    {
        if (ac == null || sr == null)
            return null;

        // 마지막 프레임 시간 계산
        float epsilon = 1f / Mathf.Max(ac.frameRate, 30f) * 0.5f;
        float sampleTime = Mathf.Max(0f, ac.length - epsilon);

        // 현재 sprite 기억
        Sprite original = sr.sprite;

        // 샘플링해서 마지막 프레임 적용
        ac.SampleAnimation(sr.gameObject, sampleTime);
        Sprite last = sr.sprite;

        // 원래 sprite로 복구 (부작용 방지)
        sr.sprite = original;

        return last;
    }

    #endregion

    #region Is
    // 애니메이션이 끝났는지 판별
    public static bool Is_AnimIsDone(Animator at)
    {
        // 현재 애니메이터 상태 정보 가져오기가 1이상(1번이상 진행?)
        // 애니메이션이 종료되었는지 판별
        if (at.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && !at.IsInTransition(0))
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
    public static Tween Play_Tween(Tween tween, Dele start = null, Dele update = null, Dele complete = null)
    {
        tween
            .OnStart(() => { if (start != null) start(); })
            .OnUpdate(() => { if (update != null) update(); })
            .OnComplete(() => { if (complete != null) complete(); });

        return tween;
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

    public readonly static int skillAmount = 2;
    public readonly static int buMaxLevel = 10;
    public readonly static int buLevelInterval = 3;

    #region Index from DmgType, Critical

    // 데미지와 크리티컬로 인덱스 구하기
    // 0: PB / 1: PC / 2: EB / 3: EC
    public static int Get_IndexOfDmgTypeAndCritical(eDamageType dmgType, bool isCritical)
    {
        if (dmgType == eDamageType.Physics)
        {
            if (!isCritical)
            { return 0; }
            else
            { return 1; }
        }
        else
        {
            if (!isCritical)
            { return 2; }
            else
            { return 3; }
        }
    }

    // 1:P / 2:E
    public static eDamageType Get_DmgTypeFromIndex(int index)
    {
        if (index == 0 || index == 1)
        {
            return eDamageType.Physics;
        }
        else
        {
            return eDamageType.Energy;
        }
    }

    // 0,2: B / 1,3: C
    public static bool Get_CriticalFromIndex(int index)
    {
        if (index == 0 || index == 2)
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

    public static Vector2 Get_DirForPlayer<T>(T tType) where T : MonoBehaviour
    {
        return Get_Dir(tType.gameObject, PlayerManager.instance.playerController.gameObject);
    }

    public static float Get_DisForPlayer<T>(T tType) where T : MonoBehaviour
    {
        return Get_Dis(tType.gameObject, PlayerManager.instance.playerController.gameObject);
    }

    #endregion

    #region Base Upgrade

    // 인풋의 EUI값으로 부터 맞는 BUState을 반환
    public static BUState<T> Get_ThisData<T>(List<BUShopData<T>> shopDataList, TxtAmountForBuyEUIController inMTAFB)
    {
        for (int i = 0; i < shopDataList.Count; i++)
        {
            if (shopDataList[i].upgradeEUI == inMTAFB)
            {
                return shopDataList[i].state;
            }
        }
        return null;
    }

    #endregion

    #endregion

    #region About Buff

    // 냉기 데미지 갑소 계산 (효과)
    public static float Get_DmgEffectByCold(float baseDmg, EnemyBuffController enemyBuff)
    {
        return baseDmg * (1f - 
            (enemyBuff.ColdStack.currentStack * (enemyBuff.AbsoluteZeroStack.currentStack + 1) * 0.01f));
    }

    // 부식 데미지 증가 계산 (효과)
    public static float Get_DmgEffectByCorrosion(float baseDmg, EnemyBuffController enemyBuff)
    {
        return baseDmg *= (1f + 
            (enemyBuff.CorrosionStack.currentStack * (enemyBuff.DecayStack.currentStack + 1) * 0.01f));
    }


    // 화염
    public static float Get_FrameDmg(EnemyBuffController buff)
    {
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 0.01f
            * buff.FlameStack.currentStack
            * (buff.InfernoStack.currentStack + 1);
    }
    public static float Get_FlameExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Physics;
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 10f;
    }


    // 냉기
    public static float Get_ColdExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Energy;
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 7.5f;
    }


    // 전기
    public static float Get_ElectricityDmg(EnemyBuffController buff)
    {
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 0.005f
            * buff.ElectricityStack.currentStack
            * (buff.PlasmaStack.currentStack + 1);
    }
    public static float Get_ElectricityExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Energy;
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 7.5f;
    }

    // 부식
    public static float Get_CorrosionExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Physics;
        return PlayerManager.instance.playerController.BaseWeapon.BaseDamage.buffedState
            * 5f;
    }

    #endregion

    #region About Collider
    public static bool Can_Collding<T>(Collider2D col, string tag, HashSet<StaticDepthController> alreadyList, out T tType) where T : StaticDepthController
    {
        tType = null;

        return col.tag == tag &&
            col.transform.parent.TryGetComponent(out tType) &&
            !alreadyList.Contains(tType);
    }

    public static bool Can_Collding<T>(Collider2D col, string tag, out T tType) where T : StaticDepthController
    {
        tType = null;

        return col.tag == tag &&
            col.transform.parent.TryGetComponent(out tType);
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
    public static bool Is_Exist_UseLine<T>(T start, T end, string layerName) where T : MonoBehaviour
    {
        return Physics2D.Linecast(start.transform.position, end.transform.position, LayerMask.GetMask(layerName)).collider != null;
    }

    public static bool Is_Exist_UseLine(Transform start, Transform end, string layerName)
    {
        return Physics2D.Linecast(start.position, end.position, LayerMask.GetMask(layerName)).collider != null;
    }

    public static bool Is_Exist_UseLine(Vector2 start, Vector2 end, string layerName)
    {
        return Physics2D.Linecast(start, end, LayerMask.GetMask(layerName)).collider != null;
    }


    public static bool Is_Exist_UseCircle(Transform startTF, Transform endTF, string layerName, float radius)
    {
        return Physics2D.CircleCast(startTF.position, radius, (endTF.position - startTF.position).normalized,
            Vector2.Distance(startTF.position, endTF.position), LayerMask.GetMask(layerName)).collider != null;
    }
    public static bool IsOnNavMesh(Vector2 point, float maxDis = 0.1f, int areaMask = NavMesh.AllAreas, bool planeXY = true)
    {
        // Vector2 → Vector3 변환
        Vector3 pos3 = planeXY
            ? new Vector3(point.x, point.y, 0f) // XY 평면
            : new Vector3(point.x, 0f, point.y); // XZ 평면

        return NavMesh.SamplePosition(pos3, out _, maxDis, areaMask);
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

    public static void Set_Color<T>(Color clr, List<T> targetList) where T : Component
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            switch (targetList[i]) 
            {
                case TMP_Text tmp:
                    Set_Color(clr, tmp);
                    break;

                case Image img:
                    Set_Color(clr, img);
                    break;


                default:
                    break;
            }
        }
    }

    public static void Set_Color(Color clr, TMP_Text comp)
    {
        if (comp == null) return;
        comp.color = new Color(clr.r, clr.g, clr.b, comp.color.a);
    }

    public static void Set_Color(Color clr, Image comp)
    {
        if (comp == null) return;
        comp.color = new Color(clr.r, clr.g, clr.b, comp.color.a);
    }

    public static void Set_AlphaColor(Light2D light, float a)
    {
        light.color = Get_AlphaColor(light, a);
    }

    public static void Set_AlphaColor(Image img, float a)
    {
        img.color = Get_AlphaColor(img, a);
    }

    public static void Set_AlphaColor(TMP_Text txt, float a)
    {
        txt.color = Get_AlphaColor(txt, a);
    }


    public static Color Get_AlphaColor(Light2D light, float a)
    {
        Color clr = light.color;
        clr.a = a;
        return clr;
    }

    public static Color Get_AlphaColor(Image img, float a)
    {
        Color clr = img.color;
        clr.a = a;
        return clr;
    }

    public static Color Get_AlphaColor(TMP_Text txt, float a)
    {
        Color clr = txt.color;
        clr.a = a;
        return clr;
    }


    public static void Set_TxtList(List<TMP_Text> txt, string s)
    {
        for (int i = 0; i< txt.Count; i++)
        {
            txt[i].text = s;
        }
    }

    #endregion

    #region Image

    // 커지고 작아지는 효과
    public static Tween Play_ScalePulse(RectTransform rt, float bigScale, float originalScale = 1f, float durTime = 0.4f)
    {
        return rt.DOScale(bigScale, durTime * 0.5f)
            .OnComplete(() =>
            {
                rt.DOScale(originalScale, durTime * 0.5f);
            });
    }

    // 투명도 효과
    public static Tween Play_FadePulse(Image img, float highAplha = 1f, float originalAlpha = 0.25f, float durTime = 0.4f)
    {
        return img.DOFade(highAplha, durTime * 0.5f)
           .OnComplete(() =>
           {
               img.DOFade(originalAlpha, durTime * 0.5f);
           });
    }

    #endregion

    #region Interact

    public static string Get_InteractingAnnoTxt(IInteract ii, out bool canInteract)
    {
        canInteract = true;
        if (ii == null)
            return "";

        string result = ii.Get_InteractName(out bool out_canInteract);
        canInteract = out_canInteract;

        return result;
    }

    #endregion

    #region Dur

    public static void Set_Dur(int _durAmount, List<Image> imgList, TMP_Text txt)
    {
        txt.text = _durAmount.ToString();
        for (int i = 0; i < imgList.Count; i++)
        {
            if (_durAmount > i) // On
            {
                imgList[i].color = Color.white;
            }
            else // Off
            {
                imgList[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    #endregion

    #endregion

    #region About Interface

    public static void Play_AllIWhen<T>(List<T> iWhenList) where T : IWhen
    {
        if (iWhenList.Count <= 0) return;

        for (int i = 0; i < iWhenList.Count; i++)
            iWhenList[i].Play_When();
    }

    public static T Get_SyncValue<T>(List<T> tList, int syncRank)
    {
        return tList[syncRank - 1];
    }

    #endregion

    #region About TrueShadow

    public static int Get_TSChildIndex<T>(T t, int index) where T : MonoBehaviour
    {
        return Get_TSChildIndex(t.gameObject, index);
    }

    public static int Get_TSChildIndex(GameObject go, int index)
    {
        return go.transform.GetChild(index).name != $"{go.name}'s Shadow" ?
            index : index + 1;
    }

    public static int Get_TSChildIndex(Transform tf, int index)
    {
        return tf.GetChild(index).name != $"{tf.gameObject.name}'s Shadow" ?
            index : index + 1;
    }

    public static int Get_TSChildIndex(Component comp, int index)
    {
        return comp.gameObject.transform.GetChild(index).name != $"{comp.gameObject.name}'s Shadow" ?
            index : index + 1;
    }

    #endregion
}

#endregion

#region ========== CLASS

#region Class : Title UI

[System.Serializable]
public class TitleElement
{
    public RectTransform movingRT;

    public float movingPowerX;
    public float movingPowerY;
}

[System.Serializable]
public class TitleTSTFElement
{
    public Transform thisTSParentTF;

    public float min;
    public float max;

    public float durTime;

    public List<TrueShadow> Get_TargetTSList()
    {
        return DevTool.Get_ChildList<TrueShadow>(thisTSParentTF);
    }
}

[System.Serializable]
public class TitleTSElement
{
    public List<TrueShadow> thisTSList;

    public float min;
    public float max;

    public float durTime;
}

#endregion

#region Class : PublicData

[System.Serializable]
public class RefData<T>
{
    public T value;

    public RefData(T t)
    {
        value = t;
    }

}



[System.Serializable]
public class TrioData<T>
{
    [SerializeField] public T typeA;
    [SerializeField] public T typeSpecial;
    [SerializeField] public T typeB;
}

[System.Serializable]
public class CoupleData<T>
{
    [SerializeField] public T typeBase;
    [SerializeField] public T typeSpecial;

    public CoupleData(CoupleData<T> data)
    {
        typeBase = data.typeBase;
        typeSpecial = data.typeSpecial;
    }

    public CoupleData(T _base, T special)
    {
        typeBase = _base;
        typeSpecial = special;
    }
    public T Get_Base(bool isBase)
    {
        if (isBase)
        {
            return typeBase;
        }
        else
        {
            return typeSpecial;
        }
    }

    public T Get_Special(bool isSpecial)
    {
        if (isSpecial)
        {
            return typeSpecial;
        }
        else
        {
            return typeBase;
        }
    }
}

[System.Serializable]
public class CouplePair<T>
{
    [SerializeField] public CoupleData<T> typeBase;
    [SerializeField] public CoupleData<T> typeSpecial;

    public CouplePair(CoupleData<T> baseData, CoupleData<T> specialData)
    {
        typeBase = baseData;
        typeSpecial = specialData;
    }

    public CoupleData<T> Get_Base(bool isBase)
    {
        if (isBase)
        {
            return typeBase;
        }
        else
        {
            return typeSpecial;
        }
    }

    public CoupleData<T> Get_Special(bool isSpecial)
    {
        if (isSpecial)
        {
            return typeSpecial;
        }
        else
        {
            return typeBase;
        }
    }
}


[System.Serializable]
public class CooltimeData
{
    [SerializeField] public float max;
    [SerializeField] public float current;

    public CooltimeData()
    {
        max = 0f;
        current = 0f;
    }

    public CooltimeData(float max, float current = 0f)
    {
        this.max = max;
        this.current = current;
    }
}

[System.Serializable]
public class ChargeCooltimeData : CooltimeData
{
    public ChargeCooltimeData() : base() { }

    public ChargeCooltimeData(float max, float current = 0f) : base(max, current) { }

    public bool Is_Charge(float deltaTime)
    {
        if (current >= max)
        {
            current = 0;
            return true;
        }
        else
        {
            current += deltaTime;
            return false;
        }
    }
}

[System.Serializable]
public class AlwaysCooltimeData : CooltimeData
{
    public AlwaysCooltimeData() : base() { }

    public AlwaysCooltimeData(float max, float current = 0f) : base(max, current) { }

    public bool Is_Full(float deltaTime)
    {
        current += deltaTime;

        if (current >= max)
        {
            current -= max;
            return true;
        }
        else
        {
            return false;
        }
    }
}

#endregion


#region Class : Movable


[System.Serializable]
public class CurrentKnockbackState
{
    public Vector2 dir;
    public float power;
    public float time;

    public CurrentKnockbackState(Vector2 knockbackDir, float knockbackPower, float knockbackTime)
    {
        dir = knockbackDir;
        power = knockbackPower;
        time = knockbackTime;
    }

    public Tween Start_Knockback()
    {
        return DOTween.To(() => power, x => power = x, 0, time);
    }

    public Vector2 Get_Knockback()
    {
        return dir.normalized * power;
    }
}


#endregion


#region Class : Alive

[System.Serializable]
public class DeadParticleElement
{
    public Sprite sprite;
    public Vector2 shadowSize;
}


#endregion

#region Class : State : Combat

[System.Serializable]
public class CombatOwner
{
    public eCombatOwner owner;
    public int id;

    public CombatOwner(CombatOwner owner)
    {
        this.owner = owner.owner;
        id = owner.id;
    }

    public CombatOwner(eCombatOwner typeOwner, int id = -1)
    {
        owner = typeOwner;
        this.id = id;
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
    [SerializeField] public CombatOwner ownerData;

    [Space(10)]
    [Header("=== Damage")]
    [SerializeField] public DmgState dmgState;

    [Space(10)]
    [Header("=== Critical")]
    [SerializeField] public CriticalState criticalState;

    [Space(10)]
    [Header("=== Knockback")]
    [SerializeField] public KnockbackState knockbackState;

    #endregion

    #region Constructor

    public CombatState(CombatState state)
    {
        ownerData = new CombatOwner(state.ownerData);
        dmgState = new DmgState(state.dmgState);
        criticalState = new CriticalState(state.criticalState);
        knockbackState = new KnockbackState(state.knockbackState);
    }

    public CombatState(CombatOwner owner, DmgState dmgState, CriticalState criticalState, KnockbackState knockbackState)
    {
        ownerData = new CombatOwner(owner);
        this.dmgState = new DmgState(dmgState);
        this.criticalState = new CriticalState(criticalState);
        this.knockbackState = new KnockbackState(knockbackState);
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        dmgState.Reset_State();
        criticalState.Reset_State();
        knockbackState.Reset_State();
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
    [SerializeField] public bool isCritical;
    [SerializeField] public float muzzleSpeed;
    [SerializeField] public float aliveTime;

    [SerializeField] public bool isStatus = false;
    [SerializeField] public eStatusEffect statusType;

    #endregion

    #region Constructor

    public BulletState(CombatState state, bool checkIsCritical, float muzzleSpeed, float aliveTime) : 
        base(state)
    {
        if (checkIsCritical)
        {
            isCritical = DevTool.Is_ChanceSuccess(state.criticalState.criticalChance);
        }
        else
        {
            isCritical = false;
        }

        this.muzzleSpeed = muzzleSpeed;
        this.aliveTime = aliveTime;
    }

    public BulletState(BulletState state, bool checkIsCritical) : 
        base(state.ownerData, state.dmgState, state.criticalState, state.knockbackState)
    {
        if (checkIsCritical)
        {
            isCritical = DevTool.Is_ChanceSuccess(state.criticalState.criticalChance);
        }
        else
        {
            isCritical = state.isCritical;
        }

        muzzleSpeed = state.muzzleSpeed;
        aliveTime = state.aliveTime;
    }


    #endregion

    #region Status

    public void Set_Status(bool isOn, eStatusEffect statueType)
    {
        isStatus = isOn;
        statusType = statueType;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        base.Reset_State();

        isCritical = false;
        muzzleSpeed = 0;
        aliveTime = 0;
        isStatus = false;
    }

    #endregion
}

#endregion

#region Class : State : Combat : Attacker

[System.Serializable]
public class AttackerState : CombatState
{
    #region Constructor

    public AttackerState(CombatState state) : 
        base(state.ownerData, state.dmgState, state.criticalState, state.knockbackState) { }

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
    [SerializeField] public AttackSizeState attackSizeState;

    public bool isFire = false;
    public bool isCold = false;
    public bool isElectricity = false;
    public bool isCorrosion = false;

    #endregion

    #region Constructor

    public ExplosionState(CombatState state, AttackSizeState sizeState, List<bool> isStatusList) : 
        base(state.ownerData, state.dmgState, state.criticalState, state.knockbackState) 
    {
        attackSizeState = new AttackSizeState(sizeState);

        isFire = isStatusList[0];
        isCold = isStatusList[1];
        isElectricity = isStatusList[2];
        isCorrosion = isStatusList[3];
    }
    public ExplosionState(ExplosionState state) : 
        base(state.ownerData, state.dmgState, state.criticalState, state.knockbackState)
    {
        attackSizeState = new AttackSizeState(state.attackSizeState);

        isFire = state.isFire;
        isCold = state.isCold;
        isElectricity = state.isElectricity;
        isCorrosion = state.isCorrosion;
    }

    #endregion

    #region Get 

    public List<bool> Get_AttributeCondition()
    {
        return new List<bool> { isFire, isCold, isElectricity, isCorrosion };
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

    [SerializeField] public eDamageType dmgType;
    [SerializeField] public float dmg;

    #endregion

    #region Constructor

    public DmgState(DmgState state)
    {
        dmgType = state.dmgType;
        dmg = state.dmg;
    }

    public DmgState(eDamageType dmgType, float dmg)
    {
        this.dmgType = dmgType;
        this.dmg = dmg;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        dmgType = eDamageType.Physics;
        dmg = 0;
    }

    #endregion
}

[System.Serializable]
public class CriticalState : ElementState
{
    #region Value

    [SerializeField] public float criticalChance;
    [SerializeField] public float criticalDmg;

    #endregion

    #region Constructor

    public CriticalState(CriticalState state)
    {
        criticalChance = state.criticalChance;
        criticalDmg = state.criticalDmg;
    }

    public CriticalState(float criticalChance, float criticalDmg)
    {
        this.criticalChance = criticalChance;
        this.criticalDmg = criticalDmg;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        criticalChance = 0;
        criticalDmg = 0;
    }

    #endregion
}

[System.Serializable]
public class KnockbackState : ElementState
{
    #region Value

    [SerializeField] public bool canKB;
    [SerializeField] public float kbPower;
    [SerializeField] public float kbTime;

    #endregion

    #region Constructor

    public KnockbackState(KnockbackState state)
    {
        canKB = state.canKB;
        kbPower = state.kbPower;
        kbTime = state.kbTime;
    }

    public KnockbackState(bool canKB, float kbPower, float kbTime)
    {
        this.canKB = canKB;
        this.kbPower = kbPower;
        this.kbTime = kbTime;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        canKB = false;
        kbPower = 0;
        kbTime = 0;
    }

    #endregion
}

[System.Serializable]
public class AttackSizeState : ElementState
{
    #region Value

    [SerializeField] public float size;

    #endregion

    #region Constructor

    public AttackSizeState(AttackSizeState state)
    {
        size = state.size;
    }

    public AttackSizeState(float size)
    {
        this.size = size;
    }

    #endregion

    #region Reset

    public override void Reset_State()
    {
        size = 1f;
    }

    #endregion
}

#endregion


#region Class : State : Player : BU Shop

// Level, State을 저장 관리하고, 상점까지 통괄

[System.Serializable]
public class BUShopSkillData<T, U>
{
    public BUShopData<T> skill_CooltimeShop;
    public BUShopData<T> skill_PowerShop;
    public BUShopData<U> skill_TierShop;
}

[System.Serializable]
public class BUShopData<T>
{
    #region Value

    #region - Insprector

    [SerializeField] public TxtAmountForBuyEUIController upgradeEUI;

    #endregion

    #region - Hide

    [HideInInspector] public BUState<T> state;
    [HideInInspector] private BULevelData<T> levelData;

    [HideInInspector] public string name;
    [HideInInspector] public string desc;

    #endregion

    #endregion

    #region Offset

    public void Offset(
        BUState<T> state,
        BULevelData<T> levelData,
        List<BUShopData<T>> allList,
        BaseUpgradeUIController owner)
    {
        this.state = state;
        this.levelData = levelData;

        upgradeEUI.Offset(owner);

        this.state.Offset(upgradeEUI, this.levelData);
        this.state.Set_BuffedState();

        allList.Add(this);
    }

    public void Set_LanguageTxt(string name, string desc)
    {
        this.name = name;
        this.desc = desc;

        upgradeEUI.Set_LanguageTxt(this.name, this.desc);
    }

    #endregion

    #region Buy

    private bool Can_Buy()
    {
        return PlayerManager.instance.playerController.Is_EnoughChargedBettery(levelData.levelDataList[state.currentLevel.Value].needEC_ForUpgrade) &&
            BaseUpgradeController.UsingShop != null &&
            BaseUpgradeController.UsingShop.CurrentDur > 0;
    }

    public void Try_Buy()
    {
        if (Can_Buy())
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

            // Dur
            BaseUpgradeController.UsingShop.Take_Damage(_SpawnItem: false, _SoundOn: false);

            // Cost
            PlayerManager.instance.playerController.Use_ChargedBettery(levelData.levelDataList[state.currentLevel.Value].needEC_ForUpgrade);

            Set_LevelUp();
        }
    }

    private void Set_LevelUp()
    {
        // Lv Up
        state.currentLevel.Value++;
        state.actualState.Value = levelData.levelDataList[state.currentLevel.Value - 1].upgradeValue;
        state.Set_BuffedState();

        // Can Lv Up
        upgradeEUI.BuyBtn.ThisBtn.interactable = DevTool.buMaxLevel <= state.currentLevel.Value ? false : true;

        // Desc
        MainGameUIManager.instance.baseUpgrade_UIController.SetOn_Desc(upgradeEUI, upgradeEUI.SkillNameTxt.text);
    }

    #endregion
}

#endregion

#region Class : State : Player : BU Level

// BU Manager로 미리 수치를 저장하기 위함

[System.Serializable]
public class BULevelSkillData<T, U>
{
    public BULevelData<T> skill_Cooltime_BUData;
    public BULevelData<T> skill_Power_BUData;
    public BULevelData<U> skill_Tier_BUData;
}

[System.Serializable]
public class BULevelData<T>
{
    [Header("=== Level Value")]
    public List<BUEachLevelData<T>> levelDataList;

    // Base Offset
    public void Offset(BUState<T> baseValue)
    {
        levelDataList = new List<BUEachLevelData<T>>();

        if (baseValue.baseState.GetType() == typeof(float))
        {
            Offset_Float(
                float.Parse(baseValue.baseState.ToString()),
                DevTool.Get_ParseFloatList(baseValue.upgradeValueByLevelRange));
        }
        else if (baseValue.baseState.GetType() == typeof(int))
        {
            Offset_Int(
                int.Parse(baseValue.baseState.ToString()),
                DevTool.Get_ParseIntList(baseValue.upgradeValueByLevelRange));
        }
    }

    // FLOAT
    public void Offset_Float(float floatValue, List<float> upgradeValue)
    {
        for (int i = 0; i < DevTool.buMaxLevel; i++)
        {
            BUEachLevelData<T> eachLevelData = new BUEachLevelData<T>();

            decimal stateValue = i == 0 ?
                 (decimal)(floatValue + upgradeValue[0]) :
                 (decimal)((float)levelDataList[i - 1].Get_UpgradeValue() + upgradeValue[(int)(i / DevTool.buLevelInterval)]);

            eachLevelData.Set_UpgradeValue(Mathf.RoundToInt((float)stateValue * 100f) / 100f);
            eachLevelData.needEC_ForUpgrade = (int)(i / DevTool.buLevelInterval) + 1;

            levelDataList.Add(eachLevelData);
        }
    }

    // INT
    public void Offset_Int(int intValue, List<int> upgradeValue)
    {
        for (int i = 0; i < DevTool.buMaxLevel; i++)
        {
            BUEachLevelData<T> test = new BUEachLevelData<T>();

            int stateValue = i == 0 ?
                (intValue + upgradeValue[0]) :
                ((int)levelDataList[i - 1].Get_UpgradeValue() + upgradeValue[(int)(i / DevTool.buLevelInterval)]);
            
            test.Set_UpgradeValue((int)stateValue);
            test.needEC_ForUpgrade = (int)(i / DevTool.buLevelInterval) + 1; 
            
            levelDataList.Add(test);
        }
    }
}

[System.Serializable]
public class BUEachLevelData<T>
{
    public T upgradeValue;
    public int needEC_ForUpgrade;

    public void Set_UpgradeValue(object value)
    {
        upgradeValue = (T)value;
    }

    public object Get_UpgradeValue()
    {
        return upgradeValue;
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

    [SerializeField] public T baseState;
    [SerializeField] public List<T> upgradeValueByLevelRange;
    [SerializeField] public ReactiveProperty<T> actualState;

    #endregion

    #region - Hide

    [HideInInspector] public ReactiveProperty<int> currentLevel = new();
    [HideInInspector] private List<BuffState<T>> buffList = new List<BuffState<T>>();

    [HideInInspector] public T buffedState { get; set; }

    #endregion

    #endregion

    #region Offset

    public void Offset(
        TxtAmountForBuyEUIController eachEUI, 
        BULevelData<T> upgradeLevelData)
    {
        currentLevel.Value = 0;
        currentLevel
           .Subscribe(_CurrentLevel =>
           {
               if (_CurrentLevel < DevTool.buMaxLevel)
               {
                   eachEUI.Set(_CurrentLevel, upgradeLevelData.levelDataList[_CurrentLevel].needEC_ForUpgrade);
               }
               else
               {
                   eachEUI.Set(_CurrentLevel, 0);
               }

               eachEUI.Set_InnerAlpha((float)_CurrentLevel / (float)DevTool.buMaxLevel);
           });
    }

    #endregion

    #region Gain / Lose
    // Gain
    public void Gain_Buff(BuffState<T> state)
    {
        DevTool.Add_InList(buffList, state);
    }

    // Remove
    public void Lose_Buff(BuffState<T> state)
    {
        DevTool.Remove_InList(buffList, state);
    }

    #endregion

    #region Buffed

    public void Set_BuffedState()
    {
        if (actualState.Value.GetType() == typeof(float))
        {
            Set_BufftedState_Float();
        }
        else if (actualState.Value.GetType() == typeof(int))
        {
            Set_BufftedState_Int();
        }
    }

    private void Set_BufftedState_Float()
    {
        float state = 1.0f;
        for (int i = 0; i < buffList.Count; i++)
        {
            state += float.Parse(buffList[i].actualValue.ToString());
        }
        state *= float.Parse(actualState.Value.ToString());
        buffedState = (T)(object)state;
    }

    private void Set_BufftedState_Int()
    {
        int state = 0;
        for (int i = 0; i < buffList.Count; i++)
        {
            state += int.Parse(buffList[i].actualValue.ToString());
        }
        state += int.Parse(actualState.Value.ToString());
        buffedState = (T)(object)state;
    }

    #endregion
}

#endregion



#region Class : State : Player : Module ItemData

[System.Serializable]
public class ItemData_Field
{
    [Header("=== ID")]
    public int id;

    [HideInInspector] public int rank = 1;

    public ItemData_Field(int id)
    {
        this.id = id;
    }

    public ItemData_Field(int id, int rank)
    {
        this.id = id;
        this.rank = rank;
    }

    public ItemData_Field(ItemData_Field data)
    {
        id = data.id;
        rank = data.rank;
    }
}

[System.Serializable]
public class ItemData : ItemData_Field
{
    [Header("=== Info")]
    public string name;
    public string desc;
    public string equipDesc;
    public Sprite itemIcon;

    [Header("=== MainChip")]
    public int r1_MainChipID;
    public int r3_MainChipID;
    public int r5_MainChipID;

    public ItemData(int id) : base(id) { }

    public ItemData(ItemData_Field data) : base(data) { }

    public ItemData(ItemData itemData) : base(itemData)
    {
        Set_LanguageTxt(itemData);
        itemIcon = itemData.itemIcon;

        r1_MainChipID = itemData.r1_MainChipID;
        r3_MainChipID = itemData.r3_MainChipID;
        r5_MainChipID = itemData.r5_MainChipID;
    }

    public void Set_LanguageTxt(ItemData itemData)
    {
        name = itemData.name;
        desc = itemData.desc;
        equipDesc = itemData.equipDesc;
    }

    public ItemData(int id, Sprite icon, int r1, int r3, int r5) : base(id)
    {
        itemIcon = icon;
        r1_MainChipID = r1;
        r3_MainChipID = r3;
        r5_MainChipID = r5;
    }
}

[System.Serializable]
public class MainChipData
{
    public Sprite thisIcon;
    public int id;
    public string name;
    public string[] amalgamationDescArr;

    public MainChipData(int id, Sprite icon)
    {
        this.id = id;
        thisIcon = icon;
    }
}

#endregion

#region Class : State : Player : MU

public class ModuleState : IWhen
{
    #region Value

    public ItemData thisItemData;

    protected ModuleItemActivityManager.ActivityFuncDele_MI thisActivityFuncDele;

    #endregion

    #region Constructor

    public ModuleState() { }

    public void Set_State(ItemData itemData)
    {
        thisItemData = new ItemData(itemData);
        thisActivityFuncDele = ModuleItemActivityManager.instance.Get_CollectActivity_MI(thisItemData.id);
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
        return thisItemData.rank;
    }

    #endregion

    #region Interface

    public virtual void Play_When(EnemyController enemy = null)
    {
        thisActivityFuncDele(Get_Rank(), enemy);
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
    public RectTransform panelRT; 
    public OwnBtnEUIController panelBtn;
    public TMP_Text panelBtnTxt;

    [HideInInspector] public CanvasGroup PanelBtnCG;

    [Space(10)]
    public OwnBtnEUIController roleBtn;
    public TMP_Text roleBtnTxt;
    public TMP_Text roleDescTxt;

    [HideInInspector] public RectTransform roleBtnTxtRT;
    [HideInInspector] public Tween rtTween = null;

    [Space(10)]
    public List<Image> innerImgs;

    #endregion

    public void Offset(ModuleUpgradeUIController muuc, string btnName, string btnDesc)
    {
        panelBtn.Offset();

        panelBtn.OwnerUIController = muuc;

        roleBtn.Offset();
        roleBtn.OwnerUIController = muuc;

        PanelBtnCG = DevTool.Get_ComponentTType<CanvasGroup>(panelBtn.gameObject);

        roleBtnTxtRT = DevTool.Get_ComponentTType<RectTransform>(roleBtnTxt.gameObject);

        Set_LanguageTxt(btnName, btnDesc);

        rtTween = roleBtnTxtRT.DOScale(1.15f, 1.0f)
                .OnPlay(() => { roleBtnTxtRT.localScale = Vector2.one; })
                .OnKill(() => { roleBtnTxtRT.localScale = Vector2.one; })
                .SetLoops(-1, LoopType.Yoyo);

        DOTween.Play(rtTween);
    }

    public void Set_LanguageTxt(string btnName, string btnDesc)
    {
        panelBtnTxt.text = btnName;
        roleBtnTxt.text = ">>  " + btnName + "  <<";
        roleDescTxt.text = btnDesc;
    }
}

#endregion

#region Class : State : Player : MC

[System.Serializable]
public class SynchoronyState : IWhenSync
{
    #region Value

    [FormerlySerializedAs("ID")] [SerializeField] public int ID = 0;
    [FormerlySerializedAs("SynergyRank")] [SerializeField] public int SynergyRank = 0;
    protected ModuleItemActivityManager.ActivityFuncDele_MC ThisActivityFuncDele;

    #endregion

    #region Constructor

    public virtual void Set_State(int _ID, int _SynergyRank)
    {
        ID = _ID;
        SynergyRank = _SynergyRank;
        ThisActivityFuncDele = ModuleItemActivityManager.instance.Get_CollectActivity_MC(ID);
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

    public static List<float> valueList = new List<float> { 0.15f, 0.35f, 0.6f };
    public static List<float> cooltimeList = new List<float> { 5f, 6f, 7f };

    public override void Set_State(int id, int synergyRank)
    {
        base.Set_State(id, synergyRank);

        if (BuffManager.instance.Get_CorrectBuff(1) is BuffDmgController dmgBuff)
        {
            dmgBuff.Set_Value(DevTool.Get_SyncValue(valueList, synergyRank));
            dmgBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(cooltimeList, synergyRank));
        }
    }
}

public class SynchoronyState006 : SynchoronyState, IWhenSync_Start
{
    public SynchoronyState006() : base() { }

    public static List<float> valueList = new List<float> { 0.2f, 0.5f, 1f };

    public override void Set_State(int id, int synergyRank)
    {
        base.Set_State(id, synergyRank);

        if (BuffManager.instance.Get_CorrectBuff(8) is BuffCDController CDBuff)
        {
            CDBuff.Set_Value(DevTool.Get_SyncValue(valueList, synergyRank));
        }
    }
}

public class SynchoronyState007 : SynchoronyState, IWhenSync_Hit
{
    public SynchoronyState007() : base() { }

    public static List<float> valueList = new List<float> { 0.04f, 0.06f, 0.08f };
    public static List<int> maxChargeList = new List<int> { 5, 7, 10 };
    public static List<float> cooltimeList = new List<float> { 3, 4, 5 };

    public override void Set_State(int id, int synergyRank)
    {
        base.Set_State(id, synergyRank);

        if (BuffManager.instance.Get_CorrectBuff(9) is BuffRofController rofBuff)
        {
            rofBuff.Set_Value(DevTool.Get_SyncValue(valueList, synergyRank));
            rofBuff.Set_MaxChargeValue(DevTool.Get_SyncValue(maxChargeList, synergyRank));
            rofBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(cooltimeList, synergyRank));
        }
    }
}

public class SynchoronyState008 : SynchoronyState, IWhenSync_AfterFire
{
    public SynchoronyState008() : base() { }

    public static List<float> valueList = new List<float> { 1f, 1.5f, 2f };
    public static List<int> maxChargeList = new List<int> { 2, 3, 4 };
    public static List<float> cooltimeList = new List<float> { 2f, 1.5f, 1f };

    public override void Set_State(int id, int synergyRank)
    {
        base.Set_State(id, synergyRank);

        if (BuffManager.instance.Get_CorrectBuff(10) is BuffDmgController dmgBuff)
        {
            dmgBuff.Set_Value(DevTool.Get_SyncValue(valueList, synergyRank));
            dmgBuff.Set_MaxChargeValue(DevTool.Get_SyncValue(maxChargeList, synergyRank));
            dmgBuff.Set_CoolTimeValue(DevTool.Get_SyncValue(cooltimeList, synergyRank));

            dmgBuff.Max_Buff();
        }
    }
}

#endregion


#region Class : State : Ally : MU

public class CopyModuleState
{
    public ModuleState state;
    public CoupleData<int> originalIndex;
    public bool isEquipped;

    public CopyModuleState(ModuleState state, CoupleData<int> originalIndex, bool isEquipped)
    {
        this.state = state;
        this.originalIndex = originalIndex;
        this.isEquipped = isEquipped;
    }
}

#endregion

#region Class : State : Ally : Sync

public class AllySyncState : IWhenAlly
{
    #region Value

    [SerializeField] public int id = 0;
    [SerializeField] public int synergyRank = 0;

    protected AllyController thisAlly;
    protected AllySyncManager.ActivityFuncDele_Sync thisActivityFuncDele;

    #endregion

    #region Constructor

    public virtual void Set_State(AllyController ally, int id, int synergyRank)
    {
        thisAlly = ally;
        this.id = id;
        this.synergyRank = synergyRank;
        thisActivityFuncDele = AllySyncManager.instance.Get_CollectActivity_Sync(this.id);
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
        EnemyController enemy = null, 
        BulletController bullet = null, 
        DroppingBombController droppingBullet = null)
    {
        thisActivityFuncDele(thisAlly, synergyRank, enemy, bullet, droppingBullet);
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

    public override void Set_State(AllyController ally, int id, int synergyRank)
    {
        base.Set_State(ally, id, synergyRank);

        AllyBuff buff = ally.BuffController.Get_AllyBuff("Sync005");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState005.valueList, synergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState005.cooltimeList, synergyRank));

        buff.SetOn_State(showAlwaysOnOff: false);
    }
}
public class AllySyncState006 : AllySyncState, IWhenAlly_Start
{ 
    public AllySyncState006() : base() { }

    public override void Set_State(AllyController ally, int id, int synergyRank)
    {
        base.Set_State(ally, id, synergyRank);

        AllyBuff buff = ally.BuffController.Get_AllyBuff("Sync006");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState006.valueList, synergyRank));

        buff.SetOn_State(showAlwaysOnOff: false);
    }
}
public class AllySyncState007 : AllySyncState, IWhenAlly_AfterFire
{ 
    public AllySyncState007() : base() { }

    public override void Set_State(AllyController ally, int id, int synergyRank)
    {
        base.Set_State(ally, id, synergyRank);

        AllyBuff buff = ally.BuffController.Get_AllyBuff("Sync007");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState007.valueList, synergyRank));
        buff.Set_MaxAmount(DevTool.Get_SyncValue(SynchoronyState007.maxChargeList, synergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState007.cooltimeList, synergyRank));

        buff.SetOn_State(showAlwaysOnOff: false);
    }
}

public class AllySyncState008 : AllySyncState, IWhenAlly_Hit
{
    public AllySyncState008() : base() { }

    public override void Set_State(AllyController ally, int id, int synergyRank)
    {
        base.Set_State(ally, id, synergyRank);

        AllyBuff buff = ally.BuffController.Get_AllyBuff("Sync008");

        buff.Set_Value(DevTool.Get_SyncValue(SynchoronyState008.valueList, synergyRank));
        buff.Set_MaxAmount(DevTool.Get_SyncValue(SynchoronyState008.maxChargeList, synergyRank));
        buff.Set_Cooltime(DevTool.Get_SyncValue(SynchoronyState008.cooltimeList, synergyRank));

        buff.SetOn_State(showAlwaysOnOff: true);
    }
}

#endregion

#region Class : State : Ally : Buff

[Serializable]
public class OriginalAllyBuff
{
    [SerializeField] public string type;
    [SerializeField] public Sprite iconSprite;

    [SerializeField] public float buffValue = 0;
    [SerializeField] public int buffMaxAmount = 0;
    [SerializeField] public float maxCooltime = 0;
    [SerializeField] public bool isIncrease = false;
    [SerializeField] public bool isPermanent = false;
}


[Serializable]
public class AllyBuff : OriginalAllyBuff
{
    #region Value

    private AllyController ally;
    private BuffIconEUIController buffIconUI = null;

    [SerializeField] private int buffAmount = 0;
    private RefData<float> actualBuffValue;

    private bool isOn = false;
    private bool isAlwaysShowUI = false;

    [SerializeField] private float currentCooltime = 0;

    #endregion

    #region Constructor

    public AllyBuff(AllyController ally, OriginalAllyBuff original) : base()
    {
        this.ally = ally;

        type = original.type;
        iconSprite = original.iconSprite;
        buffValue = original.buffValue;
        buffMaxAmount = original.buffMaxAmount;
        maxCooltime = original.maxCooltime;
        isIncrease = original.isIncrease;
        isPermanent = original.isPermanent;

        isOn = false;
        isAlwaysShowUI = false;
        buffAmount = 0;
        currentCooltime = 0;
        actualBuffValue = new RefData<float>(0);
    }

    #endregion

    #region Set

    public void SetOn_State(bool showAlwaysOnOff)
    {
        buffAmount = 0;
        currentCooltime = 0;
        actualBuffValue.value = 0;

        ally.BuffController.BuffingState.Add_List(this, type);
        Set_OnOff(true);
        Set_AlwaysShowUI(showAlwaysOnOff); 

        ally.BuffController.BuffingState.Set_BuffedAllyState();
        ally.Set_AllState();
    }

    public void SetOff_State()
    {
        buffAmount = 0;
        currentCooltime = 0;
        actualBuffValue.value = 0;

        ally.BuffController.BuffingState.Remove_List(this, type);
        Set_OnOff(false);
        Set_AlwaysShowUI(false); 

        ally.BuffController.BuffingState.Set_BuffedAllyState();
        ally.Set_AllState();
    }

    private void Set_OnOff(bool onOff)
    {
        if (isOn == onOff) return;
        
        isOn = onOff;

        if (!isOn && buffIconUI != null) // 꺼짐
        {
            ally.HUD.TemporaryBuffUI.Remove_BuffIconUI(buffIconUI);
            buffIconUI = null;
        }
    }

    private void Set_AlwaysShowUI(bool onOff)
    {
        if (isAlwaysShowUI == onOff) return;

        isAlwaysShowUI = onOff;

        if (isAlwaysShowUI) // 항상 켜짐
        {
            buffIconUI = ally.HUD.TemporaryBuffUI.Get_BuffIconUI();
            buffIconUI.SetOn(iconSprite, isAlwaysShowUI ? true : !(buffAmount == 0));
        }
    }

    #endregion

    #region Set (Change Value)

    public void Set_Value(float value)
    {
        buffValue = value;
    }

    public void Set_MaxAmount(int value)
    {
        buffMaxAmount = value;
    }

    public void Set_Cooltime(float value)
    {
        maxCooltime = value;
    }

    #endregion

    public void SetAndGain_Buff(int stack = 1)
    {
        ally.BuffController.BuffingState.Add_List(this, type);
        Set_OnOff(true);
        Set_AlwaysShowUI(false);
        Gain_Buff(stack);
    }

    #region Gain Loss

    public void Gain_Buff(int stack = 1)
    {
        // ui first
        if (buffIconUI == null)
        {
            buffIconUI = ally.HUD.TemporaryBuffUI.Get_BuffIconUI();
        }

        // value
        buffAmount = Mathf.Min(buffAmount + stack, buffMaxAmount);
        Set_Value();
        ally.BuffController.BuffingState.Set_BuffedAllyState();
        ally.Set_AllState();

        if (!isIncrease) currentCooltime = 0;

        // ui
        buffIconUI.SetOn(iconSprite, isAlwaysShowUI ? true : !(buffAmount == 0));
    }

    public void Reduce_Buff(int stack = 1)
    {
        // value
        buffAmount = Mathf.Max(buffAmount - stack, 0);
        Set_Value();
        ally.BuffController.BuffingState.Set_BuffedAllyState();
        ally.Set_AllState();

        if (isIncrease) currentCooltime = 0;

        // ui
        buffIconUI.SetOn(iconSprite, isAlwaysShowUI ? true : !(buffAmount == 0));

        // ui last
        if (buffAmount == 0 && !isAlwaysShowUI && buffIconUI != null)
        {
            ally.HUD.TemporaryBuffUI.Remove_BuffIconUI(buffIconUI);
            buffIconUI = null;
        }
    }

    #endregion

    #region Cooltime

    public void Caculate_Cooltime(float deltaTime)
    {
        if (!isOn || isPermanent) return;

        if (isIncrease)
            Caculate_Cooltime_Increase(deltaTime);
        else
            Caculate_Cooltime_Decrease(deltaTime);

        if (buffIconUI != null)
        {
            buffIconUI.Set_Cooltime(currentCooltime / maxCooltime);
            buffIconUI.Set_Icon(buffAmount, buffMaxAmount);
        }
    }

    private void Caculate_Cooltime_Increase(float deltaTime)
    {
        if (buffAmount >= buffMaxAmount) return;

        if (maxCooltime <= currentCooltime) // 스택 감소
        {
            currentCooltime -= maxCooltime;
            Gain_Buff(1);
        }
        else // 쿨타임 돌림
        {
            currentCooltime += deltaTime;
        }
    }

    private void Caculate_Cooltime_Decrease(float deltaTime)
    {
        if (buffAmount <= 0) return;

        if (maxCooltime <= currentCooltime) // 스택 감소
        {
            currentCooltime -= maxCooltime;
            Reduce_Buff(1);
        }
        else // 쿨타임 돌림
        {
            currentCooltime += deltaTime;
        }
    }

    #endregion

    #region Value

    private void Set_Value()
    {
        actualBuffValue.value = buffAmount * buffValue;
    }
    
    public float Get_Value()
    {
        return actualBuffValue.value;
    }

    #endregion

}


#endregion


#region Class : State : Player : Other

[System.Serializable]
public class Shield
{
    public string shieldID;
    public float shieldMaxValue;
    public float shieldCurrentValue;
}


[System.Serializable]
public class BuffState<T>
{
    public string buffID;
    public T baseValue;
    public T actualValue;
}

#endregion



#region Class : Spawn : Enemy

[System.Serializable]
public class EnemySpot
{
    [SerializeField] public int enemyID;
    [SerializeField] public Transform enemySpawnTF;
}



#endregion


#region Class : State : Enemy : Pattern

[System.Serializable]
public class ContinuousEnemyPattern
{
    public List<EnemyPattern> enemyPatternList;
}

[System.Serializable]
public class OrderOfPriorityEnemyPattern
{
    public List<ContinuousEnemyPattern> enemyPatternList;
}

#endregion


#region Class : State : Enemy : Buff : Status Effect 

[Serializable]
public class StatusEffect
{
    #region Value

    public delegate void EffectDele();

    [HideInInspector] public EnemyController enemy;
    [HideInInspector] public BuffIconEUIController buffIconUI = null;
    [HideInInspector] public Sprite iconSprite;

    public bool isOn;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect(EnemyController enemy, Sprite iconSprite)
    {
        isOn = false;

        this.enemy = enemy;
        this.iconSprite = iconSprite;
    }

    #endregion

    #region Clear

    public virtual void Set_Clear()
    {
        isOn = false;
    }

    #endregion
}

#endregion

#region Class : State : Enemy : Buff : Temporary Effect

[Serializable]
public class StatusEffect_Temporary : StatusEffect
{
    #region Value

    public float maxCooltime;
    public float currentCooltime;

    protected EffectDele gainDele = null;
    protected EffectDele reduceDele = null;

    #endregion

    #region Contruct
    // 생성자
    public StatusEffect_Temporary(
        EnemyController enemy, float maxCooltime,
        EffectDele gainFunc, EffectDele reduceFunc, Sprite iconSprite)
        : base(enemy, iconSprite)
    {
        base.enemy = enemy;

        this.maxCooltime = maxCooltime;
        currentCooltime = 0;

        gainDele = gainFunc;
        reduceDele = reduceFunc;

        base.iconSprite = iconSprite;
    }
    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int gainAmount, bool showTxt, CombatOwner combatOwner)
    {
        if (buffIconUI == null)
        { Start_FirstStack(showTxt); }

        if (gainDele != null)
        { gainDele(); }
    }

    // 버프 감소
    public virtual void Reduce_Stack(int gainAmount)
    {
        if (reduceDele != null)
        { reduceDele(); }
    }

    // 버프 시작
    protected virtual void Start_FirstStack(bool showTxt)
    {
        if (buffIconUI == null)
        {
            buffIconUI = enemy.HUD.TemporaryBuffUI.Get_BuffIconUI();
            buffIconUI.SetOn(iconSprite, showTxt);
        }
        isOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (buffIconUI != null)
        {
            enemy.HUD.TemporaryBuffUI.Remove_BuffIconUI(buffIconUI);
            buffIconUI = null;
        }

        isOn = false;
        currentCooltime = 0;
    }




    public virtual void Caculate_Cooltime(float deltaTime)
    {
        buffIconUI.Set_Cooltime(currentCooltime / maxCooltime);
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

    public eStatusEffect statusType;

    public int maxStack;
    public int currentStack;
    public int onceTimeReduceAmount;

    private bool isResetWhenGain;

    protected EffectDele fullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Temporary_WithAmount(
        EnemyController enemy, eStatusEffect statusType, int maxStack, float maxCooltime, int onceTimeReduceAmount, bool isResetWhenGain,
        EffectDele gainFunc, EffectDele reduceFunc, EffectDele fullStack,
        Sprite iconSprite)
        : base(enemy, maxCooltime, gainFunc, reduceFunc, iconSprite)
    {
        this.statusType = statusType;

        this.maxStack = maxStack;
        currentStack = 0;
        this.onceTimeReduceAmount = onceTimeReduceAmount;

        this.isResetWhenGain = isResetWhenGain;

        this.fullStack = fullStack;
    }

    #endregion

    #region Active Func

    private DeleEnemy Get_PlayerIDele()
    {
        switch (statusType)
        {
            case eStatusEffect.Flame:
                return new DeleEnemy(ModuleItemManager.instance.ActiveSync_EnemyTakingFire);

            case eStatusEffect.Cold:
                return new DeleEnemy(ModuleItemManager.instance.ActiveSync_EnemyTakingCold);

            case eStatusEffect.Electricity:
                return new DeleEnemy(ModuleItemManager.instance.ActiveSync_EnemyTakingElectricity);

            case eStatusEffect.Corrosion:
                return new DeleEnemy(ModuleItemManager.instance.ActiveSync_EnemyTakingCorrosion);
        }
        return null;
    }

    private DeleEnemy Get_AllyIDele(int id)
    {
        AllyController ally = AllyManager.instance.allAlly[id];
        switch (statusType)
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
    public override void Gain_Stack(int gainAmount, bool showTxt, CombatOwner combatOwner)
    {
        currentStack = System.Math.Clamp(currentStack + gainAmount, 0, maxStack);

        // 플레이어 속성
        CurrentGainStack = gainAmount;
        if (combatOwner.owner == eCombatOwner.Player)
            Get_PlayerIDele()(enemy);
        else if (combatOwner.owner == eCombatOwner.Ally)
            Get_AllyIDele(combatOwner.id)(enemy);

        if (isResetWhenGain)
        {
            currentCooltime = 0;
        }

        base.Gain_Stack(gainAmount, showTxt, combatOwner);

        if (currentStack >= maxStack && fullStack != null)
        {
            fullStack();
        }
    }

    public void ReGain_Stack()
    {
        currentStack = System.Math.Clamp(currentStack + CurrentGainStack, 0, maxStack);
    }

    // 버프 감소
    public override void Reduce_Stack(int reduceAmount)
    {
        base.Reduce_Stack(reduceAmount);

        currentStack = System.Math.Clamp(currentStack - reduceAmount, 0, maxStack);
        if (currentStack <= 0)
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
        currentStack = 0;
    }

    // 쿨타임
    public override void Caculate_Cooltime(float deltaTime)
    {
        if (isOn)
        {
            if (maxCooltime <= currentCooltime) // 스택 감소
            {
                currentCooltime -= maxCooltime;
                Reduce_Stack(1);
            }
            else // 쿨타임 돌림
            {
                currentCooltime += deltaTime;
            }
        }

        if (buffIconUI != null)
        {
            base.Caculate_Cooltime(deltaTime);
            buffIconUI.Set_Icon(currentStack, maxStack);
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
        EnemyController enemy, float maxCooltime,
        EffectDele gainFunc, EffectDele reduceFunc,
        Sprite iconSprite)
        : base(enemy, maxCooltime, gainFunc, reduceFunc, iconSprite)
    { }

    #endregion

    #region Func

    // 버프 획득
    public override void Gain_Stack(int gainAmount, bool showTxt, CombatOwner combatOwner)
    {
        currentCooltime = 0;
        base.Gain_Stack(gainAmount, showTxt, combatOwner);
    }

    // 버프 제거
    public override void Remove_AllStack()
    {
        base.Reduce_Stack(0);
        base.Remove_AllStack();
    }

    // 쿨타임
    public override void Caculate_Cooltime(float deltaTime)
    {
        if (isOn)
        {
            if (maxCooltime <= currentCooltime) // 스택 감소
            {
                Remove_AllStack();
            }
            else // 쿨타임 돌림
            {
                currentCooltime += deltaTime;
            }
        }

        if (buffIconUI != null)
        {
            base.Caculate_Cooltime(deltaTime);
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

    protected EffectDele gainDele = null;

    #endregion

    #region Contruct

    public StatusEffect_Permanent(
        EnemyController enemy, Sprite iconSprite,
        EffectDele gainFunc)
        : base(enemy, iconSprite)
    {
        gainDele = gainFunc;
    }

    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int gainAmount, bool showTxt)
    {
        if (buffIconUI == null)
        { Start_FirstStack(showTxt); }

        if (gainDele != null)
        { gainDele(); }
    }

    // 버프 감소
    // 필요 없음!

    // 버프 시작
    protected virtual void Start_FirstStack(bool showTxt)
    {
        if (buffIconUI == null)
        {
            buffIconUI = enemy.HUD.PermanentBuffUI.Get_BuffIconUI();
            buffIconUI.SetOn(iconSprite, showTxt);
        }
        isOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (buffIconUI != null)
        {
            enemy.HUD.PermanentBuffUI.Remove_BuffIconUI(buffIconUI);
            buffIconUI = null;
        }

        isOn = false;
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

    public int maxStack;
    public int currentStack;

    protected EffectDele fullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Permanent_WithAmount(
        EnemyController enemy, Sprite iconSprite,
        EffectDele gainFunc, EffectDele fullStack,
        int maxStack)
        : base(enemy, iconSprite, gainFunc)
    {
        this.maxStack = maxStack;
        this.fullStack = fullStack;
        currentStack = 0;
    }
    #endregion

    #region Func

    // 버프 증가
    public override void Gain_Stack(int gainAmount, bool showTxt)
    {
        currentStack = System.Math.Clamp(currentStack + gainAmount, 0, maxStack);

        base.Gain_Stack(gainAmount, showTxt);
        buffIconUI.Set_Icon(currentStack, maxStack);

        if (currentStack >= maxStack && fullStack != null)
        {
            fullStack();
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
        currentStack = 0;
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
        EnemyController enemy, Sprite iconSprite, EffectDele gainFunc)
        : base(enemy, iconSprite, gainFunc)
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
    [FormerlySerializedAs("thisPhase")] public int ThisPhase;
    [FormerlySerializedAs("thisPhase")] public float ThisPhaseLimitPercentHP;
    [FormerlySerializedAs("thisPhase")] public List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    [FormerlySerializedAs("thisPhase")] public ContinuousEnemyPattern SpecialPattern;

}

#endregion

#region Class : Enemy : DropItem

[System.Serializable]
public class EnemyDropItemPercent
{
    [Space(5)]
    [Header("-- Module")]
    [FormerlySerializedAs("thisPhase")] [SerializeField] public float ModuleDropPercent = 0.0f;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public List<int> ModuleRankPercents;

    [Space(5)]
    [Header("-- Keycard")]
    [FormerlySerializedAs("thisPhase")] [SerializeField] public float keycardDropPercent = 0.0f;

    [Space(5)]
    [Header("-- Goods")]
    [FormerlySerializedAs("thisPhase")] [SerializeField] public CoupleData<int> BSAmountMinMax;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public CoupleData<int> MSAmountMinMax;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public CoupleData<int> CreditAmountMinMax;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public CoupleData<int> OverriderAmountMinMax;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public CoupleData<float> JouleAmountMinMax;
}

[System.Serializable]
public class CoreDropItemPercent
{
    [Space(5)]
    [Header("-- Core")]
    [FormerlySerializedAs("thisPhase")] [SerializeField] public int CoreItemID = 0;
    [FormerlySerializedAs("thisPhase")] [SerializeField] public float CoreItemPercent = 0.0f;
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

#region Class : Map Reso

[System.Serializable]
public class MapReso
{
    public MapResoElement[] MapResoElements;

    public MapReso(MapResoElement[] _MapResoElements)
    {
        MapResoElements = _MapResoElements;
    }
}

[System.Serializable]
public class MapResoElement
{
    public Sprite Sprite;
    public int MaterialIndex;

    public MapResoElement(Sprite _Sprite, int _MaterialIndex)
    {
        Sprite = _Sprite;
        MaterialIndex = _MaterialIndex;
    }
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
    
    public void Offset(MapReso _Reso)
    {
        MapSpriteReso.Offset(_Reso, MapIndexName);
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

    public AllPassageMiddleSpriteData(MapReso _AllSprite)
    {
        PassageMiddleSpriteDict = new Dictionary<string, EachPassageMiddleSpriteData>();
        for (int i = 0; i < _AllSprite.MapResoElements.Length; i++)
        {
            string[] fullName = _AllSprite.MapResoElements[i].Sprite.name.Split("_");

            PassageMiddleSpriteDict.Add(
                $"{fullName[1]}_{fullName[3]}_{fullName[5]}",
                new EachPassageMiddleSpriteData(_AllSprite.MapResoElements[i].Sprite, _AllSprite.MapResoElements[i].MaterialIndex));
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
        Dmg = new RefData<float>(_StateValue.Dmg.value);
        Rof = new RefData<float>(_StateValue.Rof.value);
        MovementSpeed = new RefData<float>(_StateValue.MovementSpeed.value);
        AttackSize = new RefData<float>(_StateValue.AttackSize.value);
        CC = new RefData<float>(_StateValue.CC.value);
        CD = new RefData<float>(_StateValue.CD.value);
        MuzzleSpeed = new RefData<float>(_StateValue.MuzzleSpeed.value);
        KBPower = new RefData<float>(_StateValue.KBPower.value);
        Dur = new RefData<float>(_StateValue.Dur.value);
    }

    public void Reset()
    {
        Dmg.value = 1;
        Rof.value = 1;
        MovementSpeed.value = 1;
        AttackSize.value = 1;
        CC.value = 1;
        CD.value = 1;
        MuzzleSpeed.value = 1;
        KBPower.value = 1;
        Dur.value = 1;
    }

    public static AllyState Get_Multiple(AllyState _State0, AllyState _State1)
    {
        AllyState result = new AllyState();

        result.Dmg = new RefData<float>(_State0.Dmg.value * _State1.Dmg.value);
        result.Rof = new RefData<float>(_State0.Rof.value * _State1.Rof.value);
        result.MovementSpeed = new RefData<float>(_State0.MovementSpeed.value * _State1.MovementSpeed.value);
        result.AttackSize = new RefData<float>(_State0.AttackSize.value * _State1.AttackSize.value);
        result.CC = new RefData<float>(_State0.CC.value * _State1.CC.value);
        result.CD = new RefData<float>(_State0.CD.value * _State1.CD.value);
        result.MuzzleSpeed = new RefData<float>(_State0.MuzzleSpeed.value * _State1.MuzzleSpeed.value);
        result.KBPower = new RefData<float>(_State0.KBPower.value * _State1.KBPower.value);
        result.Dur = new RefData<float>(_State0.Dur.value * _State1.Dur.value);

        return result;
    }

    public static AllyState Get_Subtraction(AllyState _Original, AllyState _Exclude)
    {
        AllyState result = new AllyState();

        result.Dmg = new RefData<float>(_Original.Dmg.value - _Exclude.Dmg.value);
        result.Rof = new RefData<float>(_Original.Rof.value - _Exclude.Rof.value);
        result.MovementSpeed = new RefData<float>(_Original.MovementSpeed.value - _Exclude.MovementSpeed.value);
        result.AttackSize = new RefData<float>(_Original.AttackSize.value - _Exclude.AttackSize.value);
        result.CC = new RefData<float>(_Original.CC.value - _Exclude.CC.value);
        result.CD = new RefData<float>(_Original.CD.value - _Exclude.CD.value);
        result.MuzzleSpeed = new RefData<float>(_Original.MuzzleSpeed.value - _Exclude.MuzzleSpeed.value);
        result.KBPower = new RefData<float>(_Original.KBPower.value - _Exclude.KBPower.value);
        result.Dur = new RefData<float>(_Original.Dur.value - _Exclude.Dur.value);

        return result;
    }

    public void Set_ValueLimitRange(float _Min)
    {
        Dmg.value = Mathf.Max(_Min, Dmg.value);
        Rof.value = Mathf.Max(_Min, Rof.value);
        MovementSpeed.value = Mathf.Max(_Min, MovementSpeed.value);
        AttackSize.value = Mathf.Max(_Min, AttackSize.value);
        CC.value = Mathf.Max(_Min, CC.value);
        CD.value = Mathf.Max(_Min, CD.value);
        MuzzleSpeed.value = Mathf.Max(_Min, MuzzleSpeed.value);
        KBPower.value = Mathf.Max(_Min, KBPower.value);
        Dur.value = Mathf.Max(_Min, Dur.value);
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
            { AllyManager.stateTypeList[0], Dmg_BuffList },
            { AllyManager.stateTypeList[1], Rof_BuffList },
            { AllyManager.stateTypeList[2], MovementSpeed_BuffList },
            { AllyManager.stateTypeList[3], AttackSize_BuffList },
            { AllyManager.stateTypeList[4], CC_BuffList },
            { AllyManager.stateTypeList[5], CD_BuffList },
            { AllyManager.stateTypeList[6], MuzzleSpeed_BuffList },
            { AllyManager.stateTypeList[7], KBPower_BuffList },
            { AllyManager.stateTypeList[8], Dur_BuffList }
        };

        BuffIsOnDict = new Dictionary<string, RefData<bool>>
        {
            { AllyManager.stateTypeList[0], Dmg_IsExist },
            { AllyManager.stateTypeList[1], Rof_IsExist },
            { AllyManager.stateTypeList[2], MovementSpeed_IsExist },
            { AllyManager.stateTypeList[3], AttackSize_IsExist },
            { AllyManager.stateTypeList[4], CC_IsExist },
            { AllyManager.stateTypeList[5], CD_IsExist },
            { AllyManager.stateTypeList[6], MuzzleSpeed_IsExist },
            { AllyManager.stateTypeList[7], KBPower_IsExist },
            { AllyManager.stateTypeList[8], Dur_IsExist }
        };
    }

    public void Add_List(AllyBuff _Buff, string _Type)
    {
        DevTool.Add_InList(BuffDict[_Type], _Buff);

        if (!BuffIsOnDict[_Type].value)
            BuffIsOnDict[_Type].value = true;
    }

    public void Remove_List(AllyBuff _Buff, string _Type)
    {
        DevTool.Remove_InList(BuffDict[_Type], _Buff);

        if (BuffDict[_Type].Count <= 0)
            BuffIsOnDict[_Type].value = false;
    }

    public AllyState Get_BuffedAllyState()
    {
        return this;
    }

    public void UpdateData(float _DeltaTime)
    {
        foreach(var data in BuffDict)
            if (BuffIsOnDict[data.Key].value)
                UpdateData(data.Value);
            
        void UpdateData(List<AllyBuff> _BuffList)
        {
            for (int i = 0; i < _BuffList.Count; i++)
                _BuffList[i].Caculate_Cooltime(_DeltaTime);
        }
    }



    public void Set_BuffedAllyState()
    {
        MovementSpeed.value = Get_BuffValue(MovementSpeed_BuffList);
        Dmg.value = Get_BuffValue(Dmg_BuffList);
        Rof.value = Get_BuffValue(Rof_BuffList);
        AttackSize.value = Get_BuffValue(AttackSize_BuffList);
        CC.value = Get_BuffValue(CC_BuffList);
        CD.value = Get_BuffValue(CD_BuffList);
        MuzzleSpeed.value = Get_BuffValue(MuzzleSpeed_BuffList);
        KBPower.value = Get_BuffValue(KBPower_BuffList);
        Dur.value = Get_BuffValue(Dur_BuffList);
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
        AllyIdle = ResourceManager.instance.Get_AllySprite(_Name, "Idle");
        AllyMove = ResourceManager.instance.Get_AllySprite(_Name, "Move");
        AllyAttack = ResourceManager.instance.Get_AllySprite(_Name, "Attack");
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

    #endregion

    #region Constructor

    public AllyRequest(AllyController _Ally)
    {
        Ally = _Ally;

        Rank = Get_RandomRank();
        List<string> keyList = RewardDict.Keys.ToList();
        RewardType = keyList[UnityEngine.Random.Range(0, keyList.Count)];

        CompleteProgress = 0;
        FailProgress = 0;

        Set_IWhenAdd();

        Ally.HUD.RequestUI.Set_Request_CompleteProgress(0);
        Ally.HUD.RequestUI.Set_Request_FailProgress(0);
    }

    #endregion

    #region Get

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

    protected int Rank = 0;
    public int Get_Rank()
    {
        return Rank;
    }

    // Min: 0 <-> Max: 4
    private static List<int> RankPercent = new List<int>() { 7, 5, 3, 2, 1 };
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
        Ally.Gain_Trust(Rank + 1);
        PlayerManager.instance.playerController.Gain_Reputation((Rank + 1) * 0.2f);
        RewardDict[RewardType](Rank);
        Ally.DataOff_Request();
        Set_IWhenRemove();

        Ally = null;
        RewardType = null;
    }

    public void Fail()
    {
        Ally.Reduce_Trust(Rank + 1);
        PlayerManager.instance.playerController.Reduce_Reputation((Rank + 1) * 0.2f);
        Ally.DataOff_Request();
        Set_IWhenRemove();

        Ally = null;
        RewardType = null;
    }

    #endregion

    #region Reward

    private string RewardType = "";
    public string Get_RewardType()
    {
        return RewardType;
    }

    private static Dictionary<string, Dele_T<int>> RewardDict = new Dictionary<string, Dele_T<int>>
    {
        { "BC", new Dele_T<int>(Gain_Reward_BC) },
        { "Credit", new Dele_T<int>(Gain_Reward_Credit) },
        { "EP", new Dele_T<int>(Gain_Reward_EP) }
    };

    public static Dictionary<string, Func<int, int>> RewardCaculateDict = new Dictionary<string, Func<int, int>>
    {
        { "BC", new Func<int, int>(Get_BookReward_BC) },
        { "Credit", new Func<int, int>(Get_BookReward_Credit) },
        { "EP", new Func<int, int>(Get_BookReward_EP) }
    };

    private static void Gain_Reward_BC(int _Rank) { PlayerManager.instance.playerController.Add_CurrentBettery(Get_BookReward_BC(_Rank)); }
    private static void Gain_Reward_Credit(int _Rank) { PlayerManager.instance.playerController.Add_CurrentCredit(Get_BookReward_Credit(_Rank)); }
    private static void Gain_Reward_EP(int _Rank) { PlayerManager.instance.playerController.Add_CurrentEP(Get_BookReward_EP(_Rank)); }

    private static int Get_BookReward_BC(int _Rank) { return _Rank + 1; }
    private static int Get_BookReward_Credit(int _Rank) { return (_Rank + 1) * 3; }
    private static int Get_BookReward_EP(int _Rank) { return (_Rank + 1) * 5; }

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

        if (!Ally) return;
        Ally.HUD.RequestUI.Set_Request_CompleteTxt(CompleteProgress / MaxCompleteProgress, $"{CompleteProgress}/{MaxCompleteProgress}");
    }

    protected override void Inc_FailProgress()
    {
        base.Inc_FailProgress();

        if (!Ally) return;
        Ally.HUD.RequestUI.Set_Request_FailTxt(FailProgress / MaxFailProgress, $"{FailProgress}/{MaxFailProgress}");
    }

    #endregion

    #region Get

    public override string Get_Name() { return ResourceManager.instance.Get_RequestName(0); }
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

    public override string Get_Name() { return ResourceManager.instance.Get_RequestName(1); }
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
    public Vector2Int[] RoomVec;
    public Vector2 SpritePivot;

    public MinimapIcon(CouplePair<Sprite> _Pair, Vector2Int[] _RoomVec, Vector2 _Pivot)
    {
        MinimapElementIcon = _Pair;
        RoomVec = _RoomVec;
        SpritePivot = _Pivot;
    }
}

[System.Serializable]
public class LanguageTxt
{
    public int ID;
    public TMP_FontAsset[] FontAssets;

    public LanguageTxt(int _ID, TMP_FontAsset[] _FontAssets)
    {
        ID = _ID;
        FontAssets = _FontAssets;
    }
}

[System.Serializable]
public class SpriteTypeName
{
    public string Name;
    public Sprite Sprite;
}

#endregion


#region Class : Stage : Reso

[System.Serializable]
public class StageMapSprite
{
    [Header("=== Sprtie: Based on the outer surface")]

    public Dictionary<string, SpriteMaterial> MapSprite = new Dictionary<string, SpriteMaterial>();

    public void Offset(MapReso _Reso, string _MapIndexName)
    {
        for (int i = 0; i < _Reso.MapResoElements.Length; i++)
        {
            if (_Reso.MapResoElements[i].Sprite.name.Length > 5)
            {
                if (_Reso.MapResoElements[i].Sprite.name[5] == 'A') continue; 
                
                MapSprite.Add(
                    _Reso.MapResoElements[i].Sprite.name.Substring(5, _Reso.MapResoElements[i].Sprite.name.Length - 5),
                    new SpriteMaterial(_Reso.MapResoElements[i].Sprite, _Reso.MapResoElements[i].MaterialIndex));
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

    public ConverterReso()
    {
        ConverterResoList = new List<EachConverterReso>();
    }

    public void Add(EachConverterReso _Reso)
    {
        ConverterResoList.Add(_Reso);
    }
}

[System.Serializable]
public class EachConverterReso
{
    public AnimationClip AC;
    public Material Material;

    public EachConverterReso(AnimationClip _AC, Material _Material)
    {
        AC = _AC;
        Material = _Material;
    }
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
    public Sprite[] AllAnswerSet;
}

#endregion


#region Class : CSV : Word

public abstract class WordSet<T>
{
    protected Dictionary<int, T> AllWord;

    public WordSet(Dictionary<int, T> _AllWordData)
    {
        AllWord = _AllWordData;
    }

    public Dictionary<int, T> Get_WordData() => AllWord;
    public int Get_Amount() => AllWord.Count;

    public abstract string Get_Word(int _ID);
}


[System.Serializable]
public class WordSet_Just : WordSet<WordElement_Just>
{
    public WordSet_Just(Dictionary<int, WordElement_Just> _Dict) : base(_Dict) 
    { }

    public override string Get_Word(int _ID)
    {
        if (AllWord.ContainsKey(_ID))
            return AllWord[_ID].Words[GameManager.languageID];

        return "";
    }

    public string[] Get_Words(int _ID)
    {
        if (AllWord.ContainsKey(_ID))
            return AllWord[_ID].Words;

        return null;
    }
}

[System.Serializable]
public class WordSet_WithClr : WordSet<WordElement_WithClr>
{
    public WordSet_WithClr(Dictionary<int, WordElement_WithClr> _Dict) : base(_Dict)
    { }

    public override string Get_Word(int _ID)
    {
        if (AllWord.ContainsKey(_ID))
        {
            WordElement_WithClr data = AllWord[_ID];
            return $"<color=#{data.ClrHex}><b>\"{data.Words[GameManager.languageID]}\"</color></b>";
        }
        else
        {
            return "";
        }
    }
}



[System.Serializable]
public class WordElement_Just
{
    public int ID;
    public string[] Words;

    public WordElement_Just(int _ID, string[] _Words)
    {
        ID = _ID;
        Words = _Words;
    }
}

[System.Serializable]
public class WordElement_WithClr : WordElement_Just
{
    public string ClrHex;

    public WordElement_WithClr(int _ID, string _ClrHex, string[] _Names) : base(_ID, _Names)
    {
        ClrHex = _ClrHex;
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



#region Class : QueueSet

[System.Serializable]
public class QueueSet<T> where T : Component
{
    [SerializeField] private Transform ThisParentTF;
    [HideInInspector] protected T[] ThisArr;

    private int totalAmount = 0;
    private int currentAmount = -1;

    public virtual void Offset()
    {
        totalAmount = ThisParentTF.childCount;
        ThisArr = new T[totalAmount];
        for (int i = 0; i < totalAmount; i++)
        {
            ThisArr[i] = DevTool.Get_ComponentTType<T>(ThisParentTF.GetChild(i).gameObject);
        }
    }

    public T Get_T()
    {
        currentAmount++;
        if (currentAmount >= totalAmount)
        {
            currentAmount = 0;
        }
        return ThisArr[currentAmount];
    }

    public void Set_All(bool _OnOff)
    {
        for (int i = 0; i < totalAmount; i++)
            ThisArr[i].gameObject.SetActive(_OnOff);
    }
}

[System.Serializable]
public class ASQueueSet : QueueSet<AudioSource>
{
    public void StopAll()
    {
        for (int i = 0; i < ThisArr.Length; i++)
            ThisArr[i].Stop();
    }
}

[System.Serializable]
public class ImgQueueSet : QueueSet<Image>
{
    public override void Offset()
    {
        base.Offset();
        Set_All(false);
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
        RankIcon = ResourceManager.instance.Get_RankIcon(Rank);
    }

    public ItemData_UIVisual(ItemData _ItemData)
    {
        Icon = _ItemData.itemIcon;
        Rank = _ItemData.rank;
        RankIcon = ResourceManager.instance.Get_RankIcon(Rank);
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
    public string Get_WhenDesc() { return ResourceManager.instance.Get_RequestCompleteDesc(0); }
    public void Add_IWhenList() { AllyRequestManager.instance.Add_RequestComplete("KillNormalEnemy", this); }
    public void Remove_IWhenList() { AllyRequestManager.instance.Remove_RequestComplete("KillNormalEnemy", this); }
}
public interface IWhen_Complete_KillEliteEnemy : IWhen_Request
{
    public string Get_WhenDesc() { return ResourceManager.instance.Get_RequestCompleteDesc(1); }
    public void Add_IWhenList() { AllyRequestManager.instance.Add_RequestComplete("KillEliteEnemy", this); }
    public void Remove_IWhenList() { AllyRequestManager.instance.Remove_RequestComplete("KillEliteEnemy", this); }
}


// Fail
public interface IWhen_Fail
{
    public abstract void Play_When_Fail();
}

public interface IWhen_Fail_TakingDamage : IWhen_Fail 
{
    public string Get_WhenDesc() { return ResourceManager.instance.Get_RequestFailDesc(0); }
    public void Add_IWhenList() { AllyRequestManager.instance.Add_RequestFail("TakingDamage", this); }
    public void Remove_IWhenList() { AllyRequestManager.instance.Remove_RequestFail("TakingDamage", this); }
}
public interface IWhen_Fail_UsingSkill : IWhen_Fail
{
    public string Get_WhenDesc() { return ResourceManager.instance.Get_RequestFailDesc(1); }
    public void Add_IWhenList() { AllyRequestManager.instance.Add_RequestFail("UsingSkill", this); }
    public void Remove_IWhenList() { AllyRequestManager.instance.Remove_RequestFail("UsingSkill", this); }
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
    Completed, KillAll, Survived, Safe, Prison
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
    BasePanel, OptionPanel, StatePanel, InfoPanel
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