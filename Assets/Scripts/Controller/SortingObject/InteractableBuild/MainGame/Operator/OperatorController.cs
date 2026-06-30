using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class OperatorController : InteractableBuildController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Operator ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected StateAnimController iconStateAnim;

    [SerializeField] protected SortingGroup paySg;
    [SerializeField] protected TMP_Text payTxt;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();
        Set_StateAnim();

        base.Offset();
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);
        iconStateAnim.sr.sortingOrder = sortingOrder - 1;

        paySg.sortingOrder = sortingOrder + 1;
    }

    #endregion

    #region Set

    protected virtual void Set_AnimValue()
    {
        var prefab = StaticResourceManager.instance.BuildPrefab;
        onOffAc = prefab.operOnOffAnimation;
        onOffStateAc = prefab.operLightAnimation;
    }

    public void Set_TargetBuildBroken()
    {
        isOn = false;
        Set_StateAnim();
        paySg.gameObject.SetActive(false);
    }

    #endregion

    #region Interact

    public abstract string Get_InteractName(out bool canInteract);

    public abstract void PlayInteract();

    #endregion
}
