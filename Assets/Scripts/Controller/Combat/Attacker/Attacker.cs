using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Attacker")]

    [Space(10)]
    [Header("=== Object")]
    [SerializeField] protected List<MovableObject> HittedObjectList = new List<MovableObject>();

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected CapsuleCollider2D ThisCol;
    [SerializeField] private Animator ThisAnimator;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState AttackerState = new AttackerState();

    [HideInInspector] private AnimatorOverrideController aoc;
    #endregion

    #region Framework

    private void OnEnable()
    {
        HittedObjectList.Clear();
    }

    #endregion

    #region Set State

    public Sequence SetState_Bigger(Vector2 _SpawnedPos, AttackerState _AttackerState, 
        AnimationClip _AC, float _animSizeMultiple,
        Vector2 _ColSize, float _StartSize, float _MaxSize, float _BiggerTime)
    {
        SetState(_SpawnedPos, _AttackerState, _ColSize);

        Sequence totalSeq = DOTween.Sequence();
        Sequence seq = DOTween.Sequence();
        Sequence fadeSeq = DOTween.Sequence();

        ThisCol.gameObject.transform.localScale = Vector2.one * _StartSize;
        ThisAnimator.gameObject.transform.localScale = Vector2.one *_StartSize * _animSizeMultiple;

        if (ThisAnimator.TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = new Color(1, 1, 1, 0);

            fadeSeq.Append(sr.DOFade(1f, _BiggerTime / 10));
            fadeSeq.AppendInterval(_BiggerTime * 6 / 10);
            fadeSeq.Append(sr.DOFade(0f, _BiggerTime * 3 / 10));
        }

        seq.Append(ThisCol.transform.DOScale(_MaxSize, _BiggerTime));
        seq.Join(ThisAnimator.gameObject.transform.DOScale(_MaxSize * _animSizeMultiple, _BiggerTime));

        // Anim
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;

        this.gameObject.SetActive(true);

        totalSeq.Join(seq);
        totalSeq.Join(fadeSeq);

        return totalSeq;
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
