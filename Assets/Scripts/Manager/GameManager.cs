using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] public Color RandomColor = Color.red;
    [HideInInspector] private Sequence RandomColorSetSeq;

    [Space(10)]
    [Header("=== Passing Data")]
    [SerializeField] private List<GameObject> AllPlayerPrefabs;
    [SerializeField] public GameObject DesignatedPlayerPrefab;

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        if(GameManager.Instance == this)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        SetBaseOption();
        SetRainbowColorDotween();
    }

    private void Start()
    {
        DesignatedPlayerPrefab = AllPlayerPrefabs[1];
    }

    #endregion

    #region Option

    private void SetBaseOption()
    {
        Application.targetFrameRate = 144;
    }

    #endregion

    #region Module

    // Get Type if it Can Cast
    public static T CastIfPossible<T>(object input) where T : class
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

    // Set List by Component
    public static List<T> SetList<T>(Transform _Parent)
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

    #endregion

    #region Color

    private void SetRainbowColorDotween()
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

public enum eEnemy
{
    Normal, Elite, Boss
}

public interface IInteract
{
    public void Interact();
}