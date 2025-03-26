using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPattern_Range : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private BulletState ThisBS;

    [Space(10)]
    [Header("=== Size")]
    [SerializeField] private Vector2 BulletShadowScale;
    [SerializeField] private Vector2 BulletColSize;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<DepthController> SpawnDepthList;
    [SerializeField] private AnimationClip BulletAC;

    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float MaxRange = 4f;
    [SerializeField] private float MinRange = 3f;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] private int ShootExplAmount = 3;
    [SerializeField] private int ExplAmount = 3;

    #endregion

    #region Framework

    private void Update()
    {
        if (!IsPlaying)
        {
            StopCoroutine(Play_ThisPattern_Cor());
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        float forPlayerDis = Vector2.Distance(ThisEnemy.transform.position, PlayerManager.Instance.PlayerController.transform.position);
        if (forPlayerDis >= MinRange && forPlayerDis < MaxRange)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        Vector2 targetDir = DevTool.Get_DirForPlayer(ThisEnemy);

        yield return new WaitForSeconds(StartDelay);

        #region Actual 

        for (int i = 0; i < SpawnDepthList.Count; i++)
            Play_ActualPattern(SpawnDepthList[i], targetDir);
        

        #endregion

        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    private void Play_ActualPattern(DepthController _Depth, Vector2 _TargetDir)
    {
        EnemyBulletController bullet = PoolingManager.Instance.Get_OP_EnemyBullet();
        bullet.Enemy = ThisEnemy;
        float targetShadow = _Depth.TargetRange;
        bullet.Set_State(
            ThisBS, 
            State_PosAndRot(_Depth.transform, _TargetDir), 
            State_Size(), 
            State_Anim(),
            State_Effect(),
            targetShadow);

        bullet.ThisSR.sortingOrder = _Depth.ThisSR.sortingOrder - 1;

        // Effect
        UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy_Shoot(
            (Vector2)_Depth.TargetObject.transform.position + (_TargetDir * 0.3f),
            _TargetDir, ShootExplAmount);
    }

    #endregion

    #region State

    private BulletState_PosAndRot State_PosAndRot(Transform _TF, Vector2 _TargetDir)
    {
        return new BulletState_PosAndRot(_TF.position, _TargetDir, 0);
    }

    private BulletState_Size State_Size()
    {
        return new BulletState_Size(BulletShadowScale, BulletColSize);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(BulletAC, 1);
    }

    private BulletState_Effect State_Effect()
    {
        return new BulletState_Effect(ExplAmount);
    }

    #endregion
}

