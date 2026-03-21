using UnityEngine;
using UnityEngine.Serialization;

public class AllyShopRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [FormerlySerializedAs("InRoom_BUShopParentTF")][SerializeField] public Transform inRoom_buShopParentTf;
    [FormerlySerializedAs("InRoom_MUShopParentTF")][SerializeField] public Transform inRoom_muShopParentTf;
    [FormerlySerializedAs("InRoom_BURepairOperactorParentTF")][SerializeField] public Transform inRoom_buRepairOperactorParentTf;
    [FormerlySerializedAs("InRoom_MURepairOperactorParentTF")][SerializeField] public Transform inRoom_muRepairOperactorParentTf;

    [HideInInspector] public AllyBaseUpgradeController buShop;
    [HideInInspector] public AllyModuleUpgradeController muShop;
    [HideInInspector] public RepairOperatorController buRepairOperator;
    [HideInInspector] public RepairOperatorController muRepairOperator;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        needKeyCardId = 4;
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
        if (buShop) buShop.gameObject.SetActive(true);
        if (buRepairOperator) buRepairOperator.gameObject.SetActive(true);

        if (muShop) muShop.gameObject.SetActive(true);
        if (muRepairOperator) muRepairOperator.gameObject.SetActive(true);
    }

    #endregion
}
