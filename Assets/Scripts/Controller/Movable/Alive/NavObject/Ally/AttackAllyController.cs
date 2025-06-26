using System.Collections;
using UnityEngine;

public class AttackAllyController : AllyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Attack")]

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
    [SerializeField] private Gradient TrailGradient;

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
            if (Enemy != null)
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
            CurrentRof = 0;
        }
    }

    private void Set_CaculateAttack(float _DeltaTime)
    {
        if (!IsAttacking || Enemy == null) return;

        if (CurrentRof < 1)
        {
            CurrentRof += _DeltaTime * ActualAllyState.Rof.Value;
        }
        else
        {
            CurrentRof -= 1;
            Play_Attack(PoolingManager.Instance.Get_OP_AllyBullet());
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

        // Sync
        ActiveAlly_Fire(_Bullet);

        _Bullet.SetOn_LightIntensity(LightIntensity);
        _Bullet.SetOn_TrailState(TrailTime, TrailStartWidth, TrailGradient);

        // 이미지
        _Bullet.ThisSR.sprite = BulletSprite;
    }

    #endregion

    #region State (Bullet)

    private BulletState Get_BulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Ally, ID),
                new DmgState(eDamageType.Physics, ActualAllyState.Dmg.Value),
                new CriticalState(0, 0),
                new KnockbackState(false, 0, 0)),
            false,
            1f,
            10f);
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
            BulletObjSize,
            BulletColSize
            );
    }

    #endregion

    #region Set (Ping)

    private void Set_PingedState() // 적에게
    {
        // 따라가기
        if (Is_FollowState(Enemy.transform, ForEnemyDis, true))
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


    private void Set_NoPingedState() // 플레이어에게
    {
        Set_Attacking(false);

        // 따라가기
        if (Is_FollowState(Player.transform, ForPlayerDis, false))
        {
            Set_NavDir(Player.transform);
            Set_AllyStateMode(eAllyStateMode.Move);
        }
        // 정지
        else
        {
            Stop_Follow();
            Set_AllyStateMode(eAllyStateMode.Idle);
        }
    }

    #endregion
}
