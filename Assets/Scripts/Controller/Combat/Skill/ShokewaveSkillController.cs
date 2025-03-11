using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private DepthController ThisHST;
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
        Play_ActualActive();
    }

    private void Play_ActualActive()
    {
        InputManager.Instance.AimController.Set_ActivingSkill(1, true);

        DmgState dmgState = new DmgState(eDamageType.Energy, PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value);
        CriticalState criticalState = new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.ActualState.Value);
        KnockbackState knockbackState = new KnockbackState(true, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * (Tier.ActualState.Value + 1) * 10f, 0.4f);

        AttackerState state = new AttackerState(new CombatState(dmgState, criticalState, knockbackState));

        float usableMaxSize = Get_UsableMaxSize();


        PlayerAttackerController pa = PoolingManager.Instance.Get_OP_PlayerAttacker();

        AttackerState_Juge<CapsuleCollider2D> juge
            = new AttackerState_Juge<CapsuleCollider2D>(
                ColSize, 
                _IsVertical:false);

        State_Anim anim
            = new State_Anim(
                ShockwaveAnimation, 
                AnimSpeed);

        State_TF2D startTF
            = new State_TF2D(
                (Vector2)ThisHST.transform.position,
                Quaternion.identity,
                Vector2.one * StartSize);

        AttackerState_EndTF endTF
            = new AttackerState_EndTF(
                (Vector2)ThisHST.transform.position,
                Quaternion.identity,
                Vector2.one * usableMaxSize, JugeAndTweenTime);

        pa.Set_State(state, juge, anim, startTF, endTF);

        UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Skill1(PlayerController.Get_ID(), (Vector2)ThisHST.TargetObject.gameObject.transform.position);

        BuffManager.Instance.Gain_Buff(0);
        BuffManager.Instance.SetOn_Buff(0);
    }

    #endregion

    #region Get

    public float Get_UsableMaxSize()
    {
        return MaxSize * (1 + (Tier.ActualState.Value * 0.15f));
    }

    #endregion
}
