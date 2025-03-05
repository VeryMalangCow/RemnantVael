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

    #region About Variable

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

#endregion

