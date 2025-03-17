using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Shield")]
    [SerializeField] public StatusEffect_Temporary_WithoutAmount ShieldBuff;
    [SerializeField] private float ShieldPercent = 10;
    [SerializeField] private float ShieldDurTime = 10;

    [Space(10)]
    [Header("=== Flame")]
    [SerializeField] public StatusEffect_Temporary_WithAmount FlameStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount InfernoStack;
    [HideInInspector] private static int FlameMax = 30;

    [Space(10)]
    [Header("=== Cold")]
    [SerializeField] public StatusEffect_Temporary_WithAmount ColdStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount AbsoluteZeroStack;
    [HideInInspector] private static int ColdMax = 15;

    [Space(10)]
    [Header("=== Electricity")]
    [SerializeField] public StatusEffect_Temporary_WithAmount ElectricityStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount PlasmaStack;
    [HideInInspector] private static int ElectricityMax = 20;
    [HideInInspector] private static float ElectricityRange = 10f;

    [Space(10)]
    [Header("=== Corrosion")]
    [SerializeField] public StatusEffect_Temporary_WithAmount CorrosionStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount DecayStack;
    [HideInInspector] private static int CorrosionMax = 25;


    [HideInInspector] public EnemyController Enemy;

    #endregion

    #region Offset

    public void Offset(EnemyController _Enemy)
    {
        Enemy = _Enemy;

        #region Normal Status Buff

        ShieldBuff = new StatusEffect_Temporary_WithoutAmount(
            Enemy,
            _MaxCooltime: ShieldDurTime,
            _GainFunc: ShieldGainEffect,
            _ReduceFunc: ShieldReduceEffect,
            EnemyManager.Instance.ShieldIcon);

        #endregion

        #region Normal Status Debuff

        FlameStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Flame, FlameMax, 
            _MaxCooltime: 3f,
            _OnceTimeReduceAmount: 1,
            _IsResetWhenGain: false,
            _GainFunc: null,
            _ReduceFunc: Active_FlameReduce,
            _FullStack: Active_FlameFullStack,
            EnemyManager.Instance.FlameIcon);

        ColdStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Cold, ColdMax,
            _MaxCooltime: 6f, 
            _OnceTimeReduceAmount: 1,
            _IsResetWhenGain: true,
            _GainFunc: null,
            _ReduceFunc: null, 
            _FullStack: Active_ColdFullStack,
            EnemyManager.Instance.ColdIcon);

        ElectricityStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Electricity, ElectricityMax, 
            _MaxCooltime: 5f, 
            _OnceTimeReduceAmount: 1,
            _IsResetWhenGain: true,
            _GainFunc: Active_ElectricityGain,
            _ReduceFunc: null, 
            _FullStack: Active_ElectricityFullStack,
            EnemyManager.Instance.ElectricityIcon);

        CorrosionStack = new StatusEffect_Temporary_WithAmount(
            Enemy, eStatusEffect.Corrosion, CorrosionMax,
            _MaxCooltime: 4f, 
            _OnceTimeReduceAmount: 1, 
            _IsResetWhenGain: false,
            _GainFunc: null,
            _ReduceFunc: null, 
            _FullStack: Active_CorrosionFullStack,
            EnemyManager.Instance.CorrosionIcon);

        #endregion

        #region High Level Status Debuff

        InfernoStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.InfernoIcon,
            _GainFunc: null, 
            _FullStack: null, 
            _MaxStack: 3);

        AbsoluteZeroStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.AbsoluteZeroIcon,
            _GainFunc: null, 
            _FullStack: null, 
            _MaxStack: 3);

        PlasmaStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.PlasmaIcon,
            _GainFunc: null, 
            _FullStack: null, 
            _MaxStack: 3);

        DecayStack = new StatusEffect_Permanent_WithAmount(
            Enemy, EnemyManager.Instance.DecayIcon,
            _GainFunc: null, 
            _FullStack: null, 
            _MaxStack: 3);

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
        float time = Time.deltaTime;

        ShieldBuff.Caculate_Cooltime(time);
        FlameStack.Caculate_Cooltime(time);
        ColdStack.Caculate_Cooltime(time);
        ElectricityStack.Caculate_Cooltime(time);
        CorrosionStack.Caculate_Cooltime(time);
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
        Enemy.Set_CurrentSP(Enemy.Get_PercentHP(ShieldPercent));
    }

    private void ShieldReduceEffect()
    {
        Enemy.Set_CurrentSP_Zero();
    }

    #endregion

    #region Debuff

    #region Dur Effect

    // 화염 속성이 줄어들때
    private void Active_FlameReduce()
    {
        Enemy.Take_Damage(DevTool.Get_FrameDmg(this), eDamageType.Physics);
    }

    // 전기 속성을 얻을 때
    private void Active_ElectricityGain()
    {
        float dmg = DevTool.Get_ElectricityDmg(this);

        List<EnemyController> targetEnemies = new List<EnemyController>() { Enemy };
        EnemyController targetEnemy = Enemy;

        for (int i = 0; i < ElectricityStack.CurrentStack; i++)
        {
            List<EnemyController> closerEnemies = EnemyManager.Instance.Get_CloserEnemies(targetEnemy.gameObject, ElectricityRange);
            
            // 연쇄가  계속 되었는지 판별 => 안되었다면 Break
            bool willExpand = false;

            // 가까운 적들이 더이상 없다면
            if (closerEnemies.Count <= 0 || 
                closerEnemies == null) 
                break;

            // 가까운 적들을 가져와, 중간에 벽이 없고, 이미 포함하고 있지않으면 추가
            for (int j = 0; j < closerEnemies.Count; j++)
            {
                if (!targetEnemies.Contains(closerEnemies[j]) &&
                    !DevTool.Is_Exist_UseLine(targetEnemy.transform, closerEnemies[j].transform, "Wall"))
                {
                    willExpand = true;
                    targetEnemies.Add(closerEnemies[j]);
                    targetEnemy = closerEnemies[j];
                    break;
                }
            }

            if (!willExpand)
            { break; }
        }

        // 감전 적용의 수만큼 데미지는 증폭
        dmg *= targetEnemies.Count;

        for (int i = 0; i < targetEnemies.Count; i++)
        {
            targetEnemies[i].Take_Damage(dmg, eDamageType.Energy);
        }



#if UNITY_EDITOR
        ShowDebug(targetEnemies);
#endif
    }

    #endregion

    #region Full Stack Effect

    private void Active_FullStack(
        StatusEffect_Temporary_WithAmount _FullStackBuff,
        StatusEffect_Permanent_WithAmount _PermanentBuff,
        float _Dmg, eDamageType _DmgType)
    {
        _FullStackBuff.Reduce_Stack(_FullStackBuff.MaxStack);
        _PermanentBuff.Gain_Stack(1, true);

        Enemy.Take_Damage(_Dmg, _DmgType);
    }

    // 각각 속성이 최대치일 때
    private void Active_FlameFullStack()
    {
        Active_FullStack(
            FlameStack, 
            InfernoStack, 
            DevTool.Get_FlameExplDmg(out eDamageType dmgType),
            dmgType);
    }

    private void Active_ColdFullStack()
    {
        Active_FullStack(
            ColdStack,
            AbsoluteZeroStack,
            DevTool.Get_ColdExplDmg(out eDamageType dmgType),
            dmgType);
    }

    private void Active_ElectricityFullStack()
    {
        Active_FullStack(
            ElectricityStack,
            PlasmaStack,
            DevTool.Get_ElectricityExplDmg(out eDamageType dmgType),
            dmgType);
    }

    private void Active_CorrosionFullStack()
    {
        Active_FullStack(
            CorrosionStack,
            DecayStack,
            DevTool.Get_CorrosionExplDmg(out eDamageType dmgType),
            dmgType);
    }
    #endregion

    #endregion

    #region Editor

#if UNITY_EDITOR

    private void ShowDebug(List<EnemyController> targetEnemies)
    {
        for (int i = 0; i < targetEnemies.Count - 1; i++)
        {
            Debug.DrawRay(targetEnemies[i].transform.position,
                (targetEnemies[i + 1].transform.position - targetEnemies[i].transform.position),
                Color.blue, 3f);
        }
    }

#endif

    #endregion
}
