using System.Collections.Generic;
using UnityEngine;

public class VaultRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform inRoom_vaultParentTf;
    [SerializeField] public Transform inRoom_RepairOperactorParentTf;
    [SerializeField] public Transform inRoom_RerollOperactorParentTf;
    [SerializeField] public Transform inRoom_UpgradeOperactorParentTf;

    [HideInInspector] public VaultController vault;
    [HideInInspector] public RepairOperatorController repairOperator;
    [HideInInspector] public VaultUpgradeOperatorController upgradeOperator;
    [HideInInspector] public VaultRerollOperatorController rerollOperator;

    [HideInInspector] private static List<int> percentVaultGrade = new List<int>
        { 8, 6, 3, 2, 1 };

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        needKeyCardId = 1;
    }

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Vault();
    }

    private void SetOn_Vault()
    {
        vault.gameObject.SetActive(true);
        vault.Set_Grade(DevTool.Get_Grade(percentVaultGrade));
        repairOperator.gameObject.SetActive(true);
        upgradeOperator.gameObject.SetActive(true);
        rerollOperator.gameObject.SetActive(true);
    }

    #endregion

    #region Vault & Oper

    public void SetVault(VaultController _vault, RepairOperatorController repairOper, VaultRerollOperatorController rerollOper, VaultUpgradeOperatorController upgradeOper)
    {
        vault = _vault;
        vault.transform.SetParent(inRoom_vaultParentTf);
        vault.gameObject.transform.localPosition = Vector2.zero;
        vault.gameObject.SetActive(false);

        repairOperator = repairOper;
        repairOperator.transform.SetParent(inRoom_RepairOperactorParentTf);
        repairOperator.Set_TargetBuild(vault);
        repairOperator.gameObject.transform.localPosition = Vector2.zero;
        repairOperator.gameObject.SetActive(false);

        rerollOperator = rerollOper;
        rerollOperator.transform.SetParent(inRoom_RerollOperactorParentTf);
        rerollOperator.Set_TargetBuild(vault);
        rerollOperator.gameObject.transform.localPosition = Vector2.zero;
        rerollOperator.gameObject.SetActive(false);

        upgradeOperator = upgradeOper;
        upgradeOperator.transform.SetParent(inRoom_UpgradeOperactorParentTf);
        upgradeOperator.Set_TargetBuild(vault);
        upgradeOperator.gameObject.transform.localPosition = Vector2.zero;
        upgradeOperator.gameObject.SetActive(false);
    }

    #endregion
}
