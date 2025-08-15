using UnityEngine;

public class EnemyBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Animator ThisAnimator;
    [SerializeField] public CapsuleCollider2D ThisCol;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] protected int ExplAmount = 4;

    //Other
    [HideInInspector] public EnemyController Enemy;
    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region State

    // 그림자 오브젝트의 크기, 판정 크기 (그림자 크기에 배수가 된다), 애니메이션의 산출
    public override void Set_State_Size(BulletState_Size? _State_Size)
    {
        if (_State_Size.HasValue)
        {
            base.Set_State_Size(_State_Size);

            ThisCol.transform.localScale = _State_Size.Value.ObjSize;
            ThisCol.size = _State_Size.Value.ColSize;
        }
    }

    public override void Set_State_Anim(State_Anim? _State_Anim) 
    {
        if (_State_Anim.HasValue)
        {
            base.Set_State_Anim(_State_Anim);

            DevTool.Set_Anim(ref AOC, ThisAnimator, _State_Anim.Value.AC);
            ThisAnimator.speed = _State_Anim.Value.Speed;
        }
    }

    public override void Set_State_Effect(BulletState_Effect? _State_Effect)
    {
        if (_State_Effect.HasValue)
        {
            base.Set_State_Effect(_State_Effect);

            ExplAmount = _State_Effect.Value.ExplAmount;
        }
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Player(_Col);

        base.OnTriggerEnter2D(_Col);
    }

    protected void Try_Hit_Player(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Player", out PlayerController pc))
        {
            pc.Try_Hitted(this);
        }
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        switch (PoolingString)
        {
            case "EnemyBullet":
                UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy_ObjectDestroy(TargetObject.transform.position, ExplAmount);
                PoolingManager.Instance.EnemyBullets.Queue.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Light

    public void SetOn_LightIntensity(float _Intensity)
    {
        ThisLight.intensity = _Intensity;
        ThisLight.lightCookieSprite = ThisSR.sprite;
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
