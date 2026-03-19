using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class DroppingDepthController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected Transform ShadowTF;
    [SerializeField] protected TrailRenderer ThisTrail;
    [SerializeField] protected Light2D ThisLight;

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

    [HideInInspector] private Sequence Seq = null;
    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.instance.Add_NeedSortObj(this);
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.instance.Remove_NeedSortObj(this);

        Init_Data();
    }

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
        TargetObject.transform.localScale = _State_Size.objSize;
        ShadowSize = _State_Size.colSize;
    }

    protected virtual void SetOn_State()
    {
        Init_Data();

        Seq = DOTween.Sequence();

        float _droppingTime = 1 / DroppingSpeed;
        Seq.Join(ShadowTF.DOScale(ShadowSize, _droppingTime).SetEase(Ease.Linear)); // 그림자
        Seq.Join(DOTween.To(() => TargetRange, x => TargetRange = x, DropBottomYPos, _droppingTime).SetEase(Ease.InCubic)); // 떨어지는 이미지
        Seq.Join(ThisSR.DOFade(1f, _droppingTime * 0.3f).SetEase(Ease.Linear));
        Seq.OnComplete(() =>
        {
            Active();
            Seq = null;
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

    #region Light

    public virtual void SetOn_LightIntensity(float _Intensity)
    {
        ThisLight.intensity = _Intensity;
        ThisLight.lightCookieSprite = ThisSR.sprite;
        DevTool.Set_AlphaColor(ThisLight, 0.5f);
    }

    #endregion

    #region Trail

    public void SetOn_TrailState(float _Time, float _StartWidth, Gradient _Gradient)
    {
        ThisTrail.time = _Time;
        ThisTrail.startWidth = _StartWidth;
        ThisTrail.colorGradient = _Gradient;
    }

    #endregion
}
