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
    [SerializeField] protected EnemyController enemy;
    [SerializeField] protected int repeatAmount = 1;
    
    [SerializeField] protected float startDelay = 0f;
    [SerializeField] protected float endDelay = 0f;

    [Space(10)]
    [Header("=== Special")]
    [SerializeField] protected bool isSpecialPattern = false;
    [SerializeField] private bool isSpecialStartPattern = false;
    [SerializeField] private bool isSpecialEndPattern = false;

    #endregion

    #region - Hide

    [HideInInspector] public bool isPlaying = false;
    [HideInInspector] protected int currentRepeatAmount = 0;

    #endregion

    #endregion

    #region Offset

    protected virtual void Offset()
    {
        isPlaying = false;
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
        isPlaying = true;
        enemy.isPlayingPattern = true;
        currentRepeatAmount = 0;

        // Pattern
        enemy.currentPatternCor = Play_ThisPattern_Cor();
        StartCoroutine(enemy.currentPatternCor);

        if (isSpecialStartPattern)
        {
            enemy.hud.Set_Patterning(true);
        }
    }

    public virtual void End_Pattern()
    {
        // Value
        isPlaying = false;
        enemy.isPlayingPattern = false;
        currentRepeatAmount = 0;

        if (isSpecialEndPattern)
        {
            enemy.Reset_ChargeState();
        }
    }

    #endregion
}
