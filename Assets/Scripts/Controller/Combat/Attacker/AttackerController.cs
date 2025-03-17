using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class AttackerController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Attacker")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected GameObject ThisColGO;
    [HideInInspector] protected Collider2D ThisCol;
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private Light2D ThisLight;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState AttackerState;

    [Space(10)]
    [Header("=== Object")]
    [SerializeField] protected List<StaticDepthController> HittedObjectList = new List<StaticDepthController>();


    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region Reset

    public void Reset_State()
    {
        AttackerState.Reset_State();

        Reset_BaseAttacker();
        Reset_Other();
    }

    private void Reset_BaseAttacker()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        DevTool.Remove_Component(ThisCol);
        ThisLight.pointLightOuterRadius = 0f;
        gameObject.SetActive(false);

        AOC = null;
        HittedObjectList = new List<StaticDepthController>();
    }

    protected virtual void Reset_Other()
    {

    }

    #endregion

    #region State

    public Sequence Set_State<T>(
        AttackerState _State,
        AttackerState_Juge<T> _State_Juge,
        State_Anim _State_Anim,
        State_TF2D _State_StartTF,
        AttackerState_EndTF _State_EndTF,
        float _TargetRange = 0.4f) where T : Collider2D
    {
        Sequence seq = DOTween.Sequence();

        Set_State_Base(_State, _TargetRange);
        Set_State_Juge<T>(_State_Juge);
        Set_State_Anim(_State_Anim);
        Set_State_StartTF(_State_StartTF);
        seq.Join(Set_State_EndTF(_State_EndTF));
        Set_State_Extra();

        SetOn_State(seq);

        return seq;
    }

    public virtual void Set_State_Base(AttackerState _State, float _TargetRange = 0.4f)
    {
        this.AttackerState = new AttackerState(_State);

        TargetRange = _TargetRange;
    }

    public virtual void Set_State_Juge<T>(AttackerState_Juge<T> _State_Juge) where T : Collider2D
    {
        ThisCol = DevTool.Gen_Component<T>(ThisColGO);
        ThisCol.isTrigger = true;

        if (DevTool.Can_CastingTType(ThisCol, out CapsuleCollider2D capsule2D))
        {
            capsule2D.size = _State_Juge.ColSize;
            capsule2D.direction = _State_Juge.IsVertical ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
        }
        else if (DevTool.Can_CastingTType(ThisCol, out CircleCollider2D circle2D))
        {
            circle2D.radius = _State_Juge.ColSize.x;
        }
    }

    public virtual void Set_State_Anim(State_Anim _State_Anim)
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, _State_Anim.AC);
        ThisAnimator.speed = _State_Anim.Speed;
    }

    public virtual void Set_State_StartTF(State_TF2D _State_StartTF)
    {
        this.transform.position = _State_StartTF.Pos;
        this.transform.rotation = _State_StartTF.Rot;
        this.transform.localScale = _State_StartTF.LocalScale;
    }

    public virtual Sequence Set_State_EndTF(AttackerState_EndTF _State_EndTF) 
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(this.transform.DOMove(_State_EndTF.TF.Pos, _State_EndTF.Time));
        seq.Join(TargetObject.transform.DORotateQuaternion(_State_EndTF.TF.Rot, _State_EndTF.Time));
        seq.Join(this.transform.DOScale(_State_EndTF.TF.LocalScale, _State_EndTF.Time));

        return seq;
    }

    public virtual void Set_State_Extra() { }


    private void SetOn_State(Sequence _TotalSeq)
    {
        this.gameObject.SetActive(true);

        _TotalSeq.OnComplete(() =>
        {
            Remove_Object();
        });
    }

    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_DestructibleObject(_Col);
    }

    protected void Try_Hit_DestructibleObject(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "DestructibleObject", 
            HittedObjectList, out DestructibleBuildController dbc))
        {
            dbc.Take_Damage(true);
            HittedObjectList.Add(dbc);
        }
    }

    #endregion

    #region Remove

    protected abstract void Remove_Condition();

    public virtual void Remove_Object()
    {
        Remove_Condition();
        Reset_State();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Light

    // 빛이 생성되어 커지고 사라질 때 줄어듬
    public void Set_Light(float _BiggestSize, float _DurTime, float _IntroTime = 0.15f, float _StayTime = 0.7f, float _VanishTime = 0.15f)
    {
        if (DevTool.Is_Usable(ThisLight))
        {
            Sequence seq = DOTween.Sequence();
            ThisLight.pointLightOuterRadius = 0f;
            seq.Append(Get_LightSize(_BiggestSize, _DurTime * _IntroTime));
            seq.AppendInterval(_DurTime * _StayTime);
            seq.Append(Get_LightSize(0, _DurTime * 0.15f));
        }
    }

    private Tween Get_LightSize(float _TargetSize, float _Time)
    {
        return DOTween.To(() => ThisLight.pointLightOuterRadius, x => ThisLight.pointLightOuterRadius = x, _TargetSize, _Time);
    }

    #endregion
}