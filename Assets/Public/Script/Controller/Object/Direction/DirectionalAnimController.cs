using UniRx;
using UnityEngine;

public class DirectionalAnimController : DirectionalController<AnimationClip, Animator>
{
    #region Value

    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();

        currentIndex.Subscribe(index =>
        {
            DevTool.Set_Anim(ref aoc, comp, dirList[index]);
        });
    }

    #endregion
}
