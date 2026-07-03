using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabSO", menuName = "ScriptableObject/EnemyPrefabSO")]
public class EnemyPrefabSO : ScriptableObject
{
    public Sprite[] enemyPhaseSprites;
}
