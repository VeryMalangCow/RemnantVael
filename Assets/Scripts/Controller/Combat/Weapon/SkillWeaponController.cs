using UnityEngine;
using UniRx;

public class SkillWeaponController : SatelliteController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Skill")]

    [Header("-- ActiveSkill")]
    [SerializeField] public ActiveSkillController Skill_0;
    [SerializeField] public ActiveSkillController Skill_1;

    #endregion

    #region Framework

    private void Start()
    {
        if (Hands[0].ObjectTF.gameObject.TryGetComponent(out ActiveSkillController ASC_Q))
        { 
            Skill_0 = ASC_Q; 
            Skill_0.PlayerController = PlayerController; 
        }
        if (Hands[1].ObjectTF.gameObject.TryGetComponent(out ActiveSkillController ASC_E))
        { 
            Skill_1 = ASC_E; 
            Skill_1.PlayerController = PlayerController; 
        }

        Skill_0.NeedEP.Subscribe(_Value =>
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.SetCostText(
                _Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value);
        });
        Skill_1.NeedEP.Subscribe(_Value =>
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.SetCostText(
                _Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value);
        });
    }

    private void Update()
    {
        PitchTF.transform.localRotation = RotateSmooth(InputManager.Instance.DirFromPlayerPos.normalized, PitchTF, rotateSpeed);

        foreach (Satellite hand in Hands)
        {
            hand.SetPos(PlayerSR.sortingOrder);
        }

        MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.SetShadowFillAmount(
            Skill_0.GetFillAmount());
        MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.SetShadowFillAmount(
            Skill_1.GetFillAmount());
    }

    #endregion


}
