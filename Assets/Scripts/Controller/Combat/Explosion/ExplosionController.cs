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
    [SerializeField] private SortingGroup ThisSG;
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] protected CircleCollider2D ThisCol;
    [SerializeField] protected Light2D ThisLight;
    [SerializeField] private AudioSource ThisAS;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public ExplosionState State;

    #endregion

    #region - Hide

    [HideInInspector] protected HashSet<StaticDepthController> HittedObjectList = new HashSet<StaticDepthController>();

    [HideInInspector] private AnimatorOverrideController AOC;

    [HideInInspector] public static readonly float BigExplostionColSize = 1.8f;

    [HideInInspector] public static readonly float AnimSpeed = 2f;
    [HideInInspector] public static readonly float JugeTime = 0.5f;

    #endregion

    #endregion

    #region State (Reset)

    protected virtual void Reset_State()
    {
        AOC = null;
        State.Reset_State();
        HittedObjectList.Clear();
        ThisLight.intensity = 0;
    }

    #endregion

    #region State (Set)


    public void Set_State(
        ExplosionState _State,
        AnimationClip _AC,
        State_TF2D _State_StartTF,
        float _TargetRange = 0.4f)
    {
        Set_State_Base(_State, _TargetRange);
        Set_State_Juge(BigExplostionColSize, _State.attackSizeState.size);
        Set_State_Anim(_AC);
        Set_State_StartTF(_State_StartTF);
        Set_State_Extra();

        SetOn_State();
    }

    public virtual void Set_State_Base(ExplosionState _State, float _TargetRange = 0.4f)
    {
        this.State = new ExplosionState(_State);

        ThisSG.sortingOrder = LayerOrderManager.order_Explosion;
        TargetRange = _TargetRange;
    }

    public virtual void Set_State_Juge(float _ExplosionSize, float _ColRadius)
    {
        TargetObject.transform.localScale = Vector2.one * _ColRadius;
        ThisCol.radius = _ExplosionSize * _ColRadius;
        ThisLight.pointLightOuterRadius = _ExplosionSize * _ColRadius;
    }

    public virtual void Set_State_Anim(AnimationClip _AC)
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, _AC);
        ThisAnimator.speed = AnimSpeed;
    }

    public virtual void Set_State_StartTF(State_TF2D _State_StartTF)
    {
        this.transform.position = _State_StartTF.pos;
        this.transform.localScale = _State_StartTF.localScale;
    }

    public virtual void Set_State_Extra() { }


    private void SetOn_State()
    {
        this.gameObject.SetActive(true);

        SoundManager.instance.Play_2D_SFX_Combat(ThisAS, "Explosion");

        if (State.isFire) SoundManager.instance.Play_2D_SFX_Status("Fire");
        if (State.isCold) SoundManager.instance.Play_2D_SFX_Status("Cold");
        if (State.isElectricity) SoundManager.instance.Play_2D_SFX_Status("Electricity");
        if (State.isCorrosion) SoundManager.instance.Play_2D_SFX_Status("Corrosion");

        StartCoroutine(Start_Play_Cor());
    }

    #endregion

    #region Play

    private IEnumerator Start_Play_Cor()
    {
        DOTween.To(() => ThisLight.intensity, x => ThisLight.intensity = x, 1f, JugeTime * 0.2f);
        yield return new WaitForSeconds(JugeTime * 0.2f);

        DOTween.To(() => ThisLight.intensity, x => ThisLight.intensity = x, 0f, JugeTime * 0.2f);
        yield return new WaitForSeconds(JugeTime * 0.8f);

        Remove_Object();
    }


    #endregion

    #region Add

    public void Add_HittedObjectList(StaticDepthController _Object)
    {
        HittedObjectList.Add(_Object);
    }
    
    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_DestructibleObject(_Col);
    }

    protected void Try_Hit_DestructibleObject(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "DestructibleObject", HittedObjectList, out DestructibleBuildController dbc))
        {
            dbc.Take_Damage(_SpawnItem: true, _SoundOn: true);
            HittedObjectList.Add(dbc);
        }
        else if (DevTool.Can_Collding(_Col, "FieldObj", HittedObjectList, out DestructibleObjectController doc))
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
