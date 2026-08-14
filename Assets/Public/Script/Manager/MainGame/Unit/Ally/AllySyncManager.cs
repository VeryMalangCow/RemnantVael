using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AllySyncManager : Singleton<AllySyncManager>
{
    #region Value

    public delegate void ActivityFuncDele_Sync(
        AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null);

    [HideInInspector] public ActivityFuncDele_Sync[] activitySyncFuncList;

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        activitySyncFuncList = Init_DelegateList_Sync("Activity_Sync_");
    }

    #endregion

    #region Get

    public ActivityFuncDele_Sync Get_CollectActivity_Sync(int id)
    {
        return activitySyncFuncList[id];
    }

    public ActivityFuncDele_Sync[] Init_DelegateList_Sync(string methodPrefix)
    {
        List<ActivityFuncDele_Sync> delegateList = new List<ActivityFuncDele_Sync>();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(methodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 5 &&
                method.GetParameters()[0].ParameterType == typeof(AllyController) &&
                method.GetParameters()[1].ParameterType == typeof(int) &&
                method.GetParameters()[2].ParameterType == typeof(EnemyController) &&
                method.GetParameters()[3].ParameterType == typeof(BulletController) &&
                method.GetParameters()[4].ParameterType == typeof(DroppingBombController))
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(methodPrefix.Length);
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

        return delegateList.ToArray();
    }

    #endregion

    #region Func (Synchrony)

    // 유도
    private void Activity_Sync_000(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        if (bullet != null)
            bullet.Set_Guided(true, rank);
    }

    // 화염
    private void Activity_Sync_001(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        Activity_InflictStatusOneMoreEffect(StatusEffectType.Flame, rank, enemy);
    }

    // 냉기
    private void Activity_Sync_002(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        Activity_InflictStatusOneMoreEffect(StatusEffectType.Cold, rank, enemy);
    }

    // 전기
    private void Activity_Sync_003(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        Activity_InflictStatusOneMoreEffect(StatusEffectType.Electricity, rank, enemy);
    }

    // 부식
    private void Activity_Sync_004(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        Activity_InflictStatusOneMoreEffect(StatusEffectType.Corrosion, rank, enemy);
    }

    // 치명타 발생 => 공격력 버프
    private void Activity_Sync_005(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        ally.buffController.Get_AllyBuff("Sync005").Gain_Buff();
    }

    // 치명타 배수 버프
    private void Activity_Sync_006(AllyController ally, int rank,
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        ally.buffController.Get_AllyBuff("Sync006").Gain_Buff();
    }

    // 기본 공격 적중 => 공속 버프
    private void Activity_Sync_007(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        ally.buffController.Get_AllyBuff("Sync007").Gain_Buff();
    }

    // 공격 일정 시간 하지 않으면 => 공격력 버프
    private void Activity_Sync_008(AllyController ally, int rank, 
        EnemyController enemy = null, 
        BulletController bullet = null,
        DroppingBombController droppingBullet = null)
    {
        ally.buffController.Get_AllyBuff("Sync008").Reduce_Buff(10);
    }

    #endregion

    #region Unique

    // 상태이상을 한번 더 가하는 함수
    private void Activity_InflictStatusOneMoreEffect(StatusEffectType kind, int rank, EnemyController enemy)
    {
        if (UnityEngine.Random.Range(0, 100) < new List<int>() { 25, 60, 100 }[rank - 1])
        {
            switch (kind)
            {
                case StatusEffectType.Flame:
                    enemy.buff.flameStack.ReGain_Stack();
                    return;

                case StatusEffectType.Cold:
                    enemy.buff.coldStack.ReGain_Stack();
                    return;

                case StatusEffectType.Electricity:
                    enemy.buff.electricityStack.ReGain_Stack();
                    return;

                case StatusEffectType.Corrosion:
                    enemy.buff.corrosionStack.ReGain_Stack();
                    return;

                default: return;
            }
        }
    }

    #endregion
}
