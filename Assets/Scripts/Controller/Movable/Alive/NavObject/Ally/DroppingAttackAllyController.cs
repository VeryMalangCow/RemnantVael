using UnityEngine;

public class DroppingAttackAllyController : DroppingAllyController
{
    #region Value
/*
    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Bullet")]
*/
    #endregion

    #region Attack

    protected override bool Can_Shot()
    {
        return base.Can_Shot();
    }
    
    protected override void Shot()
    {
        base.Shot();
        if (EnemyManager.instance.currentEnemyList.Count > 0)
        {
            Fire_Bullet(PoolingManager.instance.Get_OP_DroppingAllyBullet(), Get_TargetEnemy().transform.position);
        }
        Debug.Log(Name[1] + ": Bomb Attack");
    }

    private EnemyController Get_TargetEnemy()
    {
        EnemyController targetEnemy = PlayerManager.instance.Get_PingedEnemy();
        if (targetEnemy == null)
        {
            targetEnemy = EnemyManager.instance.currentEnemyList[Random.Range(0, EnemyManager.instance.currentEnemyList.Count)];
        }
        return targetEnemy;
    }

    private void Fire_Bullet(AllyDroppingBombController _Bullet, Vector2 _TargetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        _Bullet.Set_State(
            Get_BulletState(), 
            _DroppingTime: ActualAllyState.muzzleSpeed.value, 
            _TopYPos: 5f, 
            _BottomYPos: DropBottomYPos,
            _State_PosAndRot: Get_BulletState_PosAndRot(_TargetPos),
            _State_Size: Get_BulletState_Shadow_Size());

        // Light & Trail
        _Bullet.SetOn_LightIntensity(LightIntensity);
        _Bullet.SetOn_TrailState(TrailTime, TrailStartWidth * ActualAllyState.attackSize.value, ThisExtraGradient);

        // Sync
        ActiveAlly_Fire(null, _Bullet);

        // ÀÌ¹ÌÁö
        _Bullet.ThisSR.sprite = ThisSprite;

        // HUD

    }

    #endregion

    #region State (Bullet)

    private CombatState Get_BulletState()
    {
        return new CombatState(
            new CombatOwner(eCombatOwner.Ally, id),
            new DmgState(eDamageType.Physics, ActualAllyState.dmg.value),
            new CriticalState(ActualAllyState.criticalChacne.value, 1 + ActualAllyState.criticalDmg.value),
            new KnockbackState(true, ActualAllyState.kbPower.value, 0.2f));
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
            Vector2.one * ActualAllyState.attackSize.value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
