using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : Singleton<DropItemManager>, IMainGameInitializer
{

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [SerializeField] public PoolSystem<JouleController> joulePool;
    [SerializeField] public PoolSystem<BetteryShardController> betteryShardPool;
    [SerializeField] public PoolSystem<ModuleShardController> moduleShardPool;
    [SerializeField] public PoolSystem<OverriderController> overriderPool;
    [SerializeField] public PoolSystem<CreditController> creditPool;
    [SerializeField] public PoolSystem<ModuleItemController> modulePool;
    [SerializeField] public PoolSystem<KeycardItemController> keycardItemPool;
    [SerializeField] public PoolSystem<CoreItemController> coreItemPool;

    // Init
    public IEnumerator Initialize()
    {
        yield return joulePool.InitAsync(32, 8f);
        yield return betteryShardPool.InitAsync(32, 8f);
        yield return moduleShardPool.InitAsync(32, 8f);
        yield return overriderPool.InitAsync(32, 8f);
        yield return creditPool.InitAsync(32, 8f);

        yield return modulePool.InitAsync(32, 8f);
        yield return keycardItemPool.InitAsync(32, 8f);
        yield return coreItemPool.InitAsync(32, 8f);

        enabled = true;
    }

    // Centralized Update
    private void Update()
    {
        float dt = Time.deltaTime;

        HandleAbsorbItem(joulePool, dt);
        HandleAbsorbItem(betteryShardPool, dt);
        HandleAbsorbItem(moduleShardPool, dt);
        HandleAbsorbItem(overriderPool, dt);
        HandleAbsorbItem(creditPool, dt);

        HandleSpreadItem(modulePool, dt);
        HandleSpreadItem(keycardItemPool, dt);
        HandleSpreadItem(coreItemPool, dt);
    }

    private void HandleAbsorbItem<T>(PoolSystem<T> pool, float dt) where T : AbsorbItemController
    {
        var activeIndices = pool.activeIndices;
        var objs = pool.objs;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleAbsorb(dt);
    }
    private void HandleSpreadItem<T>(PoolSystem<T> pool, float dt) where T : InteractItemController
    {
        var activeIndices = pool.activeIndices;
        var objs = pool.objs;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleSpread(dt);
    }
    
    // Spawn & Remove
    public JouleController SpawnJoule() => joulePool.Dequeue();
    public void RemoveJoule(JouleController item) => joulePool.Enqueue(item);

    public BetteryShardController SpawnBetteryShard() => betteryShardPool.Dequeue();
    public void RemoveBetteryShard(BetteryShardController item) => betteryShardPool.Enqueue(item);

    public ModuleShardController SpawnModuleShard() => moduleShardPool.Dequeue();
    public void RemoveModuleShard(ModuleShardController item) => moduleShardPool.Enqueue(item);

    public OverriderController SpawnOverrider() => overriderPool.Dequeue();
    public void RemoveOverrider(OverriderController item) => overriderPool.Enqueue(item);

    public CreditController SpawnCredit() => creditPool.Dequeue();
    public void RemoveCredit(CreditController item) => creditPool.Enqueue(item);

    public ModuleItemController SpawnModule() => modulePool.Dequeue();
    public void RemoveModule(ModuleItemController item) => modulePool.Enqueue(item);

    public KeycardItemController SpawnKeycard() => keycardItemPool.Dequeue();
    public void RemoveKeycard(KeycardItemController item) => keycardItemPool.Enqueue(item);

    public CoreItemController SpawnCore() => coreItemPool.Dequeue();
    public void RemoveCore(CoreItemController item) => coreItemPool.Enqueue(item);
}
