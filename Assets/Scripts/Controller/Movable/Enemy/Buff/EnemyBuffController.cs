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

        #region Buff

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
            Enemy, EnemyManager.Instance.FlameIcon, 
            null, null, 3);

        AbsoluteZeroStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.ColdIcon, 
            null, null, 3);

        PlasmaStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.ElectricityIcon, 
            null, null, 3);

        DecayStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.CorrosionIcon, 
            null, null, 3);

        #endregion
    }

    #endregion

    #region Framework


    private void Update()
    {
        float deltaTime = Time.deltaTime;

        ShieldBuff.UpdateCooltime(deltaTime);
        FlameStack.UpdateCooltime(deltaTime);
        ColdStack.UpdateCooltime(deltaTime);
        ElectricityStack.UpdateCooltime(deltaTime);
        CorrosionStack.UpdateCooltime(deltaTime);

        Debug.Log("Test : Need To Delete"); 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FlameStack.GainStack(5, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ColdStack.GainStack(5, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ElectricityStack.GainStack(5, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CorrosionStack.GainStack(5, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ShieldGain(10);
        }
    }


    #endregion

    #region Buff

    public void ShieldGain(float _Value)
    {
        ShieldPercent = _Value;
        ShieldBuff.GainStack(1, false);
    }

    private void ShieldGainEffect()
    {
        Enemy.SetPercentSP(ShieldPercent);
    }

    private void ShieldReduceEffect()
    {
        Enemy.SetPercentSP(0);
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

        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Physics, dmg);
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

    // 각각 속성이 최대치일 때
    private void FlameFullStack()
    {
        FlameStack.ReduceStack(30);
        InfernoStack.GainStack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 3f;
        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Physics, dmg);

    }
    private void ColdFullStack()
    {
        ColdStack.ReduceStack(15);
        AbsoluteZeroStack.GainStack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 1.5f;
        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Energy, dmg);

    }
    private void ElectricityFullStack()
    {
        ElectricityStack.ReduceStack(20);
        PlasmaStack.GainStack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 2f;
        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Energy, dmg);

    }
    private void CorrosionFullStack()
    {
        CorrosionStack.ReduceStack(25);
        DecayStack.GainStack(1, true);

        float dmg =
            PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.BuffedState
            * 2.5f;
        Enemy.TakeDamaged_NoneExtraEffect(eDamageType.Physics, dmg);

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
    [HideInInspector] public EnemyBuffIconUIController BuffIconUI = null;
    [HideInInspector] public Sprite IconSprite;

    public bool IsOn;

    #endregion

    #region Contruct

    // 생성자
    public StatusEffect(EnemyController _Enemy, Sprite _IconSprite)
    {
        Enemy = _Enemy;
        IconSprite = _IconSprite;
    }

    #endregion

}

#endregion

#region Temporary

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
        IsOn = false;

        MaxCooltime = _MaxCooltime;
        CurrentCooltime = 0;

        GainDele = _GainFunc;
        ReduceDele = _ReduceFunc;

        IconSprite = _IconSprite;
    }
    #endregion

    #region Func

    // 버프 증가
    public virtual void GainStack(int _GainAmount, bool _ShowTxt)
    {
        if (BuffIconUI == null)
        { GetFirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    public virtual void ReduceStack(int _GainAmount)
    {
        if (ReduceDele != null)
        { ReduceDele(); }
    }

    // 버프 시작
    protected virtual void GetFirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.TemporaryBuffUI.GetBuffIconUI();
            BuffIconUI.On(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void RemoveAllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.TemporaryBuffUI.ExpiredBuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
        CurrentCooltime = 0;
    }




    public virtual void UpdateCooltime(float _DeltaTime)
    {
        BuffIconUI.SetBuffState(CurrentCooltime / MaxCooltime);
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
    public override void GainStack(int _GainAmount, bool _ShowTxt)
    {
        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        if (IsResetWhenGain)
        { 
            CurrentCooltime = 0; 
        }

        base.GainStack(_GainAmount, _ShowTxt);

        if (CurrentStack >= MaxStack && FullStack != null) 
        {
            FullStack();
        }
    }

    // 버프 감소
    public override void ReduceStack(int _ReduceAmount)
    {
        base.ReduceStack(_ReduceAmount);

        CurrentStack = Math.Clamp(CurrentStack - _ReduceAmount, 0, MaxStack);
        if (CurrentStack <= 0)
        {
            RemoveAllStack();
        }
    }


    // 버프 시작
    // 필요 없음! 

    // 버프 종료
    public override void RemoveAllStack()
    {
        base.RemoveAllStack();
        CurrentStack = 0;
    }

    // 쿨타임
    public override void UpdateCooltime(float _DeltaTime)
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

        if (BuffIconUI != null)
        {
            base.UpdateCooltime(_DeltaTime);
            BuffIconUI.SetBuffState(CurrentStack);
        }

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
    public override void GainStack(int _GainAmount, bool _ShowTxt)
    {
        CurrentCooltime = 0;
        base.GainStack(_GainAmount, _ShowTxt);
    }

    // 버프 제거
    public override void RemoveAllStack()
    {
        base.ReduceStack(0);
        base.RemoveAllStack();
    }

    // 쿨타임
    public override void UpdateCooltime(float _DeltaTime)
    {
        if (IsOn)
        {
            if (MaxCooltime <= CurrentCooltime) // 스택 감소
            {
                RemoveAllStack();
            }
            else // 쿨타임 돌림
            {
                CurrentCooltime += _DeltaTime;
            }
        }

        if (BuffIconUI != null)
        {
            base.UpdateCooltime(_DeltaTime);
        }

    }

    #endregion
}

#endregion

#region Permanent

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
    public virtual void GainStack(int _GainAmount, bool _ShowTxt)
    {
        if (BuffIconUI == null)
        { GetFirstStack(_ShowTxt); }

        if (GainDele != null)
        { GainDele(); }
    }

    // 버프 감소
    // 필요 없음!

    // 버프 시작
    protected virtual void GetFirstStack(bool _ShowTxt)
    {
        if (BuffIconUI == null)
        {
            BuffIconUI = Enemy.HUD.PermanentBuffUI.GetBuffIconUI();
            BuffIconUI.On(IconSprite, _ShowTxt);
        }
        IsOn = true;
    }

    // 버프 종료
    public virtual void RemoveAllStack()
    {
        if (BuffIconUI != null)
        {
            Enemy.HUD.PermanentBuffUI.ExpiredBuffIconUI(BuffIconUI);
            BuffIconUI = null;
        }

        IsOn = false;
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
    public override void GainStack(int _GainAmount, bool _ShowTxt)
    {
        CurrentStack = Math.Clamp(CurrentStack + _GainAmount, 0, MaxStack);

        base.GainStack(_GainAmount, _ShowTxt);
        BuffIconUI.SetBuffState(CurrentStack);

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
    // 필요 없음!

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
}

#endregion
