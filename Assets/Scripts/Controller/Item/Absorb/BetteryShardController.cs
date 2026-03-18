using TMPro;
using UnityEngine;

public class BetteryShardController : RangeAbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bettery Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int BetteryValue = 1;
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

    public void Set_State(Vector2 _SpawnPos, int _Value)
    {
        base.Set_State(_SpawnPos);

        BetteryValue = _Value;
        AmountTxt.text = $"(<size=150%>{_Value}</size>)";

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        IsSpawnNow = false;

        PlayerManager.Instance.playerController.Add_CurrentBetteryShard(BetteryValue);
        PoolingManager.Instance.betteryShard.Enqueue(this);
    }

    #endregion
}
