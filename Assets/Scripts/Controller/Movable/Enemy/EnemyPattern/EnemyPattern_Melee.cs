using UnityEngine;
using DG.Tweening;
using System.Collections;
using static DG.Tweening.DOTweenAnimation;

public class EnemyPattern_Melee : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private AttackerState ThisAS;
    [SerializeField] private AnimationClip ThisAC;
    [SerializeField] private float TweenTime = 1f;
    [SerializeField] private float AnimSpeed = 1f;
    [SerializeField] private float SpawnDis = 1f;
    [SerializeField] private float EndDis = 2f;
    [SerializeField] bool LightOn = false;
    [SerializeField] float LightTime = 1f;
    [SerializeField] float LightSize = 1f;

    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float MaximumRange = 1f;
    [SerializeField] private float MinimumRange = 0f;

    #endregion

    #region Framework

    private void Update()
    {
        if (!IsPlayingThisPattern)
        {
            StopCoroutine(ThisPattern());
        }
    }

    #endregion

    #region Can Check

    public override bool CanPlayPattern()
    {
        float forPlayerDis = Vector2.Distance(ThisEnemy.transform.position, PlayerManager.Instance.PlayerController.transform.position);
        if (forPlayerDis > MaximumRange || forPlayerDis < MinimumRange)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Start End

    public override void StartPattern()
    {

        base.StartPattern();
    }

    public override void EndPattern()
    {

        base.EndPattern();
    }

    #endregion

    #region Actual

    protected override IEnumerator ThisPattern()
    {
        Vector2 targetDir =
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;

        yield return new WaitForSeconds(StartDelay);

        EnemyAttacker ea = PoolingManager.Instance.GetOP_EnemyAttacker(); 
        
        if (LightOn)
        {
            ea.SetLight(LightSize, LightTime);
        }

        ea.SetState_SetRotationAndMoveForward(
            ((Vector2)ThisEnemy.transform.position + (targetDir * SpawnDis)),
            ThisAS, ThisAC, Vector2.one,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, targetDir)),
            ((Vector2)ThisEnemy.transform.position + (targetDir * EndDis)),
            TweenTime, AnimSpeed)
            .OnComplete(() =>
            {
                ea.EndState();
            }); ;
        

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}
