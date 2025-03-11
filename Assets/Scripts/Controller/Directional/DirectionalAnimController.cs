using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DirectionalAnimController : DirectionalController<AnimationClip, Animator>
{
    #region Value

    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();

        CurrentIndex.Subscribe(index =>
        {
            DevTool.Set_Anim(ref AOC, ThisComp, ThisDirectionalList[index]);
        });
    }

    #endregion
}
