using UnityEngine;
using UnityEngine.Rendering;

public class NpcController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> NPC")]
    [SerializeField] private SortingGroup sg;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(NPCManager.instance.allNpcs, this);
        AddSortingLayer();
    }

    private void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        // Base
        sg.sortingOrder = sortingOrder;
    }

    #endregion
}
