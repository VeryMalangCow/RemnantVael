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
    [HideInInspector] public EnemyController OwnerEC;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

    }

    #endregion

    #region Set State

    public void SetState(Vector2 _SpawnVec, BulletState _BulletState, Vector2 _Dir, Vector2 _ShadowScale, Vector2 _ColSize, AnimationClip _AC, float _TargetRange)
    {
        this.transform.localRotation = GetRotByVec2(_Dir);

        AnimatorOverrideController aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;

        ThisCol.transform.localScale = _ShadowScale;
        ThisCol.size = _ColSize;

        base.SetState(_SpawnVec, 0, _BulletState, _TargetRange);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Delete

    protected override void DeleteThis()
    {
        OwnerEC.MEI.GenExplosionImgs(
            TargetObject.transform.position,
            16, 0.15f, 0.75f,
            0.6f, 0.05f, 0.1f,
            0.2f, 0.5f, 1.0f,
            0, OwnerEC.ThisSmokeM);

        base.DeleteThis();
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
                PC.TryHitted(this);
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildingController DBC))
            {
                DBC.TakeDamage(true);
            }
        }

        if (DestroyTagList.Contains(_Col.tag))
        {
            DeleteThis();
        }
    }

    #endregion

}
