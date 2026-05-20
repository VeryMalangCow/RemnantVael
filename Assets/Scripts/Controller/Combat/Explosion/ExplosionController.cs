using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class ExplosionController : StaticDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Explosion")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private SortingGroup sg;
    [SerializeField] private Animator at;
    [SerializeField] protected CircleCollider2D col;
    [SerializeField] protected Light2D light2d;
    [SerializeField] private AudioSource _as;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public ExplosionState state;

    #endregion

    #region - Hide

    [HideInInspector] protected HashSet<StaticDepthController> hittedObjectList = new HashSet<StaticDepthController>();

    [HideInInspector] private AnimatorOverrideController aoc;

    [HideInInspector] public static readonly float bigExplostionColSize = 1.8f;

    [HideInInspector] public static readonly float animSpeed = 2f;
    [HideInInspector] public static readonly float jugeTime = 0.5f;

    #endregion

    #endregion

    #region State (Reset)

    protected virtual void Reset_State()
    {
        aoc = null;
        state.Reset_State();
        hittedObjectList.Clear();
        light2d.intensity = 0;
    }

    #endregion

    #region State (Set)


    public void Set_State(
        ExplosionState state,
        AnimationClip ac,
        State_TF2D state_StartTF,
        float targetRange = 0.4f)
    {
        Set_State_Base(state, targetRange);
        Set_State_Juge(bigExplostionColSize, state.attackSizeState.size);
        Set_State_Anim(ac);
        Set_State_StartTF(state_StartTF);
        Set_State_Extra();

        SetOn_State();
    }

    public virtual void Set_State_Base(ExplosionState state, float targetRange = 0.4f)
    {
        this.state = new ExplosionState(state);

        sg.sortingOrder = SortingOrderManager.order_Explosion;
        base.targetRange = targetRange;
    }

    public virtual void Set_State_Juge(float explosionSize, float colRadius)
    {
        targetObject.transform.localScale = Vector2.one * colRadius;
        col.radius = explosionSize * colRadius;
        light2d.pointLightOuterRadius = explosionSize * colRadius;
    }

    public virtual void Set_State_Anim(AnimationClip _AC)
    {
        DevTool.Set_Anim(ref aoc, at, _AC);
        at.speed = animSpeed;
    }

    public virtual void Set_State_StartTF(State_TF2D state_StartTF)
    {
        this.transform.position = state_StartTF.pos;
        this.transform.localScale = state_StartTF.localScale;
    }

    public virtual void Set_State_Extra() { }


    private void SetOn_State()
    {
        this.gameObject.SetActive(true);

        SoundManager.instance.Play_2D_SFX_Combat(_as, "Explosion");

        if (state.isFire) SoundManager.instance.Play_2D_SFX_Status("Fire");
        if (state.isCold) SoundManager.instance.Play_2D_SFX_Status("Cold");
        if (state.isElectricity) SoundManager.instance.Play_2D_SFX_Status("Electricity");
        if (state.isCorrosion) SoundManager.instance.Play_2D_SFX_Status("Corrosion");

        StartCoroutine(Start_Play_Cor());
    }

    #endregion

    #region Play

    private IEnumerator Start_Play_Cor()
    {
        DOTween.To(() => light2d.intensity, x => light2d.intensity = x, 1f, jugeTime * 0.2f);
        yield return new WaitForSeconds(jugeTime * 0.2f);

        DOTween.To(() => light2d.intensity, x => light2d.intensity = x, 0f, jugeTime * 0.2f);
        yield return new WaitForSeconds(jugeTime * 0.8f);

        Remove_Object();
    }


    #endregion

    #region Add

    public void Add_HittedObjectList(StaticDepthController obj)
    {
        hittedObjectList.Add(obj);
    }
    
    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_DestructibleObject(col);
    }

    protected void Try_Hit_DestructibleObject(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "DestructibleObject", hittedObjectList, out DestructibleBuildController dbc))
        {
            dbc.Take_Damage(spawnItem: true, soundOn: true);
            hittedObjectList.Add(dbc);
        }
        else if (DevTool.Can_Collding(col, "FieldObj", hittedObjectList, out DestructibleObjectController doc))
        {
            doc.Destruct();
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
}
