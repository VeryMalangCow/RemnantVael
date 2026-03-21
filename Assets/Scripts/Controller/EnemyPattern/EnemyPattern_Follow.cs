using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class EnemyPattern_Follow : EnemyPattern
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Follow")]

    [Space(10)]
    [Header("=== Condition")]
    [FormerlySerializedAs("FollowingSpeed")][SerializeField] private float followingSpeed = 3f;

    [Header("-- Range")]
    [FormerlySerializedAs("UntilForTargetRange")][SerializeField] private bool untilForTargetRange = false;
    [FormerlySerializedAs("TargetRange")][SerializeField] private float targetRange = 0f;
    [FormerlySerializedAs("IgnoreWall")][SerializeField] private bool ignoreWall = false;

    [Header("-- Time")]
    [FormerlySerializedAs("UntilForTargetTime")][SerializeField] private bool untilForTargetTime = false;
    [FormerlySerializedAs("TargetTime")][SerializeField] private float targetTime = 0f;
    [FormerlySerializedAs("CurrentTime")][SerializeField] private float currentTime = 0f;

    #endregion

    #region - Hide

    [HideInInspector] private float followInitDelay = 0.2f;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        followInitDelay = 0.4f / followingSpeed; 
    }

    #endregion

    #region Framework

    private void Update()
    {
        if (untilForTargetTime && 
            isPlaying &&
            currentTime < targetTime)
        {
            currentTime += Time.deltaTime;
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
        if (untilForTargetRange && targetRange >= DevTool.Get_DisForPlayer(enemy))
        {
            if (!ignoreWall && enemy.Is_ExistWall(PlayerManager.instance.playerController.transform))
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
        if (untilForTargetTime && currentTime >= targetTime)
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

        currentTime = 0f;
    }

    public override void End_Pattern()
    {
        base.End_Pattern();

        currentTime = 0f;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        yield return new WaitForSeconds(startDelay);

        #region Actual

        enemy.Set_MoveSpeed(followingSpeed);
        PlayerController targetPc = PlayerManager.instance.playerController;

        while (true)
        {
            if (Can_PlayPattern())
            {
                enemy.Set_NavDir(targetPc.transform);
                yield return new WaitForSeconds(followInitDelay);
            }
            else
            {
                break;
            }

        }
        enemy.End_Nav();

        #endregion

        yield return new WaitForSeconds(endDelay);

        End_Pattern();
        enemy.Play_Pattern();
    }

    #endregion
}
