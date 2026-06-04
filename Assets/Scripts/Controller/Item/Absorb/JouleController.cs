using TMPro;
using UnityEngine;

public class JouleController : AbsorbItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Joule")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float energyValue = 1f;
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

    public void Set_State(Vector2 spawnPos, float value)
    {
        base.Set_State(spawnPos);

        energyValue = value;
        string txt = value % 1 == 0 ? value.ToString() : value.ToString("0.0");
        amountTxt.text = $"(<size=150%>{txt}</size>)";

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        PlayerManager.instance.playerController.AddCurrentEp(energyValue);
        DropItemManager.instance.RemoveJoule(this);
    }

    #endregion
}
