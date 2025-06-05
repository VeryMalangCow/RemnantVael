using System.Collections;
using UnityEngine;

public abstract class EnemyPattern : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Pattern")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected EnemyController ThisEnemy;
    [SerializeField] protected int RepeatAmount = 1;
    
    [SerializeField] protected float StartDelay = 0f;
    [SerializeField] protected float EndDelay = 0f;

    [Space(10)]
    [Header("=== Special")]
    [SerializeField] protected bool IsSpecialPattern = false;
    [SerializeField] private bool IsSpecialStartPattern = false;
    [SerializeField] private bool IsSpecialEndPattern = false;

    #endregion

    #region - Hide

    [HideInInspector] public bool IsPlaying = false;
    [HideInInspector] protected int CurrentRepeatAmount = 0;

    #endregion

    #endregion

    #region Offset

    protected virtual void Offset()
    {
        IsPlaying = false;
    }

    #endregion


    #region Framework

    private void Awake()
    {
        Offset();
    }

    #endregion

    #region Can Check

    public abstract bool Can_PlayPattern();

    protected abstract IEnumerator Play_ThisPattern_Cor();

    #endregion

    #region Start End

    public virtual void Start_Pattern()
    {
        // Value
        IsPlaying = true;
        ThisEnemy.IsPlayingPattern = true;
        CurrentRepeatAmount = 0;

        // Pattern
        ThisEnemy.CurrentPatternCor = Play_ThisPattern_Cor();
        StartCoroutine(ThisEnemy.CurrentPatternCor);

        if (IsSpecialStartPattern)
        {
            ThisEnemy.HUD.Set_Patterning(true);
        }
    }

    public virtual void End_Pattern()
    {
        // Value
        IsPlaying = false;
        ThisEnemy.IsPlayingPattern = false;
        CurrentRepeatAmount = 0;

        if (IsSpecialEndPattern)
        {
            ThisEnemy.Reset_ChargeState();
        }
    }

    #endregion
}
