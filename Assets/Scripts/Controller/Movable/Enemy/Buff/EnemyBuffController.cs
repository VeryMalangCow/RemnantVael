using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("=== Buff")]
    [SerializeField] public EnemyController Enemy;

    [Space(10)]
    [Header("-- Flame")]
    [SerializeField] public StatusEffect FlameStack;

    [Space(10)]
    [Header("-- Cold")]
    [SerializeField] public StatusEffect ColdStack;

    [Space(10)]
    [Header("-- Electricity")]
    [SerializeField] public StatusEffect ElectricityStack;

    [Space(10)]
    [Header("-- Corrosion")]
    [SerializeField] public StatusEffect CorrosionStack;

    #endregion

    #region Framework

    private void Start()
    {
        FlameStack = new StatusEffect(eStatusEffect.Flame, 
            50, 3f, false, 
            null, FlameReduceEffect);

        ColdStack = new StatusEffect(eStatusEffect.Cold, 
            30, 6f, true, 
            null, null);

        ElectricityStack = new StatusEffect(eStatusEffect.Electricity, 
            40, 5f, true, 
            null, null);

        CorrosionStack = new StatusEffect(eStatusEffect.Corrosion, 
            50, 4f, false, 
            null, null);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        FlameStack.UpdateCooltime(deltaTime);
        ColdStack.UpdateCooltime(deltaTime);
        ElectricityStack.UpdateCooltime(deltaTime);
        CorrosionStack.UpdateCooltime(deltaTime);

        /*if (Input.GetKeyDown(KeyCode.V))
        {
            ElectricityStack.GainStack(40);
        }*/
    }


    #endregion

    #region Debuff

    private void FlameReduceEffect()
    {
        float dmg = PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState;
        dmg *= 0.01f;
        dmg *= FlameStack.CurrentStack;
        Debug.Log(dmg);
        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Physics, dmg);
    }

    #endregion
}


[Serializable]
public class StatusEffect
{
    public bool IsOn;
    public eStatusEffect StatusType;

    public int MaxStack;
    public int CurrentStack;

    public float MaxCooltime;
    public float CurrentCooltime;

    private bool IsResetWhenGain;

    public delegate void EffectDele();
    EffectDele GainDele = null;
    EffectDele ReduceDele = null;

    // 생성자
    public StatusEffect(eStatusEffect _StatusType, int _MaxStack, float _MaxCooltime, bool _IsResetWhenGain,
        EffectDele _GainFunc, EffectDele _ReduceFunc)
    {
        IsOn = false;
        StatusType = _StatusType;

        MaxStack = _MaxStack;
        CurrentStack = 0;

        MaxCooltime = _MaxCooltime;
        CurrentCooltime = 0;

        IsResetWhenGain = _IsResetWhenGain;


        GainDele = _GainFunc;
        ReduceDele = _ReduceFunc;
    }

    // 버프 증가
    public void GainStack(int _GainAmount)
    {
        if (GainDele != null)
        { GainDele(); }

        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);
        IsOn = true;

        if (IsResetWhenGain)
        { 
            CurrentCooltime = 0; 
        }
    }

    // 버프 감소
    public void ReduceStack()
    {
        if (ReduceDele != null)
        { ReduceDele(); }

        CurrentStack = Math.Clamp(CurrentStack - 1, 0, MaxStack);
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
                ReduceStack();
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }
    }
}
