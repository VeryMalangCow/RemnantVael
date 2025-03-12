using System.Collections.Generic;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region Value

    [Space(10)]
    [Header("=== Data")]

    [Header("-- Attack")]
    [SerializeField] public BULevelData<float> BaseDamage_BUData;
    [SerializeField] public BULevelData<float> BaseROF_BUData;
    [SerializeField] public BULevelData<float> BaseCC_BUData;
    [SerializeField] public BULevelData<float> BaseCD_BUData;
    [SerializeField] public BULevelData<float> BaseMuzzleSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseAccuracyRate_BUData;
    [SerializeField] public BULevelData<float> Knockback_BUData;

    [Header("-- EP")]
    [SerializeField] public BULevelData<float> BaseMaxEP_BUData;
    [SerializeField] public BULevelData<float> BaseSpawnESMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseNeedEP_ForSkillMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseDecEnergyPointMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseResist_BUData;

    [Header("-- Movement")]
    [SerializeField] public BULevelData<float> BaseWalkSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseWalkSpeedWhenShotMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseDashSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseAvoidChance_BUData;

    [Header("-- Skill")]
    [SerializeField] public List<BULevelSkillData<float, int>> Skill_BUDataList;

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        BaseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage);
        BaseROF_BUData.Offset(_PC.BaseWeapon.ROF);
        BaseCC_BUData.Offset(_PC.BaseWeapon.CC);
        BaseCD_BUData.Offset(_PC.BaseWeapon.CD);
        BaseMuzzleSpeed_BUData.Offset(_PC.BaseWeapon.MuzzleSpeed);
        BaseAccuracyRate_BUData.Offset(_PC.BaseWeapon.AccuracyRate);
        Knockback_BUData.Offset(_PC.BaseWeapon.KnockbackPower);

        BaseMaxEP_BUData.Offset(_PC.MaxEP);
        BaseSpawnESMultiple_BUData.Offset(_PC.SpawnESMultiple);
        BaseNeedEP_ForSkillMultiple_BUData.Offset(_PC.NeedEP_ForSkillMultiple);
        BaseDecEnergyPointMultiple_BUData.Offset(_PC.DecEnergyPointMultiple);
        BaseResist_BUData.Offset(_PC.TakingDmgMultiple);

        BaseWalkSpeed_BUData.Offset(_PC.WalkSpeed);
        BaseWalkSpeedWhenShotMultiple_BUData.Offset(_PC.WalkSpeedWhenShotMultiple);
        BaseDashSpeed_BUData.Offset(_PC.DashController.DashSpeed);
        BaseAvoidChance_BUData.Offset(_PC.AvoidChance);

        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            Skill_BUDataList[i].Skill_Cooltime_BUData.Offset(_PC.SkillWeapon.SkillList[i].MaxCooltime);
            Skill_BUDataList[i].Skill_Power_BUData.Offset(_PC.SkillWeapon.SkillList[i].Power);
            Skill_BUDataList[i].Skill_Tier_BUData.Offset(_PC.SkillWeapon.SkillList[i].Tier);
        }
    }

    #endregion
}