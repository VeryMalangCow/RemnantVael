using UnityEngine;

public class ShopRuleController : RoomRuleController
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

    [HideInInspector] public BaseUpgradeController buShop;
    [HideInInspector] public ModuleUpgradeController muShop;
    [HideInInspector] public RepairOperatorController buRepairOperator;
    [HideInInspector] public RepairOperatorController muRepairOperator;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        needKeyCardId = 3;
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

    #region Shop & Oper

    public void SetBuShop(BaseUpgradeController bu, RepairOperatorController repairOper)
    {
        buShop = bu;
        buShop.transform.SetParent(inRoom_buShopParentTf);
        buShop.gameObject.transform.localPosition = Vector2.zero;
        buShop.gameObject.SetActive(false);

        buRepairOperator = repairOper;
        buRepairOperator.transform.SetParent(inRoom_buRepairOperactorParentTf);
        buRepairOperator.Set_TargetBuild(buShop);
        buRepairOperator.gameObject.transform.localPosition = Vector2.zero;
        buRepairOperator.gameObject.SetActive(false);
    }

    public void SetMuShop(ModuleUpgradeController mu, RepairOperatorController repairOper)
    {
        muShop = mu;
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
