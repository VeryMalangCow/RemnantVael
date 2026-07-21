using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleePatternSO",
    menuName = "ScriptableObject/EnemyPatternSO/PatternElement/Melee")]
public class EnemyPatternMeleeSO : EnemyPatternSO
{
    public override PatternPreviewElement GetPreview()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        throw new System.NotImplementedException();
    }
}
