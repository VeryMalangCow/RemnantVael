using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Attacker")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected CapsuleCollider2D ThisCol;
    [SerializeField] private Animator ThisAnimator;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState AttackerState = new AttackerState();

    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Set State

    public Sequence SetState_SetRotationAndMoveForward(Vector2 _SpawnedPos, AttackerState _AttackerState, AnimationClip _AC, Vector2 _ColSize
        , Quaternion _Rotation, Vector2 _EndPos, float _AnimTime)
    {
        Sequence seq = DOTween.Sequence();
        SetState(_SpawnedPos, _AttackerState, _ColSize);
        SetAnim(_AC);

        ThisCol.gameObject.transform.localScale = Vector2.one;
        this.transform.rotation = _Rotation;
        seq.Append(ThisCol.transform.DOMove(_EndPos, _AnimTime));

        
        this.gameObject.SetActive(true);
        return seq;
    }

    public Sequence SetState_Bigger(Vector2 _SpawnedPos, AttackerState _AttackerState, AnimationClip _AC, Vector2 _ColSize, 
        float _StartSize, float _MaxSize, float _AnimTime)
    {
        Sequence seq = DOTween.Sequence();
        SetState(_SpawnedPos, _AttackerState, _ColSize);
        SetAnim(_AC);

        ThisCol.gameObject.transform.localScale = Vector2.one * _StartSize;
        seq.Append(ThisCol.transform.DOScale(_MaxSize, _AnimTime));

        this.gameObject.SetActive(true);
        return seq;
    }

    private void SetAnim(AnimationClip _AC)
    {
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;
    }    

    private void SetState(Vector2 _SpawnedPos, AttackerState _AttackerState, Vector2 _ColSize)
    {
        this.transform.position = _SpawnedPos;
        this.AttackerState = _AttackerState;
        ThisCol.size = _ColSize;
    }

    public void EndState()
    {
        //if (ThisAnimator.isPlaying)
        //{
        //    ThisAnimator.Stop();
        //}

        gameObject.SetActive(false);

        if (aoc != null)
        {
            aoc = null;
        }

        if (this is PlayerAttacker pa)
        {
            PoolingManager.Instance.PlayerAttackers.Queue.Enqueue(pa);
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
