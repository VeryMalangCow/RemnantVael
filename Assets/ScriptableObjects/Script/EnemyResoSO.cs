using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabSO", menuName = "ScriptableObject/EnemyPrefabSO")]
public class EnemyResoSO : ScriptableObject
{
    public Sprite[] enemyPhaseSprites;

    public Material hittedVfxMaterial;

    public Material explosionMaterial;
}
