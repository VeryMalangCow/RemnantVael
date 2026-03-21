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
        Debug.Log(_name[1] + ": Bomb Attack");
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

    private void Fire_Bullet(AllyDroppingBombController bullet, Vector2 targetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        bullet.Set_State(
            Get_BulletState(), 
            droppingTime: actualAllyState.muzzleSpeed.value, 
            topYPos: 5f, 
            bottomYPos: dropBottomYPos,
            state_PosAndRot: Get_BulletState_PosAndRot(targetPos),
            state_Size: Get_BulletState_Shadow_Size());

        // Light & Trail
        bullet.SetOn_LightIntensity(lightIntensity);
        bullet.SetOn_TrailState(trailTime, trailStartWidth * actualAllyState.attackSize.value, extraGradient);

        // Sync
        ActiveAlly_Fire(null, bullet);

        // ÀÌ¹ÌÁö
        bullet.thisSr.sprite = sprite;

        // HUD

    }

    #endregion

    #region State (Bullet)

    private CombatState Get_BulletState()
    {
        return new CombatState(
            new CombatOwner(eCombatOwner.Ally, id),
            new DmgState(eDamageType.Physics, actualAllyState.dmg.value),
            new CriticalState(actualAllyState.criticalChacne.value, 1 + actualAllyState.criticalDmg.value),
            new KnockbackState(true, actualAllyState.kbPower.value, 0.2f));
    }

    private BulletState_PosAndRot Get_BulletState_PosAndRot(Vector2 targetPos)
    {
        return new BulletState_PosAndRot(
            targetPos,
            Vector2.zero,
            0);
    }

    private BulletState_Size Get_BulletState_Shadow_Size()
    {
        return new BulletState_Size(
            Vector2.one * actualAllyState.attackSize.value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
