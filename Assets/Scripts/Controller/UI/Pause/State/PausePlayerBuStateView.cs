using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PausePlayerBuStateView : MonoBehaviour
{
    [SerializeField] private ScrollPanelEUIController playerBuScrollEui;
    [SerializeField] private StandbyPlayerBUEUIController[] buEuiArr;

    public void Init()
    {
        for (int i = 0; i < buEuiArr.Length; i++)
            buEuiArr[i].Offset(); 
    }

    public void Set_Color(Color imgClr, Color txtClr)
    {
        for (int i = 0; i < buEuiArr.Length; i++)
            buEuiArr[i].Set_Color(imgClr, txtClr);
    }

    public void SetLanguageTxt()
    {
        // DMG, ROF, CC, CD, MS, AR, KB
        buEuiArr[0].Set_LanguageTxt(Get(12));
        buEuiArr[1].Set_LanguageTxt(Get(13));
        buEuiArr[2].Set_LanguageTxt(Get(15));
        buEuiArr[3].Set_LanguageTxt(Get(16));
        buEuiArr[4].Set_LanguageTxt(Get(43));
        buEuiArr[5].Set_LanguageTxt(Get(14));
        buEuiArr[6].Set_LanguageTxt(Get(44));

        // MaxEP, ESValue, SkillCost, Resist
        buEuiArr[7].Set_LanguageTxt(Get(8));
        buEuiArr[8].Set_LanguageTxt(Get(38));
        buEuiArr[9].Set_LanguageTxt(Get(39));
        buEuiArr[10].Set_LanguageTxt(Get(37));

        // WalkS, WalkSWhileS, DashP, AvoidC
        buEuiArr[11].Set_LanguageTxt(Get(40));
        buEuiArr[12].Set_LanguageTxt(Get(41));
        buEuiArr[13].Set_LanguageTxt(Get(10));
        buEuiArr[14].Set_LanguageTxt(Get(36));

        // Skill 00: Cooltime, Power, Tier
        buEuiArr[15].Set_LanguageTxt(Get(45));
        buEuiArr[16].Set_LanguageTxt(Get(18));
        buEuiArr[17].Set_LanguageTxt(Get(17));

        // Skill 01: Cooltime, Power, Tier
        buEuiArr[18].Set_LanguageTxt(Get(45));
        buEuiArr[19].Set_LanguageTxt(Get(18));
        buEuiArr[20].Set_LanguageTxt(Get(17));
    }

    string Get(int index) => ResourceManager.instance.Get_StaticWord(index);

    public void SetState()
    {
        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.baseWeapon;
        SkillWeaponController swc = pc.skillWeapon;
        PlayerDashController pdc = pc.dash;

        // DMG, ROF, CC, CD, MS, AR, KB
        buEuiArr[0].Set(pwc.baseDamage.currentLevel.Value);
        buEuiArr[1].Set(pwc.rof.currentLevel.Value);
        buEuiArr[2].Set(pwc.cc.currentLevel.Value);
        buEuiArr[3].Set(pwc.cd.currentLevel.Value);
        buEuiArr[4].Set(pwc.muzzleSpeed.currentLevel.Value);
        buEuiArr[5].Set(pwc.accRate.currentLevel.Value);
        buEuiArr[6].Set(pwc.kbPower.currentLevel.Value);
        // MaxEP, ESValue, SkillCost, Resist
        buEuiArr[7].Set(pc.maxEP.currentLevel.Value);
        buEuiArr[8].Set(pc.spawnESMultiple.currentLevel.Value);
        buEuiArr[9].Set(pc.needEP_ForSkillMultiple.currentLevel.Value);
        buEuiArr[10].Set(pc.takingDmgMultiple.currentLevel.Value);
        // WalkS, WalkSWhileS, DashP, AvoidC
        buEuiArr[11].Set(pc.walkSpeed.currentLevel.Value);
        buEuiArr[12].Set(pc.walkSpeedWhenShotMultiple.currentLevel.Value);
        buEuiArr[13].Set(pdc.dashSpeed.currentLevel.Value);
        buEuiArr[14].Set(pc.avoidChance.currentLevel.Value);
        // Skill 00: Cooltime, Power, Tier
        buEuiArr[15].Set(swc.skillList[0].maxCooltime.currentLevel.Value);
        buEuiArr[16].Set(swc.skillList[0].power.currentLevel.Value);
        buEuiArr[17].Set(swc.skillList[0].tier.currentLevel.Value);
        // Skill 01: Cooltime, Power, Tier
        buEuiArr[18].Set(swc.skillList[1].maxCooltime.currentLevel.Value);
        buEuiArr[19].Set(swc.skillList[1].power.currentLevel.Value);
        buEuiArr[20].Set(swc.skillList[1].tier.currentLevel.Value);
    }
}
