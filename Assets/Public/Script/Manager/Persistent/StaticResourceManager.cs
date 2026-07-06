using UnityEngine;

public class StaticResourceManager : PersistentSingleton<StaticResourceManager>
{
    [SerializeField] private StageResoSO stageReso;
    [SerializeField] private BuildResoSO buildReso;
    [SerializeField] private EnemyResoSO enemyReso;
    [SerializeField] private AllyResoSO allyReso;
    [SerializeField] private ExplosionResoSO explosionReso;
    [SerializeField] private ItemResoSO itemReso;
    [SerializeField] private EventResoSO eventReso;
    [SerializeField] private SoundResoSO soundReso;

    public StageResoSO StageReso => stageReso;
    public BuildResoSO BuildReso => buildReso;
    public EnemyResoSO EnemyReso => enemyReso;
    public AllyResoSO AllyReso => allyReso;
    public ExplosionResoSO ExplosionReso => explosionReso;
    public ItemResoSO ItemReso => itemReso;
    public EventResoSO EventReso => eventReso;
    public SoundResoSO SoundReso => soundReso;

}
