using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "E999_IdleSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Idle")]

public class EnemyPatternIdleSO : EnemyPatternSO
{
    private static float waitTime = 0.5f;
    private static readonly WaitForSeconds followWait = new(waitTime);

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        yield return followWait;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("Idle",
            new PatternPreviewDetail("-"),
            new PatternPreviewDetail("-"),
            new PatternPreviewDetail("-"),
            new PatternPreviewDescription("this is idle which noable choose pattern."));
    }
#endif
}
