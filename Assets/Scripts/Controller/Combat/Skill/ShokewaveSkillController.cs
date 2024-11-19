using DG.Tweening;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private HaveShadowThing ThisHST;
    [SerializeField] private Vector2 _ColSize = new Vector2(1f, 0.5f);
    [SerializeField] private float _StartSize = 3f; 
    [SerializeField] private float _MaxSize = 7.5f;
    [SerializeField] private float _BiggerTime = 0.3f;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private AnimationClip ShockwaveAnimation;

    #endregion

    #region Active

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        ActualActive();
    }

    private void ActualActive()
    {
        InputManager.Instance.AimController.SetOnSkill(1, true);

        AttackerState ThisState = new AttackerState(
            eDamageType.Energy, 
            PlayerController.BaseWeapon.BaseDamage.ActualState.Value * Power.ActualState.Value,
            true,
            PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * (Tier.ActualState.Value + 1) * 10f,
            0.4f,
            PlayerController.BaseWeapon.CC.ActualState.Value,
            PlayerController.BaseWeapon.CD.ActualState.Value);

        float usableMaxSize = _MaxSize + (_MaxSize * Tier.ActualState.Value * 0.1f);

        PlayerAttacker pa = PoolingManager.Instance.GetOP_PlayerAttacker();


        pa.SetState_Bigger(ThisHST.TargetObject.transform.position, ThisState, ShockwaveAnimation,
            _ColSize, _StartSize, usableMaxSize, _BiggerTime)
            .OnComplete(() =>
            {
                InputManager.Instance.AimController.SetOnSkill(1, false);
                pa.EndState();
            });

        // Effect Explosion -> Energy DMG

        ExplosionEffect((Vector2)ThisHST.TargetObject.gameObject.transform.position,
            ThisHST.ThisSR.sortingOrder + 1,
            PlayerController.BaseWeapon.CC.ActualState.Value, usableMaxSize);
    }

    #endregion

    #region Effect

    private void ExplosionEffect(Vector2 _SpawndPos, int _SortLayer, float _CriticalChance, float _UsableMaxSize)
    {
        PlayerController.PlayerMEI.GenExplosionImgs(
            _SpawndPos,
            (int)(36f * (1f - _CriticalChance)), 
            _UsableMaxSize / 4, _UsableMaxSize / 2,
            1.4f, 0.2f, 0.3f,
            0.7f, 0.4f, 0.5f,
            2, new Vector2(1, 0.5f), PlayerController.ThisPlayerMaterial_000);
        PlayerController.PlayerMEI.GenExplosionImgs(
            _SpawndPos,
            (int)(36f * _CriticalChance), 
            _UsableMaxSize / 4, _UsableMaxSize / 2,
            1.4f, 0.2f, 0.3f,
            0.7f, 0.4f, 0.5f,
            3, new Vector2(1, 0.5f), PlayerController.ThisPlayerMaterial_000);
    }

    #endregion
}
