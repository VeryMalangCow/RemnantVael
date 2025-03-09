using System.Collections.Generic;
using UnityEngine;

public class ModuleItemActivityManager : Singleton<ModuleItemActivityManager>
{
    #region Value

    public delegate void ActivityFuncDele(int _Rank, int BoostLv, EnemyController _EC = null);
    [HideInInspector] public List<ActivityFuncDele> ActivityFuncList = new List<ActivityFuncDele>();

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        ActivityFuncList = new List<ActivityFuncDele>
        {
            Activity_MI_000,
            Activity_MI_001,
            Activity_MI_002,
            Activity_MI_003,
            Activity_MI_004,
            Activity_MI_005,
        };
    }

    #endregion

    #region Get

    public ActivityFuncDele Get_CollectActivity(int _ID)
    {
        return ActivityFuncList[_ID];
    }

    #endregion

    #region ModuleItem

    private void Activity_MI_000(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        Activity_Derivative(_Rank, _BoostLv, eDamageType.Energy, PoolingManager.Instance.MI_000_Bullets);
    }

    private void Activity_MI_001(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        Activity_Derivative(_Rank, _BoostLv, eDamageType.Physics, PoolingManager.Instance.MI_001_Bullets);
    }

    private void Activity_MI_002(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        float percent = 0.25f * _BoostLv;
        if (percent > Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Flame, _Rank, _EC);
        }
    }

    private void Activity_MI_003(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        float percent = 0.25f * _BoostLv;
        if (percent > Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Cold, _Rank, _EC);
        }
    }

    private void Activity_MI_004(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        float percent = 0.25f * _BoostLv;
        if (percent > Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Electricity, _Rank, _EC);
        }
    }

    private void Activity_MI_005(int _Rank, int _BoostLv, EnemyController _EC = null)
    {
        float percent = 0.25f * _BoostLv;
        if (percent > Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Corrosion, _Rank, _EC);
        }
    }

    #endregion

    #region Unique

    // 데미지 타입을 통해서, 유도탄을 발사하는 함수
    private void Activity_Derivative(int _Rank, int _BoostLv, eDamageType _DmgType, TTypePooling<PlayerBulletController> _Bullet)
    {
        // 편의성
        PlayerController PC = PlayerManager.Instance.PlayerController;
        PlayerWeaponController PCWeapon = PC.BaseWeapon;

        // 확률
        if ((_Rank * _BoostLv) > UnityEngine.Random.Range(0, 100))
        {
            // 데미지 계산
            float dmg = _Rank * PCWeapon.BaseDamage.ActualState.Value;

            PlayerBulletController pbc = PoolingManager.Instance.Get_OP(_Bullet);
            Vector2 dir = PCWeapon.Get_Dir(PC.transform.position);

            // 스폰 탄 스탯

            DmgState dmgState = new DmgState(_DmgType, dmg);
            CriticalState criticalState = new CriticalState(0, 1);
            KnockbackState knockbackState = new KnockbackState(false, 0, 0);

            BulletState bulletState = new BulletState(new CombatState(dmgState, criticalState, knockbackState), true, PCWeapon.MuzzleSpeed.ActualState.Value * 0.7f, 2f);
            BulletState_PosAndRot posAndRot = new BulletState_PosAndRot(PC.transform.position, dir, 10);
            BulletState_Size? size = null;
            BulletState_Anim? anim = null;

            pbc.Set_State(bulletState, posAndRot, size, anim, 0.35f);

            // Sorting Layer
            if (PC.TargetObject.gameObject.TryGetComponent(out DepthController hst))
            { pbc.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }
        }
    }

    // 상태이상을 적에게 가하는 함수
    private void Activity_InflictStatusEffect(eStatusEffect _Kind, int _GainAmount, EnemyController _Enemy)
    {
        if (_Kind == eStatusEffect.Flame)
        {
            _Enemy.BuffController.FlameStack.Gain_Stack(_GainAmount, true);
        }
        else if (_Kind == eStatusEffect.Cold)
        {
            _Enemy.BuffController.ColdStack.Gain_Stack(_GainAmount, true);
        }
        else if (_Kind == eStatusEffect.Electricity)
        {
            _Enemy.BuffController.ElectricityStack.Gain_Stack(_GainAmount, true);
        }
        else
        {
            _Enemy.BuffController.CorrosionStack.Gain_Stack(_GainAmount, true);
        }
    }

    #endregion
}
