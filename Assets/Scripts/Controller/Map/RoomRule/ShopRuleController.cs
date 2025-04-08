using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ShopRuleController : RoomRuleController
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

    [HideInInspector] public BaseUpgradeController BUShop;
    [HideInInspector] public ModuleUpgradeController MUShop;
    [HideInInspector] public RepairOperatorController BURepairOperator;
    [HideInInspector] public RepairOperatorController MURepairOperator;

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
