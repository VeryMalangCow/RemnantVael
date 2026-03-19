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

    public void Offset(PlayerController player)
    {
        baseDamage_BUData.Offset(player.BaseWeapon.BaseDamage);
        baseROF_BUData.Offset(player.BaseWeapon.ROF);
        baseCC_BUData.Offset(player.BaseWeapon.CC);
        baseCD_BUData.Offset(player.BaseWeapon.CD);
        baseMuzzleSpeed_BUData.Offset(player.BaseWeapon.MuzzleSpeed);
        baseAccuracyRate_BUData.Offset(player.BaseWeapon.AccuracyRate);
        knockback_BUData.Offset(player.BaseWeapon.KnockbackPower);

        baseMaxEP_BUData.Offset(player.MaxEP);
        baseSpawnESMultiple_BUData.Offset(player.SpawnESMultiple);
        baseNeedEP_ForSkillMultiple_BUData.Offset(player.NeedEP_ForSkillMultiple);
        baseResist_BUData.Offset(player.TakingDmgMultiple);

        baseWalkSpeed_BUData.Offset(player.WalkSpeed);
        baseWalkSpeedWhenShotMultiple_BUData.Offset(player.WalkSpeedWhenShotMultiple);
        baseDashSpeed_BUData.Offset(player.DashController.DashSpeed);
        baseAvoidChance_BUData.Offset(player.AvoidChance);

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skill_BUDataList[i].skill_Cooltime_BUData.Offset(player.SkillWeapon.SkillList[i].MaxCooltime);
            skill_BUDataList[i].skill_Power_BUData.Offset(player.SkillWeapon.SkillList[i].Power);
            skill_BUDataList[i].skill_Tier_BUData.Offset(player.SkillWeapon.SkillList[i].Tier);
        }
    }

    #endregion
}