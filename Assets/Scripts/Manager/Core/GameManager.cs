using DG.Tweening;
using System.Collections.Generic;
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

    #region Module

    #region Get

    // 캐스팅
    public static T Get_CastIfPossible<T>(object input) where T : class
    {
        if (input is T variable)
        {
            return variable;
        }
        else
        {
            return null;
        }
    }

    // 인자의 자식들의 T 타입 리스트
    public static List<T> Get_List<T>(Transform _Parent)
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

    // List를 무작위 섞기
    public static List<T> Get_ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = Random.Range(0, list.Count);
            random2 = Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
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

    #endregion

}

// Enum

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


// Interface

public interface IInteract
{
    public void Play_Interact();
}