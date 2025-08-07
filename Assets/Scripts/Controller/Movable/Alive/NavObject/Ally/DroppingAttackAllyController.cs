using UnityEngine;

public class DroppingAttackAllyController : DroppingAllyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Vector2 BulletObjSize;

    #endregion

    #region Attack

    protected override bool Can_Shot()
    {
        return base.Can_Shot() && EnemyManager.Instance.CurrentEnemyList.Count > 0;
    }
    
    protected override void Shot()
    {
        base.Shot();
        Fire_Bullet(PoolingManager.Instance.Get_OP_DroppingAllyBullet(), Get_TargetEnemy().transform.position);
    }

    private EnemyController Get_TargetEnemy()
    {
        EnemyController targetEnemy = PlayerManager.Instance.Get_PingedEnemy();
        if (targetEnemy == null)
        {
            targetEnemy = EnemyManager.Instance.CurrentEnemyList[Random.Range(0, EnemyManager.Instance.CurrentEnemyList.Count)];
        }
        return targetEnemy;
    }

    private void Fire_Bullet(AllyDroppingBombController _Bullet, Vector2 _TargetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        _Bullet.Set_State(
            Get_BulletState(), 
            _DroppingTime: ActualAllyState.MuzzleSpeed.Value, 
            _TopYPos: 5f, 
            _BottomYPos: DropBottomYPos,
            _State_PosAndRot: Get_BulletState_PosAndRot(_TargetPos),
            _State_Size: Get_BulletState_Shadow_Size());

        // Light & Trail
        _Bullet.SetOn_LightIntensity(LightIntensity);
        _Bullet.SetOn_TrailState(TrailTime, TrailStartWidth * ActualAllyState.AttackSize.Value, ThisExtraGradient);

        // Sync
        ActiveAlly_Fire(null, _Bullet);

        // ÀÌ¹ÌÁö
        _Bullet.ThisSR.sprite = ThisSprite;
    }

    #endregion

    #region State (Bullet)

    private CombatState Get_BulletState()
    {
        return new CombatState(
            new CombatOwner(eCombatOwner.Ally, ID),
            new DmgState(eDamageType.Physics, ActualAllyState.Dmg.Value),
            new CriticalState(ActualAllyState.CC.Value, 1 + ActualAllyState.CD.Value),
            new KnockbackState(true, ActualAllyState.KBPower.Value, 0.2f));
    }

    private BulletState_PosAndRot Get_BulletState_PosAndRot(Vector2 _TargetPos)
    {
        return new BulletState_PosAndRot(
            _TargetPos,
            Vector2.zero,
            0);
    }

    private BulletState_Size Get_BulletState_Shadow_Size()
    {
        return new BulletState_Size(
            BulletObjSize * ActualAllyState.AttackSize.Value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
