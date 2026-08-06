using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternChargeSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Charge")]
public class EnemyPatternChargeSO : EnemyPatternSO
{

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        throw new System.NotImplementedException();
    }
#endif
}
