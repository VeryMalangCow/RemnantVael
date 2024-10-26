using DG.Tweening;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 _ColSize = new Vector2(1f, 0.5f);
    [SerializeField] private float _StartSize = 3f; 
    [SerializeField] private float _MaxSize = 7.5f;
    [SerializeField] private float _BiggerTime = 0.3f;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private AnimationClip ShockwaveAnimation;

    [Header("=== Effect")]
    [SerializeField] private MakeExplosionImage MEI;

    #endregion

    #region Active

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        ActualActive();
    }

    private void ActualActive()
    {
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
        pa.SetState_Bigger(transform.position, ThisState,
            ShockwaveAnimation, 0.5f,
            _ColSize, _StartSize, usableMaxSize, _BiggerTime)
            .OnComplete(() =>
            {
                pa.EndState();
            });

        // Effect Explosion -> Energy DMG
        
        MEI.GenExplosionImgs(
            (Vector2)MEI.gameObject.transform.position,
            64, usableMaxSize / 5f, usableMaxSize / 4f,
            3.0f, 0.15f, 0.2f,
            0.0f, 0.5f, 1.0f,
            1);
    }

    #endregion
}
