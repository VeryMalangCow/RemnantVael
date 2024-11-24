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
    [SerializeField] public bool IsPlayingThisPattern = false;
    [SerializeField] protected float StartDelay = 0f;
    [SerializeField] protected float EndDelay = 0f;

    #endregion

    #region Can Check

    public abstract bool CanPlayPattern();
    protected abstract IEnumerator ThisPattern();

    #endregion

    #region Start End

    public virtual void StartPattern()
    {
        IsPlayingThisPattern = true;
        ThisEnemy.IsPlayingPattern = true;

        StartCoroutine(ThisPattern());
    }

    public virtual void EndPattern()
    {
        IsPlayingThisPattern = false;
        ThisEnemy.IsPlayingPattern = false;

        ThisEnemy.TryGetAnyPattern();
    }

    public void ForceEndPattern()
    {
        StopCoroutine(ThisPattern());
        EndPattern();
    }

    #endregion
}
