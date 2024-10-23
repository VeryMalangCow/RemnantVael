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
    [SerializeField] private GameObject TestSpriteGO;
    [SerializeField] protected CapsuleCollider2D ThisCol;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState AttackerState = new AttackerState();


    #endregion

    #region Framework

    private void OnEnable()
    {
        HittedObjectList.Clear();
    }

    #endregion

    #region Set State

    public Sequence SetState_Bigger(Vector2 _SpawnedPos, AttackerState _AttackerState, Vector2 _ColSize, 
        float _StartSize, float _MaxSize,
        float _BiggerTime, float _SmallerTime)
    {
        SetState(_SpawnedPos, _AttackerState, _ColSize);

        Sequence seq = DOTween.Sequence();

        ThisCol.gameObject.transform.localScale = Vector2.one * _StartSize;

        seq.Append(ThisCol.transform.DOScale(_MaxSize, _BiggerTime));
        seq.Join(TestSpriteGO.transform.DOScale(_MaxSize, _BiggerTime));
        seq.Append(ThisCol.transform.DOScale(0, _SmallerTime));
        seq.Join(TestSpriteGO.transform.DOScale(0, _SmallerTime));

        this.gameObject.SetActive(true);

        return seq;
    }

    private void SetState(Vector2 _SpawnedPos, AttackerState _AttackerState, Vector2 _ColSize)
    {
        this.transform.position = _SpawnedPos;
        this.AttackerState = _AttackerState;
        ThisCol.size = _ColSize;

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
