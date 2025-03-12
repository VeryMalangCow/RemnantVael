using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SkillWeaponController : PlayerSolarController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Skill")]

    [Header("-- ActiveSkill")]
    [SerializeField] public List<ActiveSkillController> SkillList;

    #endregion

    #region Offset

    private void Offset()
    {
        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            Offset_Variable(SkillList[i], SatelliteSideList[i]);
            Offset_Subscribe(SkillList[i], MainGameUIManager.Instance.PlayerHUD_UIController.SkillList[i]);
        }
    }

    private void Offset_Variable(ActiveSkillController _Skill, SatelliteSideController _Satellite)
    {
        DevTool.Set_ComponentTType(ref _Skill, _Satellite.Follower.gameObject);
        _Skill.PlayerController = PlayerController;
    }

    private void Offset_Subscribe(ActiveSkillController _Skill, SkillEUIController _SkillEUI)
    {
        _Skill.NeedEP.Subscribe(_Value =>
        {
            _SkillEUI.Set_CostText(_Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value);
        });
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
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
        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.SkillList[i].Set_ShadowFillAmount(
                SkillList[i].Get_FillAmount());
        }
    }

    #endregion
}
