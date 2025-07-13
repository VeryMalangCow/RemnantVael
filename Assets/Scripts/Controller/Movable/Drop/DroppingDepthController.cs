using DG.Tweening;
using UnityEngine;

public abstract class DroppingDepthController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected Transform ShadowTF;

    [Space(10)]
    [Header("=== Value")]

    [Space(5)]
    [Header("-- Time")]
    [SerializeField] private float DroppingSpeed;
    [SerializeField] private float CurrentDroppingTime;

    [Space(5)]
    [Header("-- Shadow")]
    [SerializeField] private Vector2 ShadowSize;

    [Space(5)]
    [Header("-- Drop Y")]
    [SerializeField] private float DropBottomYPos;

    [Space(5)]
    [Header("-- Alpha")]
    [SerializeField] private float ZeroToOneTime;

    #endregion

    #region State

    public virtual void Set_State_Base(CombatState _State, float _DroppingSpeed, float _TopYPos = 5f, float _DropBottomYPos = 0f)
    {
        TargetRange = _TopYPos;
        DropBottomYPos = _DropBottomYPos;
        DroppingSpeed = _DroppingSpeed;
        Set_TargetPos();
    }


    public virtual void Set_State_ShadowSize(BulletState_Size _State_Size)
    {
        TargetObject.transform.localScale = _State_Size.ObjSize;
        ShadowSize = _State_Size.ColSize;
    }

    protected virtual void SetOn_State()
    {
        Init_Data();

        Sequence seq = DOTween.Sequence();

        float _droppingTime = 1 / DroppingSpeed;
        seq.Join(ShadowTF.DOScale(ShadowSize, _droppingTime).SetEase(Ease.Linear)); // 그림자
        seq.Join(DOTween.To(() => TargetRange, x => TargetRange = x, DropBottomYPos, _droppingTime).SetEase(Ease.Linear)); // 떨어지는 이미지
        seq.Join(ThisSR.DOFade(1f, _droppingTime * 0.3f).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            Active();
        });
    }

    #endregion

    #region Init

    private void Init_Data()
    {
        ThisSR.color = new Color(1f, 1f, 1f, 0f);
        ShadowTF.transform.localScale = Vector2.zero;
    }

    #endregion

    #region Active

    protected abstract void Active();

    #endregion
}
