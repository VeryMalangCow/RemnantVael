using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SkillWeaponController : PlayerSolarController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Skill")]

    [Header("-- ActiveSkill")]
    [SerializeField] public List<ActiveSkillController> skillList;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Offset

    private void Offset_Variable(ActiveSkillController skill, SatelliteSideController satellite)
    {
        DevTool.Set_ComponentTType(ref skill, satellite.follower.gameObject);
    }

    private void Offset_Subscribe(ActiveSkillController skill, SkillEUIController skillEUI)
    {
        skill.needEP.Subscribe(_Value =>
        {
            skillEUI.Set_CostText(_Value * PlayerManager.instance.playerController.needEP_ForSkillMultiple.actualState);
        });
    }

    private void Offset_Start()
    {
        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            Offset_Variable(skillList[i], satelliteSideList[i]);
            Offset_Subscribe(skillList[i], MainGameUIManager.instance.hud.SkillView.skillList[i]);
        }
    }

    #endregion

    #region Framework

    public void TestStart()
    {
        Offset_Start();
        enabled = true;
    }

    protected override void Update()
    {
        base.Update();

        Set_FillAmount();
    }

    #endregion

    #region UI Shadow

    private void Set_FillAmount()
    {
        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            MainGameUIManager.instance.hud.SkillView.skillList[i].Set_ShadowFillAmount(
                skillList[i].Get_FillAmount());
        }
    }

    #endregion
}
