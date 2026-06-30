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

}
