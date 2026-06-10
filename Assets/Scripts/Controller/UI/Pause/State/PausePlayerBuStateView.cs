using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PausePlayerBuStateView : MonoBehaviour
{
    [SerializeField] private ScrollPanelEUIController playerBuScrollEui;

    [Space(5)]
    [SerializeField] private StandbyPlayerBUEUIController playerBuScrollEuiPrefab;
    [SerializeField] private ScrollPanelEUISet[] ScrollPanelEUISets;
    [System.Serializable]
    public class ScrollPanelEUISet
    {
        public Transform parentTf;
        public int amount;
    }
    [SerializeField] private float intervalY = -68f;

    private List<StandbyPlayerBUEUIController> buEuis = new List<StandbyPlayerBUEUIController>();

    public IEnumerator InitAsync(Color imgClr, Color txtClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        string s = "";
#endif
        for (int i = 0; i < ScrollPanelEUISets.Length; i++)
        {
#if UNITY_EDITOR
            sw.Restart();
#endif
            for (int j = 0; j < ScrollPanelEUISets[i].amount; j++)
            {
                StandbyPlayerBUEUIController eui = Instantiate(playerBuScrollEuiPrefab, ScrollPanelEUISets[i].parentTf);
                eui.Offset();
                eui.SetColor(imgClr, txtClr);
                eui.rt.anchoredPosition = new Vector2(0, intervalY * j);
                buEuis.Add(eui);
            }
#if UNITY_EDITOR
            s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif
            yield return null;
        }

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=#FFFF80>Bu State (Element) View</color> : <color=red>{s}</color> ms");
#endif

        playerBuScrollEui.Offset();
        yield return null;
    }

    public void SetLanguageTxt()
    {
        // DMG, ROF, CC, CD, MS, AR, KB
        buEuis[0].Set_LanguageTxt(Get(12));
        buEuis[1].Set_LanguageTxt(Get(13));
        buEuis[2].Set_LanguageTxt(Get(15));
        buEuis[3].Set_LanguageTxt(Get(16));
        buEuis[4].Set_LanguageTxt(Get(43));
        buEuis[5].Set_LanguageTxt(Get(14));
        buEuis[6].Set_LanguageTxt(Get(44));

        // MaxEP, ESValue, SkillCost, Resist
        buEuis[7].Set_LanguageTxt(Get(8));
        buEuis[8].Set_LanguageTxt(Get(38));
        buEuis[9].Set_LanguageTxt(Get(39));
        buEuis[10].Set_LanguageTxt(Get(37));

        // WalkS, WalkSWhileS, DashP, AvoidC
        buEuis[11].Set_LanguageTxt(Get(40));
        buEuis[12].Set_LanguageTxt(Get(41));
        buEuis[13].Set_LanguageTxt(Get(10));
        buEuis[14].Set_LanguageTxt(Get(36));

        // Skill 00: Cooltime, Power, Tier
        buEuis[15].Set_LanguageTxt(Get(45));
        buEuis[16].Set_LanguageTxt(Get(18));
        buEuis[17].Set_LanguageTxt(Get(17));

        // Skill 01: Cooltime, Power, Tier
        buEuis[18].Set_LanguageTxt(Get(45));
        buEuis[19].Set_LanguageTxt(Get(18));
        buEuis[20].Set_LanguageTxt(Get(17));
    }

    string Get(int index) => ResourceManager.instance.Get_StaticWord(index);

    public void SetState()
    {
        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.baseWeapon;
        SkillWeaponController swc = pc.skillWeapon;
        PlayerDashController pdc = pc.dash;

        // DMG, ROF, CC, CD, MS, AR, KB
        buEuis[0].Set(pwc.baseDamage.currentLevel.Value);
        buEuis[1].Set(pwc.rof.currentLevel.Value);
        buEuis[2].Set(pwc.cc.currentLevel.Value);
        buEuis[3].Set(pwc.cd.currentLevel.Value);
        buEuis[4].Set(pwc.muzzleSpeed.currentLevel.Value);
        buEuis[5].Set(pwc.accRate.currentLevel.Value);
        buEuis[6].Set(pwc.kbPower.currentLevel.Value);
        // MaxEP, ESValue, SkillCost, Resist
        buEuis[7].Set(pc.maxEP.currentLevel.Value);
        buEuis[8].Set(pc.spawnESMultiple.currentLevel.Value);
        buEuis[9].Set(pc.needEP_ForSkillMultiple.currentLevel.Value);
        buEuis[10].Set(pc.takingDmgMultiple.currentLevel.Value);
        // WalkS, WalkSWhileS, DashP, AvoidC
        buEuis[11].Set(pc.walkSpeed.currentLevel.Value);
        buEuis[12].Set(pc.walkSpeedWhenShotMultiple.currentLevel.Value);
        buEuis[13].Set(pdc.dashSpeed.currentLevel.Value);
        buEuis[14].Set(pc.avoidChance.currentLevel.Value);
        // Skill 00: Cooltime, Power, Tier
        buEuis[15].Set(swc.skillList[0].maxCooltime.currentLevel.Value);
        buEuis[16].Set(swc.skillList[0].power.currentLevel.Value);
        buEuis[17].Set(swc.skillList[0].tier.currentLevel.Value);
        // Skill 01: Cooltime, Power, Tier
        buEuis[18].Set(swc.skillList[1].maxCooltime.currentLevel.Value);
        buEuis[19].Set(swc.skillList[1].power.currentLevel.Value);
        buEuis[20].Set(swc.skillList[1].tier.currentLevel.Value);
    }
}
