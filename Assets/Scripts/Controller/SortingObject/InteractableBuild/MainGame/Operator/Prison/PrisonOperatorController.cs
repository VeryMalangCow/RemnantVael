using UnityEngine;

public class PrisonOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected PrisonController targetPrison;

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        var prefab = StaticResourceManager.instance.BuildPrefab.prisonPuzzleOperPrefab;
        iconStateAnim.Set_Anim(new State_Anim(prefab.animation, 1f), 1f);
        thisSr.material = prefab.material;
    }

    public virtual void Set_TargetBuild(PrisonController targetPrison)
    {
        this.targetPrison = targetPrison;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !targetPrison.isOn;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = false;
        return "";
    }

    public override void PlayInteract()
    {
        
    }

    #endregion
}
