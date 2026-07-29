using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleePatternSO",
    menuName = "ScriptableObject/EnemyPatternSO/PatternElement/Melee")]
public class EnemyPatternMeleeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [Header("-- Anim")]
    [SerializeField] private AnimationClip animation;
    [SerializeField] private bool isShadowRangeByDepthController = true;
    [SerializeField] private bool lightOn;
    [SerializeField] private float lightSize;

    [Header("=== State")]
    [Header("-- Time")]
    [SerializeField] private float jugeAndTweenTime;
    [SerializeField] private float animSpeed;

    [Header("-- Distance")]
    [SerializeField] private float spawnDis;
    [SerializeField] private float endDis;

    [Header("-- Size")]
    [SerializeField] private Vector2 size;

    protected override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement($"Melee ({animation.name})",
            new PatternPreviewDetail($"JugeTime: {jugeAndTweenTime}s", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Dis: {spawnDis} ~ {endDis}", Color.cyan));
    }
#endif
}
