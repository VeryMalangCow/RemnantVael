using UnityEngine;

public abstract class ConverterController : SortingObjectController
{
    [Space(20)]
    [Header("<><><><><> Cvt ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] protected Animator at;

    [HideInInspector] private AnimationClip ac;
    [HideInInspector] protected AnimatorOverrideController aoc;

    protected override void Offset()
    {
        base.Offset();

        var prefab = StaticResourceManager.instance.BuildPrefab.GetConverter(id);
        ac = prefab.animation;
        DevTool.Set_Anim(ref aoc, at, ac);
    }
}
