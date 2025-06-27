using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AllySyncManager : Singleton<AllySyncManager>
{
    #region Value

    public delegate void ActivityFuncDele_Sync(AllyController _Ally, int _Rank, EnemyController _EC = null, BulletController _Bullet = null);

    [HideInInspector] public List<ActivityFuncDele_Sync> ActivitySyncFuncList = new List<ActivityFuncDele_Sync>();

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        ActivitySyncFuncList = Init_DelegateList_Sync("Activity_Sync_");
    }

    #endregion

    #region Get

    public ActivityFuncDele_Sync Get_CollectActivity_Sync(int _ID)
    {
        return ActivitySyncFuncList[_ID];
    }

    public List<ActivityFuncDele_Sync> Init_DelegateList_Sync(string _MethodPrefix)
    {
        List<ActivityFuncDele_Sync> delegateList = new List<ActivityFuncDele_Sync>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(_MethodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 4 &&
                method.GetParameters()[0].ParameterType == typeof(AllyController) &&
                method.GetParameters()[1].ParameterType == typeof(int) &&
                method.GetParameters()[2].ParameterType == typeof(EnemyController) &&
                method.GetParameters()[3].ParameterType == typeof(BulletController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(_MethodPrefix.Length);
                return int.TryParse(numberPart, out int result) ? result : int.MaxValue;
            });

        foreach (MethodInfo method in filteredMethods)
        {
            try
            {
                ActivityFuncDele_Sync del = method.IsStatic
                    ? (ActivityFuncDele_Sync)Delegate.CreateDelegate(typeof(ActivityFuncDele_Sync), method)
                    : (ActivityFuncDele_Sync)Delegate.CreateDelegate(typeof(ActivityFuncDele_Sync), this, method);

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

    #region Func (Synchrony)

    // 유도
    private void Activity_Sync_000(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        _Bullet.Set_Guided(true, _Rank);
    }

    // 화염
    private void Activity_Sync_001(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Flame, _Rank, _Enemy);
    }

    // 냉기
    private void Activity_Sync_002(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Cold, _Rank, _Enemy);
    }

    // 전기
    private void Activity_Sync_003(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Electricity, _Rank, _Enemy);
    }

    // 부식
    private void Activity_Sync_004(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        Activity_InflictStatusOneMoreEffect(eStatusEffect.Corrosion, _Rank, _Enemy);
    }

    // 치명타 발생 => 공격력 버프
    private void Activity_Sync_005(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        _Ally.BuffController.Get_AllyBuff("Sync005").Gain_Buff();
    }

    // 치명타 배수 버프
    private void Activity_Sync_006(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {
        _Ally.BuffController.Get_AllyBuff("Sync006").Gain_Buff();
    }

    // 기본 공격 적중 => 공속 버프
    private void Activity_Sync_007(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {

    }

    // 공격 일정 시간 하지 않으면 => 공격력 버프
    private void Activity_Sync_008(AllyController _Ally, int _Rank, EnemyController _Enemy = null, BulletController _Bullet = null)
    {

    }

    #endregion

    #region Unique

    // 상태이상을 한번 더 가하는 함수
    private void Activity_InflictStatusOneMoreEffect(eStatusEffect _Kind, int _Rank, EnemyController _Enemy)
    {
        if (UnityEngine.Random.Range(0, 100) < new List<int>() { 25, 60, 100 }[_Rank - 1])
        {
            switch (_Kind)
            {
                case eStatusEffect.Flame:
                    _Enemy.BuffController.FlameStack.ReGain_Stack();
                    return;

                case eStatusEffect.Cold:
                    _Enemy.BuffController.ColdStack.ReGain_Stack();
                    return;

                case eStatusEffect.Electricity:
                    _Enemy.BuffController.ElectricityStack.ReGain_Stack();
                    return;

                case eStatusEffect.Corrosion:
                    _Enemy.BuffController.CorrosionStack.ReGain_Stack();
                    return;

                default: return;
            }
        }
    }

    #endregion
}
