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
        Application.targetFrameRate = -1/*fps*/;
    }

    #endregion
}

#region ========== DEV TOOL

public class DevTool
{
    #region About Math

    #region Get

    // 퍼센트값을 도출
    public static float GetPercent(float percent, float value)
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
        tf.position = structTF2D.pos;
        tf.rotation = structTF2D.rot;
        tf.localScale = structTF2D.localScale;
    }

    public static void Set_MatAndClr_FromStruct(SpriteRenderer sr, State_Sprite spriteExtra)
    {
        sr.material = spriteExtra.material;
        sr.color = spriteExtra.clr;
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

    public static bool Add_InList<T>(List<T> targetList, T targetValue, out int index)
    {
        index = -1;
        if (!targetList.Contains(targetValue))
        {
            targetList.Add(targetValue);
            index = targetList.Count - 1;
            return true;
        }
        return false;
    }

    #endregion

    #region Insert

    public static bool Insert_InList<T>(List<T> targetList, T targetValue, int index)
    {
        if (!targetList.Contains(targetValue))
        {
            targetList.Insert(index, targetValue);
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

    #region About Algorism


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
            DevTool.Set_AnimSpeedAndSize(targetList[i].comp, speed, animSize: 1);
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

    public static void SetKillTween<T>(T _Comp)
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
            (enemyBuff.coldStack.currentStack * (enemyBuff.absoluteZeroStack.currentStack + 1) * 0.01f));
    }

    // 부식 데미지 증가 계산 (효과)
    public static float Get_DmgEffectByCorrosion(float baseDmg, EnemyBuffController enemyBuff)
    {
        return baseDmg *= (1f + 
            (enemyBuff.corrosionStack.currentStack * (enemyBuff.decayStack.currentStack + 1) * 0.01f));
    }


    // 화염
    public static float Get_FrameDmg(EnemyBuffController buff)
    {
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
            * 0.01f
            * buff.flameStack.currentStack
            * (buff.infernoStack.currentStack + 1);
    }
    public static float Get_FlameExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Physics;
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
            * 10f;
    }


    // 냉기
    public static float Get_ColdExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Energy;
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
            * 7.5f;
    }


    // 전기
    public static float Get_ElectricityDmg(EnemyBuffController buff)
    {
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
            * 0.005f
            * buff.electricityStack.currentStack
            * (buff.plasmaStack.currentStack + 1);
    }
    public static float Get_ElectricityExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Energy;
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
            * 7.5f;
    }

    // 부식
    public static float Get_CorrosionExplDmg(out eDamageType dmgType)
    {
        dmgType = eDamageType.Physics;
        return PlayerManager.instance.playerController.baseWeapon.baseDamage.buffedState
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

    public static void SetColorImgs(Color clr, Image[] imgs)
    {
        for (int i = 0; i < imgs.Length; i++)
            imgs[i].color = new Color(clr.r, clr.g, clr.b, imgs[i].color.a);
    }

    public static void SetColorImgs(Color clr, List<Image> imgs)
    {
        for (int i = 0; i < imgs.Count; i++)
            imgs[i].color = new Color(clr.r, clr.g, clr.b, imgs[i].color.a);
    }

    public static void SetColorTmps(Color clr, TMP_Text[] tmps)
    {
        for (int i = 0; i < tmps.Length; i++)
            tmps[i].color = new Color(clr.r, clr.g, clr.b, tmps[i].color.a);
    }

    public static void SetColorTmps(Color clr, List<TMP_Text> tmps)
    {
        for (int i = 0; i < tmps.Count; i++)
            tmps[i].color = new Color(clr.r, clr.g, clr.b, tmps[i].color.a);
    }







    public static void Set_Color<T>(Color clr, List<T> targetList) where T : Component
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            switch (targetList[i]) 
            {
                case TMP_Text tmp:
                    SetColor(clr, tmp);
                    break;

                case Image img:
                    SetColor(clr, img);
                    break;


                default:
                    break;
            }
        }
    }


    public static void SetColor(Color clr, TMP_Text comp)
    {
        if (comp == null) return;
        comp.color = new Color(clr.r, clr.g, clr.b, comp.color.a);
    }

    public static void SetColor(Color clr, Image comp)
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

