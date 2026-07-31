using System.Collections;

public class EnemyPatternRunner
{
    private EnemyPatternSelector selector;
    private EnemyAIContext context;

    public EnemyPatternRunner(EnemyPatternSelector selector, EnemyAIContext context)
    {
        this.selector = selector;
        this.context = context;
    }

    public IEnumerator RunLoop()
    {
        while (true)
        {
            EnemyPatternSequenceSO seqSO = selector.GetPlayablePattern(context);

            yield return PlaySequence(seqSO);
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
