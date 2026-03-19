using System.Collections;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 ColSize = new Vector2(1.2f, 0.75f);

    [SerializeField] private float StartSize = 1.5f;
    [SerializeField] public float MaxSize = 2f;

    [SerializeField] private float JugeAndTweenTime = 0.25f;
    [SerializeField] private float AnimSpeed = 1.62f;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private AnimationClip ShockwaveAnimation;

    #endregion

    #region Active

    public override void Active_Skill()
    {
        base.Active_Skill();

        StartCoroutine(Play_ActualActive_Cor());
    }

    #endregion

    #region Actual Active

    private IEnumerator Play_ActualActive_Cor()
    {
        Start_SkillUI();
        // ==========


        PlayerAttackerController pac = PoolingManager.instance.Get_OP_PlayerAttacker();

        pac.Set_State(
            Get_CurrentAttackerState(),
            State_Juge(),
            State_Anim(),
            State_StartTF(),
            State_EndTF(),
            DepthController.TargetRange);

        // 폭발
        UnitManager.instance.player_ExplImgGenerator.Expl_Player_Skill1(
            PlayerController.Get_ID(), 
            (Vector2)DepthController.TargetObject.gameObject.transform.position);

        // 버프
        BuffManager.instance.Gain_Buff(0);

        // 사운드
        SoundManager.instance.Play_2D_SFX_Combat(PlayerController.Get_AS(), "Explosion");

        yield return new WaitForSeconds(JugeAndTweenTime);

        // ==========
        End_SkillUI();
    }

    #endregion

    #region Get

    public float Get_UsableMaxSize()
    {
        return MaxSize * (1 + (Tier.actualState.Value * 0.15f));
    }

    private AttackerState Get_CurrentAttackerState()
    {
        return new AttackerState(
            new CombatState(
                new CombatOwner(
                    eCombatOwner.Player),
                new DmgState(
                    eDamageType.Energy, 
                    PlayerController.BaseWeapon.BaseDamage.buffedState * Power.actualState.Value),
                new CriticalState(
                    PlayerController.BaseWeapon.CC.actualState.Value, 
                    PlayerController.BaseWeapon.CD.buffedState),
                new KnockbackState(
                    true, 
                    PlayerController.BaseWeapon.KnockbackPower.actualState.Value * (Tier.actualState.Value + 1) * 10f,
                    0.4f)));
    }

    #endregion

    #region State

    private AttackerState_Juge<CapsuleCollider2D> State_Juge()
    {
        return new AttackerState_Juge<CapsuleCollider2D>(
            ColSize,
            _IsVertical: false);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(
            ShockwaveAnimation,
            AnimSpeed);
    }

    private State_TF2D State_StartTF()
    {
        return new State_TF2D(
            DepthController.transform.position,
            Quaternion.identity,
            Vector2.one * StartSize);
    }

    private AttackerState_EndTF State_EndTF()
    {
        return new AttackerState_EndTF(
            DepthController.transform.position,
            Quaternion.identity,
            Vector2.one * Get_UsableMaxSize(),
            JugeAndTweenTime);
    }

    #endregion
}
