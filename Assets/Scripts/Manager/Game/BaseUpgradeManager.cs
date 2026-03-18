using System.Collections.Generic;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region Value

    [Space(10)]
    [Header("=== Data")]

    [Header("-- Attack")]
    [SerializeField] public BULevelData<float> baseDamage_BUData;
    [SerializeField] public BULevelData<float> baseROF_BUData;
    [SerializeField] public BULevelData<float> baseCC_BUData;
    [SerializeField] public BULevelData<float> baseCD_BUData;
    [SerializeField] public BULevelData<float> baseMuzzleSpeed_BUData;
    [SerializeField] public BULevelData<float> baseAccuracyRate_BUData;
    [SerializeField] public BULevelData<float> knockback_BUData;

    [Header("-- EP")]
    [SerializeField] public BULevelData<float> baseMaxEP_BUData;
    [SerializeField] public BULevelData<float> baseSpawnESMultiple_BUData;
    [SerializeField] public BULevelData<float> baseNeedEP_ForSkillMultiple_BUData;
    [SerializeField] public BULevelData<float> baseResist_BUData;

    [Header("-- Movement")]
    [SerializeField] public BULevelData<float> baseWalkSpeed_BUData;
    [SerializeField] public BULevelData<float> baseWalkSpeedWhenShotMultiple_BUData;
    [SerializeField] public BULevelData<float> baseDashSpeed_BUData;
    [SerializeField] public BULevelData<float> baseAvoidChance_BUData;

    [Header("-- Skill")]
    [SerializeField] public BULevelSkillData<float, int>[] skill_BUDataList;


    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Sprite[] costSpriteList;

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        baseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage);
        baseROF_BUData.Offset(_PC.BaseWeapon.ROF);
        baseCC_BUData.Offset(_PC.BaseWeapon.CC);
        baseCD_BUData.Offset(_PC.BaseWeapon.CD);
        baseMuzzleSpeed_BUData.Offset(_PC.BaseWeapon.MuzzleSpeed);
        baseAccuracyRate_BUData.Offset(_PC.BaseWeapon.AccuracyRate);
        knockback_BUData.Offset(_PC.BaseWeapon.KnockbackPower);

        baseMaxEP_BUData.Offset(_PC.MaxEP);
        baseSpawnESMultiple_BUData.Offset(_PC.SpawnESMultiple);
        baseNeedEP_ForSkillMultiple_BUData.Offset(_PC.NeedEP_ForSkillMultiple);
        baseResist_BUData.Offset(_PC.TakingDmgMultiple);

        baseWalkSpeed_BUData.Offset(_PC.WalkSpeed);
        baseWalkSpeedWhenShotMultiple_BUData.Offset(_PC.WalkSpeedWhenShotMultiple);
        baseDashSpeed_BUData.Offset(_PC.DashController.DashSpeed);
        baseAvoidChance_BUData.Offset(_PC.AvoidChance);

        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            skill_BUDataList[i].Skill_Cooltime_BUData.Offset(_PC.SkillWeapon.SkillList[i].MaxCooltime);
            skill_BUDataList[i].Skill_Power_BUData.Offset(_PC.SkillWeapon.SkillList[i].Power);
            skill_BUDataList[i].Skill_Tier_BUData.Offset(_PC.SkillWeapon.SkillList[i].Tier);
        }
    }

    #endregion
}