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


        PlayerAttackerController pac = PoolingManager.Instance.Get_OP_PlayerAttacker();

        pac.Set_State(
            Get_CurrentAttackerState(), 
            new AttackerState_Juge<CapsuleCollider2D>(
                ColSize, 
                _IsVertical: false),
            new State_Anim(
                ShockwaveAnimation,
                AnimSpeed),
            new State_TF2D(
                DepthController.transform.position,
                Quaternion.identity,
                Vector2.one * StartSize),
            new AttackerState_EndTF(
                DepthController.transform.position,
                Quaternion.identity,
                Vector2.one * Get_UsableMaxSize(), 
                JugeAndTweenTime));

        // Æø¹ß
        UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Skill1(
            PlayerController.Get_ID(), 
            (Vector2)DepthController.TargetObject.gameObject.transform.position);

        // ¹öÇÁ
        BuffManager.Instance.Gain_Buff(0);

        yield return new WaitForSeconds(JugeAndTweenTime);


        // ==========
        End_SkillUI();
    }

    #endregion

    #region Get

    public float Get_UsableMaxSize()
    {
        return MaxSize * (1 + (Tier.ActualState.Value * 0.15f));
    }

    private AttackerState Get_CurrentAttackerState()
    {
        return new AttackerState(
            new CombatState(
                new DmgState(
                    eDamageType.Energy, 
                    PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value),
                new CriticalState(
                    PlayerController.BaseWeapon.CC.ActualState.Value, 
                    PlayerController.BaseWeapon.CD.ActualState.Value),
                new KnockbackState(
                    true, 
                    PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * (Tier.ActualState.Value + 1) * 10f,
                    0.4f)));
    }


    #endregion
}
