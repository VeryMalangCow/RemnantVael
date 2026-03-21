using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ModuleItemActivityManager : Singleton<ModuleItemActivityManager>
{
    #region Value

    public delegate void ActivityFuncDele_MI(int rank, EnemyController enemy = null);
    public delegate void ActivityFuncDele_MC(int rank, EnemyController enemy = null, BulletController bullet = null);

    [HideInInspector] public ActivityFuncDele_MI[] activityMIFuncList;
    [HideInInspector] public ActivityFuncDele_MC[] activityMCFuncList;

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        activityMIFuncList = InitDelegateListMI("Activity_MI_");
        activityMCFuncList = InitDelegateListMC("Activity_MC_");
    }

    #endregion

    #region Get

    public ActivityFuncDele_MI Get_CollectActivity_MI(int id) => activityMIFuncList[id];
    public ActivityFuncDele_MC Get_CollectActivity_MC(int id) => activityMCFuncList[id];
    

    public ActivityFuncDele_MI[] InitDelegateListMI(string methodPrefix)
    {
        List<ActivityFuncDele_MI> delegateList = new List<ActivityFuncDele_MI>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(methodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 2 &&
                method.GetParameters()[0].ParameterType == typeof(int) &&
                method.GetParameters()[1].ParameterType == typeof(EnemyController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(methodPrefix.Length);
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

        return delegateList.ToArray();
    }
    public ActivityFuncDele_MC[] InitDelegateListMC(string methodPrefix)
    {
        List<ActivityFuncDele_MC> delegateList = new List<ActivityFuncDele_MC>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(methodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 3 &&
                method.GetParameters()[0].ParameterType == typeof(int) &&
                method.GetParameters()[1].ParameterType == typeof(EnemyController) &&
                method.GetParameters()[2].ParameterType == typeof(BulletController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(methodPrefix.Length);
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

        return delegateList.ToArray();
    }


    #endregion

    #region Func (Module Item)

    #region Derivative Bullet

    // 유도탄 발사
    private void Activity_MI_000(int rank, EnemyController enemy = null)
    {
        Activity_Derivative(rank, eDamageType.Energy, PoolingManager.instance.moduleItem_000_Bullets);
    }

    private void Activity_MI_001(int rank, EnemyController enemy = null)
    {
        Activity_Derivative(rank, eDamageType.Physics, PoolingManager.instance.moduleItem_001_Bullets);
    }

    #endregion

    #region Inflict Status When Critical

    // 크리티컬 시, 상태이상 부여
    private void Activity_MI_002(int rank, EnemyController enemy = null)
    {
        Activity_InflictStatusEffect(eStatusEffect.Flame, rank, enemy);
    }

    private void Activity_MI_003(int rank, EnemyController enemy = null)
    {
        Activity_InflictStatusEffect(eStatusEffect.Cold, rank, enemy);
    }

    private void Activity_MI_004(int rank, EnemyController enemy = null)
    {
        Activity_InflictStatusEffect(eStatusEffect.Electricity, rank, enemy);
    }

    private void Activity_MI_005(int rank, EnemyController enemy = null)
    {
        Activity_InflictStatusEffect(eStatusEffect.Corrosion, rank, enemy);
    }

    #endregion

    #endregion

    #region Func (Synchrony)

    // 유도
    private void Activity_MC_000(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        bullet.Set_Guided(true, rank);
    }

    // 화염
    private void Activity_MC_001(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Flame, rank, enemy);
    }

    // 냉기
    private void Activity_MC_002(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Cold, rank, enemy);
    }

    // 전기
    private void Activity_MC_003(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Electricity, rank, enemy);
    }

    // 부식
    private void Activity_MC_004(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Corrosion, rank, enemy);
    }

    // 치명타 발생 => 공격력 버프
    private void Activity_MC_005(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        BuffManager.instance.Gain_Buff(1);
    }

    // 치명타 배수 버프
    private void Activity_MC_006(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        BuffManager.instance.Gain_Buff(8);
    }

    // 기본 공격 적중 => 공속 버프
    private void Activity_MC_007(int _Rank, EnemyController enemy = null, BulletController bullet = null)
    {
        BuffManager.instance.Gain_Buff(9);
    }

    // 공격 일정 시간 하지 않으면 => 공격력 버프
    private void Activity_MC_008(int rank, EnemyController enemy = null, BulletController bullet = null)
    {
        BuffManager.instance.Reduce_Buff(10);
    }

    #endregion

    #region Unique (MI)

    // 데미지 타입을 통해서, 유도탄을 발사하는 함수
    private void Activity_Derivative(int rank, eDamageType dmgType, TTypePooling<PlayerBulletController> bullet)
    {
        // 편의성
        PlayerController PC = PlayerManager.instance.playerController;
        PlayerWeaponController PCWeapon = PC.baseWeapon;

        // 확률
        if (rank > UnityEngine.Random.Range(0, 10))
        {
            // 데미지 계산
            float dmg = rank * PCWeapon.baseDamage.actualState.Value;

            PlayerBulletController pbc = PoolingManager.instance.Get_OP(bullet);
            Vector2 dir = DevTool.Get_MinFireDir(PC.transform.position);

            // 스폰 탄 스탯

            DmgState dmgState = new DmgState(dmgType, dmg);
            CriticalState criticalState = new CriticalState(0, 1);
            KnockbackState knockbackState = new KnockbackState(false, 0, 0);

            BulletState bulletState = 
                new BulletState(
                    new CombatState(
                        new CombatOwner(eCombatOwner.Player),
                        dmgState, 
                        criticalState, 
                        knockbackState), 
                    false, 
                    PCWeapon.muzzleSpeed.actualState.Value * 0.7f, 2f);
            BulletState_PosAndRot posAndRot = new BulletState_PosAndRot(PC.transform.position, dir, 10);
            BulletState_Size? size = null;
            State_Anim? anim = null;

            pbc.Set_State(bulletState, posAndRot, size, anim, state_Effect: null, 0.35f);
            pbc.Set_Guided(true, rank * rank);
        }
    }

    // 상태이상을 적에게 가하는 함수
    private void Activity_InflictStatusEffect(eStatusEffect kind, int gainAmount, EnemyController enemy)
    {
        switch (kind)
        {
            case eStatusEffect.Flame: 
                enemy.buff.flameStack.Gain_Stack(gainAmount, true, new CombatOwner(eCombatOwner.Player));
                return;

            case eStatusEffect.Cold:
                enemy.buff.coldStack.Gain_Stack(gainAmount, true, new CombatOwner(eCombatOwner.Player));
                return;

            case eStatusEffect.Electricity:
                enemy.buff.electricityStack.Gain_Stack(gainAmount, true, new CombatOwner(eCombatOwner.Player));
                return;

            case eStatusEffect.Corrosion:
                enemy.buff.corrosionStack.Gain_Stack(gainAmount, true, new CombatOwner(eCombatOwner.Player));
                return;

            default: return;
        }
    }

    // 상태이상을 한번 더 가하는 함수
    private void Activity_InflictStatusOneMoreEffect(eStatusEffect kind, int rank, EnemyController enemy)
    {
        if (UnityEngine.Random.Range(0, 100) < new List<int>() { 25, 60, 100 }[rank - 1])
        {
            switch (kind)
            {
                case eStatusEffect.Flame:
                    enemy.buff.flameStack.ReGain_Stack();
                    return;

                case eStatusEffect.Cold:
                    enemy.buff.coldStack.ReGain_Stack();
                    return;

                case eStatusEffect.Electricity:
                    enemy.buff.electricityStack.ReGain_Stack();
                    return;

                case eStatusEffect.Corrosion:
                    enemy.buff.corrosionStack.ReGain_Stack();
                    return;

                default: return;
            }
        }
    }

    #endregion
}