[System.Serializable]
public class SerializableArray<T>
{
    public T[] array;
}



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

    public bool Is_Full(float dt)
    {
        current += dt;

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

    [HideInInspector] public TxtAmountForBuyEUIController upgradeEUI;

    [HideInInspector] public BUState<T> state;
    [HideInInspector] private BULevelData<T> levelData;

    [HideInInspector] public string name;
    [HideInInspector] public string desc;

    #endregion

    #region Offset

    public void Offset(
        BUState<T> state,
        BULevelData<T> levelData,
        List<BUShopData<T>> allList,
        BaseUpgradeUIController owner,
        TxtAmountForBuyEUIController eui, int index, Sprite icon)
    {
        upgradeEUI = eui;
        upgradeEUI.Offset(owner, icon);
        upgradeEUI.rt.anchoredPosition = new Vector2(0, index * -250);

        this.state = state;
        this.levelData = levelData;


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
        return PlayerManager.instance.playerController.IsEnoughChargedBettery(levelData.levelDataList[state.currentLevel.Value].needEC_ForUpgrade) &&
            BaseUpgradeController.usingShop != null &&
            BaseUpgradeController.usingShop.currentDur > 0;
    }

    public bool Try_Buy()
    {
        if (Can_Buy())
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

            // Dur
            BaseUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

            // Cost
            PlayerManager.instance.playerController.UseChargedBettery(levelData.levelDataList[state.currentLevel.Value].needEC_ForUpgrade);

            Set_LevelUp();

            return true;
        }

        return false;
    }

    private void Set_LevelUp()
    {
        // Lv Up
        state.currentLevel.Value++;
        state.actualState = levelData.levelDataList[state.currentLevel.Value - 1].upgradeValue;
        state.Set_BuffedState();

        // Can Lv Up
        upgradeEUI.buyBtn.btn.interactable = DevTool.buMaxLevel <= state.currentLevel.Value ? false : true;

        // Desc
        MainGameUIManager.instance.buUi.SetOn_Desc(upgradeEUI, upgradeEUI.skillNameTxt.text);
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
    [SerializeField] public T actualState;
    

    #endregion

    #region - Hide

    [SerializeField] public ReactiveProperty<int> currentLevel = new();
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
        actualState = baseState;
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
        if (actualState.GetType() == typeof(float))
        {
            Set_BufftedState_Float();
        }
        else if (actualState.GetType() == typeof(int))
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
        state *= float.Parse(actualState.ToString());
        buffedState = (T)(object)state;
    }

    private void Set_BufftedState_Int()
    {
        int state = 0;
        for (int i = 0; i < buffList.Count; i++)
        {
            state += int.Parse(buffList[i].actualValue.ToString());
        }
        state += int.Parse(actualState.ToString());
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

        AllyBuff buff = ally.buffController.Get_AllyBuff("Sync005");

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

        AllyBuff buff = ally.buffController.Get_AllyBuff("Sync006");

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

        AllyBuff buff = ally.buffController.Get_AllyBuff("Sync007");

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

        AllyBuff buff = ally.buffController.Get_AllyBuff("Sync008");

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

        ally.buffController.buffingState.Add_List(this, type);
        Set_OnOff(true);
        Set_AlwaysShowUI(showAlwaysOnOff); 

        ally.buffController.buffingState.Set_BuffedAllyState();
        ally.Set_AllState();
    }

    public void SetOff_State()
    {
        buffAmount = 0;
        currentCooltime = 0;
        actualBuffValue.value = 0;

        ally.buffController.buffingState.Remove_List(this, type);
        Set_OnOff(false);
        Set_AlwaysShowUI(false); 

        ally.buffController.buffingState.Set_BuffedAllyState();
        ally.Set_AllState();
    }

    private void Set_OnOff(bool onOff)
    {
        if (isOn == onOff) return;
        
        isOn = onOff;

        if (!isOn && buffIconUI != null) // 꺼짐
        {
            ally.hud.temporaryBuffUi.Remove_BuffIconUI(buffIconUI);
            buffIconUI = null;
        }
    }

    private void Set_AlwaysShowUI(bool onOff)
    {
        if (isAlwaysShowUI == onOff) return;

        isAlwaysShowUI = onOff;

        if (isAlwaysShowUI) // 항상 켜짐
        {
            buffIconUI = ally.hud.temporaryBuffUi.Get_BuffIconUI();
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
        ally.buffController.buffingState.Add_List(this, type);
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
            buffIconUI = ally.hud.temporaryBuffUi.Get_BuffIconUI();
        }

        // value
        buffAmount = Mathf.Min(buffAmount + stack, buffMaxAmount);
        Set_Value();
        ally.buffController.buffingState.Set_BuffedAllyState();
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
        ally.buffController.buffingState.Set_BuffedAllyState();
        ally.Set_AllState();

        if (isIncrease) currentCooltime = 0;

        // ui
        buffIconUI.SetOn(iconSprite, isAlwaysShowUI ? true : !(buffAmount == 0));

        // ui last
        if (buffAmount == 0 && !isAlwaysShowUI && buffIconUI != null)
        {
            ally.hud.temporaryBuffUi.Remove_BuffIconUI(buffIconUI);
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
            buffIconUI = enemy.hud.temporaryBuffUi.Get_BuffIconUI();
            buffIconUI.SetOn(iconSprite, showTxt);
        }
        isOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (buffIconUI != null)
        {
            enemy.hud.temporaryBuffUi.Remove_BuffIconUI(buffIconUI);
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
            buffIconUI = enemy.hud.permanentBuffUi.Get_BuffIconUI();
            buffIconUI.SetOn(iconSprite, showTxt);
        }
        isOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (buffIconUI != null)
        {
            enemy.hud.permanentBuffUi.Remove_BuffIconUI(buffIconUI);
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
    public int thisPhase;
    public float thisPhaseLimitPercentHP;
    public List<OrderOfPriorityEnemyPattern> orderOfPriorityEnemyPatternList;
    public ContinuousEnemyPattern specialPattern;

}

#endregion

#region Class : Enemy : DropItem

[System.Serializable]
public class EnemyDropItemPercent
{
    [Space(5)]
    [Header("-- Module")]
    [SerializeField] public float moduleDropPercent = 0.0f;
    [SerializeField] public List<int> moduleRankPercents;

    [Space(5)]
    [Header("-- Keycard")]
    [SerializeField] public float keycardDropPercent = 0.0f;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] public CoupleData<int> bsAmountMinMax;
    [SerializeField] public CoupleData<int> msAmountMinMax;
    [SerializeField] public CoupleData<int> creditAmountMinMax;
    [SerializeField] public CoupleData<int> overriderAmountMinMax;
    [SerializeField] public CoupleData<float> jouleAmountMinMax;
}

[System.Serializable]
public class CoreDropItemPercent
{
    [Space(5)]
    [Header("-- Core")]
    [SerializeField] public int coreItemID = 0;
    [SerializeField] public float coreItemPercent = 0.0f;
}

#endregion



#region Class : Satellite

[System.Serializable]
public abstract class SatelliteController
{
    #region Value

    [Space(5)]
    [Header("<><><><><> Satellite")]

    [SerializeField] public Transform target;
    [SerializeField] public DepthController follower;

    [SerializeField] public int upperOrder;

    #endregion

    #region Func

    public void Set_Pos()
    {
        follower.transform.position = target.position;
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

    [SerializeField] public int farFromCenter;

    #endregion

    #region Func

    public override void Set_SortingOrder()
    {
        follower.SetSortingOrder(upperOrder + 
            (Is_LocalUpper(follower.transform) ? -farFromCenter : farFromCenter));
    }

    private bool Is_LocalUpper(Transform targetTF)
    {
        return targetTF.localPosition.y > 0;
    }

    #endregion
}

[System.Serializable]
public class SatelliteCenterController : SatelliteController
{
    #region Func

