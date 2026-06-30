using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>, IMainGameInitializer
{
    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [SerializeField] private PoolSystem<DeadParticleController> deadParticlePool;
    [SerializeField] private PoolSystem<PoolableSpriteRenderer> afterImgPool;
    [SerializeField] private PoolSystem<PoolableSpriteRenderer> explosionImgPool;
    [SerializeField] private PoolSystem<OnceTimeAnimController> onlyOnceAnimatorPool;
    [SerializeField] private PoolSystem<PoolableSpriteRenderer> areaSpoterPool;

    [SerializeField] private PoolSystem<WorldTxtEUIController> dmgTxtPool;
    [SerializeField] private PoolSystem<BuffIconEUIController> buffIconPool;

    [Space(10)]
    [SerializeField] public PlayerExplImgGenerator player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator enemy_ExplImgGenerator;
    [SerializeField] public OnceTimeAnimGenerator onceTime_AnimGenerator;

    // Init
    public IEnumerator Initialize()
    {
        yield return deadParticlePool.InitAsync(64, 8f);
        yield return afterImgPool.InitAsync(256, 8f);
        yield return explosionImgPool.InitAsync(256, 8f);
        yield return onlyOnceAnimatorPool.InitAsync(64, 8f);
        yield return areaSpoterPool.InitAsync(256, 8f);

        yield return dmgTxtPool.InitAsync(32, 8f);
        yield return buffIconPool.InitAsync(32, 8f);

        yield return null;

        enabled = true;
    }

    // Centralized Update
    private void Update()
    {
        HandleCheckingAnimEnd();
    }

    private void HandleCheckingAnimEnd()
    {
        var pool = onlyOnceAnimatorPool;
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleCheckingEndAnim();
    }

    // Return All
    public void ReturnAll()
    {
        deadParticlePool.ReturnAll();
        afterImgPool.ReturnAll();
        explosionImgPool.ReturnAll();
        onlyOnceAnimatorPool.ReturnAll();
        areaSpoterPool.ReturnAll();

        dmgTxtPool.ReturnAll();
        buffIconPool.ReturnAll();
    }

    #region Vfx

    public DeadParticleController SpawnDeadParticle() => deadParticlePool.Dequeue();
    public void RemoveDeadParticle(DeadParticleController particle) => deadParticlePool.Enqueue(particle);

    public PoolableSpriteRenderer SpawnAfterImg() => afterImgPool.Dequeue();
    public void RemoveAfterImg(PoolableSpriteRenderer particle) => afterImgPool.Enqueue(particle);

    public PoolableSpriteRenderer SpawnExplosionImg() => explosionImgPool.Dequeue();
    public void RemoveExplosionImg(PoolableSpriteRenderer particle) => explosionImgPool.Enqueue(particle);

    public OnceTimeAnimController SpawnOnlyOnceAnim() => onlyOnceAnimatorPool.Dequeue();
    public void RemoveOnlyOnceAnim(OnceTimeAnimController particle) => onlyOnceAnimatorPool.Enqueue(particle);

    public PoolableSpriteRenderer SpawnAreaSpoter() => areaSpoterPool.Dequeue();
    public void SpawnAreaSpoters(int amount, List<PoolableSpriteRenderer> list) => areaSpoterPool.DequeueMany(amount, list);
    public void RemoveAreaSpoter(PoolableSpriteRenderer particle) => areaSpoterPool.Enqueue(particle);

    #endregion

    #region Ui

    public WorldTxtEUIController SpawnDmgTxtCanvas() => dmgTxtPool.Dequeue();
    public void RemoveDmgTxtCanvas(WorldTxtEUIController particle) => dmgTxtPool.Enqueue(particle);

    public BuffIconEUIController SpawnBuffIcon() => buffIconPool.Dequeue();
    public void RemoveBuffIcon(BuffIconEUIController particle) => buffIconPool.Enqueue(particle);


    #endregion
}
