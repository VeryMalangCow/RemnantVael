using UnityEngine;
using DG.Tweening;
using System.Collections;

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

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private DepthController SpawnHST;

    #endregion

    #region Framework

    private void Update()
    {
        if (!IsPlayingThisPattern)
        {
            StopCoroutine(Play_ThisPattern_Cor());
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
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

    public override void Start_Pattern()
    {

        base.Start_Pattern();
    }

    public override void End_Pattern()
    {

        base.End_Pattern();
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        ThisEnemy.LookTargetPoint = 
            PlayerManager.Instance.PlayerController.transform.position - ThisEnemy.transform.position;
        Vector2 targetDir =
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;

        yield return new WaitForSeconds(StartDelay);

        EnemyAttackerController ea = PoolingManager.Instance.Get_OP_EnemyAttacker();
        ea.Enemy = this.ThisEnemy;

        if (LightOn)
        {
            ea.Set_Light(LightSize, LightTime);
        }

        ea.Set_ShadowDis(SpawnHST);
        ea.Play_RotAndPosForward(
            ((Vector2)ThisEnemy.transform.position + (targetDir * SpawnDis)),
            ThisAS, ThisAC, Vector2.one,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, targetDir)),
            ((Vector2)ThisEnemy.transform.position + (targetDir * EndDis)),
            TweenTime, AnimSpeed)
            .OnComplete(() =>
            {
                ea.End_State();
            });

        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    #endregion
}
