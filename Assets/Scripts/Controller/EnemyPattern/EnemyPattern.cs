using System.Collections;
using UnityEngine;

public abstract class EnemyPattern : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Pattern")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected EnemyController ThisEnemy;
    [SerializeField] protected float StartDelay = 0f;
    [SerializeField] protected float EndDelay = 0f;

    [HideInInspector] public bool IsPlaying = false;

    #endregion

    #region Framework

    private void Start()
    {
        IsPlaying = false;
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

        // Pattern
        ThisEnemy.CurrentPatternCor = Play_ThisPattern_Cor();
        StartCoroutine(ThisEnemy.CurrentPatternCor);
    }

    public virtual void End_Pattern()
    {
        // Value
        IsPlaying = false;
        ThisEnemy.IsPlayingPattern = false;

        // Enemy
        ThisEnemy.MoveAtPoint = Vector2.zero;
        ThisEnemy.MoveAtDir = Vector2.zero;
        ThisEnemy.MoveSpeed = 0;
    }

    #endregion
}
