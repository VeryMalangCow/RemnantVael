using UnityEngine;

public class AllyShopRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform InRoom_BUShopParentTF;
    [SerializeField] public Transform InRoom_MUShopParentTF;
    [SerializeField] public Transform InRoom_BURepairOperactorParentTF;
    [SerializeField] public Transform InRoom_MURepairOperactorParentTF;

    [HideInInspector] public AllyBaseUpgradeController BUShop;
    [HideInInspector] public AllyModuleUpgradeController MUShop;
    [HideInInspector] public RepairOperatorController BURepairOperator;
    [HideInInspector] public RepairOperatorController MURepairOperator;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        NeedKeyCardID = 4;
    }

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Shop();
    }

    private void SetOn_Shop()
    {
        BUShop.gameObject.SetActive(true);
        MUShop.gameObject.SetActive(true);
        BURepairOperator.gameObject.SetActive(true);
        MURepairOperator.gameObject.SetActive(true);
    }

    #endregion
}
