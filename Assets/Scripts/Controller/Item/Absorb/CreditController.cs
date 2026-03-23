using TMPro;
using UnityEngine;

public class CreditController : RangeAbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Credit")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int gainAmount = 1;
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

    public override void Set_SortingOrder(int sortingOrder)
    {
        base.Set_SortingOrder(sortingOrder);

        mesh.sortingOrder = sortingOrder;
    }

    #endregion

    #region State

    public void Set_State(Vector2 spawnPos, int value)
    {
        base.Set_State(spawnPos);

        gainAmount = value;
        amountTxt.text = $"(<size=150%>{value}</size>)";

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        isSpawnNow = false;

        PlayerManager.instance.playerController.Add_CurrentCredit(gainAmount);
        PoolingManager.instance.credit.Enqueue(this);
    }

    #endregion
}
