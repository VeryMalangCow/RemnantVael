using UnityEngine;
using System.Collections;

public class EnemyPattern_Follow : EnemyPattern
{
    #region Value

    #region - Inspector

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

    #endregion

    #region - Hide

    [HideInInspector] private float FollowInitDelay = 0.2f;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        FollowInitDelay = 0.4f / FollowingSpeed; 
    }

    #endregion

    #region Framework

    private void Update()
    {
        if (UntilForTargetTime && 
            IsPlaying &&
            CurrentTime < TargetTime)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        if (Can_PlayPattern_ConditionByRange() && Can_PlayPattern_ConditionByTime())
        {
            return true;
        }

        return false;
    }

    // 거리 조건 충족?
    private bool Can_PlayPattern_ConditionByRange()
    {
        if (UntilForTargetRange && TargetRange >= DevTool.Get_DisForPlayer(ThisEnemy))
        {
            if (!IgnoreWall && ThisEnemy.Is_ExistWall(PlayerManager.Instance.playerController.transform))
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
    private bool Can_PlayPattern_ConditionByTime()
    {
        if (UntilForTargetTime && CurrentTime >= TargetTime)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Start End

    public override void Start_Pattern()
    {
        base.Start_Pattern();

        CurrentTime = 0f;
    }

    public override void End_Pattern()
    {
        base.End_Pattern();

        CurrentTime = 0f;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        yield return new WaitForSeconds(StartDelay);

        #region Actual

        ThisEnemy.Set_MoveSpeed(FollowingSpeed);
        PlayerController targetPc = PlayerManager.Instance.playerController;

        while (true)
        {
            if (Can_PlayPattern())
            {
                ThisEnemy.Set_NavDir(targetPc.transform);
                yield return new WaitForSeconds(FollowInitDelay);
            }
            else
            {
                break;
            }

        }
        ThisEnemy.End_Nav();

        #endregion

        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    #endregion
}
