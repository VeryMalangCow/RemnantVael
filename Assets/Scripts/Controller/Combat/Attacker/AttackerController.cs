using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AttackerController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Attacker")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected CapsuleCollider2D ThisCol;
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private Light2D ThisLight;
    [SerializeField] private StaticDepthController HSTS;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState AttackerState = new AttackerState();

    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Set State by Cond

    public void Set_ShadowDis(DepthController _HST)
    {
        if (HSTS == null)
        { return; }

        HSTS.TargetRange = _HST.TargetRange;
        HSTS.TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * HSTS.TargetRange);
    }

    public Sequence Play_RotAndPosForward(Vector2 _SpawnedPos, AttackerState _AttackerState, AnimationClip _AC, Vector2 _ColSize,
        Quaternion _Rotation, Vector2 _EndPos, float _TweenTime, float _AnimSpeed)
    {
        Sequence seq = DOTween.Sequence();
        Set_State(_SpawnedPos, _AttackerState, _ColSize);
        Set_Anim(_AC);

        ThisCol.gameObject.transform.localScale = Vector2.one;
        this.transform.rotation = _Rotation;
        seq.Append(this.transform.DOMove(_EndPos, _TweenTime / _AnimSpeed));
        ThisAnimator.speed = _AnimSpeed;


        this.gameObject.SetActive(true);
        return seq;
    }

    public Sequence Play_Bigger(Vector2 _SpawnedPos, AttackerState _AttackerState, AnimationClip _AC, Vector2 _ColSize, 
        float _StartSize, float _MaxSize, float _TweenTime)
    {
        Sequence seq = DOTween.Sequence();
        Set_State(_SpawnedPos, _AttackerState, _ColSize);
        Set_Anim(_AC);

        ThisCol.gameObject.transform.localScale = Vector2.one * _StartSize;
        seq.Append(ThisCol.transform.DOScale(_MaxSize, _TweenTime));

        this.gameObject.SetActive(true);
        return seq;
    }

    #endregion

    #region Light

    public void Set_Light(float _BiggestSize, float _StayTime)
    {
        if (ThisLight == null)
        { return; }

        Sequence seq = DOTween.Sequence();
        ThisLight.pointLightOuterRadius = 0f;

        seq.Append(DOTween.To(() => ThisLight.pointLightOuterRadius, 
            x => ThisLight.pointLightOuterRadius = x,
            _BiggestSize, _StayTime / 10));
        seq.AppendInterval(_StayTime * 8 / 10);
        seq.Append(DOTween.To(() => ThisLight.pointLightOuterRadius,
            x => ThisLight.pointLightOuterRadius = x,
            0, _StayTime / 10));
    }

    #endregion

    #region Module

    private void Set_Anim(AnimationClip _AC)
    {
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;
    }

    private void Set_State(Vector2 _SpawnedPos, AttackerState _AttackerState, Vector2 _ColSize)
    {
        this.transform.position = _SpawnedPos;
        this.AttackerState = new AttackerState(_AttackerState);
        ThisCol.size = _ColSize;
    }

    public void End_State()
    {
        gameObject.SetActive(false);

        if (aoc != null)
        {
            aoc = null;
        }

        if (this is PlayerAttackerController pa)
        {
            PoolingManager.Instance.PlayerAttackers.Queue.Enqueue(pa);
        }
        if (this is EnemyAttackerController ea)
        {
            PoolingManager.Instance.EnemyAttackers.Queue.Enqueue(ea);
        }
    }

    #endregion
}

[System.Serializable]
public class AttackerState
{
    [SerializeField] public eDamageType DamageType;
    [SerializeField] public float BaseDamage;

    [SerializeField] public bool AbleKnockback;
    [SerializeField] public float KnockbackPower;
    [SerializeField] public float KnockbackTime;

    [SerializeField] public float CC;
    [SerializeField] public float CD;

    public AttackerState()
    { }

    public AttackerState(AttackerState _AttakerState)
    {
        DamageType = _AttakerState.DamageType;
        BaseDamage = _AttakerState.BaseDamage;

        AbleKnockback = _AttakerState.AbleKnockback;
        KnockbackPower = _AttakerState.KnockbackPower;
        KnockbackTime = _AttakerState.KnockbackTime;

        CC = _AttakerState.CC;
        CD = _AttakerState.CD;
    }
    public AttackerState(eDamageType _DamageType, float _BaseDamage, bool _AbleKnockback, float _KnockbackPower, float _KnockbackTime, float _CC, float _CD)
    {
        DamageType = _DamageType;
        BaseDamage = _BaseDamage;

        AbleKnockback = _AbleKnockback;
        KnockbackPower = _KnockbackPower;
        KnockbackTime = _KnockbackTime;

        CC = _CC;
        CD = _CD;
    }
}
