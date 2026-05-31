using TMPro;
using UnityEngine;

public class ModuleShardController : RangeAbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Module Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int moduleValue = 1;
    [SerializeField] private TMP_Text amountTxt;

    [HideInInspector] private MeshRenderer mesh;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        mesh = DevTool.Get_ComponentTType(amountTxt.gameObject, out MeshRenderer mr) ? mr : null;
    }

    #endregion

    #region Set

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        mesh.sortingOrder = sortingOrder;
    }

    #endregion

    #region State

    public void Set_State(Vector2 spawnPos, int value)
    {
        base.Set_State(spawnPos);

        moduleValue = value;
        amountTxt.text = $"(<size=150%>{value}</size>)";

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        isSpawnNow = false;

        PlayerManager.instance.playerController.Add_CurrentModuleShard(moduleValue);
        DropItemManager.instance.RemoveModuleShard(this);
    }

    #endregion
}
