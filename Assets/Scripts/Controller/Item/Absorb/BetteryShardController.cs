using TMPro;
using UnityEngine;

public class BetteryShardController : AbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bettery Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int betteryValue = 1;
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

        betteryValue = value;
        amountTxt.text = $"(<size=150%>{value}</size>)";
    }

    #endregion

    #region Get Item

    protected override void GainItem()
    {
        base.GainItem();

        PlayerManager.instance.playerController.GainBetteryShard(betteryValue);
        DropItemManager.instance.RemoveBetteryShard(this);
    }

    #endregion
}
