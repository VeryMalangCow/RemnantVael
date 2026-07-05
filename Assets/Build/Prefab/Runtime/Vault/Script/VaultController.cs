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
    [SerializeField] protected StateAnimController panelStateAnim;
    public StateAnimController PanelStateAnim => panelStateAnim;

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int currentGrade = 0;

    // Grade
    [HideInInspector] private int maxGrade = 4;

    // Oper
    [HideInInspector] public VaultRerollOperatorController rerollOper = null;
    [HideInInspector] public VaultUpgradeOperatorController upgradeOper = null;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();

        var prefab = StaticResourceManager.instance.BuildReso.vaultPrefab;
        thisSr.material = prefab.material;
        stateAnim.sr.material = prefab.iconMaterial;
        panelStateAnim.sr.material = prefab.panelMaterial;
        base.Offset();
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        // Vault의 안에 아이콘이 보이는 이미지
        panelStateAnim.sr.sortingOrder = sortingOrder - 1;
    }

    #endregion

    #region Set

    public virtual void Set_Grade(int grade)
    {
        currentGrade = Mathf.Clamp(grade, 0, maxGrade);

        Set_AnimValue();
        Set_StateAnim();
    }

    private void Set_AnimValue()
    {
        var vaultPrefab = StaticResourceManager.instance.BuildReso.vaultPrefab;
        onOffAc = new CoupleData<AnimationClip>(null, vaultPrefab.onAnimations[currentGrade]);
        onOffStateAc = vaultPrefab.stateIconAnimation;

        brokenAc = vaultPrefab.brokenAnimations[currentGrade];
        brokenStateAc = vaultPrefab.stateIconAnimation.typeBase;
    }

    public void Set_Upgrade()
    {
        currentGrade = Mathf.Min(currentGrade + 1, maxGrade);
        Set_Grade(currentGrade);
    }

    #endregion

    #region Is

    public bool Is_MaxGrade()
    {
        return currentGrade >= maxGrade;
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

        VaultController targetVault = Instantiate(StageManager.instance.GetVaultCorrectType(resultType), gameObject.transform.parent);
        targetVault.transform.localPosition = transform.localPosition;
        targetVault.Change_OperValue(repairOper, rerollOper, upgradeOper);

        targetVault.Change_VaultValue(currentDur);

        Destroy(gameObject);
    }

    private void Change_OperValue(RepairOperatorController repairOper, VaultRerollOperatorController rerollOper, VaultUpgradeOperatorController upgradeOper)
    {
        if (repairOper != null) repairOper.Set_TargetBuild(this);
        if (rerollOper != null) rerollOper.Set_TargetBuild(this);
        if (upgradeOper != null) upgradeOper.Set_TargetBuild(this);
    }


    public void Change_VaultValue(int dur)
    {
        StartCoroutine(Change_VaultValue_Cor(dur));
    }

    private IEnumerator Change_VaultValue_Cor(int dur)
    {
        yield return new WaitForEndOfFrame();

        currentDur = dur;
        Set_DurAmount(currentDur);
        Debug.Log("전달");
    }

    #endregion

    #region Break

    protected override void Play_NowBreak(bool spawnItem)
    {
        base.Play_NowBreak(spawnItem);

        if (repairOper != null) repairOper.Set_TargetBuildBroken();
        if (rerollOper != null) rerollOper.Set_TargetBuildBroken();
        if (upgradeOper != null) upgradeOper.Set_TargetBuildBroken();
    }

    #endregion
}
