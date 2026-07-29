using System.Collections;
using UnityEngine;

public abstract class EnemyPatternSO : ScriptableObject
{
    [Header("=== (Pattern Commonness) Value")]
    [SerializeField] protected float startDelay = 0f;
    [SerializeField] protected float endDelay = 0f;
    [SerializeField] protected int repeatAmount = 1;

    /// <summary> 실제 패턴 구동 </summary>
    protected abstract IEnumerator PlayPattern(EnemyAIContext aiContext);

#if UNITY_EDITOR

    /// <summary> 패턴 설명 </summary>
    public abstract PatternPreviewElement GetPreview();

#endif
}
