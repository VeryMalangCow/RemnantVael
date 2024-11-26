using UnityEngine;
using System.Collections;
using System;

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
    [SerializeField] private Transform SpawnTF;
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
        yield return new WaitForSeconds(StartDelay);

        ThisEnemy.LookTargetPoint =
            PlayerManager.Instance.PlayerController.transform.position - ThisEnemy.transform.position;

        EnemyBulletController EBC = PoolingManager.Instance.GetOP_EnemyBullet();

        Vector2 targetDir =
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;
        
        // Shadow
        float targetShadow = 0.4f;
        if (SpawnTF.TryGetComponent(out HaveShadowThing HST))
        { targetShadow = HST.TargetRange; }

        // Base State 
        EBC.OwnerEC = ThisEnemy;
        EBC.SetState(SpawnTF.position, ThisBS, targetDir, BulletShadowScale, BulletColSize, BulletAC, targetShadow);

        // Sorting Layer
        if (SpawnTF.gameObject.TryGetComponent(out HaveShadowThing hst))
        { EBC.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }

        // Effect
        ThisEnemy.MEI.GenExplosionImgs_Fan(
            (Vector2)HST.TargetObject.transform.position + (targetDir * 0.3f),
            targetDir, 45f,
            3, 0.2f, 1f,
            0.8f, 0.05f, 0.1f,
            0.4f, 0.5f, 1.0f,
            0, ThisEnemy.ThisSmokeM);

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}

