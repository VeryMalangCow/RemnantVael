using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionPrefabSO", menuName = "ScriptableObject/ExplosionPrefabSO")]
public class ExplosionResoSO : ScriptableObject
{
    public AnimationClip explosionAnimation;
    public AnimationClip[] attributeExplosionAnimations;

    [Space(10)]
    [Header("=== Player")]
    public PlayerExplosionController playerEplosionPrefab;

    [Space(10)]
    [Header("=== Ally")]
    public AllyExplosionController allyEplosionPrefab;

    [Space(10)]
    [Header("=== Enemy")]
    public EnemyExplosionController enemyEplosionPrefab;

    public Material enemyHittedVfxMaterial;

    public Material enemyExplosionMaterial;
}
