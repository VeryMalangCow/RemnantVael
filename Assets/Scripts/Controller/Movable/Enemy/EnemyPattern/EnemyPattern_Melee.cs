using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyPattern_Melee : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private AttackerState ThisAS;
    [SerializeField] private AnimationClip ThisAC;
    [SerializeField] private float SpawnDis = 1f;
    [SerializeField] private float EndDis = 2f;

    #endregion

    #region Can Check

    public override bool CanPlayPattern()
    {
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
        yield return new WaitForSeconds(StartDelay);

        #region Actual

        Vector2 targetDir = 
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;

        yield return new WaitForSeconds(1f);

        EnemyAttacker ea = PoolingManager.Instance.GetOP_EnemyAttacker();
        ea.SetState_SetRotationAndMoveForward(
            ((Vector2)ThisEnemy.transform.position + (targetDir * SpawnDis)),
            ThisAS, ThisAC, Vector2.zero,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, targetDir)),
            ((Vector2)ThisEnemy.transform.position + (targetDir * EndDis)),
            1f)
            .OnComplete(() =>
            {
                ea.EndState();
            }); ;

        #endregion

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
    }

    #endregion
}