    public override void Set_SortingOrder()
    {
        follower.SetSortingOrder(upperOrder);
    }

    #endregion
}

#endregion

#region Class : Map Reso


[System.Serializable]
public class MapResoElement
{
    public Sprite sprite;
    public int materialIndex;

    public MapResoElement(Sprite sprite, int materialIndex)
    {
        this.sprite = sprite;
        this.materialIndex = materialIndex;
    }
}


#endregion


#region Class : AllyUpgrade : Card

[System.Serializable]
public class AllyCardBaseData
{
    public int id;
    public int rank;
    public int essentialId;

    public AllyCardBaseData(int id, int rank, int essentialID)
    {
        this.id = id;
        this.rank = rank;
        essentialId = essentialID;
    }
}

[System.Serializable]
public class AllyCardData
{
    public int id;
    public int rank;
    public int essentialID;

    public string name;
    public string desc;

    public AllyCardData(AllyCardBaseData baseData, string name, string desc)
    {
        id = baseData.id;
        rank = baseData.rank;
        essentialID = baseData.essentialId;

        Set_LanguageTxt(name, desc);
    }

    public void Set_LanguageTxt(string name, string desc)
    {
        this.name = name;
        this.desc = desc;
    }
}

#endregion

#region Class : AllyUpgrade : Base(Tuner)

[System.Serializable]
public class AllyBaseTunerData
{
    public AllyEachBaseTunerData positive0;
    public AllyEachBaseTunerData positive1;
    public AllyEachBaseTunerData negative;

    public AllyBaseTunerData(AllyTunerData data)
    {
        positive0 = new AllyEachBaseTunerData(data.positive0);
        positive1 = new AllyEachBaseTunerData(data.positive1);
        negative = new AllyEachBaseTunerData(data.negative);
    }
}

[System.Serializable]
public class AllyEachBaseTunerData
{
    public string type = "";
    public int rank = 0;

    public AllyEachBaseTunerData(AllyEachTunerData _Data)
    {
        type = _Data.type;
        rank = _Data.rank;
    }
}


[System.Serializable]
public class AllyBaseUpradeTunerSet
{
    public List<AllyTunerData> allyTunerDataList;

    public void Offset(int amount, List<string> typeList, List<int> rankPercent)
    {
        allyTunerDataList = new List<AllyTunerData>();
        for (int i = 0; i < amount; i++)
        {
            allyTunerDataList.Add(new AllyTunerData(typeList, rankPercent));
        }
    }
}

[System.Serializable]
public class AllyTunerData
{
    public AllyEachTunerData positive0;
    public AllyEachTunerData positive1;
    public AllyEachTunerData negative;

    public int needPay = 0;

    public AllyTunerData(List<string> typeList, List<int> rankPercent)
    {
        Set_Data(typeList, rankPercent);
    }

    public void Set_Data(List<string> typeList, List<int> rankPercent)
    {
        positive0 = new AllyEachTunerData(typeList, rankPercent);
        positive1 = new AllyEachTunerData(typeList, rankPercent);
        negative = new AllyEachTunerData(typeList, rankPercent);

        needPay = (int)((positive0.rank + positive1.rank - negative.rank + 5) * 0.5f);
    }
}

[System.Serializable]
public class AllyEachTunerData
{
    public string type = "";
    public int rank = 0;

    public AllyEachTunerData(List<string> typeList, List<int> rankPercent)
    {
        Set_Data(typeList, rankPercent);
    }

    public void Set_Data(List<string> typeList, List<int> rankPercent)
    {
        type = typeList[UnityEngine.Random.Range(0, typeList.Count)];
        rank = DevTool.Get_Rank(rankPercent);
    }
}

#endregion


#region Class : Ally State


[System.Serializable]
public class AllyState
{
    #region Value

    public RefData<float> dmg;
    public RefData<float> rof;
    public RefData<float> movementSpeed;
    public RefData<float> attackSize;
    public RefData<float> criticalChacne;
    public RefData<float> criticalDmg;
    public RefData<float> muzzleSpeed;
    public RefData<float> kbPower;
    public RefData<float> dur;

    #endregion

    #region Constructor

    public AllyState()
    {
        dmg = new RefData<float>(1);
        rof = new RefData<float>(1);
        movementSpeed = new RefData<float>(1);
        attackSize = new RefData<float>(1);
        criticalChacne = new RefData<float>(1);
        criticalDmg = new RefData<float>(1);
        muzzleSpeed = new RefData<float>(1);
        kbPower = new RefData<float>(1);
        dur = new RefData<float>(1);
    }

    public AllyState(AllyState stateValue)
    {
        dmg = new RefData<float>(stateValue.dmg.value);
        rof = new RefData<float>(stateValue.rof.value);
        movementSpeed = new RefData<float>(stateValue.movementSpeed.value);
        attackSize = new RefData<float>(stateValue.attackSize.value);
        criticalChacne = new RefData<float>(stateValue.criticalChacne.value);
        criticalDmg = new RefData<float>(stateValue.criticalDmg.value);
        muzzleSpeed = new RefData<float>(stateValue.muzzleSpeed.value);
        kbPower = new RefData<float>(stateValue.kbPower.value);
        dur = new RefData<float>(stateValue.dur.value);
    }

    public void Reset()
    {
        dmg.value = 1;
        rof.value = 1;
        movementSpeed.value = 1;
        attackSize.value = 1;
        criticalChacne.value = 1;
        criticalDmg.value = 1;
        muzzleSpeed.value = 1;
        kbPower.value = 1;
        dur.value = 1;
    }

