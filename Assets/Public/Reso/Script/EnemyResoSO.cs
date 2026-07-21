using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabSO", menuName = "ScriptableObject/EnemyPrefabSO")]
public class EnemyResoSO : ScriptableObject
{
    public Sprite[] enemyPhaseSprites;

    public Material hittedVfxMaterial;

    public Material explosionMaterial;

    [Space(30)]
    [Header("=== CSV")]
    public TextAsset enemyNameCsv;
}
