using System.Collections.Generic;
using UnityEngine;

public class VaultRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform InRoom_VaultParentTF;
    [SerializeField] public Transform InRoom_RepairOperactorParentTF;
    [SerializeField] public Transform InRoom_RerollOperactorParentTF;
    [SerializeField] public Transform InRoom_UpgradeOperactorParentTF;

    [HideInInspector] public VaultController Vault;
    [HideInInspector] public RepairOperatorController RepairOperator;
    [HideInInspector] public VaultUpgradeOperatorController UpgradeOperator;
    [HideInInspector] public VaultRerollOperatorController RerollOperator;

    [HideInInspector] private static List<float> PercentVaultGrade = new List<float>
    { 0.4f, 0.3f, 0.15f, 0.1f, 0.05f };

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        NeedKeyCardID = 1;
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
        Vault.gameObject.SetActive(true);
        Vault.Set_Grade(DevTool.Get_Grade(PercentVaultGrade));
        RepairOperator.gameObject.SetActive(true);
        UpgradeOperator.gameObject.SetActive(true);
        RerollOperator.gameObject.SetActive(true);
    }

    #endregion
}