    public static AllyState Get_Multiple(AllyState state0, AllyState state1)
    {
        AllyState result = new AllyState();

        result.dmg = new RefData<float>(state0.dmg.value * state1.dmg.value);
        result.rof = new RefData<float>(state0.rof.value * state1.rof.value);
        result.movementSpeed = new RefData<float>(state0.movementSpeed.value * state1.movementSpeed.value);
        result.attackSize = new RefData<float>(state0.attackSize.value * state1.attackSize.value);
        result.criticalChacne = new RefData<float>(state0.criticalChacne.value * state1.criticalChacne.value);
        result.criticalDmg = new RefData<float>(state0.criticalDmg.value * state1.criticalDmg.value);
        result.muzzleSpeed = new RefData<float>(state0.muzzleSpeed.value * state1.muzzleSpeed.value);
        result.kbPower = new RefData<float>(state0.kbPower.value * state1.kbPower.value);
        result.dur = new RefData<float>(state0.dur.value * state1.dur.value);

        return result;
    }

    public static AllyState Get_Subtraction(AllyState original, AllyState exclude)
    {
        AllyState result = new AllyState();

        result.dmg = new RefData<float>(original.dmg.value - exclude.dmg.value);
        result.rof = new RefData<float>(original.rof.value - exclude.rof.value);
        result.movementSpeed = new RefData<float>(original.movementSpeed.value - exclude.movementSpeed.value);
        result.attackSize = new RefData<float>(original.attackSize.value - exclude.attackSize.value);
        result.criticalChacne = new RefData<float>(original.criticalChacne.value - exclude.criticalChacne.value);
        result.criticalDmg = new RefData<float>(original.criticalDmg.value - exclude.criticalDmg.value);
        result.muzzleSpeed = new RefData<float>(original.muzzleSpeed.value - exclude.muzzleSpeed.value);
        result.kbPower = new RefData<float>(original.kbPower.value - exclude.kbPower.value);
        result.dur = new RefData<float>(original.dur.value - exclude.dur.value);

        return result;
    }

    public void Set_ValueLimitRange(float min)
    {
        dmg.value = Mathf.Max(min, dmg.value);
        rof.value = Mathf.Max(min, rof.value);
        movementSpeed.value = Mathf.Max(min, movementSpeed.value);
        attackSize.value = Mathf.Max(min, attackSize.value);
        criticalChacne.value = Mathf.Max(min, criticalChacne.value);
        criticalDmg.value = Mathf.Max(min, criticalDmg.value);
        muzzleSpeed.value = Mathf.Max(min, muzzleSpeed.value);
        kbPower.value = Mathf.Max(min, kbPower.value);
        dur.value = Mathf.Max(min, dur.value);
    }

    #endregion
}

[System.Serializable]
public class AllyBuffState : AllyState
{
    [HideInInspector] public List<AllyBuff> dmg_BuffList;
    [HideInInspector] public List<AllyBuff> rof_BuffList;
    [HideInInspector] public List<AllyBuff> movementSpeed_BuffList;
    [HideInInspector] public List<AllyBuff> attackSize_BuffList;
    [HideInInspector] public List<AllyBuff> cc_BuffList;
    [HideInInspector] public List<AllyBuff> cd_BuffList;
    [HideInInspector] public List<AllyBuff> muzzleSpeed_BuffList;
    [HideInInspector] public List<AllyBuff> kbPower_BuffList;
    [HideInInspector] public List<AllyBuff> dur_BuffList;

    [HideInInspector] private RefData<bool> dmg_IsExist;
    [HideInInspector] private RefData<bool> rof_IsExist;
    [HideInInspector] private RefData<bool> movementSpeed_IsExist;
    [HideInInspector] private RefData<bool> attackSize_IsExist;
    [HideInInspector] private RefData<bool> cc_IsExist;
    [HideInInspector] private RefData<bool> cd_IsExist;
    [HideInInspector] private RefData<bool> muzzleSpeed_IsExist;
    [HideInInspector] private RefData<bool> kbPower_IsExist;
    [HideInInspector] private RefData<bool> dur_IsExist;

    [HideInInspector] Dictionary<string, List<AllyBuff>> buffDict;
    [HideInInspector] Dictionary<string, RefData<bool>> buffIsOnDict;

    public AllyBuffState() : base()
    {
        dmg_BuffList = new List<AllyBuff>();
        rof_BuffList = new List<AllyBuff>();
        movementSpeed_BuffList = new List<AllyBuff>();
        attackSize_BuffList = new List<AllyBuff>();
        cc_BuffList = new List<AllyBuff>();
        cd_BuffList = new List<AllyBuff>();
        muzzleSpeed_BuffList = new List<AllyBuff>();
        kbPower_BuffList = new List<AllyBuff>();
        dur_BuffList = new List<AllyBuff>();

        dmg_IsExist = new RefData<bool>(false);
        rof_IsExist = new RefData<bool>(false);
        movementSpeed_IsExist = new RefData<bool>(false);
        attackSize_IsExist = new RefData<bool>(false);
        cc_IsExist = new RefData<bool>(false);
        cd_IsExist = new RefData<bool>(false);
        muzzleSpeed_IsExist = new RefData<bool>(false);
        kbPower_IsExist = new RefData<bool>(false);
        dur_IsExist = new RefData<bool>(false);

        buffDict = new Dictionary<string, List<AllyBuff>>
        {
            { AllyManager.stateTypeList[0], dmg_BuffList },
            { AllyManager.stateTypeList[1], rof_BuffList },
            { AllyManager.stateTypeList[2], movementSpeed_BuffList },
            { AllyManager.stateTypeList[3], attackSize_BuffList },
            { AllyManager.stateTypeList[4], cc_BuffList },
            { AllyManager.stateTypeList[5], cd_BuffList },
            { AllyManager.stateTypeList[6], muzzleSpeed_BuffList },
            { AllyManager.stateTypeList[7], kbPower_BuffList },
            { AllyManager.stateTypeList[8], dur_BuffList }
        };

        buffIsOnDict = new Dictionary<string, RefData<bool>>
        {
            { AllyManager.stateTypeList[0], dmg_IsExist },
            { AllyManager.stateTypeList[1], rof_IsExist },
            { AllyManager.stateTypeList[2], movementSpeed_IsExist },
            { AllyManager.stateTypeList[3], attackSize_IsExist },
            { AllyManager.stateTypeList[4], cc_IsExist },
            { AllyManager.stateTypeList[5], cd_IsExist },
            { AllyManager.stateTypeList[6], muzzleSpeed_IsExist },
            { AllyManager.stateTypeList[7], kbPower_IsExist },
            { AllyManager.stateTypeList[8], dur_IsExist }
        };
    }

