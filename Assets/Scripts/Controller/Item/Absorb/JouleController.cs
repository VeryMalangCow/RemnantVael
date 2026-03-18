using TMPro;
using UnityEngine;

public class JouleController : RangeAbsorbItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Joule")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float EnergyValue = 1f;
    [SerializeField] private TMP_Text AmountTxt;

    [HideInInspector] private MeshRenderer TxtMR;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        TxtMR = DevTool.Get_ComponentTType(AmountTxt.gameObject, out MeshRenderer mr) ? mr : null;
    }

    #endregion

    #region Set

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        TxtMR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region State

    public void Set_State(Vector2 _SpawnPos, float _Value)
    {
        base.Set_State(_SpawnPos);

        EnergyValue = _Value;
        string txt = _Value % 1 == 0 ? _Value.ToString() : _Value.ToString("0.0");
        AmountTxt.text = $"(<size=150%>{txt}</size>)";

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        IsSpawnNow = false;

        PlayerManager.Instance.playerController.Add_CurrentEP(EnergyValue);
        PoolingManager.Instance.joule.Enqueue(this);
    }

    #endregion
}
