using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternCollectionSO",
    menuName = "ScriptableObject/EnemyPatternSO/PatternCollection")]
public class EnemyPatternCollectionSO : ScriptableObject
{
    [SerializeField] public List<EnemyPatternEntry> patterns;

#if UNITY_EDITOR
    public void SortPatterns()
    {
        if (patterns == null || patterns.Count == 0)
            return;

        patterns.Sort((a, b) => a.priority.CompareTo(b.priority));
    }
#endif
}


[System.Serializable]
public class EnemyPatternEntry
{
    public EnemyPatternSequenceSO so;
    [Min(0)] public int priority;
    [Min(0)] public int weight;
}