    public void Add_List(AllyBuff buff, string type)
    {
        DevTool.Add_InList(buffDict[type], buff);

        if (!buffIsOnDict[type].value)
            buffIsOnDict[type].value = true;
    }

    public void Remove_List(AllyBuff buff, string type)
    {
        DevTool.Remove_InList(buffDict[type], buff);

        if (buffDict[type].Count <= 0)
            buffIsOnDict[type].value = false;
    }

    public AllyState Get_BuffedAllyState()
    {
        return this;
    }

    public void UpdateData(float deltaTime)
    {
        foreach(var data in buffDict)
            if (buffIsOnDict[data.Key].value)
                UpdateData(data.Value);
            
        void UpdateData(List<AllyBuff> buffList)
        {
            for (int i = 0; i < buffList.Count; i++)
                buffList[i].Caculate_Cooltime(deltaTime);
        }
    }



    public void Set_BuffedAllyState()
    {
        movementSpeed.value = Get_BuffValue(movementSpeed_BuffList);
        dmg.value = Get_BuffValue(dmg_BuffList);
        rof.value = Get_BuffValue(rof_BuffList);
        attackSize.value = Get_BuffValue(attackSize_BuffList);
        criticalChacne.value = Get_BuffValue(cc_BuffList);
        criticalDmg.value = Get_BuffValue(cd_BuffList);
        muzzleSpeed.value = Get_BuffValue(muzzleSpeed_BuffList);
        kbPower.value = Get_BuffValue(kbPower_BuffList);
        dur.value = Get_BuffValue(dur_BuffList);
    }

    private float Get_BuffValue(List<AllyBuff> buffList)
    {
        float result = 1;
        for (int i = 0; i < buffList.Count; i++)
        {
            result += buffList[i].Get_Value();
        }
        return result;
    }
}


#endregion


#region Class : Ally Sprite

[System.Serializable]
public class AllySpriteSet
{
    public List<Sprite> allyIdle;
    public List<Sprite> allyMove;
    public List<Sprite> allyAttack;

    public AllySpriteSet(string name)
    {
        allyIdle = ResourceManager.instance.Get_AllySprite(name, "Idle");
        allyMove = ResourceManager.instance.Get_AllySprite(name, "Move");
        allyAttack = ResourceManager.instance.Get_AllySprite(name, "Attack");
    }
}

#endregion


#region Class : Ally Request

public class AllyCompleteList<T> where T : IWhen_Request
{
    public List<T> list = new List<T>();

    public void Play_Request()
    {
        if (list.Count <= 0) return;

        for (int i = 0; i < list.Count; i++)
            list[i].Play_When_Request();
    }

}

public class AllyFailList<T> where T : IWhen_Fail
{
    public List<T> list = new List<T>();

    // KillEnemy
    public void Start_Fail(T request)
    {
        list.Add(request);
    }

    public void Play_Request()
    {
        if (list.Count <= 0) return;

        for (int i = 0; i < list.Count; i++)
            list[i].Play_When_Fail();
    }
}


public abstract class AllyRequest
{
    #region Variable

    protected AllyController ally;

    protected float completeProgress = 0;
    protected float maxCompleteProgress = 0;
    protected float gainCompleteOnceProgress = 0;

    protected float failProgress = 0;
    protected float maxFailProgress = 0;
    protected float gainFailOnceProgress = 0;

    #endregion

    #region Constructor

    public AllyRequest(AllyController ally)
    {
        this.ally = ally;

        rank = Get_RandomRank();
        List<string> keyList = rewardDict.Keys.ToList();
        rewardType = keyList[UnityEngine.Random.Range(0, keyList.Count)];

        completeProgress = 0;
        failProgress = 0;

        Set_IWhenAdd();

        this.ally.hud.requestUi.Set_Request_CompleteProgress(0);
        this.ally.hud.requestUi.Set_Request_FailProgress(0);
    }

    #endregion

    #region Get

    #endregion

    #region Extra Constructor Func (Static)

    private static List<Func<AllyController, AllyRequest>> requestTypeList = new()
    {
        Get_AllyRequestType_000,
        Get_AllyRequestType_001
    };

    private static AllyRequest Get_AllyRequestType_000(AllyController ally) => new AllyRequest_Slayer(ally);
    private static AllyRequest Get_AllyRequestType_001(AllyController ally) => new AllyRequest_BountyHunter(ally);


    public static AllyRequest Get_AllyRequestType(AllyController ally)
    {
        return requestTypeList[UnityEngine.Random.Range(0, requestTypeList.Count)](ally);
    }


    #endregion

    #region Rank

    protected int rank = 0;
    public int Get_Rank()
    {
        return rank;
    }

    // Min: 0 <-> Max: 4
    private static List<int> rankPercent = new List<int>() { 7, 5, 3, 2, 1 };
    public static int Get_RandomRank()
    {
        return DevTool.Get_Grade(rankPercent);
    }

    #endregion

    #region Progress

    protected virtual void Inc_CompleteProgress()
    {
        completeProgress = Mathf.Min(completeProgress + gainCompleteOnceProgress, maxCompleteProgress);
        ally.hud.requestUi.Set_Request_CompleteProgress(completeProgress / maxCompleteProgress);

        if (completeProgress >= maxCompleteProgress) Complete();
    }

