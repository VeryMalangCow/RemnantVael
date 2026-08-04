using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternRangeSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Range")]
public class EnemyPatternRangeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [SerializeField] private AnimationClip animation;
    [Space(10)]
    [SerializeField] private Gradient trailGradient;
    [SerializeField] private float trailStartWidth;
    [SerializeField] private float trailTime;
    [Space(10)]
    [SerializeField] private Vector2 bulletShadowScale;

    [Space(10)]
    [SerializeField] private float lightIntensity;

    [Header("=== State")]
    [SerializeField] private BulletState bulletState;
    [SerializeField] private float baseAngle = 0f;
    [SerializeField] private Vector2 size;


    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement($"Range ({animation.name})",
            new PatternPreviewDetail($"Dmg: {bulletState.dmgState.dmg}", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Speed: {bulletState.muzzleSpeed}", Color.cyan));

    }
#endif

}
