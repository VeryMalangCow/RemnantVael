using System.Collections;
using UnityEngine;

public class ShootingAllyController : FieldUnitAllyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Attack")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private bool isAlwaysStatus;
    [SerializeField] private eStatusEffect stateType;


    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private Vector2 bulletObjSize;
    [SerializeField] private Vector2 bulletColSize;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Transform bulletSpawnTF;

    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] private float trailTime;
    [SerializeField] private float trailStartWidth;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] private float lightIntensity;

    #endregion

    #region - Hide

    [HideInInspector] private bool isAttacking = false;

    [HideInInspector] private float currentRof = 0f;

    #endregion

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        Set_CaculateAttack(Time.deltaTime);
    }

    #endregion

    #region Play

    protected override IEnumerator Play_Main_Cor()
    {
        yield return StartCoroutine(base.Play_Main_Cor());

        while (true)
        {
            if (PlayerManager.instance.Get_PingedEnemy() != null)
            {
                Set_PingedState();
                yield return new WaitForSeconds(followInitDelay);
            }
            else
            {
                Set_NoPingedState();
                yield return new WaitForSeconds(followInitDelay);
            }
        }
    }

    #endregion

    #region Attacking

    private void Set_Attacking(bool onOff)
    {
        if (isAttacking != onOff)
        {
            isAttacking = onOff;
        }
    }

    private void Set_CaculateAttack(float deltaTime)
    {
        if (currentRof < 1)
        {
            currentRof += deltaTime * actualAllyState.rof.value;
        }

        if (!isAttacking || enemy == null) return;

        if (currentRof >= 1)
        {
            currentRof -= 1;
            Play_Attack(BulletManager.instance.SpawnAllyBullet());
        }
    }

    private void Play_Attack(AllyBulletController bullet)
    {
        // 총알 스탯과 SortingOrder 설정
        bullet.Set_State(
            Get_BulletState(),
            state_PosAndRot: Get_BulletState_PosAndRot(),
            state_Size: Get_BulletState_Size(),
            state_Anim: null,
            state_Effect: null,
            0.5f);

        bullet.thisSr.color = this.extraClr;

        // 상태이상 총알이면
        if (isAlwaysStatus)
            bullet.state.Set_Status(isAlwaysStatus, stateType);
        
        // Sync
        ActiveAlly_Fire(bullet, null);

        // 모듈 싱크 효과 => 사격 후
        ActiveAlly_AfterFire();

        bullet.SetOn_LightIntensity(lightIntensity);
        bullet.SetOn_TrailState(trailTime, trailStartWidth * actualAllyState.attackSize.value, extraGradient);

        // 이미지
        bullet.thisSr.sprite = bulletSprite;
    }

    #endregion

    #region State (Bullet)

    private BulletState Get_BulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Ally, id),
                new DmgState(eDamageType.Physics, actualAllyState.dmg.value),
                new CriticalState(actualAllyState.criticalChacne.value, 1 + actualAllyState.criticalDmg.value),
                new KnockbackState(false, 0, 0)),
            checkIsCritical: true,
            muzzleSpeed: actualAllyState.muzzleSpeed.value,
            aliveTime: 10f);
    }

    private BulletState_PosAndRot Get_BulletState_PosAndRot()
    {
        return new BulletState_PosAndRot(
            bulletSpawnTF.position, 
            (enemy.transform.position - this.transform.position).normalized, 
            0);
    }

    private BulletState_Size Get_BulletState_Size()
    {
        return new BulletState_Size(
            bulletObjSize * actualAllyState.attackSize.value,
            bulletColSize * actualAllyState.attackSize.value);
    }

    #endregion

    #region Set (Ping)

    private void Set_PingedState() // 핑 상태
    {
        // 따라가기
        if (Is_FollowState(enemy.transform.position, forEnemyDis, true))
        {
            Set_NavDir(enemy.transform);
            Set_Attacking(false);
            Set_AllyStateMode(eAllyStateMode.Move);
        }
        // 공격
        else
        {
            Stop_Follow();
            Set_Attacking(true);
            Set_AllyStateMode(eAllyStateMode.Attack);
        }
    }


    private void Set_NoPingedState() // 일반 상태
    {
        EnemyController closestEnemy = EnemyManager.instance.Get_ClosestEnemy(gameObject);

        bool isFollow = true;
        if (closestEnemy != null)
            isFollow = Is_FollowState(closestEnemy.transform.position, forEnemyDis, true);
        
        // 따라가기
        if (Is_FollowState(randomPos, 1f, false))
        {
            if (isFollow)
            {
                Set_NavDir(randomPos);
                Set_Attacking(false);
                enemy = null;
                Set_AllyStateMode(eAllyStateMode.Move);
            }
            else
            {
                Stop_Follow();
                Set_Attacking(true);
                enemy = closestEnemy;
                Set_AllyStateMode(eAllyStateMode.Attack);
            }

        }
        // 정지
        else
        {
            Stop_Follow();

            if (isFollow)
            {
                Set_Attacking(false);
                enemy = null;
                Set_AllyStateMode(eAllyStateMode.Idle);
            }
            else
            {
                Set_Attacking(true);
                enemy = closestEnemy;
                Set_AllyStateMode(eAllyStateMode.Attack);
            }
        }


    }

    #endregion

}