    protected virtual void Inc_FailProgress()
    {
        failProgress = Mathf.Min(failProgress + gainFailOnceProgress, maxFailProgress);
        ally.hud.requestUi.Set_Request_FailProgress(failProgress / maxFailProgress);

        if (failProgress >= maxFailProgress) Fail();
    }

    #endregion

    #region Result

    public void Complete()
    {
        ally.Gain_Trust(rank + 1);
        PlayerManager.instance.playerController.GainReputation((rank + 1) * 0.2f);
        rewardDict[rewardType](rank);
        ally.DataOff_Request();
        Set_IWhenRemove();

        ally = null;
        rewardType = null;
    }

    public void Fail()
    {
        ally.Reduce_Trust(rank + 1);
        PlayerManager.instance.playerController.ReduceReputation((rank + 1) * 0.2f);
        ally.DataOff_Request();
        Set_IWhenRemove();

        ally = null;
        rewardType = null;
    }

    #endregion

    #region Reward

    private string rewardType = "";
    public string Get_RewardType()
    {
        return rewardType;
    }

    private static Dictionary<string, Dele_T<int>> rewardDict = new Dictionary<string, Dele_T<int>>
    {
        { "BC", new Dele_T<int>(Gain_Reward_BC) },
        { "Credit", new Dele_T<int>(Gain_Reward_Credit) },
        { "EP", new Dele_T<int>(Gain_Reward_EP) }
    };

    public static Dictionary<string, Func<int, int>> rewardCaculateDict = new Dictionary<string, Func<int, int>>
    {
        { "BC", new Func<int, int>(Get_BookReward_BC) },
        { "Credit", new Func<int, int>(Get_BookReward_Credit) },
        { "EP", new Func<int, int>(Get_BookReward_EP) }
    };

    private static void Gain_Reward_BC(int rank) { PlayerManager.instance.playerController.GainEmptyBettery(Get_BookReward_BC(rank)); }
    private static void Gain_Reward_Credit(int rank) { PlayerManager.instance.playerController.GainCredit(Get_BookReward_Credit(rank)); }
    private static void Gain_Reward_EP(int rank) { PlayerManager.instance.playerController.AddCurrentEp(Get_BookReward_EP(rank)); }

    private static int Get_BookReward_BC(int rank) { return rank + 1; }
    private static int Get_BookReward_Credit(int rank) { return (rank + 1) * 3; }
    private static int Get_BookReward_EP(int rank) { return (rank + 1) * 5; }

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
        maxCompleteProgress = (rank + 1) * 5; // 총 처치 수
        gainCompleteOnceProgress = 1;

        maxFailProgress = 6 - rank; // 실패 피격 수
        gainFailOnceProgress = 1; 
        
        ally.hud.requestUi.Set_Request_CompleteTxt(0, $"{completeProgress}/{maxCompleteProgress}");
        ally.hud.requestUi.Set_Request_FailTxt(0, $"{failProgress}/{maxFailProgress}");
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

        if (!ally) return;
        ally.hud.requestUi.Set_Request_CompleteTxt(completeProgress / maxCompleteProgress, $"{completeProgress}/{maxCompleteProgress}");
    }

