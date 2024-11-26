using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Animator ThisAnimator;
    [SerializeField] public CapsuleCollider2D ThisCol;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        // Time
        CurrentAliveTime += Time.deltaTime;
        if (CurrentAliveTime >= BulletState.AliveTime)
        {
            Debug.Log("- Time");
            DeleteThis();
        }
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
        Debug.Log(CurrentAliveTime);
        gameObject.SetActive(true);
        Debug.Log(CurrentAliveTime);
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        AttackPointEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);
        ExplosionEffect(TargetObject.transform.position, ThisSR.sortingOrder + 1, BulletState.DamageType, BulletState.IsCritical);


        ResetState();
        this.gameObject.SetActive(false);

        PoolingManager.Instance.EnemyBullets.Queue.Enqueue(this);
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
                /*PC.HittedPointEffect(
                    this.TargetObject.transform.position,
                    BulletState.DamageType,
                    BulletState.IsCritical,
                    transform.rotation);*/
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

    #region Effect

    private void ExplosionEffect(Vector2 _SpawndPos, int _SortLayer, eDamageType _DamageType, bool _IsCritical)
    {
        int index = 0;
        if (!_IsCritical)
        { index = 0; }
        else
        { index = 1; }
        

        /*PlayerManager.Instance.PlayerController.PlayerMEI.GenExplosionImgs(
                   _SpawndPos,
                   4, 0.3f, 0.4f,
                   0.6f, 0.05f, 0.1f,
                   0.3f, 0.5f, 1.0f,
                   index, PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000);*/
    }

    private void AttackPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota.StartAnim(
            PlayerManager.Instance.PlayerController.GetCorrectHitted_AC(_DamageType, _IsCritical),
            _SpanwedPos,
            PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000,
            2.0f, 1.0f);
    }

    #endregion
}
