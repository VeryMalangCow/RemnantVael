using UnityEngine;

public abstract class EnemyPatternConditionSO : ScriptableObject
{    
    /// <summary> 패턴을 실행 시킬 수 있는 조건 </summary>
    public abstract bool CanPlayPattern(EnemyAIContext aiContext);

#if UNITY_EDITOR

    /// <summary> 조건 설명 </summary>
    public abstract PatternPreviewElement GetPreview();

#endif
}
