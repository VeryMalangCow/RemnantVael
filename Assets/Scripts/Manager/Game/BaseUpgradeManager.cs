using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>, IMainGameInitializer
{
    #region Value

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;


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

    #region Init
    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        PlayerController player = PlayerManager.instance.playerController;

        baseDamage_BUData.Offset(player.baseWeapon.baseDamage);
        baseROF_BUData.Offset(player.baseWeapon.rof);
        baseCC_BUData.Offset(player.baseWeapon.cc);
        baseCD_BUData.Offset(player.baseWeapon.cd);
        baseMuzzleSpeed_BUData.Offset(player.baseWeapon.muzzleSpeed);
        baseAccuracyRate_BUData.Offset(player.baseWeapon.accRate);
        knockback_BUData.Offset(player.baseWeapon.kbPower);

        baseMaxEP_BUData.Offset(player.maxEP);
        baseSpawnESMultiple_BUData.Offset(player.spawnESMultiple);
        baseNeedEP_ForSkillMultiple_BUData.Offset(player.needEP_ForSkillMultiple);
        baseResist_BUData.Offset(player.takingDmgMultiple);

        baseWalkSpeed_BUData.Offset(player.walkSpeed);
        baseWalkSpeedWhenShotMultiple_BUData.Offset(player.walkSpeedWhenShotMultiple);
        baseDashSpeed_BUData.Offset(player.dash.dashSpeed);
        baseAvoidChance_BUData.Offset(player.avoidChance);

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skill_BUDataList[i].skill_Cooltime_BUData.Offset(player.skillWeapon.skillList[i].maxCooltime);
            skill_BUDataList[i].skill_Power_BUData.Offset(player.skillWeapon.skillList[i].power);
            skill_BUDataList[i].skill_Tier_BUData.Offset(player.skillWeapon.skillList[i].tier);
        }

        sw.Stop();
        UnityEngine.Debug.Log($"BaseUpgradeManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    #endregion
}