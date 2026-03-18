using UnityEngine;
using UnityEngine.Rendering;

public class NPCController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> NPC")]
    [SerializeField] private SortingGroup ThisSG;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(NPCManager.instance.allNpcs, this);
        LayerOrderManager.instance.Add_NeedSortObj(this);
    }

    private void OnDisable()
    {
        LayerOrderManager.instance.Remove_NeedSortObj(this);
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        // Base
        ThisSG.sortingOrder = _SortingOrder;
    }

    #endregion
}
