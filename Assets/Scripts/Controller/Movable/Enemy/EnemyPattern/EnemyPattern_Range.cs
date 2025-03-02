using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPattern_Range : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private BulletState ThisBS;
    [SerializeField] private Vector2 BulletShadowScale;
    [SerializeField] private Vector2 BulletColSize;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<Transform> SpawnTFList;
    [SerializeField] private AnimationClip BulletAC;

    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float MaximumRange = 4f;
    [SerializeField] private float MinimumRange = 3f;

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
        yield return new WaitForSeconds(StartDelay);

        ThisEnemy.LookTargetPoint =
            PlayerManager.Instance.PlayerController.transform.position - ThisEnemy.transform.position;

        for (int i = 0; i < SpawnTFList.Count; i++)
        {
            EnemyBulletController EBC = PoolingManager.Instance.Get_OP_EnemyBullet();

            Vector2 targetDir =
                ((Vector2)PlayerManager.Instance.PlayerController.transform.position
                - (Vector2)ThisEnemy.transform.position).normalized;

            // Shadow
            float targetShadow = 0.4f;
            if (SpawnTFList[i].TryGetComponent(out DepthController HST))
            { targetShadow = HST.TargetRange; }

            // Base State 
            EBC.Enemy = ThisEnemy;
            EBC.Set_State(SpawnTFList[i].position, ThisBS, targetDir, BulletShadowScale, BulletColSize, BulletAC, targetShadow);

            // Sorting Layer
            if (SpawnTFList[i].gameObject.TryGetComponent(out DepthController hst))
            { EBC.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }

            // Effect
            ThisEnemy.MEI.Gen_ExplosionImgs_Fan(
                (Vector2)HST.TargetObject.transform.position + (targetDir * 0.3f),
                targetDir, 45f,
                3, 0.2f, 1f,
                0.8f, 0.05f, 0.1f,
                0.4f, 0.5f, 1.0f,
                0, ThisEnemy.ThisSmokeM);

        }

        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    #endregion
}

