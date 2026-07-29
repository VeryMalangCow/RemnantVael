using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyPatternCollectionSO",
    menuName = "ScriptableObject/EnemyPatternSO/PatternCollection")]
public class EnemyPatternCollectionSO : ScriptableObject
{
    [SerializeField] public List<EnemyPatternEntry> patterns;

    public void SortPatterns()
    {
        if (patterns == null || patterns.Count == 0)
            return;

        patterns.Sort((a, b) => a.priority.CompareTo(b.priority));
    }
}


[System.Serializable]
public class EnemyPatternEntry
{
    public EnemyPatternSequenceSO so;
    public int priority;
    public int weight;
}