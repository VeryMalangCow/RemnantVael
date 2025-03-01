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
    [SerializeField] public StatusEffect_Temporary_WithoutAmount ShieldBuff;
    [SerializeField] private float ShieldPercent = 10;
    [SerializeField] private float ShieldDurTime = 10;

    [Space(10)]
    [Header("-- Flame")]
    [SerializeField] public StatusEffect_Temporary_WithAmount FlameStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount InfernoStack;

    [Space(10)]
    [Header("-- Cold")]
    [SerializeField] public StatusEffect_Temporary_WithAmount ColdStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount AbsoluteZeroStack;

    [Space(10)]
    [Header("-- Electricity")]
    [SerializeField] public StatusEffect_Temporary_WithAmount ElectricityStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount PlasmaStack;

    [Space(10)]
    [Header("-- Corrosion")]
    [SerializeField] public StatusEffect_Temporary_WithAmount CorrosionStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount DecayStack;


    [HideInInspector] public EnemyController Enemy;

    #endregion

    #region Offset

    public void Offset(EnemyController _Enemy)
    {
        Enemy = _Enemy;

        #region Normal Status Buff

        ShieldBuff = new StatusEffect_Temporary_WithoutAmount(
            Enemy, ShieldDurTime,
            ShieldGainEffect, ShieldReduceEffect,
            EnemyManager.Instance.ShieldIcon);

        #endregion

        #region Normal Status Debuff

        FlameStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Flame, 30, 3f, 1, false,
            null, FlameReduceEffect, FlameFullStack,
            EnemyManager.Instance.FlameIcon);

        ColdStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Cold, 15, 6f, 1, true,
            null, null, ColdFullStack,
            EnemyManager.Instance.ColdIcon);

        ElectricityStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Electricity, 20, 5f, 1, true,
            ElectricityGainEffect, null, ElectricityFullStack,
            EnemyManager.Instance.ElectricityIcon);

        CorrosionStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Corrosion, 25, 4f, 1, false,
            null, null, CorrosionFullStack,
            EnemyManager.Instance.CorrosionIcon);

        #endregion

        #region High Level Status Debuff

        InfernoStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.InfernoIcon, 
            null, null, 3);

        AbsoluteZeroStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.AbsoluteZeroIcon, 
            null, null, 3);

        PlasmaStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.PlasmaIcon, 
            null, null, 3);

        DecayStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.DecayIcon, 
            null, null, 3);

        #endregion
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        ShieldBuff.Set_Clear();

        FlameStack.Set_Clear();
        InfernoStack.Set_Clear();

        ColdStack.Set_Clear();
        AbsoluteZeroStack.Set_Clear();

        ElectricityStack.Set_Clear();
        PlasmaStack.Set_Clear();

        CorrosionStack.Set_Clear();
        DecayStack.Set_Clear();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        ShieldBuff.Caculate_Cooltime(deltaTime);
        FlameStack.Caculate_Cooltime(deltaTime);
        ColdStack.Caculate_Cooltime(deltaTime);
        ElectricityStack.Caculate_Cooltime(deltaTime);
        CorrosionStack.Caculate_Cooltime(deltaTime);

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FlameStack.Gain_Stack(5, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ColdStack.Gain_Stack(5, true);
        }    
    }


    #endregion

    #region Buff

    public void ShieldGain(float _Value)
    {
        ShieldPercent = _Value;
        ShieldBuff.Gain_Stack(1, false);
    }

    private void ShieldGainEffect()
    {
        Enemy.Set_PercentSP(ShieldPercent);
    }

    private void ShieldReduceEffect()
    {
        Enemy.Set_PercentSP(0);
    }

    #endregion

    #region Debuff

    // 화염 속성이 줄어들때
    private void FlameReduceEffect()
    {
        float dmg = 
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 0.01f
            * FlameStack.CurrentStack
            * (InfernoStack.CurrentStack + 1);

        Enemy.Take_Damaged_NoneExtraEffect(eDamageType.Physics, dmg);
    }

    // 전기 속성을 얻을 때
    private void ElectricityGainEffect()
    {
        float dmg = 
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 0.005f
            * ElectricityStack.CurrentStack
            * (PlasmaStack.CurrentStack + 1);

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
            List<EnemyController> closerEnemies = EnemyManager.Instance.Get_CloserEnemies(targetEnemy.transform.position, 10f);

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
                if (!targetEnemy.Is_ExistWall(targetEnemy.transform, closerEnemies[i].transform) &&
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
            targetEnemies[i].Take_Damaged_NoneExtraEffect(eDamageType.Energy, dmg);
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

    // 각각 속성이 최대치일 때
    private void FlameFullStack()
    {
        FlameStack.Reduce_Stack(30);
        InfernoStack.Gain_Stack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 3f;
        Enemy.Take_Damaged_NoneExtraEffect(eDamageType.Physics, dmg);

    }
    private void ColdFullStack()
    {
        ColdStack.Reduce_Stack(15);
        AbsoluteZeroStack.Gain_Stack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 1.5f;
        Enemy.Take_Damaged_NoneExtraEffect(eDamageType.Energy, dmg);

    }
    private void ElectricityFullStack()
    {
        ElectricityStack.Reduce_Stack(20);
        PlasmaStack.Gain_Stack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 2f;
        Enemy.Take_Damaged_NoneExtraEffect(eDamageType.Energy, dmg);

    }
    private void CorrosionFullStack()
    {
        CorrosionStack.Reduce_Stack(25);
        DecayStack.Gain_Stack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 2.5f;
        Enemy.Take_Damaged_NoneExtraEffect(eDamageType.Physics, dmg);

    }

    #endregion
}

#region Status Effect 

[Serializable]
public class StatusEffect
{
    #region Value

    public delegate void EffectDele();

    [HideInInspector] public EnemyController Enemy;
    [HideInInspector] public ModifyBuffIcon BuffIconUI = null;
    [HideInInspector] public Sprite IconSprite;

    public bool IsOn;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect(EnemyController _Enemy, Sprite _IconSprite)
    {
        IsOn = false;

        Enemy = _Enemy;
        IconSprite = _IconSprite;
    }

    #endregion

    #region Clear

    public virtual void Set_Clear()
    {
        IsOn = false;
    }

    #endregion
}

#endregion

#region Temporary Effect

[Serializable]
public class StatusEffect_Temporary : StatusEffect
{
    #region Value

    public float MaxCooltime;
    public float CurrentCooltime;

    protected EffectDele GainDele = null;
    protected EffectDele ReduceDele = null;

    #endregion

    #region Contruct
    // 생성자
    public StatusEffect_Temporary(
        EnemyController _Enemy, float _MaxCooltime,
        EffectDele _GainFunc, EffectDele _ReduceFunc, Sprite _IconSprite)
        : base(_Enemy, _IconSprite)
    {
        Enemy = _Enemy;

        MaxCooltime = _MaxCooltime;
        CurrentCooltime = 0;

        GainDele = _GainFunc;
        ReduceDele = _ReduceFunc;

        IconSprite = _IconSprite;
    }
    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        if (BuffIconUI == null)
        { Start_FirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    public virtual void Reduce_Stack(int _GainAmount)
    {
        if (ReduceDele != null)
        { ReduceDele(); }
    }

    // 버프 시작
    protected virtual void Start_FirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.TemporaryBuffUI.Get_BuffIconUI();
            BuffIconUI.SetOn(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.TemporaryBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
        CurrentCooltime = 0;
    }




    public virtual void Caculate_Cooltime(float _DeltaTime)
    {
        BuffIconUI.Set_BuffState(CurrentCooltime / MaxCooltime);
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Temporary_WithAmount : StatusEffect_Temporary
{
    #region Value

    public eStatusEffect StatusType;

    public int MaxStack;
    public int CurrentStack;
    public int OnceTimeReduceAmount;

    private bool IsResetWhenGain;

    protected EffectDele FullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Temporary_WithAmount(
        EnemyController _Enemy, eStatusEffect _StatusType, int _MaxStack, float _MaxCooltime, int _OnceTimeReduceAmount, bool _IsResetWhenGain,
        EffectDele _GainFunc, EffectDele _ReduceFunc,  EffectDele _FullStack,
        Sprite _IconSprite) 
        : base(_Enemy, _MaxCooltime, _GainFunc, _ReduceFunc, _IconSprite)
    {
        StatusType = _StatusType;

        MaxStack = _MaxStack;
        CurrentStack = 0;
        OnceTimeReduceAmount = _OnceTimeReduceAmount;

        IsResetWhenGain = _IsResetWhenGain;

        FullStack = _FullStack;
    }

    #endregion

    #region Func

    // 버프 증가
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        if (IsResetWhenGain)
        { 
            CurrentCooltime = 0; 
        }

        base.Gain_Stack(_GainAmount, _ShowTxt);

        if (CurrentStack >= MaxStack && FullStack != null) 
        {
            FullStack();
        }
    }

    // 버프 감소
    public override void Reduce_Stack(int _ReduceAmount)
    {
        base.Reduce_Stack(_ReduceAmount);

        CurrentStack = Math.Clamp(CurrentStack - _ReduceAmount, 0, MaxStack);
        if (CurrentStack <= 0)
        {
            Remove_AllStack();
        }
    }


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
        CurrentStack = 0;
    }

    // 쿨타임
    public override void Caculate_Cooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                CurrentCooltime -= MaxCooltime;
                Reduce_Stack(1);
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }

        if (BuffIconUI != null)
        {
            base.Caculate_Cooltime(_DeltaTime);
            BuffIconUI.Set_BuffState(CurrentStack);
        }

    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Temporary_WithoutAmount : StatusEffect_Temporary
{
    #region Contruct

    public StatusEffect_Temporary_WithoutAmount(
        EnemyController _Enemy, float _MaxCooltime, 
        EffectDele _GainFunc, EffectDele _ReduceFunc, 
        Sprite _IconSprite)
        : base(_Enemy, _MaxCooltime, _GainFunc, _ReduceFunc, _IconSprite)
    { }

    #endregion

    #region Func

    // 버프 획득
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        CurrentCooltime = 0;
        base.Gain_Stack(_GainAmount, _ShowTxt);
    }

    // 버프 제거
    public override void Remove_AllStack()
    {
        base.Reduce_Stack(0);
        base.Remove_AllStack();
    }

    // 쿨타임
    public override void Caculate_Cooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                Remove_AllStack();
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }

        if (BuffIconUI != null)
        {
            base.Caculate_Cooltime(_DeltaTime);
        }

    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

#endregion

#region Permanent Effect

[Serializable]
public class StatusEffect_Permanent : StatusEffect
{
    #region Value

    protected EffectDele GainDele = null;

    #endregion

    #region Contruct

    public StatusEffect_Permanent(
        EnemyController _Enemy, Sprite _IconSprite,
        EffectDele _GainFunc)
        : base(_Enemy, _IconSprite)
    {
        GainDele = _GainFunc;
    }

    #endregion

    #region Func

    // 버프 증가
    public virtual void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        if (BuffIconUI == null)
        { Start_FirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    // 필요 없음!

    // 버프 시작
    protected virtual void Start_FirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.PermanentBuffUI.Get_BuffIconUI();
            BuffIconUI.SetOn(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void Remove_AllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.PermanentBuffUI.Remove_BuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Permanent_WithAmount : StatusEffect_Permanent
{
    #region Value

    public int MaxStack;
    public int CurrentStack;

    protected EffectDele FullStack = null;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect_Permanent_WithAmount(
        EnemyController _Enemy, Sprite _IconSprite, 
        EffectDele _GainFunc, EffectDele _FullStack,
        int _MaxStack)
        : base(_Enemy, _IconSprite, _GainFunc)
    {
        MaxStack = _MaxStack;
        FullStack = _FullStack;
        CurrentStack = 0;
    }
    #endregion

    #region Func

    // 버프 증가
    public override void Gain_Stack(int _GainAmount, bool _ShowTxt)
    {
        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        base.Gain_Stack(_GainAmount, _ShowTxt);
        BuffIconUI.Set_BuffState(CurrentStack);

        if (CurrentStack >= MaxStack && FullStack != null)
        {
            FullStack();
        }
    }

    // 버프 감소
    // 필요 없음!


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
        CurrentStack = 0;
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

[Serializable]
public class StatusEffect_Permanent_WithoutAmount : StatusEffect_Permanent
{
    #region Contruct

    public StatusEffect_Permanent_WithoutAmount(
        EnemyController _Enemy, Sprite _IconSprite, EffectDele _GainFunc)
        : base(_Enemy, _IconSprite, _GainFunc)
    {

    }

    #endregion

    #region Func

    // 버프 증가
    // 필요 없음! 

    // 버프 감소
    // 필요 없음!


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void Remove_AllStack()
    {
        base.Remove_AllStack();
    }

    #endregion

    #region Clear

    public override void Set_Clear()
    {
        base.Set_Clear();
        Remove_AllStack();
    }

    #endregion
}

#endregion
