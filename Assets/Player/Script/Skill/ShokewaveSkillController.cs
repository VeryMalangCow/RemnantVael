using System.Collections;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 colSize = new Vector2(1.2f, 0.75f);

    [SerializeField] private float startSize = 1.5f;
    [SerializeField] public float maxSize = 2f;

    [SerializeField] private float jugeAndTweenTime = 0.25f;
    [SerializeField] private float animSpeed = 1.62f;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private AnimationClip shockwaveAnimation;

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

        PlayerAttackerController pac = AttackerManager.instance.SpawnPlayerAttacker();

        pac.SetState(
            Get_CurrentAttackerState(),
            State_Juge(),
            State_Anim(),
            State_StartTF(),
            State_EndTF(),
            depthController.targetRange);

        // 폭발
        VfxManager.instance.player_ExplImgGenerator.Expl_Player_Skill1(
            playerController.Get_ID(), 
            (Vector2)depthController.targetObject.gameObject.transform.position);

        // 버프
        BuffManager.instance.Gain_Buff(0);

        // 사운드
        SoundManager.instance.PlayExplosionSfx(transform.position);

        yield return new WaitForSeconds(jugeAndTweenTime);

        // ==========
        End_SkillUI();
    }

    #endregion

    #region Get

    public float Get_UsableMaxSize()
    {
        return maxSize * (1 + (tier.actualState * 0.15f));
    }

    private AttackerState Get_CurrentAttackerState()
    {
        return new AttackerState(
            new CombatState(
                new CombatOwner(
                    CombatOwnerType.Player),
                new DmgState(
                    DamageType.Energy, 
                    playerController.baseWeapon.baseDamage.buffedState * power.actualState),
                new CriticalState(
                    playerController.baseWeapon.cc.actualState, 
                    playerController.baseWeapon.cd.buffedState),
                new KnockbackState(
                    true, 
                    playerController.baseWeapon.kbPower.actualState * (tier.actualState + 1) * 10f,
                    0.4f)));
    }

    #endregion

    #region State

    private AttackerState_Juge<CapsuleCollider2D> State_Juge()
    {
        return new AttackerState_Juge<CapsuleCollider2D>(
            colSize,
            isVertical: false);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(
            shockwaveAnimation,
            animSpeed);
    }

    private State_TF2D State_StartTF()
    {
        return new State_TF2D(
            depthController.transform.position,
            Quaternion.identity,
            Vector2.one * startSize);
    }

    private AttackerState_EndTF State_EndTF()
    {
        return new AttackerState_EndTF(
            depthController.transform.position,
            Quaternion.identity,
            Vector2.one * Get_UsableMaxSize(),
            jugeAndTweenTime);
    }

    #endregion
}
