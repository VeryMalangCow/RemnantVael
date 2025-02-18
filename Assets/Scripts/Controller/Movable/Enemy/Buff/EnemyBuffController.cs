using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("=== Buff")]

    [Space(10)]
    [Header("-- Shield")]
    [SerializeField] public StatusEffect_WithoutAmount ShieldBuff;

    [Space(10)]
    [Header("-- Flame")]
    [SerializeField] public StatusEffect_WithAmount FlameStack;

    [Space(10)]
    [Header("-- Cold")]
    [SerializeField] public StatusEffect_WithAmount ColdStack;

    [Space(10)]
    [Header("-- Electricity")]
    [SerializeField] public StatusEffect_WithAmount ElectricityStack;

    [Space(10)]
    [Header("-- Corrosion")]
    [SerializeField] public StatusEffect_WithAmount CorrosionStack;

    [HideInInspector] public EnemyController Enemy;

    #endregion

    #region Framework

    private void Start()
    {
        ShieldBuff = new StatusEffect_WithoutAmount(
            10, 
            null, null);

        FlameStack = new StatusEffect_WithAmount(
            eStatusEffect.Flame, 50, 3f, 1, false, 
            null, FlameReduceEffect);

        ColdStack = new StatusEffect_WithAmount(
            eStatusEffect.Cold, 30, 6f, 1, true, 
            null, null);

        ElectricityStack = new StatusEffect_WithAmount(
            eStatusEffect.Electricity, 40, 5f, 1, true,
            ElectricityGainEffect, null);

        CorrosionStack = new StatusEffect_WithAmount(
            eStatusEffect.Corrosion, 50, 4f, 1, false, 
            null, null);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        ShieldBuff.UpdateCooltime(deltaTime);
        FlameStack.UpdateCooltime(deltaTime);
        ColdStack.UpdateCooltime(deltaTime);
        ElectricityStack.UpdateCooltime(deltaTime);
        CorrosionStack.UpdateCooltime(deltaTime);
    }


    #endregion

    #region Debuff

    private void FlameReduceEffect()
    {
        float dmg = PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState;
        dmg *= 0.01f;
        dmg *= FlameStack.CurrentStack;

        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Physics, dmg);
    }

    private void ElectricityGainEffect()
    {
        float dmg = PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState;
        dmg *= 0.005f;
        dmg *= ElectricityStack.CurrentStack;

        List<EnemyController> targetEnemies = new List<EnemyController>();
        EnemyController targetEnemy = Enemy;
        targetEnemies.Add(targetEnemy);

        int t = 0;
        while (true)
        {
            t++;
            if (t > 100)
            {
                Debug.Assert(false);
                return;
            }

            // 범위 내의 적들을 가져옴
            List<EnemyController> closerEnemies = EnemyManager.Instance.GetCloserEnemies(targetEnemy.transform.position, 10f);

            if (closerEnemies.Contains(targetEnemy))
            { closerEnemies.Remove(targetEnemy); }

            // 범위 내에 적들이 없다면
            if (closerEnemies.Count <= 0)
            { break; }

            // 가까운 순서대로 중간에 벽이 없는지를 판단
            bool isExist = false;
            for (int i = 0; i < closerEnemies.Count; i++)
            {
                // 중간에 벽이 없고 + 타깃 적들 리스트에 이미 존재하지 않는다면 => 타겟 지정!
                if (!targetEnemy.IsExistWall(targetEnemy.transform, closerEnemies[i].transform) &&
                    !targetEnemies.Contains(closerEnemies[i]))
                {
                    isExist = true;
                    targetEnemy = closerEnemies[i];
                    targetEnemies.Add(closerEnemies[i]);
                    break;
                }
            }

            // 스택만큼의 적의 수가 연쇄공격을 받음 (최대치) || 더이상 없다면 끝
            if (targetEnemies.Count >= ElectricityStack.CurrentStack || !isExist)
            { break; }
        }

        // 감전 적용의 수만큼 데미지는 증폭
        dmg *= targetEnemies.Count;

        for (int i = 0; i < targetEnemies.Count; i++)
        {
            targetEnemies[i].TakeDamaged_NoneExtraEffect(eDamageType.Energy, dmg);
        }

#if UNITY_EDITOR
        for (int i = 0; i < targetEnemies.Count - 1; i++)
        {
            Debug.DrawRay(targetEnemies[i].transform.position,
                (targetEnemies[i + 1].transform.position - targetEnemies[i].transform.position),
                Color.blue, 3f);
        }
#endif
    }

    #endregion
}

[Serializable]
public class StatusEffect
{
    public bool IsOn;
    public EnemyBuffIconUIController BuffIconUI = null;

    public float MaxCooltime;
    public float CurrentCooltime;

    public delegate void EffectDele();
    protected EffectDele GainDele = null;
    protected EffectDele ReduceDele = null;

    public StatusEffect( 
        float _MaxCooltime, EffectDele _GainFunc, EffectDele _ReduceFunc)
    {
        IsOn = false;

        MaxCooltime = _MaxCooltime;
        CurrentCooltime = 0;

        GainDele = _GainFunc;
        ReduceDele = _ReduceFunc;
    }

    protected void GainStack()
    {
        if (GainDele != null)
        { GainDele(); }
    }

    protected void ReduceStack()
    {
        if (ReduceDele != null)
        { ReduceDele(); }
    }
}

[Serializable]
public class StatusEffect_WithAmount : StatusEffect
{
    public eStatusEffect StatusType;

    public int MaxStack;
    public int CurrentStack;
    public int OnceTimeReduceAmount;

    private bool IsResetWhenGain;

    // 생성자
    public StatusEffect_WithAmount(
        eStatusEffect _StatusType, int _MaxStack, float _MaxCooltime, int _OnceTimeReduceAmount, bool _IsResetWhenGain,
        EffectDele _GainFunc, EffectDele _ReduceFunc) 
        : base(_MaxCooltime, _GainFunc, _ReduceFunc)
    {
        StatusType = _StatusType;

        MaxStack = _MaxStack;
        CurrentStack = 0;
        OnceTimeReduceAmount = _OnceTimeReduceAmount;

        IsResetWhenGain = _IsResetWhenGain;
    }

    // 버프 증가
    public void GainStack(int _GainAmount)
    {
        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);
        IsOn = true;

        if (IsResetWhenGain)
        { 
            CurrentCooltime = 0; 
        }

        base.GainStack();
    }

    // 버프 감소
    public void ReduceStack(int _ReduceAmount)
    {
        base.ReduceStack();

        CurrentStack = Math.Clamp(CurrentStack - _ReduceAmount, 0, MaxStack);
        if (CurrentStack <= 0)
        {
            IsOn = false;
            CurrentCooltime = 0;
        }
    }

    // 쿨타임
    public void UpdateCooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                CurrentCooltime -= MaxCooltime;
                ReduceStack(1);
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }
    }
}

[Serializable]
public class StatusEffect_WithoutAmount : StatusEffect
{
    public StatusEffect_WithoutAmount(
        float _MaxCooltime, 
        EffectDele _GainFunc, EffectDele _ReduceFunc)
        : base(_MaxCooltime, _GainFunc, _ReduceFunc)
    { }

    // 버프 획득
    public void GetStack()
    {
        IsOn = true;
        CurrentCooltime = 0;
        base.GainStack();
    }

    // 버프 제거
    public void RemoveStack()
    {
        IsOn = false;
        CurrentCooltime = 0;
        base.ReduceStack();
    }

    // 쿨타임
    public void UpdateCooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                RemoveStack();
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }
    }
}