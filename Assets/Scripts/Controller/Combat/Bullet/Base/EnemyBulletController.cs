using UnityEngine;

public class EnemyBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Animator at;
    [SerializeField] public CapsuleCollider2D col;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] protected int explAmount = 4;

    //Other
    [HideInInspector] public EnemyController ownEnemy;
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region State

    // 그림자 오브젝트의 크기, 판정 크기 (그림자 크기에 배수가 된다), 애니메이션의 산출
    public override void Set_State_Size(BulletState_Size? state_Size)
    {
        if (state_Size.HasValue)
        {
            base.Set_State_Size(state_Size);

            col.transform.localScale = state_Size.Value.objSize;
            col.size = state_Size.Value.colSize;
        }
    }

    public override void Set_State_Anim(State_Anim? state_Anim) 
    {
        if (state_Anim.HasValue)
        {
            base.Set_State_Anim(state_Anim);

            DevTool.Set_Anim(ref aoc, at, state_Anim.Value.ac);
            at.speed = state_Anim.Value.speed;
        }
    }

    public override void Set_State_Effect(BulletState_Effect? state_Effect)
    {
        if (state_Effect.HasValue)
        {
            base.Set_State_Effect(state_Effect);

            explAmount = state_Effect.Value.explAmount;
        }
    }

    #endregion

    protected override void Remove_Object()
    {
        if (currentAliveTime <= 0f) return;

        //BulletManager.instance.RemovePlayerBullet(this);

        SetOff_Trail();
        SetOff_Light();

        Reset_State();

        gameObject.SetActive(false);
    }


    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Player(col);

        base.OnTriggerEnter2D(col);
    }

    protected void Try_Hit_Player(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Player", out PlayerController pc))
        {
            pc.Try_Hitted(this);
        }
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {
        switch (poolingString)
        {
            case "EnemyBullet":
                UnitManager.instance.enemy_ExplImgGenerator.Expl_Enemy_ObjectDestroy(targetObject.transform.position, explAmount);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Pooling

    protected override void PoolingSet()
    {
        switch (poolingString)
        {
            case "EnemyBullet":
                UnitManager.instance.enemy_ExplImgGenerator.Expl_Enemy_ObjectDestroy(targetObject.transform.position, explAmount);
                PoolingManager.instance.enemyBullets.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Light

    public void SetOn_LightIntensity(float intensity)
    {
        light2d.intensity = intensity;
        light2d.lightCookieSprite = thisSr.sprite;
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
