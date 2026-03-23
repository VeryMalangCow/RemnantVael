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
    [SerializeField] protected Transform shadowTf;
    [SerializeField] protected TrailRenderer trail;
    [SerializeField] protected Light2D light2D;

    [Space(10)]
    [Header("=== Value")]

    [Space(5)]
    [Header("-- Time")]
    [SerializeField] private float droppingSpeed;
    [SerializeField] private float currentDroppingTime;

    [Space(5)]
    [Header("-- Shadow")]
    [SerializeField] private Vector2 shadowSize;

    [Space(5)]
    [Header("-- Drop Y")]
    [SerializeField] private float dropBottomYPos;

    [Space(5)]
    [Header("-- Alpha")]
    [SerializeField] private float zeroToOneTime;

    [HideInInspector] private Sequence seq = null;
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

    public virtual void Set_State_Base(CombatState state, float droppingSpeed, float topYPos = 5f, float dropBottomYPos = 0f)
    {
        targetRange = topYPos;
        this.dropBottomYPos = dropBottomYPos;
        this.droppingSpeed = droppingSpeed;
        Set_TargetPos();
    }


    public virtual void Set_State_ShadowSize(BulletState_Size state_Size)
    {
        targetObject.transform.localScale = state_Size.objSize;
        shadowSize = state_Size.colSize;
    }

    protected virtual void SetOn_State()
    {
        Init_Data();

        seq = DOTween.Sequence();

        float _droppingTime = 1 / droppingSpeed;
        seq.Join(shadowTf.DOScale(shadowSize, _droppingTime).SetEase(Ease.Linear)); // 그림자
        seq.Join(DOTween.To(() => targetRange, x => targetRange = x, dropBottomYPos, _droppingTime).SetEase(Ease.InCubic)); // 떨어지는 이미지
        seq.Join(thisSr.DOFade(1f, _droppingTime * 0.3f).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            Active();
            seq = null;
        });
    }

    #endregion

    #region Init

    private void Init_Data()
    {
        thisSr.color = new Color(1f, 1f, 1f, 0f);
        shadowTf.transform.localScale = Vector2.zero;
    }

    #endregion

    #region Active

    protected abstract void Active();

    #endregion

    #region Light

    public virtual void SetOn_LightIntensity(float intensity)
    {
        light2D.intensity = intensity;
        light2D.lightCookieSprite = thisSr.sprite;
        DevTool.Set_AlphaColor(light2D, 0.5f);
    }

    #endregion

    #region Trail

    public void SetOn_TrailState(float time, float startWidth, Gradient gradient)
    {
        trail.time = time;
        trail.startWidth = startWidth;
        trail.colorGradient = gradient;
    }

    #endregion
}
