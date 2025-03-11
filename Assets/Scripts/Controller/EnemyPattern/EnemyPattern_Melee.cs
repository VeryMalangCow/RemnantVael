using UnityEngine;
using System.Collections;

public class EnemyPattern_Melee : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AttackerState ThisAS;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private AnimationClip ThisAC;
    [SerializeField] private float JugeAndTweenTime = 0.5f;
    [SerializeField] private float AnimSpeed = 2.6f;
    [SerializeField] private float SpawnDis = 1f;
    [SerializeField] private float EndDis = 1.5f;
    [SerializeField] bool LightOn = false;
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
            ea.Set_Light(LightSize, JugeAndTweenTime);
        }

        AttackerState_Juge<CircleCollider2D> juge
            = new AttackerState_Juge<CircleCollider2D>(
                Vector2.one);

        AttackerState_Anim anim
            = new AttackerState_Anim(
                ThisAC, 
                AnimSpeed);

        AttackerState_StartTF startTF 
            = new AttackerState_StartTF(
                (Vector2)ThisEnemy.transform.position + (targetDir * SpawnDis),
                Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, targetDir)),
                Vector2.one);

        AttackerState_EndTF endTF 
            = new AttackerState_EndTF(
                (Vector2)ThisEnemy.transform.position + (targetDir * EndDis),
                Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, targetDir)),
                Vector2.one, JugeAndTweenTime);


        
        ea.Set_State(ThisAS, juge, anim, startTF, endTF);

        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    #endregion
}
