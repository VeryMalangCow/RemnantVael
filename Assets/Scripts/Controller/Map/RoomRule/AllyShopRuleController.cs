using UnityEngine;

public class AllyShopRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform inRoom_buShopParentTf;
    [SerializeField] public Transform inRoom_muShopParentTf;
    [SerializeField] public Transform inRoom_buRepairOperactorParentTf;
    [SerializeField] public Transform inRoom_muRepairOperactorParentTf;

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

    public override void Complete()
    {
        base.Complete();

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

    #region AllyShop & Oper

    public void SetAbuShop(AllyBaseUpgradeController abu, RepairOperatorController repairOper)
    {
        buShop = abu;
        buShop.transform.SetParent(inRoom_buShopParentTf);
        buShop.gameObject.transform.localPosition = Vector2.zero;
        buShop.gameObject.SetActive(false);

        buRepairOperator = repairOper;
        buRepairOperator.transform.SetParent(inRoom_buRepairOperactorParentTf);
        buRepairOperator.Set_TargetBuild(buShop);
        buRepairOperator.gameObject.transform.localPosition = Vector2.zero;
        buRepairOperator.gameObject.SetActive(false);
    }

    public void SetAmuShop(AllyModuleUpgradeController amu, RepairOperatorController repairOper)
    {
        muShop = amu;
        muShop.transform.SetParent(inRoom_muShopParentTf);
        muShop.gameObject.transform.localPosition = Vector2.zero;
        muShop.gameObject.SetActive(false);
        
        muRepairOperator = repairOper;
        muRepairOperator.transform.SetParent(inRoom_muRepairOperactorParentTf);
        muRepairOperator.Set_TargetBuild(muShop);
        muRepairOperator.gameObject.transform.localPosition = Vector2.zero;
        muRepairOperator.gameObject.SetActive(false);
    }

    #endregion
}
