using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ModuleItemActivityManager : Singleton<ModuleItemActivityManager>
{
    #region Value

    public delegate void ActivityFuncDele_MI(int _Rank, EnemyController _EC = null);
    public delegate void ActivityFuncDele_MC(int _Rank, AllyController _AC = null, BulletController _Bullet = null);

    [HideInInspector] public List<ActivityFuncDele_MI> ActivityMIFuncList = new List<ActivityFuncDele_MI>();
    [HideInInspector] public List<ActivityFuncDele_MC> ActivityMCFuncList = new List<ActivityFuncDele_MC>();

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        ActivityMIFuncList = Init_DelegateList_MI("Activity_MI_");
        ActivityMCFuncList = Init_DelegateList_MC("Activity_MC_");
    }

    #endregion

    #region Get

    public ActivityFuncDele_MI Get_CollectActivity_MI(int _ID)
    {
        return ActivityMIFuncList[_ID];
    }

    public ActivityFuncDele_MC Get_CollectActivity_MC(int _ID)
    {
        return ActivityMCFuncList[_ID];
    }

    public List<ActivityFuncDele_MI> Init_DelegateList_MI(string _MethodPrefix)
    {
        List<ActivityFuncDele_MI> delegateList = new List<ActivityFuncDele_MI>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(_MethodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 2 &&
                method.GetParameters()[0].ParameterType == typeof(int) &&
                method.GetParameters()[1].ParameterType == typeof(EnemyController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(_MethodPrefix.Length);
                return int.TryParse(numberPart, out int result) ? result : int.MaxValue;
            });

        foreach (MethodInfo method in filteredMethods)
        {
            try
            {
                ActivityFuncDele_MI del = method.IsStatic
                    ? (ActivityFuncDele_MI)Delegate.CreateDelegate(typeof(ActivityFuncDele_MI), method)
                    : (ActivityFuncDele_MI)Delegate.CreateDelegate(typeof(ActivityFuncDele_MI), this, method);

                delegateList.Add(del);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"델리게이트 생성 실패: {method.Name} - {ex.Message}");
            }
        }

        return delegateList;
    }

    public List<ActivityFuncDele_MC> Init_DelegateList_MC(string _MethodPrefix)
    {
        List<ActivityFuncDele_MC> delegateList = new List<ActivityFuncDele_MC>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(_MethodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 3 &&
                method.GetParameters()[0].ParameterType == typeof(int) &&
                method.GetParameters()[1].ParameterType == typeof(AllyController) &&
                method.GetParameters()[2].ParameterType == typeof(BulletController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(_MethodPrefix.Length);
                return int.TryParse(numberPart, out int result) ? result : int.MaxValue;
            });

        foreach (MethodInfo method in filteredMethods)
        {
            try
            {
                ActivityFuncDele_MC del = method.IsStatic
                    ? (ActivityFuncDele_MC)Delegate.CreateDelegate(typeof(ActivityFuncDele_MC), method)
                    : (ActivityFuncDele_MC)Delegate.CreateDelegate(typeof(ActivityFuncDele_MC), this, method);

                delegateList.Add(del);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"델리게이트 생성 실패: {method.Name} - {ex.Message}");
            }
        }

        return delegateList;
    }

    #endregion

    #region ModuleItem

    private void Activity_MI_000(int _Rank, EnemyController _EC = null)
    {
        Activity_Derivative(_Rank, eDamageType.Energy, PoolingManager.Instance.MI_000_Bullets);
    }

    private void Activity_MI_001(int _Rank, EnemyController _EC = null)
    {
        Activity_Derivative(_Rank, eDamageType.Physics, PoolingManager.Instance.MI_001_Bullets);
    }

    private void Activity_MI_002(int _Rank, EnemyController _EC = null)
    {
        if (0.5f > UnityEngine.Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Flame, _Rank, _EC);
        }
    }

    private void Activity_MI_003(int _Rank, EnemyController _EC = null)
    {
        if (0.5f > UnityEngine.Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Cold, _Rank, _EC);
        }
    }

    private void Activity_MI_004(int _Rank, EnemyController _EC = null)
    {
        if (0.5f > UnityEngine.Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Electricity, _Rank, _EC);
        }
    }

    private void Activity_MI_005(int _Rank, EnemyController _EC = null)
    {
        if (0.5f > UnityEngine.Random.Range(0f, 1f))
        {
            Activity_InflictStatusEffect(eStatusEffect.Corrosion, _Rank, _EC);
        }
    }

    #endregion

    #region Unique (MI)

    // 데미지 타입을 통해서, 유도탄을 발사하는 함수
    private void Activity_Derivative(int _Rank, eDamageType _DmgType, TTypePooling<PlayerBulletController> _Bullet)
    {
        // 편의성
        PlayerController PC = PlayerManager.Instance.PlayerController;
        PlayerWeaponController PCWeapon = PC.BaseWeapon;

        // 확률
        if ((_Rank * 10) > UnityEngine.Random.Range(0, 100))
        {
            // 데미지 계산
            float dmg = _Rank * PCWeapon.BaseDamage.ActualState.Value;

            PlayerBulletController pbc = PoolingManager.Instance.Get_OP(_Bullet);
            Vector2 dir = DevTool.Get_MinFireDir(PC.transform.position);

            // 스폰 탄 스탯

            DmgState dmgState = new DmgState(_DmgType, dmg);
            CriticalState criticalState = new CriticalState(0, 1);
            KnockbackState knockbackState = new KnockbackState(false, 0, 0);

            BulletState bulletState = new BulletState(new CombatState(dmgState, criticalState, knockbackState), false, PCWeapon.MuzzleSpeed.ActualState.Value * 0.7f, 2f);
            BulletState_PosAndRot posAndRot = new BulletState_PosAndRot(PC.transform.position, dir, 10);
            BulletState_Size? size = null;
            State_Anim? anim = null;

            pbc.Set_State(bulletState, posAndRot, size, anim, _State_Effect: null, 0.35f);
            pbc.Set_Guided(true, _Rank * _Rank);
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

    #region MainChip

    private void Activity_MC_000(int _Rank, AllyController _AC = null, BulletController _Bullet = null)
    {
        _Bullet.Set_Guided(true, _Rank * _Rank, PlayerManager.Instance.Get_PingedEnemy());
    }

    #endregion
}
