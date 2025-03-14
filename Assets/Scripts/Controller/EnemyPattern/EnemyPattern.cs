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

    #region Framework

    private void Start()
    {
        IsPlayingThisPattern = false;
    }

    #endregion

    #region Can Check

    public abstract bool Can_PlayPattern();

    protected abstract IEnumerator Play_ThisPattern_Cor();

    #endregion

    #region Start End

    public virtual void Start_Pattern()
    {
        IsPlayingThisPattern = true;
        ThisEnemy.IsPlayingPattern = true;

        ThisEnemy.CurrentPatternCor = Play_ThisPattern_Cor();

        StartCoroutine(ThisEnemy.CurrentPatternCor);
    }

    public virtual void End_Pattern()
    {
        IsPlayingThisPattern = false;
        ThisEnemy.IsPlayingPattern = false;

        ThisEnemy.MoveAtPoint = Vector2.zero;
        ThisEnemy.MoveAtDir = Vector2.zero;
        ThisEnemy.MoveSpeed = 0;
    }

    #endregion
}
