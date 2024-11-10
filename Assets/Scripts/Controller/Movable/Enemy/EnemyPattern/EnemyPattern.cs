using UnityEngine;

public abstract class EnemyPattern : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Pattern")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected EnemyController ThisEnemy;
    [SerializeField] protected bool IsPlayingThisPattern = false;

    #endregion

    #region Offset

    public void Offset(EnemyController _ThisEC)
    {
        ThisEnemy = _ThisEC;
    }

    #endregion

    #region Can Check
    public abstract bool CanPlayPattern();

    #endregion

    #region Start End

    public virtual void StartPattern()
    {
        ThisEnemy.IsPlayingPattern = true;
        IsPlayingThisPattern = true;
        ThisEnemy.CurrentEnemyPattern = this;
    }

    public virtual void EndPattern()
    {
        ThisEnemy.IsPlayingPattern = false;
        IsPlayingThisPattern = false;
        ThisEnemy.CurrentEnemyPattern = null;

        ThisEnemy.StartTryGetAnyPattern(0.5f);
    }

    #endregion
}
