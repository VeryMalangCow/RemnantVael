using UnityEngine;

public class EnemyMeleePointer : EnemyPointer
{
    private DepthController[] attackDepths;
    public DepthController[] AttackDepths => attackDepths;

    public EnemyMeleePointer(DepthController[] depths)
    {
        attackDepths = depths;
    }

}
