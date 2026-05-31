using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Shield")]
    [SerializeField] public StatusEffect_Temporary_WithoutAmount shieldBuff;
    [SerializeField] private float shieldPercent = 10;
    [SerializeField] private float shieldDurTime = 10;

    [Space(10)]
    [Header("=== Flame")]
    [SerializeField] public StatusEffect_Temporary_WithAmount flameStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount infernoStack;
    [HideInInspector] private static int flameMax = 30;

    [Space(10)]
    [Header("=== Cold")]
    [SerializeField] public StatusEffect_Temporary_WithAmount coldStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount absoluteZeroStack;
    [HideInInspector] private static int coldMax = 15;

    [Space(10)]
    [Header("=== Electricity")]
    [SerializeField] public StatusEffect_Temporary_WithAmount electricityStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount plasmaStack;
    [HideInInspector] private static int electricityMax = 20;
    [HideInInspector] private static float electricityRange = 10f;

    [Space(10)]
    [Header("=== Corrosion")]
    [SerializeField] public StatusEffect_Temporary_WithAmount corrosionStack;
    [SerializeField] public StatusEffect_Permanent_WithAmount decayStack;
    [HideInInspector] private static int corrosionMax = 25;


    [HideInInspector] public EnemyController enemy;
    [HideInInspector] private float statusExplosionSize = 1.7f;

    #endregion

    #region Offset

    public void Offset(EnemyController enemy)
    {
        this.enemy = enemy;

        #region Normal Status Buff

        shieldBuff = new StatusEffect_Temporary_WithoutAmount(
            this.enemy,
            maxCooltime: shieldDurTime,
            gainFunc: ShieldGainEffect,
            reduceFunc: ShieldReduceEffect,
            EnemyManager.instance.shieldIcon);

        #endregion

        #region Normal Status Debuff

        flameStack = new StatusEffect_Temporary_WithAmount(
            this.enemy, eStatusEffect.Flame, flameMax, 
            maxCooltime: 3f,
            onceTimeReduceAmount: 1,
            isResetWhenGain: false,
            gainFunc: null,
            reduceFunc: Active_FlameReduce,
            fullStack: Active_FlameFullStack,
            EnemyManager.instance.flameIcon);

        coldStack = new StatusEffect_Temporary_WithAmount(
            this.enemy, eStatusEffect.Cold, coldMax,
            maxCooltime: 6f, 
            onceTimeReduceAmount: 1,
            isResetWhenGain: true,
            gainFunc: null,
            reduceFunc: null, 
            fullStack: Active_ColdFullStack,
            EnemyManager.instance.coldIcon);

        electricityStack = new StatusEffect_Temporary_WithAmount(
            this.enemy, eStatusEffect.Electricity, electricityMax, 
            maxCooltime: 5f, 
            onceTimeReduceAmount: 1,
            isResetWhenGain: true,
            gainFunc: Active_ElectricityGain,
            reduceFunc: null, 
            fullStack: Active_ElectricityFullStack,
            EnemyManager.instance.electricityIcon);

        corrosionStack = new StatusEffect_Temporary_WithAmount(
            this.enemy, eStatusEffect.Corrosion, corrosionMax,
            maxCooltime: 4f, 
            onceTimeReduceAmount: 1, 
            isResetWhenGain: false,
            gainFunc: null,
            reduceFunc: null, 
            fullStack: Active_CorrosionFullStack,
            EnemyManager.instance.corrosionIcon);

        #endregion

        #region High Level Status Debuff

        infernoStack = new StatusEffect_Permanent_WithAmount(
            this.enemy, EnemyManager.instance.infernoIcon,
            gainFunc: null, 
            fullStack: null, 
            maxStack: 3);

        absoluteZeroStack = new StatusEffect_Permanent_WithAmount(
            this.enemy, EnemyManager.instance.absoluteZeroIcon,
            gainFunc: null, 
            fullStack: null, 
            maxStack: 3);

        plasmaStack = new StatusEffect_Permanent_WithAmount(
            this.enemy, EnemyManager.instance.plasmaIcon,
            gainFunc: null, 
            fullStack: null, 
            maxStack: 3);

        decayStack = new StatusEffect_Permanent_WithAmount(
            this.enemy, EnemyManager.instance.decayIcon,
            gainFunc: null, 
            fullStack: null, 
            maxStack: 3);

        #endregion
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        shieldBuff.Set_Clear();

        flameStack.Set_Clear();
        infernoStack.Set_Clear();

        coldStack.Set_Clear();
        absoluteZeroStack.Set_Clear();

        electricityStack.Set_Clear();
        plasmaStack.Set_Clear();

        corrosionStack.Set_Clear();
        decayStack.Set_Clear();
    }

    private void Update()
    {
        float time = Time.deltaTime;

        shieldBuff.Caculate_Cooltime(time);
        flameStack.Caculate_Cooltime(time);
        coldStack.Caculate_Cooltime(time);
        electricityStack.Caculate_Cooltime(time);
        corrosionStack.Caculate_Cooltime(time);
    }


    #endregion

    #region Buff

    public void ShieldGain(float value)
    {
        shieldPercent = value;
        shieldBuff.Gain_Stack(1, false, null);
    }

    private void ShieldGainEffect()
    {
        enemy.Set_CurrentSP(enemy.Get_PercentHP(shieldPercent));
    }

    private void ShieldReduceEffect()
    {
        enemy.Set_CurrentSP_Zero();
    }

    #endregion

    #region Debuff

    #region Dur Effect

    // 화염 속성이 줄어들때
    private void Active_FlameReduce()
    {
        enemy.Take_Damage(DevTool.Get_FrameDmg(this), eDamageType.Physics);
    }

    // 전기 속성을 얻을 때
    private void Active_ElectricityGain()
    {
        float dmg = DevTool.Get_ElectricityDmg(this);

        List<EnemyController> targetEnemies = new List<EnemyController>() { enemy };
        EnemyController targetEnemy = enemy;

        for (int i = 0; i < electricityStack.currentStack; i++)
        {
            List<EnemyController> closerEnemies = EnemyManager.instance.Get_CloserEnemies(targetEnemy.gameObject, electricityRange);
            
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
                    !DevTool.Is_Exist_UseLine(targetEnemy, closerEnemies[j], "Wall"))
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
            targetEnemies[i].Take_Damage(dmg, eDamageType.Physics);
        }
    }

    #endregion

    #region Full Stack Effect

    private void Active_FullStack(
        StatusEffect_Temporary_WithAmount fullStackBuff,
        StatusEffect_Permanent_WithAmount permanentBuff,
        float dmg, eDamageType dmgType)
    {
        fullStackBuff.Reduce_Stack(fullStackBuff.maxStack);
        permanentBuff.Gain_Stack(1, true);

        enemy.Take_Damage(dmg, dmgType);
    }

    // 각각 속성이 최대치일 때

    // Inferno
    private void Active_FlameFullStack()
    {
        float dmg = DevTool.Get_FlameExplDmg(out eDamageType dmgType);
        Active_FullStack(flameStack, infernoStack, dmg, dmgType);

        Play_ExplosionAttack(dmgType, dmg, 0);
    }

    // AbsoliteZero
    private void Active_ColdFullStack()
    {
        float dmg = DevTool.Get_ColdExplDmg(out eDamageType dmgType);
        Active_FullStack(coldStack, absoluteZeroStack, dmg, dmgType);

        Play_ExplosionAttack(dmgType, dmg, 1);
    }

    // Plasma
    private void Active_ElectricityFullStack()
    {
        float dmg = DevTool.Get_ElectricityExplDmg(out eDamageType dmgType);
        Active_FullStack(electricityStack, plasmaStack, dmg, dmgType);

        Play_ExplosionAttack(dmgType, dmg, 2);
    }

    // Decay
    private void Active_CorrosionFullStack()
    {
        float dmg = DevTool.Get_CorrosionExplDmg(out eDamageType dmgType);
        Active_FullStack(corrosionStack, decayStack, dmg, dmgType);

        Play_ExplosionAttack(dmgType, dmg, 3);
    }
    #endregion

    #region Explosion

    private void Play_ExplosionAttack(eDamageType dmgType, float dmg, int statusIndex)
    {
        PlayerExplosionController pec = ExplosionManager.instance.SpawnPlayerExplosion();
        pec.Add_HittedObjectList(enemy);
        pec.Set_State(
            Get_ExlposionState(dmgType, dmg, statusIndex),
            ac: ResourceManager.instance.explosionAC,
            Get_SpawnTF(statusExplosionSize),
            enemy.targetRange);
    }

    private ExplosionState Get_ExlposionState(eDamageType dmgType, float dmg, int statusIndex)
    {
        List<bool> statusBool = new List<bool> { false, false, false, false }; // Fire, Cold, Electricity, Corrosion
        statusBool[statusIndex] = true;

        return new ExplosionState(
            new CombatState(
                new CombatOwner(eCombatOwner.Enemy),
                new DmgState(dmgType, dmg),
                new CriticalState(0, 1),
                new KnockbackState(true, 10f, 0.2f)),
            new AttackSizeState(1f),
            statusBool); 
    }

    private State_TF2D Get_SpawnTF(float size)
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one * size);
    }

    #endregion

    #endregion
}
