using UnityEngine;

public class EnemyAttackPointer
{
    private DepthController[] attackDepths;
    public DepthController[] AttackDepths => attackDepths;

    public EnemyAttackPointer(DepthController[] depths)
    {
        attackDepths = depths;
    }

}
