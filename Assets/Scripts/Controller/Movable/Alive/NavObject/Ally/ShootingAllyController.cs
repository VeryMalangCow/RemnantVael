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
    [SerializeField] private bool IsAlwaysStatus;
    [SerializeField] private eStatusEffect StateType;


    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Sprite BulletSprite;
    [SerializeField] private Vector2 BulletObjSize;
    [SerializeField] private Vector2 BulletColSize;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Transform BulletSpawnTF;

    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] private float TrailTime;
    [SerializeField] private float TrailStartWidth;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] private float LightIntensity;

    #endregion

    #region - Hide

    [HideInInspector] private bool IsAttacking = false;

    [HideInInspector] private float CurrentRof = 0f;

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
                yield return new WaitForSeconds(FollowInitDelay);
            }
            else
            {
                Set_NoPingedState();
                yield return new WaitForSeconds(FollowInitDelay);
            }
        }
    }

    #endregion

    #region Attacking

    private void Set_Attacking(bool _OnOff)
    {
        if (IsAttacking != _OnOff)
        {
            IsAttacking = _OnOff;
        }
    }

    private void Set_CaculateAttack(float _DeltaTime)
    {
        if (CurrentRof < 1)
        {
            CurrentRof += _DeltaTime * ActualAllyState.rof.value;
        }

        if (!IsAttacking || Enemy == null) return;

        if (CurrentRof >= 1)
        {
            CurrentRof -= 1;
            Play_Attack(PoolingManager.instance.Get_OP_AllyBullet());
        }
    }

    private void Play_Attack(AllyBulletController _Bullet)
    {
        // 총알 스탯과 SortingOrder 설정
        _Bullet.Set_State(
            Get_BulletState(),
            _State_PosAndRot: Get_BulletState_PosAndRot(),
            _State_Size: Get_BulletState_Size(),
            _State_Anim: null,
            _State_Effect: null,
            0.5f);

        _Bullet.ThisSR.color = this.ThisExtraColor;

        // 상태이상 총알이면
        if (IsAlwaysStatus)
            _Bullet.State.Set_Status(IsAlwaysStatus, StateType);
        
        // Sync
        ActiveAlly_Fire(_Bullet, null);

        // 모듈 싱크 효과 => 사격 후
        ActiveAlly_AfterFire();

        _Bullet.SetOn_LightIntensity(LightIntensity);
        _Bullet.SetOn_TrailState(TrailTime, TrailStartWidth * ActualAllyState.attackSize.value, ThisExtraGradient);

        // 이미지
        _Bullet.ThisSR.sprite = BulletSprite;
    }

    #endregion

    #region State (Bullet)

    private BulletState Get_BulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Ally, id),
                new DmgState(eDamageType.Physics, ActualAllyState.dmg.value),
                new CriticalState(ActualAllyState.criticalChacne.value, 1 + ActualAllyState.criticalDmg.value),
                new KnockbackState(false, 0, 0)),
            checkIsCritical: true,
            muzzleSpeed: ActualAllyState.muzzleSpeed.value,
            aliveTime: 10f);
    }

    private BulletState_PosAndRot Get_BulletState_PosAndRot()
    {
        return new BulletState_PosAndRot(
            BulletSpawnTF.position, 
            (Enemy.transform.position - this.transform.position).normalized, 
            0);
    }

    private BulletState_Size Get_BulletState_Size()
    {
        return new BulletState_Size(
            BulletObjSize * ActualAllyState.attackSize.value,
            BulletColSize * ActualAllyState.attackSize.value);
    }

    #endregion

    #region Set (Ping)

    private void Set_PingedState() // 핑 상태
    {
        // 따라가기
        if (Is_FollowState(Enemy.transform.position, ForEnemyDis, true))
        {
            Set_NavDir(Enemy.transform);
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
            isFollow = Is_FollowState(closestEnemy.transform.position, ForEnemyDis, true);
        
        // 따라가기
        if (Is_FollowState(RandomPos, 1f, false))
        {
            if (isFollow)
            {
                Set_NavDir(RandomPos);
                Set_Attacking(false);
                Enemy = null;
                Set_AllyStateMode(eAllyStateMode.Move);
            }
            else
            {
                Stop_Follow();
                Set_Attacking(true);
                Enemy = closestEnemy;
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
                Enemy = null;
                Set_AllyStateMode(eAllyStateMode.Idle);
            }
            else
            {
                Set_Attacking(true);
                Enemy = closestEnemy;
                Set_AllyStateMode(eAllyStateMode.Attack);
            }
        }


    }

    #endregion

}
