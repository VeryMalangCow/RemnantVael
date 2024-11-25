using UnityEngine;
using System.Collections;

public class EnemyPattern_Follow : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Follow")]

    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float FollowingSpeed = 3f;

    [Header("-- Range")]
    [SerializeField] private bool UntilForTargetRange = false;
    [SerializeField] private float TargetRange = 0f;
    [SerializeField] private bool IgnoreWall = false;

    [Header("-- Time")]
    [SerializeField] private bool UntilForTargetTime = false;
    [SerializeField] private float TargetTime = 0f;
    [SerializeField] private float CurrentTime = 0f;

    // Other
    private static float FindRootDelay = 0.1f;

    #endregion

    #region Framework

    private void Update()
    {
        if (UntilForTargetTime && 
            IsPlayingThisPattern &&
            CurrentTime < TargetTime)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    #endregion

    #region Can Check

    public override bool CanPlayPattern()
    {
        if (CanPlayPattern_ConditionByRange() && CanPlayPattern_ConditionByTime())
        {
            return true;
        }

        return false;
    }

    // 거리 조건 충족?
    private bool CanPlayPattern_ConditionByRange()
    {
        if (UntilForTargetRange &&
            TargetRange >= Vector2.Distance(ThisEnemy.gameObject.transform.position, PlayerManager.Instance.PlayerController.gameObject.transform.position))
        {
            if (!IgnoreWall && ThisEnemy.IsExistWall(ThisEnemy.transform, PlayerManager.Instance.PlayerController.transform))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    // 시간 조건 충족?
    private bool CanPlayPattern_ConditionByTime()
    {
        if (UntilForTargetTime &&
            CurrentTime >= TargetTime)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Start End

    public override void StartPattern()
    {
        CurrentTime = 0f;

        base.StartPattern();
    }

    public override void EndPattern()
    {
        CurrentTime = 0f;

        base.EndPattern();
    }

    #endregion

    #region Actual

    protected override IEnumerator ThisPattern()
    {
        ThisEnemy.MoveSpeed = FollowingSpeed;

        yield return new WaitForSeconds(StartDelay);

        while (true)
        {
            if (CanPlayPattern())
            {
                ThisEnemy.LookTargetPoint = ThisEnemy.FindWay()[0].ThisTF.position;
                ThisEnemy.MoveTargetPoint = ThisEnemy.FindWay()[0].ThisTF.position;
                yield return new WaitForSeconds(FindRootDelay);
            }
            else
            {
                break;
            }
        }

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}