    protected override void Inc_FailProgress()
    {
        base.Inc_FailProgress();

        if (!ally) return;
        ally.hud.requestUi.Set_Request_FailTxt(failProgress / maxFailProgress, $"{failProgress}/{maxFailProgress}");
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

    public AllyRequest_BountyHunter(AllyController ally) : base(ally)
    {
        maxCompleteProgress = (rank + 1); // 총 처치 수
        gainCompleteOnceProgress = 1;

        maxFailProgress = 8 - rank; // 실패 피격 수
        gainFailOnceProgress = 1;

        base.ally.hud.requestUi.Set_Request_CompleteTxt(0, $"{completeProgress}/{maxCompleteProgress}");
        base.ally.hud.requestUi.Set_Request_FailTxt(0, $"{failProgress}/{maxFailProgress}");
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
        ally.hud.requestUi.Set_Request_CompleteTxt(completeProgress / maxCompleteProgress, $"{completeProgress}/{maxCompleteProgress}");

        base.Inc_CompleteProgress();
    }

    protected override void Inc_FailProgress()
    {
        ally.hud.requestUi.Set_Request_FailTxt(failProgress / maxFailProgress, $"{failProgress}/{maxFailProgress}");

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
    public CouplePair<Sprite> minimapElementIcon;
    public Vector2Int[] roomVec;
    public Vector2 spritePivot;

    public MinimapIcon(CouplePair<Sprite> pair, Vector2Int[] roomVec, Vector2 pivot)
    {
        minimapElementIcon = pair;
        this.roomVec = roomVec;
        spritePivot = pivot;
    }
}

[System.Serializable]
public class LanguageTxt
{
    public int id;
    public TMP_FontAsset[] fontAssets;

    public LanguageTxt(int id, TMP_FontAsset[] fontAssets)
    {
        this.id = id;
        this.fontAssets = fontAssets;
    }
}

[System.Serializable]
public class SpriteTypeName
{
    public string name;
    public Sprite sprite;
}

#endregion


#region Class : Visual

[System.Serializable]
public class PlayerVisual<T>
{
    [SerializeField] public CoupleData<T> physics;
    [SerializeField] public CoupleData<T> energy;

    public CoupleData<T> Get_CorrectType(eDamageType dmgType)
    {
        if (dmgType == eDamageType.Physics)
        {
            return physics;
        }
        else
        {
            return energy;
        }
    }
}

#endregion


#region Class : Build : Converter

[System.Serializable]
public class ConverterReso
{
    public List<EachConverterReso> converterResoList;

    public ConverterReso()
    {
        converterResoList = new List<EachConverterReso>();
    }

    public void Add(EachConverterReso reso)
    {
        converterResoList.Add(reso);
    }
}

[System.Serializable]
public class EachConverterReso
{
    public AnimationClip ac;
    public Material material;

    public EachConverterReso(AnimationClip ac, Material material)
    {
        this.ac = ac;
        this.material = material;
    }
}

#endregion


#region Class : Ally : Prison

[System.Serializable]
public class PrisonAllySprite
{
    public Sprite bind;
    public Sprite fall;
    public Sprite stand;
    public Sprite salute;
}

#endregion


#region Class : Puzzle : NSC

[System.Serializable]
public class NSCAnswerSpriteSet
{
    public int shapeIndex;
    public Sprite[] allAnswerSet;
}

#endregion


#region Class : CSV : Word

public abstract class WordSet<T>
{
    protected Dictionary<int, T> allWord;

    public WordSet(Dictionary<int, T> allWordData)
    {
        allWord = allWordData;
    }

    public Dictionary<int, T> Get_WordData() => allWord;
    public int Get_Amount() => allWord.Count;

    public abstract string Get_Word(int _ID);
}


[System.Serializable]
public class WordSet_Just : WordSet<WordElement_Just>
{
    public WordSet_Just(Dictionary<int, WordElement_Just> dict) : base(dict) 
    { }

    public override string Get_Word(int id)
    {
        if (allWord.ContainsKey(id))
            return allWord[id].words[GameManager.languageID];

        return "";
    }

    public string[] Get_Words(int id)
    {
        if (allWord.ContainsKey(id))
            return allWord[id].words;

        return null;
    }
}

[System.Serializable]
public class WordSet_WithClr : WordSet<WordElement_WithClr>
{
    public WordSet_WithClr(Dictionary<int, WordElement_WithClr> dict) : base(dict)
    { }

    public override string Get_Word(int id)
    {
        if (allWord.ContainsKey(id))
        {
            WordElement_WithClr data = allWord[id];
            return $"<color=#{data.clrHex}><b>\"{data.words[GameManager.languageID]}\"</color></b>";
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
    public int id;
    public string[] words;

    public WordElement_Just(int id, string[] words)
    {
        this.id = id;
        this.words = words;
    }
}

[System.Serializable]
public class WordElement_WithClr : WordElement_Just
{
    public string clrHex;

    public WordElement_WithClr(int id, string clrHex, string[] names) : base(id, names)
    {
        this.clrHex = clrHex;
    }
}

#endregion

#region Class : CSV : ModuleInfo

[System.Serializable]
public class ModuleBaseData
{
    public int id;
    public List<int> moduleMainChip;

    public ModuleBaseData(int id, List<int> mainChip)
    {
        this.id = id;
        moduleMainChip = mainChip;
    }
}

#endregion



#region Class : QueueSet

[System.Serializable]
public class QueueSet<T> where T : Component
{
    [SerializeField] private Transform thisParentTF;
    [HideInInspector] protected T[] thisArr;

    private int totalAmount = 0;
    private int currentAmount = -1;

    public virtual void Offset()
    {
        totalAmount = thisParentTF.childCount;
        thisArr = new T[totalAmount];
        for (int i = 0; i < totalAmount; i++)
        {
            thisArr[i] = DevTool.Get_ComponentTType<T>(thisParentTF.GetChild(i).gameObject);
        }
    }

    public T Get_T()
    {
        currentAmount++;
        if (currentAmount >= totalAmount)
        {
            currentAmount = 0;
        }
        return thisArr[currentAmount];
    }

    public void Set_All(bool onOff)
    {
        for (int i = 0; i < totalAmount; i++)
            thisArr[i].gameObject.SetActive(onOff);
    }
}

[System.Serializable]
public class ASQueueSet : QueueSet<AudioSource>
{
    public void StopAll()
    {
        for (int i = 0; i < thisArr.Length; i++)
            thisArr[i].Stop();
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

    public Vector2 pos;
    public Quaternion rot;
    public Vector2 localScale;

    #endregion

    #region Constructor

    public State_TF2D(Vector2 pos, Quaternion rot, Vector2 localScale)
    {
        this.pos = pos;
        this.rot = rot;
        this.localScale = localScale;
    }

    #endregion
}

#endregion

#region Struct : BulletState

public struct BulletState_PosAndRot
{
    #region Value

    public Vector2 spawnPos;
    public Vector2 dir;
    public float spreadAngle;
    public float dis;

    #endregion

    #region Constructor

    public BulletState_PosAndRot(Vector2 spawnPos, Vector2 dir, float spreadAngle, float dis = 0)
    {
        this.spawnPos = spawnPos;
        this.dir = dir;
        this.spreadAngle = spreadAngle;
        this.dis = dis;
    }

    #endregion
}

public struct BulletState_Size
{
    #region Value

    public Vector2 objSize;
    public Vector2 colSize;

    #endregion

    #region Constructor 

    public BulletState_Size(Vector2 objSize, Vector2 colSize)
    {
        this.objSize = objSize;
        this.colSize = colSize;
    }

    #endregion
}

public struct BulletState_Effect
{
    #region Value

    public int explAmount;
    public float trailTime;

    #endregion

    #region Constructor

    public BulletState_Effect(int explAmount, float trailTime)
    {
        this.explAmount = explAmount;
        this.trailTime = trailTime;
    }

    #endregion
}

#endregion

#region Struct : AttackerState

public struct AttackerState_EndTF
{
    #region Value

    public State_TF2D tf;

    public float time;

    #endregion

    #region Constructor

    public AttackerState_EndTF(Vector2 pos, Quaternion rot, Vector2 size, float time)
    {
        tf.pos = pos;
        tf.rot = rot;
        tf.localScale = size;

        this.time = time;
    }

    #endregion
}

public struct AttackerState_Juge<T> where T : Collider2D
{
    #region Value

    public Vector2 colSize;
    public bool isVertical;

    #endregion

    #region Constructor

    public AttackerState_Juge(Vector2 colSize, bool isVertical = false)
    {
        this.colSize = colSize;
        this.isVertical = isVertical;
    }

    #endregion
}

#endregion


#region Struct : ItemData


public struct ItemData_UIVisual
{
    public Sprite icon;
    public int rank;
    public Sprite rankIcon;

    public ItemData_UIVisual(Sprite icon, int rank)
    {
        this.icon = icon;
        this.rank = rank;
        rankIcon = ResourceManager.instance.Get_RankIcon(this.rank);
    }

    public ItemData_UIVisual(ItemData itemData)
    {
        icon = itemData.itemIcon;
        rank = itemData.rank;
        rankIcon = ResourceManager.instance.Get_RankIcon(rank);
    }
}

#endregion


#region Struct : Visual

public struct State_Sprite
{
    #region Value

    public Material material;
    public Color clr;

    #endregion

    #region Constructor

    public State_Sprite(Material material, Color clr)
    {
        this.material = material;
        this.clr = clr;
    }

    #endregion
}

public struct State_Anim
{
    #region Value

    public AnimationClip ac;
    public float speed;

    #endregion

    #region Constructor

    public State_Anim(AnimationClip ac)
    {
        this.ac = ac;
        speed = 1f;
    }

    public State_Anim(AnimationClip ac, float speed)
    {
        this.ac = ac;
        this.speed = speed;
    }

    #endregion
}

#endregion

#region Struct : Visual : Explosion

public struct ExplState_Base
{
    #region Value

    public Vector2 spawnPos;
    public int spawnAmount;

    #endregion

    #region Constructor

    public ExplState_Base(Vector2 spawnPos, int spawnAmount)
    {
        this.spawnPos = spawnPos;
        this.spawnAmount = spawnAmount;
    }

    public ExplState_Base(ExplState_Base state)
    {
        spawnPos = state.spawnPos;
        spawnAmount = state.spawnAmount;
    }

    #endregion
}

public struct ExplState_Sprite
{
    #region Value

    public List<Sprite> sprite;
    public Material material;

    #endregion

    #region Constructor

    public ExplState_Sprite(List<Sprite> sprite, Material material)
    {
        this.sprite = sprite;
        this.material = material;
    }
    public ExplState_Sprite(ExplState_Sprite state)
    {
        sprite = state.sprite;
        material = state.material;
    }

    #endregion
}

public struct ExplState_MoveAndScale
{
    #region Value

    public Vector2 dir;
    public float dis;
    public float scale;
    public float time;
    public float randomDelayTime;

    #endregion

    #region Constructor

    public ExplState_MoveAndScale(Vector2 dir, float dis, float scale, float time, float randomDelayTime)
    {
        this.dir = dir;
        this.dis = dis;
        this.scale = scale;
        this.time = time;
        this.randomDelayTime = randomDelayTime;
    }

    public ExplState_MoveAndScale(ExplState_MoveAndScale state)
    {
        dir = state.dir;
        dis = state.dis;
        scale = state.scale;
        time = state.time;
        randomDelayTime = state.randomDelayTime;
    }

    #endregion
}


public struct ExplState
{
    public ExplState_Base baseState;
    public ExplState_Sprite spriteState;
    public ExplState_MoveAndScale firstState;
    public ExplState_MoveAndScale secondState;

    private ExplState_MoveAndScale originalFirstState;
    private ExplState_MoveAndScale originalSecondState;

    public ExplState(ExplState_Base baseState, ExplState_Sprite spriteState, ExplState_MoveAndScale firstState, ExplState_MoveAndScale secondState)
    {
        this.baseState = new ExplState_Base(baseState);
        this.spriteState = new ExplState_Sprite(spriteState);
        this.firstState = new ExplState_MoveAndScale(firstState);
        this.secondState = new ExplState_MoveAndScale(secondState);

        originalFirstState = new ExplState_MoveAndScale(firstState);
        originalSecondState = new ExplState_MoveAndScale(secondState);
    }

    public void Set_AllDir(Vector2 dir)
    {
        firstState.dir = dir;
        secondState.dir = dir;

        originalFirstState.dir = dir;
        originalSecondState.dir = dir;
    }

    public void Set_MultipleAllDir(Vector2 dir)
    {
        firstState.dir *= dir;
        secondState.dir *= dir;

        originalFirstState.dir *= dir;
        originalSecondState.dir *= dir;
    }

    public void Set_RandomValue()
    {
        firstState.time += DevTool.Get_RandomValueBaseZero(originalFirstState.randomDelayTime);
        secondState.time += DevTool.Get_RandomValueBaseZero(originalFirstState.randomDelayTime);
    }

    public void Set_RandomAngleValue_PivotZero(float angleExtent)
    {
        float randomAngle = DevTool.Get_RandomValueBaseZero(angleExtent);
        firstState.dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(originalFirstState.dir));
        secondState.dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(originalSecondState.dir));
    }

    public void Set_RandomAngleValue_JustAdd(float angleExtent)
    {
        float randomAngle = UnityEngine.Random.Range(0, angleExtent);
        firstState.dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(originalFirstState.dir));
        secondState.dir = DevTool.Get_DirFromAngle(randomAngle + DevTool.Get_AngleFromDir(originalSecondState.dir));
    }
}

#endregion

#endregion

#region ========== INTERFACE

#region Interface : Interact


public interface IInteract
{
    public void PlayInteract();

    public string Get_InteractName(out bool canInteract);
}

#endregion

#region Interface : When (Base)

public interface IWhen
{
    public abstract void Play_When(EnemyController enemy = null);
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
    public abstract void Play_When(EnemyController enemy = null, BulletController bullet = null);
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
        EnemyController enemy = null,
        BulletController bullet = null,
        DroppingBombController droppingBullet = null);
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

public delegate void Dele_T<T>(T t);

public delegate void Dele_RefT_T<T>(ref T refT, T t);

public delegate void Dele_T_U<T, U>(T t, U u);
public delegate void Dele_RefT_U<T, U>(ref T refT, U u);



public delegate void DeleEnemy(EnemyController enemy);

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