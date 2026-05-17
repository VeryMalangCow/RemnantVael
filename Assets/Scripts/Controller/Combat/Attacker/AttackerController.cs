using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class AttackerController : MovableDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Attacker")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected GameObject colGo;
    [SerializeField] private Animator at;
    [SerializeField] private Light2D light2d;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public AttackerState attackerState;

    [Space(10)]
    [Header("=== Object")]
    [SerializeField] protected HashSet<StaticDepthController> hittedObjList = new HashSet<StaticDepthController>();

    #endregion

    #region - Hide

    [HideInInspector] protected Collider2D col;
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        AddSortingLayer();
    }

    protected void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion

    #region Reset

    public void Reset_State()
    {
        attackerState.Reset_State();

        Reset_BaseAttacker();
        Reset_Other();
    }

    private void Reset_BaseAttacker()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        DevTool.Remove_Component(col);
        light2d.pointLightOuterRadius = 0f;
        gameObject.SetActive(false);

        aoc = null;
        hittedObjList = new HashSet<StaticDepthController>();
    }

    protected virtual void Reset_Other()
    {

    }

    #endregion

    #region State

    public Sequence Set_State<T>(
        AttackerState state,
        AttackerState_Juge<T> state_Juge,
        State_Anim state_Anim,
        State_TF2D state_StartTF,
        AttackerState_EndTF state_EndTF,
        float targetRange = 0.4f,
        Transform parent = null,
        bool isLocal = false) where T : Collider2D
    {
        UnitManager.instance.Add_Unit(this);

        Sequence seq = DOTween.Sequence();

        Set_State_Base(state, targetRange);
        Set_State_Juge<T>(state_Juge);
        Set_State_Anim(state_Anim);
        Set_State_StartTF(state_StartTF, parent, isLocal);
        seq.Join(Set_State_EndTF(state_EndTF, isLocal));
        Set_State_Extra();

        SetOn_State(seq);

        return seq;
    }

    public virtual void Set_State_Base(AttackerState state, float targetRange = 0.4f)
    {
        this.attackerState = new AttackerState(state);

        base.targetRange = targetRange;
    }

    public virtual void Set_State_Juge<T>(AttackerState_Juge<T> state_Juge) where T : Collider2D
    {
        col = DevTool.Gen_Component<T>(colGo);
        col.isTrigger = true;

        if (DevTool.Can_CastingTType(col, out CapsuleCollider2D capsule2D))
        {
            capsule2D.size = state_Juge.colSize;
            capsule2D.direction = state_Juge.isVertical ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
        }
        else if (DevTool.Can_CastingTType(col, out CircleCollider2D circle2D))
        {
            circle2D.radius = state_Juge.colSize.x;
        }

        at.transform.localScale = state_Juge.colSize;
    }

    public virtual void Set_State_Anim(State_Anim state_Anim)
    {
        DevTool.Set_Anim(ref aoc, at, state_Anim.ac);

        at.speed = state_Anim.speed;

        at.Rebind();
    }

    public virtual void Set_State_StartTF(State_TF2D state_StartTF, Transform parent, bool isLocalPos)
    {
        if (isLocalPos)
        {
            this.transform.SetParent(parent);
            this.transform.localPosition = state_StartTF.pos;
        }
        else
        {
            this.transform.SetParent(StageManager.instance.currentRoomController.transform);
            this.transform.position = state_StartTF.pos;
        }
        this.transform.rotation = state_StartTF.rot;
        this.transform.localScale = state_StartTF.localScale;
    }

    public virtual Sequence Set_State_EndTF(AttackerState_EndTF state_EndTF, bool isLocalPos) 
    {
        Sequence seq = DOTween.Sequence();

        if (isLocalPos)
        {
            seq.Join(this.transform.DOLocalMove(state_EndTF.tf.pos, state_EndTF.time).SetEase(Ease.Linear));
        }
        else
        {
            seq.Join(this.transform.DOMove(state_EndTF.tf.pos, state_EndTF.time).SetEase(Ease.Linear));
        }

        seq.Join(targetObject.transform.DORotateQuaternion(state_EndTF.tf.rot, state_EndTF.time).SetEase(Ease.Linear));
        seq.Join(this.transform.DOScale(state_EndTF.tf.localScale, state_EndTF.time).SetEase(Ease.Linear));

        return seq;
    }

    public virtual void Set_State_Extra() { }


    private void SetOn_State(Sequence totalSeq)
    {
        this.gameObject.SetActive(true);
        //this.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);

        totalSeq.OnComplete(() =>
        {
            Remove_Object();
        });
    }

    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_DestructibleObject(col);
    }

    protected void Try_Hit_DestructibleObject(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "DestructibleObject", hittedObjList, out DestructibleBuildController dbc))
        {
            dbc.Take_Damage(spawnItem: true, soundOn: true);
            hittedObjList.Add(dbc);
        }

        else if (DevTool.Can_Collding(col, "FieldObj", hittedObjList, out DestructibleObjectController doc))
        {
            doc.Destruct();
        }
    }

    #endregion

    #region Pooling

    protected abstract void PoolingSet();

    #endregion

    #region Remove

    public virtual void Remove_Object()
    {
        UnitManager.instance.Add_Unit(this);

        RemoveForce_Object();
    }

    public void RemoveForce_Object()
    {
        Reset_State();
        PoolingSet();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Light

    // 빛이 생성되어 커지고 사라질 때 줄어듬
    public void Set_Light(float biggestSize, float durTime, float introTime = 0.15f, float stayTime = 0.7f, float vanishTime = 0.15f)
    {
        if (DevTool.Is_Usable(light2d))
        {
            Sequence seq = DOTween.Sequence();
            light2d.pointLightOuterRadius = 0f;
            seq.Append(Get_LightSize(biggestSize, durTime * introTime));
            seq.AppendInterval(durTime * stayTime);
            seq.Append(Get_LightSize(0, durTime * 0.15f));
        }
    }

    private Tween Get_LightSize(float targetSize, float time)
    {
        return DOTween.To(() => light2d.pointLightOuterRadius, x => light2d.pointLightOuterRadius = x, targetSize, time);
    }

    #endregion
}