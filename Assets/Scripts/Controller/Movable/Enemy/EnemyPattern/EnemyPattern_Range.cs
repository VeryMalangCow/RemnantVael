using UnityEngine;
using System.Collections;

public class EnemyPattern_Range : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private BulletState ThisBS;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private Transform SpawnTF;
    [SerializeField] private AnimationClip BulletAC;
    [SerializeField] private Vector2 BulletShadowScale;
    [SerializeField] private Vector2 BulletColSize;

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

        EnemyBulletController EBC = PoolingManager.Instance.GetOP_EnemyBullet();

        Vector2 targetDir =
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;
        
        // Shadow
        float targetShadow = 0.4f;
        if (SpawnTF.TryGetComponent(out HaveShadowThing HST))
        { targetShadow = HST.TargetRange; }

        // Base State 
        EBC.SetState(SpawnTF.position, ThisBS, targetDir, BulletShadowScale, BulletColSize, BulletAC, targetShadow);

        // Sorting Layer
        if (SpawnTF.gameObject.TryGetComponent(out HaveShadowThing hst))
        { EBC.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}

