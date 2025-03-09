using System.Collections.Generic;
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

    public override void Set_State_Anim(BulletState_Anim? _State_Anim) 
    {
        if (_State_Anim.HasValue)
        {
            base.Set_State_Anim(_State_Anim);

            DevTool.Set_Anim(ref AOC, ThisAnimator, _State_Anim.Value.AC);
            ThisAnimator.speed = _State_Anim.Value.Speed;
        }
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (!this.gameObject.activeSelf)
        { return; }

        Try_Hit_Player(_Col);
        Try_Hit_DestructibleObject(_Col);

        Try_Remove(_Col.tag);
    }

    protected void Try_Hit_Player(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        {
            if (_Col.transform.parent.TryGetComponent(out PlayerController PC))
            {
                PC.Try_Hitted(this);
            }
        }
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        switch (PoolingString)
        {
            case "EnemyBullet":
                Gen_ExplosionEffect();
                PoolingManager.Instance.EnemyBullets.Queue.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Effect

    private void Gen_ExplosionEffect()
    {
        Enemy.MEI.Gen_ExplosionImgs(
            TargetObject.transform.position,
                        16, 0.15f, 0.75f,
                        0.6f, 0.05f, 0.1f,
                        0.2f, 0.5f, 1.0f,
                        0, Enemy.ThisSmokeM);
    }

    #endregion
}
