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
    [SerializeField] private Vector2 ColSize = new Vector2(0.75f, 0.5f);
    [SerializeField] private float StartSize = 3f; 
    [SerializeField] public float MaxSize = 5f;
    [SerializeField] private float BiggerTime = 0.15f;

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
        InputManager.Instance.AimController.Set_SkillState(1, true);

        DmgState dmgState = new DmgState(eDamageType.Energy, PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value);
        CriticalState criticalState = new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.ActualState.Value);
        KnockbackState knockbackState = new KnockbackState(true, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * (Tier.ActualState.Value + 1) * 10f, 0.4f);

        AttackerState ThisState = new AttackerState(new CombatState(dmgState, criticalState, knockbackState));

        float usableMaxSize = MaxSize + (MaxSize * Tier.ActualState.Value * 0.1f);


        PlayerAttackerController pa = PoolingManager.Instance.Get_OP_PlayerAttacker();

        pa.Set_ShadowDis(ThisHST);
        pa.Play_Bigger(ThisHST.transform.position, ThisState, ShockwaveAnimation,
            ColSize, StartSize, usableMaxSize, BiggerTime)
            .OnComplete(() =>
            {
                InputManager.Instance.AimController.Set_SkillState(1, false);
                Set_EndUI();
                pa.End_State();
            });

        UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Skill1(PlayerController.Get_ID(), (Vector2)ThisHST.TargetObject.gameObject.transform.position);

        BuffManager.Instance.Gain_Buff(0);
        BuffManager.Instance.SetOn_Buff(0);
    }

    #endregion
}
