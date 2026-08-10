using System.Collections.Generic;
using UnityEngine;

public class EnemyPatternSelector
{
    [Header("Priority (Lower is Higher)")]
    private List<EnemyPatternEntry> samePriorityPatternsCache = new();

    private EnemyPatternCollectionSO collectionSO;
    private EnemyPatternSequenceSO defaultSequenceSO;

    public EnemyPatternSelector(EnemyPatternCollectionSO collectionSO, EnemyPatternSequenceSO defaultSequenceSO)
    {
        this.collectionSO = collectionSO;
        this.defaultSequenceSO = defaultSequenceSO;
    }

    // patterns must be sorted by priority.
    /// <summary> Get Playable Pattern From PatternCollectionSO </summary>
    public EnemyPatternSequenceSO GetPlayablePattern(EnemyAIContext context)
    {
        SetSamePriorityPatterns(collectionSO.patterns, context);
        var entry = GetPatternEntryByWeight();
        return entry?.so ?? defaultSequenceSO;
    }

    /// <summary> Set Same Priority Pattarns -> [samePriorityPatternsCache] </summary>
    private void SetSamePriorityPatterns(List<EnemyPatternEntry> patterns, EnemyAIContext context)
    {
        samePriorityPatternsCache.Clear();

        bool selectedPrirority = false;
        int priority = int.MinValue;

        for (int i = 0; i < patterns.Count; i++)
        {
            bool canPlay = patterns[i].so.IsSatisfied(context);

            if (!selectedPrirority && canPlay)
            {
                selectedPrirority = true;
                priority = patterns[i].priority;
            }

            if (selectedPrirority)
            {
                if (priority == patterns[i].priority && canPlay)
                    samePriorityPatternsCache.Add(patterns[i]);
                else
                    break;
            }
        }
    }

    /// <summary> Get Pattern Entry by Weight </summary>
    private EnemyPatternEntry GetPatternEntryByWeight()
    {
        if (samePriorityPatternsCache.Count == 0)
            return null;

        int totalWeight = 0;

        for (int i = 0; i < samePriorityPatternsCache.Count; i++)
        {
            totalWeight += Mathf.Max(0, samePriorityPatternsCache[i].weight);
        }

        if (totalWeight <= 0)
            return samePriorityPatternsCache[0];

        int randomWeight = Random.Range(0, totalWeight);

        for (int i = 0; i < samePriorityPatternsCache.Count; i++)
        {
            randomWeight -= Mathf.Max(0, samePriorityPatternsCache[i].weight);

            if (randomWeight < 0)
                return samePriorityPatternsCache[i];
        }

        // 이론상 여기까지 오지 않지만 안전장치
        return samePriorityPatternsCache[^1];
    }

}
