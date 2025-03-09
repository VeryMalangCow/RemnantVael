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

    #region Set State

    // 그림자 오브젝트의 크기, 판정 크기 (그림자 크기에 배수가 된다), 애니메이션의 산출
    public void Set_State(AnimationClip _AC, Vector2 _ShadowScale, Vector2 _ColSize)
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, _AC);

        ThisCol.transform.localScale = _ShadowScale;
        ThisCol.size = _ColSize;
    }

    public override void Set_State(Vector2 _SpawnVec, BulletState _BulletState, float _SpreadAngle, float _TargetRange, Vector2 _Dir)
    {
        base.Set_State(_SpawnVec, _BulletState, 0, _TargetRange, _Dir);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (!this.gameObject.activeSelf)
        { return; }

        // Hit Enemy
        if (_Col.tag == "Player")
        {
            if (_Col.transform.parent.TryGetComponent(out PlayerController PC))
            {
                PC.Try_Hitted(this);
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildController DBC))
            {
                DBC.Take_Damage(true);
            }
        }

        if (DestroyTagList.Contains(_Col.tag))
        {
            Remove_Object();
        }
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        switch (PoolingString)
        {
            case "EnemyBullet":
                Enemy.MEI.Gen_ExplosionImgs(
                    TargetObject.transform.position,
                    16, 0.15f, 0.75f,
                    0.6f, 0.05f, 0.1f,
                    0.2f, 0.5f, 1.0f,
                    0, Enemy.ThisSmokeM);
                PoolingManager.Instance.EnemyBullets.Queue.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion
}
