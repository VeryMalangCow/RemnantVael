using System.Collections.Generic;
using UnityEngine;

public class VaultRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    public Transform InRoom_VaultParentTF;

    [HideInInspector] public VaultController Vault;

    [HideInInspector] private static List<float> PercentVaultGrade = new List<float>
    { 0.4f, 0.3f, 0.15f, 0.1f, 0.05f };
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
    }

    #endregion

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {

        }
    }
}
