using UnityEngine;

public class EnemyPatternMeleeProvider : MonoBehaviour
{
    [SerializeField] private DepthController[] attackDepths;
    [SerializeField] private DepthController[] vfxDepths;

    public DepthController[] AttackDepths { get { return attackDepths; } }
    public DepthController[] VfxDepths { get { return vfxDepths; } }
}
