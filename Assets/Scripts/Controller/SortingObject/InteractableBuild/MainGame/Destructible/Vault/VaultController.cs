using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class VaultController : DestructibleBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected StateAnimController IconStateAnim;

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int CurrentGrade = 0;

    // Grade
    [HideInInspector] private int MaxGrade = 4;

    // Oper
    [HideInInspector] public VaultRerollOperatorController RerollOper = null;
    [HideInInspector] public VaultUpgradeOperatorController UpgradeOper = null;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();

        base.Offset();
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        IconStateAnim.ThisSR.sortingOrder = _SortingOrder - 1;
    }

    #endregion

    #region Set

    public virtual void Set_Grade(int _Grade)
    {
        CurrentGrade = _Grade;

        Set_AnimValue();
        Set_StateAnim();
    }

    private void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Vault_AC[CurrentGrade];
        OnOffStateAC = UnitManager.Instance.Vault_StateAC;

        BrokenAC = UnitManager.Instance.Vault_BrokenAC[CurrentGrade];
        BrokenStateAC = UnitManager.Instance.Vault_StateAC.TypeBase;
    }

    public void Set_Upgrade()
    {
        CurrentGrade++;
        Set_Grade(CurrentGrade);
    }

    #endregion

    #region Is

    public bool Is_MaxGrade()
    {
        return CurrentGrade >= MaxGrade;
    }

    #endregion

    #region Change

    public void Change_ToOtherVault()
    {
        Type baseType = typeof(VaultController);
        Type resultType = null;

        List<Type> childTypes = Assembly.GetAssembly(baseType)
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseType))
            .ToList();

        while (true)
        {
            resultType = childTypes[UnityEngine.Random.Range(0, childTypes.Count)];

            if (resultType != this.GetType())
                break;
        }

        VaultController targetVault = DevTool.Get_ComponentTType<VaultController>(Instantiate(StageManager.Instance.Get_VaultCorrectType(resultType), gameObject.transform.parent));
        targetVault.transform.localPosition = transform.localPosition;
        targetVault.Change_OperValue(RepairOper, RerollOper, UpgradeOper);

        targetVault.Change_VaultValue(CurrentDur);

        Destroy(gameObject);
    }

    private void Change_OperValue(RepairOperatorController _RepairOper, VaultRerollOperatorController _RerollOper, VaultUpgradeOperatorController _UpgradeOper)
    {
        if (_RepairOper != null) _RepairOper.Set_TargetBuild(this);
        if (_RerollOper != null) _RerollOper.Set_TargetBuild(this);
        if (_UpgradeOper != null) _UpgradeOper.Set_TargetBuild(this);
    }


    public void Change_VaultValue(int _Dur)
    {
        StartCoroutine(Change_VaultValue_Cor(_Dur));
    }

    private IEnumerator Change_VaultValue_Cor(int _Dur)
    {
        yield return new WaitForEndOfFrame();

        CurrentDur = _Dur;
        Set_DurAmount(CurrentDur);
        Debug.Log("РќДо");
    }

    #endregion

    #region Break

    protected override void Play_NowBreak(bool _SpawnItem)
    {
        base.Play_NowBreak(_SpawnItem);

        if (RepairOper != null) RepairOper.Set_TargetBuildBroken();
        if (RerollOper != null) RerollOper.Set_TargetBuildBroken();
        if (UpgradeOper != null) UpgradeOper.Set_TargetBuildBroken();
    }

    #endregion
}
