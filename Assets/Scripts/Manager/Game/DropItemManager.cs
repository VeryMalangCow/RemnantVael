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
