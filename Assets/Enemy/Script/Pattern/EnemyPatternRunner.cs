using System.Collections;
using UnityEngine;

public class EnemyPatternRunner
{
    private EnemyPatternSelector selector;
    private EnemyAIContext context;

    private WaitForSeconds wait;

    public EnemyPatternRunner(EnemyPatternSelector selector, EnemyAIContext context, float patternIntervalTime)
    {
        this.selector = selector;
        this.context = context;
        wait = new WaitForSeconds(patternIntervalTime);
    }

    public IEnumerator RunLoop()
    {
        while (true)
        {
            EnemyPatternSequenceSO seqSO = selector.GetPlayablePattern(context);

            yield return PlaySequence(seqSO);
            yield return wait;
        }
    }

    public IEnumerator PlaySequence(EnemyPatternSequenceSO sequenceSO)
    {
        if (sequenceSO == null)
            yield break;

        var patterns = sequenceSO.Patterns;

        for (int i = 0; i < patterns.Count; i++)
        {
            if (patterns[i] == null)
                continue;

            yield return patterns[i].PlayPattern(context);
        }
    }
}
