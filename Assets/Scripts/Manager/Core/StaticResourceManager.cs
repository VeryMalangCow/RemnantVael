using UnityEngine;

public class StaticResourceManager : PersistentSingleton<StaticResourceManager>
{
    [Header("=== Stage")]
    [SerializeField] private StagePrefabSO stagePrefab;
    [SerializeField] private StageIconSO stageIcon;
    public StagePrefabSO StagePrefab => stagePrefab;
    public StageIconSO StageIcon => stageIcon;


    [Header("=== Build")]
    [SerializeField] private BuildPrefabSO buildPrefab;
    public BuildPrefabSO BuildPrefab => buildPrefab;

    [Header("=== Enemy")]
    [SerializeField] private EnemyPrefabSO enemyPrefab;
    public EnemyPrefabSO EnemyPrefab => enemyPrefab;

    [Header("=== Combat")]
    [SerializeField] private ExplosionPrefabSO explosionPrefab;
    public ExplosionPrefabSO ExplosionPrefab => explosionPrefab;

    [Header("=== Item")]
    [SerializeField] private ItemIconSO itemIcon;
    public ItemIconSO ItemIcon => itemIcon;

}
