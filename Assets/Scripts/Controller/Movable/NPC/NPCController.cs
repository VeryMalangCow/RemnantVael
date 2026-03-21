using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class NPCController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> NPC")]
    [FormerlySerializedAs("ThisSG")][SerializeField] private SortingGroup sg;

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

    public override void Set_SortingOrder(int sortingOrder)
    {
        // Base
        sg.sortingOrder = sortingOrder;
    }

    #endregion
}
