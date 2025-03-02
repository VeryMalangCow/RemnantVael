using DG.Tweening;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private DepthController ThisHST;
    [SerializeField] private Vector2 _ColSize = new Vector2(1f, 0.5f);
    [SerializeField] private float _StartSize = 3f; 
    [SerializeField] private float _MaxSize = 7.5f;
    [SerializeField] private float _BiggerTime = 0.3f;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private AnimationClip ShockwaveAnimation;

    #endregion

    #region Active

    public override void Active_Skill()
    {
        base.Active_Skill();
        Play_ActualActive();
    }

    private void Play_ActualActive()
    {
        InputManager.Instance.AimController.SetOn_Skill(1, true);

        AttackerState ThisState = new AttackerState(
            eDamageType.Energy, 
            PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value,
            true,
            PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * (Tier.ActualState.Value + 1) * 10f,
            0.4f,
            PlayerController.BaseWeapon.CC.ActualState.Value,
            PlayerController.BaseWeapon.CD.ActualState.Value);

        float usableMaxSize = _MaxSize + (_MaxSize * Tier.ActualState.Value * 0.1f);


        PlayerAttackerController pa = PoolingManager.Instance.Get_OP_PlayerAttacker();

        pa.Set_ShadowDis(ThisHST);
        pa.Play_Bigger(ThisHST.transform.position, ThisState, ShockwaveAnimation,
            _ColSize, _StartSize, usableMaxSize, _BiggerTime)
            .OnComplete(() =>
            {
                InputManager.Instance.AimController.SetOn_Skill(1, false);
                Set_EndUI();
                pa.End_State();
            });

        // Effect Explosion -> Energy DMG

        Gen_ExplosionEffect((Vector2)ThisHST.TargetObject.gameObject.transform.position,
            PlayerController.BaseWeapon.CC.ActualState.Value, usableMaxSize);

        BuffManager.Instance.Gain_Buff(0);
        BuffManager.Instance.SetOn_Buff(0);
    }

    #endregion

    #region Effect

    private void Gen_ExplosionEffect(Vector2 _SpawndPos, float _CriticalChance, float _UsableMaxSize)
    {
        PlayerController.PlayerMEI.Gen_ExplosionImgs(
            _SpawndPos,
            (int)(36f * (1f - _CriticalChance)), 
            _UsableMaxSize / 4, _UsableMaxSize / 2,
            1.4f, 0.2f, 0.3f,
            0.7f, 0.4f, 0.5f,
            2, new Vector2(1, 0.5f), PlayerController.ThisPlayerMaterial_000);
        PlayerController.PlayerMEI.Gen_ExplosionImgs(
            _SpawndPos,
            (int)(36f * _CriticalChance), 
            _UsableMaxSize / 4, _UsableMaxSize / 2,
            1.4f, 0.2f, 0.3f,
            0.7f, 0.4f, 0.5f,
            3, new Vector2(1, 0.5f), PlayerController.ThisPlayerMaterial_000);
    }

    #endregion
}
